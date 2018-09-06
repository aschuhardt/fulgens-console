OBJS = $(addprefix FulgensConsole.Native/src/,shell.cpp)

OBJ_NAME = FulgensConsole.Native

CC = g++

COMPILER_FLAGS = \
	-IFulgensConsole.Native/include \
	-shared \
	-Werror \
	-fno-exceptions \
	-fPIC

LINKER_FLAGS = \
	-lSDL2_ttf \
	-lSDL2_image \
	-lstdc++

BUILD_DIR = build

ifneq ($(FULGENS_SOFTWARE_RENDERER),)
	COMPILER_FLAGS += -DFULGENS_USE_SOFTWARE_RENDERER
endif

ifeq ($(OS),Windows_NT)
	ifneq ($(MINGW_DIR),)
		CLEAN_BUILD_DIR := del /S /Q $(BUILD_DIR)
		CLEAR := cls
		COMPILER_FLAGS += -D_WINDLL -DWIN32 -m64 -I"$(MINGW_DIR)\include\SDL2" -Dmain=SDL_main
		LINKER_FLAGS += -L"$(MINGW_DIR)\lib"  -lmingw32 -lSDL2 
		ifeq ($(PROCESSOR_ARCHITEW6432),AMD64)
			COMPILER_FLAGS += -DAMD64
		else
			ifeq ($(PROCESSOR_ARCHITECTURE),AMD64)
				COMPILER_FLAGS += -DAMD64
			endif
			ifeq ($(PROCESSOR_ARCHITECTURE),x86)
				COMPILER_FLAGS += -DIA32
			endif
		endif
	else
		MISSING_MINGW_DIR := YES
	endif
else
	CLEAN_BUILD_DIR := rm -rf $(BUILD_DIR) && mkdir $(BUILD_DIR)
	CLEAR := clear
	COMPILER_FLAGS += `sdl2-config --cflags`
	LINKER_FLAGS += `sdl2-config --libs` -ldl
	UNAME_S := $(shell uname -s)
	ifeq ($(UNAME_S),Linux)
		COMPILER_FLAGS += -DLINUX
	endif
	ifeq ($(UNAME_S),Darwin)
		COMPILER_FLAGS += -DOSX
	endif
	UNAME_P := $(shell uname -p)
	ifeq ($(UNAME_P),x86_64)
		COMPILER_FLAGS += -DAMD64
	endif
	ifneq ($(filter %86,$(UNAME_P)),)
		COMPILER_FLAGS += -DIA32
	endif
	ifneq ($(filter arm%,$(UNAME_P)),)
		COMPILER_FLAGS += -DARM
	endif
endif

RELEASE_COMPILER_FLAGS = \
	-O3 -s -w \
	$(COMPILER_FLAGS)

.PHONY: cls clean all compile

compile: $(OBJS)
	$(CC) $(OBJS) $(RELEASE_COMPILER_FLAGS) $(LINKER_FLAGS) -o $(BUILD_DIR)/$(OBJ_NAME).dll

cls:
	$(CLEAR)

clean:
	$(CLEAN_BUILD_DIR)

ifeq ($(MISSING_MINGW_DIR),YES) 
all:
	@echo -------------------------
	@echo MINGW_DIR was not not found, cannot compile!
	@echo Set MINGW_DIR to the 64-bit MinGW installation path and try again.
	@echo -------------------------
else
all: | cls clean compile
endif