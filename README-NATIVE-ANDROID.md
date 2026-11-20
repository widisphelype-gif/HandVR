# HandVR — Android nativo (sem Unity)

Esta branch inicia a migração do HandVR para Android nativo com Kotlin, CameraX e MediaPipe Tasks Vision. O APK de depuração é compilado pelo GitHub Actions; não é necessário instalar Unity ou Android Studio no PC.

## Recursos desta primeira versão
- Aplicativo Android nativo (Kotlin).
- Permissão e captura da câmera com CameraX.
- MediaPipe Hand Landmarker em modo VIDEO.
- Detecção inicial de pinça, palma aberta e punho.
- Alternância de layout celular / VR Box com visualização duplicada lado a lado.
- Modelo reutilizado diretamente de `Assets/Plugins/Android/HandVRMediaPipe.androidlib/src/main/assets/hand_landmarker.task`.

## Gerar APK pelo GitHub
1. Abra a aba **Actions** do repositório.
2. Selecione **Build HandVR Android APK**.
3. Clique em **Run workflow** na branch `feature/native-android`.
4. Aguarde o job terminar.
5. Baixe o artefato **HandVR-debug**; dentro dele estará `app-debug.apk`.

Também há build automático em cada push nesta branch.

## Requisitos
- Android 7.0 (API 24) ou superior.
- Permissão da câmera.
- Internet na primeira compilação do Gradle para baixar dependências.

## Limitações conhecidas
- Esta é uma primeira migração e ainda precisa ser compilada no Actions e testada em aparelho real.
- O layout VR Box duplica a imagem da câmera; ainda não há correção óptica, calibração de lentes, renderização 3D ou rastreamento de cabeça.
- Câmeras USB UVC ainda não estão implementadas.
- O reconhecimento de gestos usa heurísticas simples e precisa ser calibrado.
- A detecção é executada em sequência na thread de análise; desempenho varia por aparelho.
- O modelo MediaPipe mantém sua licença e avisos originais.

## Estrutura
- `app/`: aplicativo Android nativo.
- `.github/workflows/android-apk.yml`: build de APK no GitHub Actions.
- `Assets/Plugins/Android/HandVRMediaPipe.androidlib/src/main/assets/`: modelo oficial.
