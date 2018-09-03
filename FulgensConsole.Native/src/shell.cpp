#include "shell.h"

shell_resources *initialize(int width, int height, const char *title)
{
  shell_resources *res = new shell_resources;
  res->quitting = false;
  res->disposed = false;
  res->text_input = std::string();

  if (SDL_Init(SDL_INIT_VIDEO) < 0)
  {
    free(res);
    fprintf(stderr, "Failed to initialize SDL: %s\n", SDL_GetError());
    return nullptr;
  }

  if (TTF_Init() < 0)
  {
    free(res);
    fprintf(stderr, "Failed to initialize TTF: %s\n", TTF_GetError());
    return nullptr;
  }

  if (!(IMG_Init(IMG_INIT_PNG) & IMG_INIT_PNG))
  {
    free(res);
    fprintf(stderr, "Failed to initialize IMG: %s\n", IMG_GetError());
    return nullptr;
  }

  SDL_CreateWindowAndRenderer(width, height, NULL, &res->window, &res->renderer);
  SDL_SetRenderDrawColor(res->renderer, 0x00, 0x00, 0x00, 0xFF);
  SDL_SetWindowTitle(res->window, title);

  return res;
}

void dispose(shell_resources *res)
{
  for (auto font : res->fonts)
    if (font) TTF_CloseFont(font);

  for (auto texture : res->textures)
    if (texture) SDL_DestroyTexture(texture);

  SDL_DestroyRenderer(res->renderer);
  SDL_DestroyWindow(res->window);

  free(res);

  SDL_Quit();

  res->disposed = true;
}

bool disposed(shell_resources *res)
{
  return res->disposed;
}

bool quitting(shell_resources *res)
{
  return res->quitting;
}

void update(shell_resources *res)
{
  res->text_input.clear();
  while (!res->key_down_events.empty()) res->key_down_events.pop();
  while (!res->key_up_events.empty()) res->key_up_events.pop();

  SDL_Event evt;
  while (SDL_PollEvent(&evt))
  {
    if (evt.type == SDL_QUIT)
      res->quitting = true;
    else if (evt.type == SDL_KEYDOWN)
      res->key_down_events.push(evt.key.keysym.scancode);
    else if (evt.type == SDL_KEYUP)
      res->key_up_events.push(evt.key.keysym.scancode);
    else if (evt.type == SDL_TEXTINPUT)
      res->text_input += evt.text.text;
  }
}

void quit(shell_resources *res)
{
  res->quitting = true;
}


int get_key_down(shell_resources *res)
{
  if (!res->key_down_events.empty())
  {
    int code = res->key_down_events.front();
    res->key_down_events.pop();
    return code;
  }
  else
  {
    return -1;
  }
}

int get_key_up(shell_resources *res)
{
  if (!res->key_up_events.empty())
  {
    auto code = res->key_up_events.front();
    res->key_up_events.pop();
    return code;
  }
  else
  {
    return -1;
  }
}

void get_input_text(shell_resources *res, char * text)
{
  if (!res->text_input.empty())
    strcpy(text, res->text_input.c_str());
}

void resize(shell_resources *res, int width, int height)
{
  SDL_SetWindowSize(res->window, width, height);
}

void clear(shell_resources *res)
{
  SDL_SetRenderDrawBlendMode(res->renderer, SDL_BLENDMODE_NONE);
  SDL_SetRenderDrawColor(res->renderer, 0x00, 0x00, 0x00, 0xFF);
  SDL_RenderClear(res->renderer);
}

void flip_buffer(shell_resources *res)
{
  SDL_RenderPresent(res->renderer);
}

TTF_Font * load_ttf_font(shell_resources *res, const char *path, int size)
{
  auto font = TTF_OpenFont(path, size);
  if (!font)
    fprintf(stderr, "Failed to load TrueType font: %s", TTF_GetError());
  else
    res->fonts.push_back(font);

  return font;
}

void ttf_text_size(TTF_Font *font, const char *contents, int *w, int *h)
{
  TTF_SizeText(font, contents, w, h);
}

void draw_text(shell_resources *res, TTF_Font *font, const char *contents, int x, int y, color foreColor, color backColor)
{
  SDL_Color fc = { foreColor.R, foreColor.G, foreColor.B, foreColor.A };
  auto surface = fc.a < 255
    ? TTF_RenderText_Blended(font, contents, fc)
    : TTF_RenderText_Solid(font, contents, fc);
  if (!surface)
  {
    fprintf(stderr, "Failed to render TrueType font: %s", TTF_GetError());
  }
  else
  {
    auto texture = SDL_CreateTextureFromSurface(res->renderer, surface);
    SDL_FreeSurface(surface);

    if (!texture)
    {
      fprintf(stderr, "Failed to create texture from rendered TrueType font: %s", SDL_GetError());
      return;
    }

    SDL_Rect bounds;
    bounds.x = x;
    bounds.y = y;
    SDL_QueryTexture(texture, NULL, NULL, &bounds.w, &bounds.h);

    // if we've gotten to this point, we have the final texture bounds and can
    // therefore draw the background color
    //
    // avoid an unnecessary draw call by skipping any backgrounds that would
    // be fully transparent
    if (backColor.A > 0)
    {
      SDL_SetRenderDrawBlendMode(res->renderer,
        backColor.A < 255
        ? SDL_BLENDMODE_BLEND
        : SDL_BLENDMODE_NONE);

      SDL_Color bc = { backColor.R, backColor.G, backColor.B, backColor.A };    
      SDL_SetRenderDrawColor(res->renderer, bc.r, bc.g, bc.b, bc.a);

      SDL_RenderFillRect(res->renderer, &bounds);
    }

    SDL_RenderCopy(res->renderer, texture, NULL, &bounds);

    SDL_DestroyTexture(texture);
  }
}

SDL_Texture * load_image(shell_resources *res, const char *path)
{
  auto surface = IMG_Load(path);
  if (!surface)
  {
    fprintf(stderr, "Failed to load image \"%s\": %s\n", path, IMG_GetError());
    return nullptr;
  }
  else
  {
    auto texture = SDL_CreateTextureFromSurface(res->renderer, surface);
    SDL_FreeSurface(surface);
    if (!texture)
    {
      fprintf(stderr, "Failed to create texture from loaded image \"\"%s\n", path, SDL_GetError());
      return nullptr;
    }
    return texture;
  }
}
