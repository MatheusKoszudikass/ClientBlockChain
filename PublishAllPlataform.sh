#!/bin/sh

platforms="win-x64 linux-x64 osx-x64"

for platform in $platforms
do
    dotnet publish -c Release -r $platform -p:PublishSingleFile=true --self-contained true
done
