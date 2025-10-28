@echo off
REM RenameIt CLI wrapper script for Windows
REM This script makes it easier to run the RenameIt CLI tool

dotnet run --project "%~dp0RenameIt.CLI\RenameIt.CLI.csproj" -- %*
