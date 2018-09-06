#!/bin/sh

# uncomment this line to use software rendering
# export FULGENS_SOFTWARE_RENDERER=1

# build native library
make all

# build managed application
cd FulgensConsole.Example
dotnet publish -c release -r linux-x64 -o ../build/