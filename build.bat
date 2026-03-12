@echo off
title ACTrainer Build
color 0A

echo.
echo.

where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo  [!] .NET SDK not found.
    echo  [!] Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo  [*] Building...
echo.

dotnet publish AssaultCubeExternal.csproj ^
  -c Release ^
  -r win-x86 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -o ./dist

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo  [!] Build FAILED. Check errors above.
    pause
    exit /b 1
)

echo.
echo  [+] Done! Output: .\dist\Assault Cube 1.3.0.2 External.exe
echo  [+] Run as Administrator for memory access.
echo.
explorer dist
pause
