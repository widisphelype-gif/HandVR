# HandVR

HandVR é um projeto de controle por gestos para Android e VR móvel.

## Versão Android nativa (sem Unity)
A implementação Android nativa está sendo desenvolvida na branch [feature/native-android](https://github.com/widisphelype-gif/HandVR/tree/feature/native-android) com Kotlin, CameraX, MediaPipe e GitHub Actions para gerar o APK.

Consulte [README-NATIVE-ANDROID.md](README-NATIVE-ANDROID.md) para instruções de build e limitações atuais.

## Estado
Esta versão ainda precisa concluir a compilação no GitHub Actions e ser testada em um aparelho Android. O modo VR Box inicial duplica a imagem para as duas lentes, mas ainda não oferece renderização estereoscópica corrigida, tracking de cabeça ou suporte UVC.
