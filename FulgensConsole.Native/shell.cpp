#include <vector>
#include <queue>
#include <cstdio>
#include <string.h>
#include <string>

#define SDL_MAIN_HANDLED
#include <SDL.h>
#include <SDL_ttf.h>

#ifdef _WINDLL
	#define API __declspec(dllexport)
#else
	#define API
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

extern "C" 
API shell_resources *initialize(int width, int height, const char *title)
{
	shell_resources *resources = new shell_resources;

	resources->quitting = false;
	resources->disposed = false;
	resources->text_input = std::string();

	if (SDL_Init(SDL_INIT_VIDEO) < 0)
	{
		free(resources);
		fprintf(stderr, "Failed to initialize SDL: %s", SDL_GetError());
		return nullptr;
	}

	if (TTF_Init() < 0)
	{
		free(resources);
		fprintf(stderr, "Failed to initialize TTF: %s", TTF_GetError());
		return nullptr;
	}

	SDL_CreateWindowAndRenderer(width, height, NULL, &resources->window, &resources->renderer);
	SDL_SetRenderDrawColor(resources->renderer, 0x00, 0x00, 0x00, 0xFF);
	SDL_SetWindowTitle(resources->window, title);

	return resources;
}

extern "C"
API void dispose(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	for (auto font : resources->fonts)
		if (font) TTF_CloseFont(font);

	SDL_DestroyRenderer(resources->renderer);
	SDL_DestroyWindow(resources->window);

	free(resources);

	SDL_Quit();

	resources->disposed = true;
}

extern "C"
API bool disposed(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return true;

	return resources->disposed;
}

extern "C"
API bool quitting(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return true;

	return resources->quitting;
}

extern "C"
API void update(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	resources->text_input.clear();
	while (!resources->key_down_events.empty()) resources->key_down_events.pop();
	while (!resources->key_up_events.empty()) resources->key_up_events.pop();

	SDL_Event evt;
	while (SDL_PollEvent(&evt))
	{
		if (evt.type == SDL_QUIT)
			resources->quitting = true;
		else if (evt.type == SDL_KEYDOWN)
			resources->key_down_events.push(evt.key.keysym.scancode);
		else if (evt.type == SDL_KEYUP)
			resources->key_up_events.push(evt.key.keysym.scancode);
		else if (evt.type == SDL_TEXTINPUT)
			resources->text_input += evt.text.text;
	}
}

extern "C"
API void quit(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	resources->quitting = true;
}


extern "C"
API int get_key_down(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return -1;

	if (!resources->key_down_events.empty())
	{
		int code = resources->key_down_events.front();
		resources->key_down_events.pop();
		return code;
	}
	else
	{
		return -1;
	}
}

extern "C"
API int get_key_up(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return -1;

	if (!resources->key_up_events.empty())
	{
		auto code = resources->key_up_events.front();
		resources->key_up_events.pop();
		return code;
	}
	else
	{
		return -1;
	}
}

extern "C"
API void get_input_text(void *res, char * text)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	if (!resources->text_input.empty())
		strcpy_s(text, resources->text_input.size() + 1, resources->text_input.c_str());
}

extern "C"
API void resize(void *res, int width, int height)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	SDL_SetWindowSize(resources->window, width, height);
}

extern "C"
API void clear(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	SDL_RenderClear(resources->renderer);
}

extern "C"
API void flip_buffer(void *res)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	SDL_RenderPresent(resources->renderer);
}

extern "C"
API TTF_Font * load_ttf_font(void *res, const char *path, int size)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return nullptr;

	auto font = TTF_OpenFont(path, size);
	if (!font)
		fprintf(stderr, "Failed to load TrueType font: %s", TTF_GetError());
	else
		resources->fonts.push_back(font);

	return font;
}

extern "C"
API void ttf_text_size(void *fnt, const char *contents, int *w, int *h)
{
	auto font = static_cast<TTF_Font*>(fnt);
	if (font == nullptr) return;

	TTF_SizeText(font, contents, w, h);
}

extern "C"
API void draw_text(void *res, void *fnt, const char *contents, int x, int y, int r, int g, int b, int a)
{
	auto resources = static_cast<shell_resources*>(res);
	if (resources == nullptr) return;

	auto font = static_cast<TTF_Font*>(fnt);
	if (font == nullptr) return;

	SDL_Color color = { r, g, b, a };
	auto surface = TTF_RenderText_Solid(font, contents, color);
	if (!surface)
	{
		fprintf(stderr, "Failed to render TrueType font: %s", TTF_GetError());
	}
	else
	{
		auto texture = SDL_CreateTextureFromSurface(resources->renderer, surface);
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

		SDL_RenderCopy(resources->renderer, texture, NULL, &bounds);

		SDL_DestroyTexture(texture);
	}
}

