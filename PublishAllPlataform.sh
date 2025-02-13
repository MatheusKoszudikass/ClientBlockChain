#!/bin/sh

# Lista de plataformas
platforms="win-x64 linux-x64 osx-x64"

# Loop para gerar os pacotes para todas as plataformas
for platform in $platforms
do
    dotnet publish -c Release -r $platform -p:PublishSingleFile=true --self-contained true
done
