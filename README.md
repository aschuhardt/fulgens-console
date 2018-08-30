# fulgens-console
A text-based graphical front-end for games or GUI applications leveraging SDL2 and .NET Core.

## Planned features:
- ~TrueType text rendering~
- ~Keyboard + text input handling~
- Text position anchoring
- Word wrapping
- Sprite rendering
- Audio, maybe

# Build Instructions

## Windows
- Download the SDL2 and SDL_ttf libraries and headers and install them someplace meaningful.
### MinGW64
- Install MinGW64.
- Set your `MINGW_DIR` environment variable to the root of your MinGW64 installation (example: C:\MinGW64).
- Build the example application by running `build-example.ps1`.
### Visual Studio (MSVC)
- Use the provided solution and project files to build `FulgensConsole.Native`.

## Linux
- Install the GNU C++ Compiler or whatever compiler you like (modify the makefile accordingly).
- Install `libsdl2-dev` and `libsdl2-ttf-dev` or whichever packages are available for your system.
- Build the example application by running `build-example.sh`.
