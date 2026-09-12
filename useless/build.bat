@echo off
title Build Annoying Cat (ശല്യക്കാരൻ പൂച്ച)
echo ========================================================
echo Building Annoying Cat Windows Desktop Application...
echo ========================================================

set MSBUILD=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe

if not exist "%MSBUILD%" (
    echo Error: MSBuild.exe not found at %MSBUILD%
    pause
    exit /b 1
)

"%MSBUILD%" "%~dp0AnnoyingCat.csproj" /p:Configuration=Release /t:Rebuild /nologo

if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================================
    echo Build successful!
    echo Executable: bin\Release\AnnoyingCat.exe
    echo Double-click run.bat to launch the cat!
    echo ========================================================
) else (
    echo.
    echo Build failed with exit code %ERRORLEVEL%
    pause
)
