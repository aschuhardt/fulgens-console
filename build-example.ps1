# Note: make sure to set MINGW_DIR to the root of your MinGW64 installation
# also, ensure that your MinGW64 installation contains the SDl2 and SDL2_ttf libraries

# build native library
mingw32-make.exe all

# build managed application
cd FulgensConsole.Example
dotnet build -c release -r win-x64 -o ../build/

# copy third-party dependencies
cd ..
Copy-item -Force -Recurse -Verbose ./third-party/win-x64/* -Destination ./build/