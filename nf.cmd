@echo off
if "%1"== "build" ( goto build )
if "%1"== "run"   ( goto run   )

echo Unknown command, please use one of following
echo    build, run
echo.
echo Command usage: "nf <command> <project>"
echo Example: nf build client/lib/server
goto end

:build
if "%2"=="client" ( goto build-client )
if "%2"=="server" ( goto build-server )
if "%2"=="lib"    ( goto build-lib    )

echo Unknown project for build, please use one of following:
echo    client, lib, server
goto end

:run
if "%2"=="client" ( goto run-client )
if "%2"=="server" ( goto run-server )

echo Unknown project for run, please use one of following:
echo    client, lib, server
goto end

:build-client
echo Building test client...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe test\client.csproj
echo.
echo Task completed.
goto end

:build-server
echo Building test server...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe test\server.csproj
echo.
echo Task completed.
goto end

:build-lib
echo Building library...
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .csproj
echo.
echo Task completed.
goto end

:run-client
test\target\client.exe
goto end

:run-server
test\target\server.exe
goto end

:end