rm -rf ./CgminerMonitorClient/bin/Release
xbuild /p:Configuration=Release
# due to problems with specyfing x64 or x86 dll, without another build configuration, this is done just before bundling
rm -rf ./CgminerMonitorClient/bin/Release/NAppUpdate.Framework_x86_unix.dll
cp ./libs/NAppUpdate.Framework_x86_unix.dll ./CgminerMonitorClient/bin/Release/NAppUpdate.Framework.dll

cd "./CgminerMonitorClient/bin/Release/"
mkbundle --deps --static -z -L ./ CgminerMonitorClient.exe Newtonsoft.Json.dll NAppUpdate.Framework.dll System.Numerics.dll OpenHardwareMonitorLib.dll -o CgminerMonitorClient_Linux --machine-config /etc/mono/4.0/machine.config
cd ./../../../
rm -rf builded_linux
mkdir builded_linux
cp ./CgminerMonitorClient/bin/Release/CgminerMonitorClient_Linux ./builded_linux/CgminerMonitorClientX86Linux
