# HandVR

Protótipo Android de realidade virtual com interação por gestos das mãos, pensado para celulares sem giroscópio.

## Estado atual

Este repositório é um **starter de desenvolvimento Unity**, não um APK pronto. Ele inclui a arquitetura inicial, cena de demonstração, controles alternativos de visão e uma interface para conectar o rastreador de mãos.

### O que já está preparado
- Estrutura de projeto Unity.
- Modo de visualização normal e modo VR lado a lado (estéreo).
- Orientação sem giroscópio por arrasto/toque na tela.
- Interface de abstração para rastreamento de mãos.
- Gestos previstos: pinça, mão aberta e punho.
- Configuração inicial para Android.

### O que ainda precisa ser integrado/testado
- Plugin de visão computacional MediaPipe para Unity/Android.
- Integração de câmera USB UVC para câmera externa. O suporte depende do Android, do adaptador e do driver.
- Ajuste de distância interpupilar, distorção das lentes e teste no VR Box.
- Compilação e teste em aparelho real.

## Requisitos
- Unity 2022.3 LTS ou versão LTS compatível.
- Android Build Support (SDK, NDK e OpenJDK pelo Unity Hub).
- Android 8.0 ou superior recomendado.
- Para rastreamento pelo próprio celular, use o modo sem headset: a câmera traseira precisa enxergar as mãos.
- No VR Box, a câmera do telefone normalmente fica coberta. Para rastrear mãos nesse modo, será necessária uma câmera externa compatível ou um segundo dispositivo.

## Abrir
1. Instale o Unity Hub e o módulo Android Build Support.
2. No Unity Hub, escolha **Add > Add project from disk** e selecione esta pasta.
3. Abra `Assets/Scenes/HandVR.unity` (ou crie uma cena e adicione `HandVRBootstrap`).
4. Em **File > Build Profiles**, selecione Android.
5. Configure package name como `com.handvr.app` e conceda permissão de câmera.
6. Para um APK de teste, use **Build**.

## Arquitetura de rastreamento
`IHandTrackingProvider` é a interface que recebe landmarks normalizados (0 a 1). O `HandTrackingDemoProvider` é apenas um provedor de demonstração para validar a interface; ele **não reconhece mãos reais**. Substitua-o por um adaptador MediaPipe que alimente `HandLandmarkFrame`.

Gestos iniciais:
- Pinça: distância entre ponta do polegar e ponta do indicador.
- Mão aberta: dedos estendidos.
- Punho: dedos recolhidos.

## Sem giroscópio
`TouchLookController` controla a orientação com arrasto na tela. É uma alternativa de entrada, não rastreamento real da cabeça. Um futuro modo de câmera frontal poderá estimar orientação de cabeça, mas a precisão depende da iluminação e da câmera.

## Câmera externa
O Android não oferece uma API USB universal para toda câmera UVC. A implementação final precisará validar o dispositivo e possivelmente usar uma biblioteca UVC Android. Não há promessa de compatibilidade com qualquer webcam.

## Licença
Código inicial disponibilizado para estudo e desenvolvimento. Adicione as licenças dos SDKs/plugins de terceiros que integrar.
