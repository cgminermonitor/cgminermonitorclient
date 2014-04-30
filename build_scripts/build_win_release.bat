SET PATH=%PATH%;"C:\Program Files (x86)\Mono-2.10.9\bin"
SET ScriptDir=%CD%
cd .\..\

REM build CgminerMonitorClient
rmdir /S /Q "CgminerMonitorClient/bin/Release"
call xbuild /p:Configuration=Release /t:rebuild /p:DefineConstants="NORMAL TRACE FORRELEASE"
cd ".\CgminerMonitorClient\bin\Release\"
.\..\..\..\ilmerge.2.13.0307\ilmerge.exe /out:CgminerMonitorClient_Windows.exe CgminerMonitorClient.exe Newtonsoft.Json.dll OpenHardwareMonitorLib.dll
cd .\..\..\..\
rmdir /S /Q "builded_windows"
mkdir builded_windows
copy ".\CgminerMonitorClient\bin\Release\CgminerMonitorClient_Windows.exe" ".\builded_windows\"

cd %ScriptDir%