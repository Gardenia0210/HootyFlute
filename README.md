# Hooty Flute

A gamified oral-motor training instrument connecting an Arduino recorder to a Unity game.

```text
Arduino/HootyFlute/       Firmware, pin assignments, and pressure/contact rules
Unity/
  Assets/
    Art/                 Sprites, fonts, and animations
    Code/
      Communication/     Serial transport and instrument telemetry
      Game/              Score data, moving notes, and note spawning
      Navigation/        Result-page navigation
      ChooseSong/        Song-selection interactions
      StartGame/         Welcome-page navigation
      Community/         Session-result presentation
      Results/           Scoring, progress, and session results
      UI/                Finger hints and character feedback
      Editor/            Project validation and desktop build entry point
    Data/Songs/          Editable musical scores
    Scenes/              Welcome, song selection, gameplay, and results
    Prefabs/             Reusable game objects
    ThirdParty/          Original TextMesh Pro and visual-effect assets
  Packages/              Unity package manifests
  ProjectSettings/       Editor, desktop API, and scene configuration
Tests/                   Protocol and firmware regression checks
requirements.txt         Toolchain and dependency locations
```
