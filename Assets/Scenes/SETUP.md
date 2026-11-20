# HandVR — interface inicial

Ao abrir o projeto no Unity Editor, o script `Assets/Editor/HandVRSceneSetup.cs` cria a cena inicial `Assets/Scenes/HandVR.unity` e a adiciona às Build Settings. O app constrói a interface ao iniciar.

## Teste da interface
1. Abra o projeto no Unity 2022.3 LTS.
2. Aguarde a criação da cena `HandVR.unity`.
3. Selecione **HandVR > Create Startup Scene** se a cena não aparecer.
4. Pressione Play para testar a interface no Editor.
5. Para Android, instale Android Build Support no Unity Hub e faça o build da cena criada.

## Recursos implementados nesta etapa
- Interface escura com identidade visual HandVR.
- Solicitação de permissão de câmera no Android.
- Prévia da câmera do aparelho com `WebCamTexture`.
- Botões para iniciar/parar a câmera e selecionar o modo de uso.
- Criação automática de cena e inclusão nas Build Settings.

## Limitações honestas
- A prévia da câmera **não reconhece mãos**. MediaPipe ainda não foi integrado.
- O botão VR Box informa a limitação física: a câmera traseira do telefone fica coberta no headset. É necessária câmera externa compatível ou outro dispositivo para rastrear mãos.
- O modo sem giroscópio ainda usa toque/arrasto; não estima a rotação da cabeça.
- A interface VR Box ainda não renderiza uma cena 3D estereoscópica.
- Não há APK compilado nesta etapa; o projeto precisa ser aberto e compilado no Unity.
