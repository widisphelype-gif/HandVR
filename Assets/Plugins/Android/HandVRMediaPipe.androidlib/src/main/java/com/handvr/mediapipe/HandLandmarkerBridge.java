package com.handvr.mediapipe;

import android.content.Context;
import android.graphics.Bitmap;
import com.google.mediapipe.framework.image.BitmapImageBuilder;
import com.google.mediapipe.framework.image.MPImage;
import com.google.mediapipe.tasks.core.BaseOptions;
import com.google.mediapipe.tasks.vision.core.RunningMode;
import com.google.mediapipe.tasks.vision.handlandmarker.HandLandmarker;
import com.google.mediapipe.tasks.vision.handlandmarker.HandLandmarkerResult;
import java.util.List;

public final class HandLandmarkerBridge {
    private static HandLandmarker landmarker;

    private HandLandmarkerBridge() {}

    public static synchronized boolean initialize() {
        if (landmarker != null) return true;
        try {
            Class<?> unityPlayer = Class.forName("com.unity3d.player.UnityPlayer");
            Context context = (Context) unityPlayer.getField("currentActivity").get(null);
            BaseOptions baseOptions = BaseOptions.builder()
                .setModelAssetPath("hand_landmarker.task")
                .build();
            HandLandmarker.HandLandmarkerOptions options =
                HandLandmarker.HandLandmarkerOptions.builder()
                    .setBaseOptions(baseOptions)
                    .setRunningMode(RunningMode.VIDEO)
                    .setNumHands(2)
                    .setMinHandDetectionConfidence(0.5f)
                    .setMinHandPresenceConfidence(0.5f)
                    .setMinTrackingConfidence(0.5f)
                    .build();
            landmarker = HandLandmarker.createFromOptions(context, options);
            return true;
        } catch (Exception error) {
            android.util.Log.e("HandVR", "MediaPipe initialization failed", error);
            landmarker = null;
            return false;
        }
    }

    // Receives ARGB pixels from Unity. Returns 63 floats for the first hand (21 x/y/z),
    // or an empty array when no hand is detected.
    public static synchronized float[] detect(int[] argb, int width, int height, long timestampMs) {
        if (landmarker == null || argb == null || width <= 0 || height <= 0 ||
            argb.length != width * height) return new float[0];

        Bitmap bitmap = null;
        MPImage image = null;
        try {
            bitmap = Bitmap.createBitmap(argb, width, height, Bitmap.Config.ARGB_8888);
            image = new BitmapImageBuilder(bitmap).build();
            HandLandmarkerResult result = landmarker.detectForVideo(image, timestampMs);
            List<List<com.google.mediapipe.tasks.components.containers.NormalizedLandmark>> hands =
                result.landmarks();
            if (hands == null || hands.isEmpty()) return new float[0];

            List<com.google.mediapipe.tasks.components.containers.NormalizedLandmark> points = hands.get(0);
            if (points.size() < 21) return new float[0];
            float[] values = new float[63];
            for (int i = 0; i < 21; i++) {
                values[i * 3] = points.get(i).x();
                values[i * 3 + 1] = points.get(i).y();
                values[i * 3 + 2] = points.get(i).z();
            }
            return values;
        } catch (Exception error) {
            android.util.Log.e("HandVR", "MediaPipe frame processing failed", error);
            return new float[0];
        } finally {
            if (image != null) image.close();
            if (bitmap != null && !bitmap.isRecycled()) bitmap.recycle();
        }
    }

    public static synchronized void close() {
        if (landmarker != null) {
            landmarker.close();
            landmarker = null;
        }
    }
}
