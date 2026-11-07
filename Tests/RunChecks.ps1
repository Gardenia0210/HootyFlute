param(
    [Parameter(Mandatory=$true)][string]$CSharpCompiler,
    [Parameter(Mandatory=$true)][string]$CppCompiler,
    [string]$OutputDirectory = (Join-Path $env:TEMP 'HootyFluteChecks')
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$protocolExe = Join-Path $OutputDirectory 'ProtocolTests.exe'
& $CSharpCompiler /nologo /langversion:9.0 "/out:$protocolExe" `
    (Join-Path $PSScriptRoot 'ProtocolTests.cs') `
    (Join-Path $repo 'Unity/Assets/Code/Communication/InstrumentProtocol.cs')
if ($LASTEXITCODE -ne 0) { throw 'Protocol test compilation failed.' }
& $protocolExe
if ($LASTEXITCODE -ne 0) { throw 'Protocol regression failed.' }
$firmwareExe = Join-Path $OutputDirectory 'FirmwareTests.exe'
if ((Split-Path $CppCompiler -Leaf) -eq 'cl.exe') {
    # The harness uses only zero-initialized globals and needs no C runtime.
    & $CppCompiler /nologo /GS- /EHsc /Od "/I$PSScriptRoot/Firmware" `
        (Join-Path $PSScriptRoot 'Firmware/FirmwareTests.cpp') `
        "/Fe:$firmwareExe" "/Fo:$OutputDirectory/FirmwareTests.obj" `
        /link /entry:main /subsystem:console /nodefaultlib
} else {
    & $CppCompiler -std=c++11 "-I$PSScriptRoot/Firmware" `
        (Join-Path $PSScriptRoot 'Firmware/FirmwareTests.cpp') -o $firmwareExe
}
if ($LASTEXITCODE -ne 0) { throw 'Firmware test compilation failed.' }
& $firmwareExe
if ($LASTEXITCODE -ne 0) { throw "Firmware regression failed at source line $LASTEXITCODE." }
Write-Output 'PASS: all 256 key combinations and firmware contact/watchdog/pump/sensor regressions.'
