# Unity project context

Analyzed: 2026-10-17. Source is a local project without Git history.

Confirmed: Unity 2022.3.62f1c1, built-in render pipeline, legacy input, uGUI,
TextMesh Pro, and MonoBehaviour composition. First-party scripts use the default
Assembly-CSharp assembly. Existing MonoBehaviour GUIDs and serialized field names
are retained. StartGame -> ChooseSong -> SampleScene -> CommunityShare is the
configured navigation flow. SongDefinition holds authored score data.

InputManager owns a desktop serial connection. Unity sends the currently judgeable
score note; Arduino owns physical fingering, pressure hysteresis, pump timeout,
contact rearming, and the buzzer. Only a rising confirmed sounding state can score,
and each frame targets one unresolved note. No simulated player inputs are used.

Firmware pin assumptions: keys D2-D9, pumps D10-D12, buzzer D13, three analog
film-pressure module outputs A0-A2. The supplied module example reverses ADC values
as pressure rises, so negative polarity is the default. Thresholds and timeout are
engineering defaults requiring bench calibration. No physical wiring or clinical
training parameters were verified. Calibration runs at startup with no tongue load.

Unity MCP is unavailable. The matching Unity Editor was located through the source
project's assembly-reference paths. Final verification passed: Unity desktop Mono
build (Windows x64, 196428509 bytes), all four scenes without missing scripts,
loaded 14-note song asset, 19 standalone protocol cases, 256 exhaustive fingering
combinations plus firmware state-machine regressions, and Arduino Uno compilation
(5264 bytes flash, 330 bytes static RAM). Original source hashes matched for all
790 inspected files. Play Mode, visual interaction, and device validation were not
performed. The firmware tests use a host Arduino adapter, not physical sensors.
Evidence: ProjectVersion.txt, package manifests, GraphicsSettings.asset,
EditorBuildSettings.asset, original Assets/Scripts, Arduino/0210all, and the local
manufacturer analog film-pressure Arduino example.
