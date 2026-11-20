# HandVR

Aplicativo experimental de realidade virtual para Android, com foco em interação por mãos e suporte a celulares sem giroscópio.

## Estado do projeto

**Etapa atual: interface inicial e prévia da câmera.** O projeto é uma base Unity em desenvolvimento, ainda não é um APK pronto e não deve ser descrito como rastreador de mãos funcional.

### Já incluído
- Projeto Unity 2022.3 LTS.
- Interface inicial escura com identidade visual HandVR.
- Criação automática da cena inicial pelo Editor.
- Prévia da câmera do celular usando `WebCamTexture`.
- Solicitação de permissão de câmera no Android.
- Controles iniciais de modo celular e VR Box.
- Estrutura de dados e reconhecedor heurístico para landmarks de mão.
- Controle de visão por toque/arrasto como alternativa ao giroscópio.

### Ainda pendente
- Integração real do MediaPipe Hands (modelo e runtime Android).
- Rastreamento de mãos por câmera externa UVC.
- Renderização estereoscópica 3D adequada ao VR Box.
- Calibração de lentes e distância interpupilar.
- Testes em aparelho Android e geração/validação do APK.

## Abrir o projeto

1. Instale Unity Hub e Unity 2022.3 LTS com **Android Build Support**, SDK, NDK e OpenJDK.
2. Baixe/clonar este repositório.
3. No Unity Hub, selecione **Add project from disk** e abra a pasta.
4. Aguarde o Unity importar os arquivos e criar `Assets/Scenes/HandVR.unity`.
5. Se necessário, use **HandVR > Create Startup Scene**.
6. Pressione Play para testar a interface no Editor.
7. Para gerar um APK, selecione Android em Build Profiles e compile a cena inicial.

Consulte [Assets/Scenes/SETUP.md](Assets/Scenes/SETUP.md) para detalhes e limitações.

## Importante sobre o hardware

- Sem giroscópio, o controle de visão inicial é feito por toque/arrasto. Isso não é rastreamento preciso da cabeça.
- No VR Box, a câmera traseira do telefone costuma ficar coberta. Para rastrear as mãos dentro do headset, será necessária uma câmera externa compatível ou um segundo dispositivo.
- A prévia de câmera já está implementada, mas **não identifica mãos ainda**. O MediaPipe precisa ser integrado separadamente.

## Gestos planejados
- Pinça: polegar e indicador próximos.
- Palma aberta.
- Punho fechado.

## Licenças

Verifique e inclua os avisos/licenças dos SDKs de terceiros que forem adicionados ao projeto.
