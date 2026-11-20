package com.handvr.app

import android.Manifest
import android.content.pm.PackageManager
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Matrix
import android.os.Bundle
import android.view.Gravity
import android.view.View
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import androidx.activity.ComponentActivity
import androidx.activity.result.contract.ActivityResultContracts
import androidx.camera.core.CameraSelector
import androidx.camera.core.ImageAnalysis
import androidx.camera.core.ImageProxy
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.core.content.ContextCompat
import com.google.mediapipe.framework.image.BitmapImageBuilder
import com.google.mediapipe.tasks.core.BaseOptions
import com.google.mediapipe.tasks.vision.core.RunningMode
import com.google.mediapipe.tasks.vision.handlandmarker.HandLandmarker
import com.google.mediapipe.tasks.vision.handlandmarker.HandLandmarkerResult
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors
import kotlin.math.hypot

class MainActivity : ComponentActivity() {
    private lateinit var status: TextView
    private lateinit var gesture: TextView
    private lateinit var leftPreview: ImageView
    private lateinit var rightPreview: ImageView
    private lateinit var cameraExecutor: ExecutorService
    private var landmarker: HandLandmarker? = null
    private var vrMode = false
    private var lastInference = 0L

    private val permissionLauncher = registerForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { granted ->
        if (granted) startCamera() else status.text = "Permissão da câmera necessária."
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        cameraExecutor = Executors.newSingleThreadExecutor()
        buildInterface()
        if (hasCameraPermission()) startCamera()
        else permissionLauncher.launch(Manifest.permission.CAMERA)
    }

    private fun buildInterface() {
        val root = LinearLayout(this).apply {
            orientation = LinearLayout.VERTICAL
            setPadding(24, 28, 24, 24)
            setBackgroundColor(0xFF080C16.toInt())
        }
        val title = TextView(this).apply {
            text = "HANDVR"
            textSize = 30f
            setTextColor(0xFF73E0FF.toInt())
            gravity = Gravity.CENTER_VERTICAL
        }
        root.addView(title)
        val subtitle = TextView(this).apply {
            text = "CONTROLE DE REALIDADE VIRTUAL"
            textSize = 12f
            setTextColor(0xFFAE_BED2.toInt())
        }
        root.addView(subtitle)

        val previews = LinearLayout(this).apply {
            orientation = LinearLayout.HORIZONTAL
            setBackgroundColor(0xFF131B2B.toInt())
        }
        leftPreview = ImageView(this).apply { scaleType = ImageView.ScaleType.CENTER_CROP }
        rightPreview = ImageView(this).apply { scaleType = ImageView.ScaleType.CENTER_CROP; visibility = View.GONE }
        previews.addView(leftPreview, LinearLayout.LayoutParams(0, 0, 1f).apply { height = 0; weight = 1f })
        previews.addView(rightPreview, LinearLayout.LayoutParams(0, 0, 1f).apply { height = 0; weight = 1f })
        root.addView(previews, LinearLayout.LayoutParams(-1, 0, 1f))

        status = TextView(this).apply {
            text = "Iniciando câmera..."
            textSize = 14f
            setTextColor(0xFF82E1BE.toInt())
            setPadding(0, 18, 0, 8)
        }
        gesture = TextView(this).apply {
            text = "MediaPipe: aguardando câmera"
            textSize = 18f
            setTextColor(0xFFFFFFFF.toInt())
            setPadding(0, 8, 0, 16)
        }
        root.addView(status)
        root.addView(gesture)

        val modeButton = Button(this).apply {
            text = "Alternar celular / VR Box"
            setOnClickListener {
                vrMode = !vrMode
                rightPreview.visibility = if (vrMode) View.VISIBLE else View.GONE
                status.text = if (vrMode)
                    "Modo VR Box: imagem dividida para as duas lentes."
                else "Modo celular ativado."
            }
        }
        root.addView(modeButton)
        val retryButton = Button(this).apply {
            text = "Reiniciar câmera"
            setOnClickListener { startCamera() }
        }
        root.addView(retryButton)
        setContentView(root)
    }

    private fun hasCameraPermission() =
        ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED

    private fun startCamera() {
        val future = ProcessCameraProvider.getInstance(this)
        future.addListener({
            try {
                val provider = future.get()
                initializeLandmarker()
                val analysis = ImageAnalysis.Builder()
                    .setBackpressureStrategy(ImageAnalysis.STRATEGY_KEEP_ONLY_LATEST)
                    .setOutputImageFormat(ImageAnalysis.OUTPUT_IMAGE_FORMAT_RGBA_8888)
                    .build()
                analysis.setAnalyzer(cameraExecutor) { image -> analyzeFrame(image) }
                provider.unbindAll()
                provider.bindToLifecycle(this, CameraSelector.DEFAULT_BACK_CAMERA, analysis)
                runOnUiThread { status.text = "Câmera ativa • mostre sua mão." }
            } catch (error: Exception) {
                runOnUiThread { status.text = "Erro ao iniciar câmera: ${error.message}" }
            }
        }, ContextCompat.getMainExecutor(this))
    }

    private fun initializeLandmarker() {
        if (landmarker != null) return
        val options = HandLandmarker.HandLandmarkerOptions.builder()
            .setBaseOptions(BaseOptions.builder().setModelAssetPath("hand_landmarker.task").build())
            .setRunningMode(RunningMode.VIDEO)
            .setNumHands(2)
            .setMinHandDetectionConfidence(0.5f)
            .setMinHandPresenceConfidence(0.5f)
            .setMinTrackingConfidence(0.5f)
            .build()
        landmarker = HandLandmarker.createFromOptions(this, options)
    }

    private fun analyzeFrame(image: ImageProxy) {
        try {
            val now = System.currentTimeMillis()
            if (now - lastInference < 90L) return
            lastInference = now
            val plane = image.planes[0]
            val buffer = plane.buffer
            val bytes = ByteArray(buffer.remaining())
            buffer.get(bytes)
            val raw = Bitmap.createBitmap(image.width, image.height, Bitmap.Config.ARGB_8888)
            raw.copyPixelsFromBuffer(java.nio.ByteBuffer.wrap(bytes))
            val rotation = image.imageInfo.rotationDegrees
            val matrix = Matrix().apply { postRotate(rotation.toFloat()) }
            val upright = Bitmap.createBitmap(raw, 0, 0, raw.width, raw.height, matrix, true)
            raw.recycle()
            val mpImage = BitmapImageBuilder(upright).build()
            val result = landmarker?.detectForVideo(mpImage, now)
            mpImage.close()
            renderFrame(upright, result)
            upright.recycle()
        } catch (error: Exception) {
            runOnUiThread { status.text = "Falha no processamento: ${error.message}" }
        } finally {
            image.close()
        }
    }

    private fun renderFrame(bitmap: Bitmap, result: HandLandmarkerResult?) {
        val points = result?.landmarks()?.firstOrNull()
        val label = if (points == null) "Nenhuma mão detectada" else recognizeGesture(points)
        val frame = bitmap.copy(Bitmap.Config.ARGB_8888, false)
        runOnUiThread {
            leftPreview.setImageBitmap(frame)
            if (vrMode) rightPreview.setImageBitmap(frame)
            gesture.text = label
        }
    }

    private fun recognizeGesture(points: List<com.google.mediapipe.tasks.components.containers.NormalizedLandmark>): String {
        if (points.size < 21) return "Mão detectada"
        val thumb = points[4]
        val index = points[8]
        val pinch = hypot(thumb.x() - index.x(), thumb.y() - index.y())
        if (pinch < 0.055f) return "Gesto: PINÇA"
        val tips = listOf(8, 12, 16, 20)
        val extended = tips.count { points[it].y() < points[0].y() }
        return when {
            extended >= 3 -> "Gesto: PALMA ABERTA"
            extended == 0 -> "Gesto: PUNHO"
            else -> "Gesto: MÃO"
        }
    }

    override fun onDestroy() {
        try { landmarker?.close() } catch (_: Exception) {}
        cameraExecutor.shutdown()
        super.onDestroy()
    }
}
