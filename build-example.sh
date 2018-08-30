#!/bin/sh

# build native library
make all

# build managed application
cd FulgensConsole.Example
dotnet publish -c release -r linux-x64 -o ../build/