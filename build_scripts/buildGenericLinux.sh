#!/bin/bash

cd ./../ # go back to root

if [ "$1" = "MacOSX" ]
then
    export AS="as -arch i386"
    export CC="cc -arch i386"
    # export PATH=/Library/Frameworks/Mono.framework/Commands/:$PATH
    # export PKG_CONFIG_PATH=$PKG_CONFIG_PATH:/usr/lib/pkgconfig:/Library/Frameworks/Mono.framework/Versions/Current/lib/pkgconfig
fi

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

if [ "$1" = "MacOSX" ]
then
	MachineConfigFileName="/Library/Frameworks/Mono.framework/Versions/Current/etc/mono/2.0/machine.config"
else
	MachineConfigFileName="/etc/mono/2.0/machine.config"
fi

mkbundle --deps --static -z -L ./ CgminerMonitorClient.exe Newtonsoft.Json.dll OpenHardwareMonitorLib.dll -o CgminerMonitorClient_Linux --machine-config $MachineConfigFileName
cd ./../../../
rm -rf builded_linux/$RESULTFILENAME
mkdir builded_linux

cp ./CgminerMonitorClient/bin/Release/CgminerMonitorClient_Linux ./builded_linux/$RESULTFILENAME