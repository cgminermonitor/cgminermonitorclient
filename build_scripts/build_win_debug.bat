SET PATH=%PATH%;"C:\Program Files (x86)\Mono-2.10.9\bin"
SET ScriptDir=%CD%
cd .\..\

REM build NAppUpdateMono
cd .\NAppUpdateMono\buildScripts
call build_win_release.bat
cd .\..\..\
SET NAppUpdateLib=".\libs\NAppUpdate.Framework.dll"
IF EXIST %NAppUpdateLib% del /F %NAppUpdateLib%
IF EXIST %NAppUpdateLib% exit 1
copy ".\NAppUpdateMono\bin_mono_compilant\NAppUpdate.Framework.dll" %NAppUpdateLib%
IF NOT EXIST %NAppUpdateLib% exit 1

REM build CgminerMonitorClient
rmdir /S /Q "CgminerMonitorClient/bin/Debug"
call xbuild /p:Configuration=Debug /t:rebuild /p:DefineConstants="TRACE FORDEBUGGING"
cd ".\CgminerMonitorClient\bin\Debug\"
.\..\..\..\ilmerge.2.13.0307\ilmerge.exe /out:CgminerMonitorClient_Windows.exe CgminerMonitorClient.exe Newtonsoft.Json.dll NAppUpdate.Framework.dll OpenHardwareMonitorLib.dll
cd .\..\..\..\
rmdir /S /Q "builded_windows"
mkdir builded_windows
copy ".\CgminerMonitorClient\bin\Debug\CgminerMonitorClient_Windows.exe" ".\builded_windows\"

cd %ScriptDir%