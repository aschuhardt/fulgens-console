#ifndef SHELL_H
#define SHELL_H

#include <vector>
#include <queue>
#include <cstdio>

#include <string>

#define SDL_MAIN_HANDLED
#include <SDL.h>
#include <SDL_ttf.h>
#include <SDL_image.h>

#ifdef _WINDLL
  #define API extern "C" __declspec(dllexport)
#else
  #define API extern "C" 
#endif

#ifdef FULGENS_USE_SOFTWARE_RENDERER
  #define FULGENS_RENDERER_FLAGS SDL_RENDERER_SOFTWARE
#else
  #define FULGENS_RENDERER_FLAGS SDL_RENDERER_ACCELERATED
#endif

#pragma pack(push, 8)

struct color
{
  int R, G, B, A;
};

struct rect
{
  int X, Y, Width, Height;
};

#pragma pack(pop)

struct shell_resources
{
  SDL_Renderer *renderer;
  SDL_Window *window;
  std::vector<TTF_Font*> fonts;
  std::vector<SDL_Texture*> textures;
  std::queue<int> key_up_events;
  std::queue<int> key_down_events;
  std::string text_input;
  bool quitting;
  bool disposed;
};

API shell_resources *initialize(int width, int height, const char *title);

API void dispose(shell_resources *res);

API bool disposed(shell_resources *res);

API bool quitting(shell_resources *res);

API void update(shell_resources *res);

API void quit(shell_resources *res);

API int get_key_down(shell_resources *res);

API int get_key_up(shell_resources *res);

API void get_input_text(shell_resources *res, char * text);

API void resize(shell_resources *res, int width, int height);

API void clear(shell_resources *res);

API void flip_buffer(shell_resources *res);

API TTF_Font * load_ttf_font(shell_resources *res, const char *path, int size);

API void ttf_text_size(TTF_Font *fnt, const char *contents, int *w, int *h);

API void draw_text(shell_resources *res, TTF_Font *fnt, const char *contents, int x, int y, color foreColor, color backColor);

API SDL_Texture * load_image(shell_resources *res, const char *path);

// API void draw_sprite(shell_resources *res, void *texture, rect)

#endif // SHELL_H