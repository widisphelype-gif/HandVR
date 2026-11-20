using UnityEngine;

namespace HandVR
{
    public static class HandGestureRecognizer
    {
        // Índices do modelo MediaPipe Hands.
        private const int ThumbTip = 4;
        private const int IndexTip = 8;
        private const int Wrist = 0;

        public static HandGesture Recognize(HandLandmarkFrame frame)
        {
            if (frame == null || !frame.hasHand || frame.landmarks == null || frame.landmarks.Length < 21)
                return HandGesture.None;

            var thumb = frame.landmarks[ThumbTip];
            var index = frame.landmarks[IndexTip];
            float pinchDistance = Vector2.Distance(
                new Vector2(thumb.x, thumb.y),
                new Vector2(index.x, index.y));

            if (pinchDistance < 0.055f)
                return HandGesture.Pinch;

            // Heurística inicial: pontas acima do pulso na imagem.
            int extended = 0;
            int[] tips = { 8, 12, 16, 20 };
            foreach (int tip in tips)
                if (frame.landmarks[tip].y < frame.landmarks[Wrist].y)
                    extended++;

            if (extended >= 3) return HandGesture.OpenPalm;
            if (extended == 0) return HandGesture.Fist;
            return HandGesture.None;
        }
    }
}
