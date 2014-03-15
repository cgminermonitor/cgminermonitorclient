#!/bin/bash

cd ./../ # go back to root

# build NAppUpdateMono
cd ./NAppUpdateMono/buildScripts
sh build_linux.sh
cd ./../../
rm -rf ./libs/NAppUpdate.Framework.dll
cp ./NAppUpdateMono/bin_mono_compilant/NAppUpdate.Framework.dll ./libs/NAppUpdate.Framework.dll

# build CgminerMonitorClient
if [ $(uname -m) = "x86_64" ]
then
	ResultBits="64"
else
	ResultBits="86"
fi

rm -rf ./CgminerMonitorClient/bin/Release
xbuild /p:Configuration=Release /t:rebuild /p:DefineConstants="$1 TRACE FORRELEASE"
if [ "$1" = "NORMAL" ]
then
	RESULTFILENAME="CgminerMonitorClientX"$ResultBits"Linux"
else
	RESULTFILENAME="CgminerMonitorClientX"$ResultBits$1
fi

cd "./CgminerMonitorClient/bin/Release/"
mkbundle --deps --static -z -L ./ CgminerMonitorClient.exe Newtonsoft.Json.dll NAppUpdate.Framework.dll OpenHardwareMonitorLib.dll -o CgminerMonitorClient_Linux --machine-config /etc/mono/2.0/machine.config
cd ./../../../
rm -rf builded_linux
mkdir builded_linux

cp ./CgminerMonitorClient/bin/Release/CgminerMonitorClient_Linux ./builded_linux/$RESULTFILENAME