# fulgens-console
A text-based graphical front-end for games or GUI applications leveraging SDL2 and .NET Core.

# Build Instructions
## Windows
- Install MinGW64.
- Download the SDL2 and SDL_ttf libraries and headers and install them in the appropriate locations in your MinGW directory.
- Set your `MINGW_DIR` environment variable to the root of your MinGW64 installation (example: C:\MinGW64).
- Build the example application by running `build-example.ps1`.
## Linux
- Install the GNU C++ Compiler or whatever compiler you like (modify the makefile accordingly).
- Install `libsdl2-dev` and `libsdl2-ttf-dev` or whichever packages are available for your system.
- Build the example application by running `build-example.sh`.
