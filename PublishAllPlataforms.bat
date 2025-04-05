@echo off
set platforms=win-x64 linux-x64 osx-x64

for %%p in (%platforms%) do (
    dotnet publish -c Release -r %%p -p:PublishSingleFile=true --self-contained true
)
