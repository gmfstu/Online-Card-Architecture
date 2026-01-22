#!/bin/bash

# Golden Tests Runner for Online Card Architecture
# This script builds and runs all tests, returning 0 if all pass, non-zero if any fail.

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

echo "======================================"
echo "  Building Golden Tests..."
echo "======================================"

cd "$SCRIPT_DIR"

# Restore and build the project
dotnet restore --verbosity quiet 2>/dev/null || dotnet restore
dotnet build --configuration Release --verbosity quiet 2>/dev/null || dotnet build --configuration Release

echo ""
echo "======================================"
echo "  Running Golden Tests..."
echo "======================================"
echo ""

# Run the tests - the exit code from dotnet run will be the test runner's exit code
dotnet run --configuration Release --no-build -- "$@"
EXIT_CODE=$?

exit $EXIT_CODE
