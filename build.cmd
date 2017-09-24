@echo off
cls

.paket\paket.bootstrapper.exe 5.91.0
if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe restore -g build-tools
if errorlevel 1 (
  exit /b %errorlevel%
)

packages\build-tools\FAKE\tools\FAKE.exe build.fsx %*