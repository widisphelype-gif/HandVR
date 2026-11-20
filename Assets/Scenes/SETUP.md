# HandVR — configuração e teste

## Requisitos
- Unity 2022.3 LTS com Android Build Support, SDK, NDK e OpenJDK.
- Android 7.0 (API 24) ou superior.
- Câmera funcional e permissão concedida.
- Modelo MediaPipe Hand Landmarker (instruções abaixo).

## Modelo MediaPipe
O modelo oficial **Hand Landmarker (float16)** já está incluído no repositório em:
`Assets/Plugins/Android/HandVRMediaPipe.androidlib/src/main/assets/hand_landmarker.task`

Não é necessário baixá-lo novamente. Se precisar restaurar o arquivo, use o [download oficial](https://storage.googleapis.com/mediapipe-models/hand_landmarker/hand_landmarker/float16/1/hand_landmarker.task) e mantenha exatamente esse nome e caminho.

O modelo é distribuído pelo projeto MediaPipe. Confira os termos e avisos de licença do modelo e da dependência antes de distribuir o aplicativo.

## Abrir e gerar o APK
1. Abra o projeto no Unity 2022.3 LTS.
2. Aguarde a criação da cena `Assets/Scenes/HandVR.unity`.
3. Se necessário, use **HandVR > Create Startup Scene**.
4. Em Build Profiles, selecione Android e confirme que a cena inicial está incluída.
5. Gere um APK de teste e instale em um aparelho Android compatível.
6. Abra o app, conceda permissão da câmera e toque em **Câmera do celular**.
7. Depois que a prévia aparecer, toque em **Ativar reconhecimento de mãos**.
8. Mostre uma mão para a câmera. O app deve indicar o gesto reconhecido: Pinch, OpenPalm ou Fist.

## O que foi conectado nesta etapa
- Módulo Android `.androidlib` com MediaPipe Tasks Vision 0.10.14.
- Ponte Java que inicializa o Hand Landmarker no modo VIDEO.
- Adaptador Unity que envia quadros da `WebCamTexture` ao módulo nativo e recebe os 21 landmarks da primeira mão detectada.
- Reconhecimento heurístico inicial de pinça, palma aberta e punho.
- Limite de inferência aproximado de 10 quadros por segundo para reduzir carga.

## Limitações e validação
- A integração foi adicionada ao repositório, mas **ainda não foi compilada nem testada no Unity ou em um aparelho real**. Pode exigir ajustes de Gradle/API conforme a versão do Unity e do Android.
- O adaptador atual envia imagens da câmera do próprio telefone; ele ainda não captura câmera USB UVC.
- O código reporta os landmarks da primeira mão detectada, embora o modelo esteja configurado para detectar até duas mãos.
- A prévia pode usar a câmera traseira. Para gestos diante do telefone, teste também a câmera frontal e ajuste a seleção da câmera se necessário.
- Sem giroscópio, o olhar continua controlado por toque/arrasto; MediaPipe não substitui um sensor de orientação.
- O modo VR Box ainda não tem renderização estereoscópica 3D, calibração de lentes ou rastreamento de cabeça.
- Para usar o telefone dentro do VR Box, a câmera traseira fica obstruída. Será necessária uma câmera externa compatível ou um segundo dispositivo para rastrear as mãos.
- Não há APK pronto publicado nesta etapa.

## Diagnóstico
- **MediaPipe não inicializa:** confirme se `hand_landmarker.task` está na pasta exata e se o módulo Android foi importado pelo Unity.
- **Nenhuma mão detectada:** confira a permissão, iluminação, enquadramento e se a mão está visível na prévia.
- **Erro de compilação Android:** confira Android SDK/NDK/JDK instalados pelo Unity Hub e os logs completos do Gradle.
