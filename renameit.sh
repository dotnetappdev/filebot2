#!/bin/bash
# RenameIt CLI wrapper script
# This script makes it easier to run the RenameIt CLI tool

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Run the RenameIt CLI
dotnet run --project "$SCRIPT_DIR/RenameIt.CLI/RenameIt.CLI.csproj" -- "$@"
