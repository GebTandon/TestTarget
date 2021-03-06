﻿@echo off
REM ------------------------------------
REM File creates a TestTarget website that can do GET calls.
REM ------------------------------------

SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
SET DOTNET="C:\Program Files\dotnet\dotnet.exe"
SET REMOTEOUTDIR=G:\temp\TestTarget\

mkdir "%REMOTEOUTDIR%"

@echo Publish the TestTarget
DEL /F /S /Q %REMOTEOUTDIR%

%DOTNET% restore "../TestTarget.csproj"
%DOTNET% publish "../TestTarget.csproj" -c Release -r linux-x64  --no-self-contained -o "%REMOTEOUTDIR%"

docker build -f ../Docker/Dockerfile.local -t leancnsl.local:1 --rm  ../.
REM docker run -it --rm --entrypoint bash leancnsl.local:1 

REM RMDIR /S /Q "%REMOTEOUTDIR%"
