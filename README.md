# HandVR

Aplicativo experimental de realidade virtual para Android, com foco em interação por mãos e suporte a celulares sem giroscópio.

## Estado do projeto

**Etapa atual: interface, prévia da câmera e integração inicial do MediaPipe Android.** O código de integração foi adicionado, mas ainda não foi compilado nem validado em aparelho. Portanto, este repositório ainda não é um APK pronto.

### Incluído
- Projeto Unity 2022.3 LTS.
- Interface inicial escura com identidade visual HandVR.
- Criação automática da cena inicial pelo Editor.
- Prévia da câmera do aparelho usando `WebCamTexture`.
- Solicitação de permissão de câmera no Android.
- Módulo Android com MediaPipe Tasks Vision 0.10.14.
- Ponte Java para o Hand Landmarker no modo VIDEO.
- Adaptador Unity para enviar quadros e receber 21 landmarks da primeira mão detectada.
- Reconhecimento heurístico inicial de pinça, palma aberta e punho.
- Controle de visão por toque/arrasto como alternativa ao giroscópio.

## Preparar o modelo
Baixe o modelo oficial Hand Landmarker float16 e coloque-o em:

`Assets/Plugins/Android/HandVRMediaPipe.androidlib/src/main/assets/hand_landmarker.task`

Download:
https://storage.googleapis.com/mediapipe-models/hand_landmarker/hand_landmarker/float16/1/hand_landmarker.task

Consulte [Assets/Scenes/SETUP.md](Assets/Scenes/SETUP.md) para o passo a passo completo, build Android e diagnóstico.

## Limitações atuais
- A integração MediaPipe está no código, mas **ainda não foi compilada nem testada**; ajustes podem ser necessários.
- A câmera USB UVC ainda não está implementada.
- O app ainda não renderiza uma cena VR Box estereoscópica 3D nem calibra lentes/IPD.
- Sem giroscópio, o olhar usa toque/arrasto; não é rastreamento preciso da cabeça.
- A câmera traseira fica obstruída quando o telefone está dentro de muitos modelos VR Box; rastreamento dentro do headset exigirá câmera externa compatível ou outro dispositivo.
- Ainda não existe APK validado/publicado.

## Licenças
Confira e mantenha os avisos de licença do Unity, MediaPipe Tasks Vision e do modelo baixado antes de redistribuir o aplicativo.
