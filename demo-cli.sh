#!/bin/bash

# Demo script for RenameIt.CLI
# This script demonstrates how to use the console GUI application

echo "==================================="
echo "RenameIt.CLI - Console GUI Demo"
echo "==================================="
echo ""

# Build the project
echo "Building RenameIt.CLI..."
dotnet build RenameIt.CLI/RenameIt.CLI.csproj --verbosity quiet

if [ $? -eq 0 ]; then
    echo "✓ Build successful!"
    echo ""
    
    # Show usage
    echo "Usage Instructions:"
    echo "-------------------"
    echo "To launch the console GUI, run:"
    echo "  dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj -- -gui"
    echo ""
    echo "Or after building:"
    echo "  ./RenameIt.CLI/bin/Debug/net8.0/RenameIt.CLI -gui"
    echo ""
    
    # Show features
    echo "Features:"
    echo "---------"
    echo "• Full mouse support in the terminal"
    echo "• Dual-pane file browser (original vs preview)"
    echo "• Template management"
    echo "• FileBot-compatible format syntax"
    echo "• Multiple metadata sources (TheMovieDB, TheTVDB, TVMaze)"
    echo "• Backup before rename option"
    echo "• XTree Gold-style interface"
    echo ""
    
    # Prompt to launch
    read -p "Would you like to launch the GUI now? (y/n) " -n 1 -r
    echo ""
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "Launching RenameIt.CLI..."
        dotnet run --project RenameIt.CLI/RenameIt.CLI.csproj -- -gui
    fi
else
    echo "✗ Build failed!"
    exit 1
fi
