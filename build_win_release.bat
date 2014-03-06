SET PATH=%PATH%;"C:\Program Files (x86)\Mono-3.2.3\bin"

rmdir /S /Q "CgminerMonitorClient/bin/Release"
call xbuild /p:Configuration=Release
cd ".\CgminerMonitorClient\bin\Release\"
.\..\..\..\ilmerge.2.13.0307\ilmerge.exe /out:CgminerMonitorClient_Windows.exe CgminerMonitorClient.exe Newtonsoft.Json.dll NAppUpdate.Framework_windows.dll OpenHardwareMonitorLib.dll
cd .\..\..\..\
rmdir /S /Q "builded_windows"
mkdir builded_windows
xcopy ".\CgminerMonitorClient\bin\Release\CgminerMonitorClient_Windows.exe" ".\builded_windows\"