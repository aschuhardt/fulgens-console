#ifndef SHELL_H
#define SHELL_H

#include <vector>
#include <queue>
#include <cstdio>

#include <string>

#define SDL_MAIN_HANDLED
#include <SDL.h>
#include <SDL_ttf.h>

#ifdef _WINDLL
  #define API extern "C" __declspec(dllexport)
#else
  #define API extern "C" 
#endif

struct shell_resources
{
  SDL_Renderer *renderer;
  SDL_Window *window;
  std::vector<TTF_Font*> fonts;
  std::queue<int> key_up_events;
  std::queue<int> key_down_events;
  std::string text_input;
  bool quitting;
  bool disposed;
};

API shell_resources *initialize(int width, int height, const char *title);

API void dispose(void *res);

API bool disposed(void *res);

API bool quitting(void *res);

API void update(void *res);

API void quit(void *res);

API int get_key_down(void *res);

API int get_key_up(void *res);

API void get_input_text(void *res, char * text);

API void resize(void *res, int width, int height);

API void clear(void *res);

API void flip_buffer(void *res);

API TTF_Font * load_ttf_font(void *res, const char *path, int size);

API void ttf_text_size(void *fnt, const char *contents, int *w, int *h);

API void draw_text(void *res, void *fnt, const char *contents, int x, int y, int r, int g, int b, int a);

#endif // SHELL_H