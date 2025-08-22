// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.

#include "Crimson/Platform/Surface.h"

#include <SDL3/SDL.h>
#include <stdexcept>
#include <format>

namespace Crimson::Platform
{
    static SDL_Window* _window;

    void Surface::Create()
    {
        if (!SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS))
            throw std::runtime_error(std::format("Failed to initialize SDL: {}", SDL_GetError()));

        _window = SDL_CreateWindow("Crimson Engine", 1280, 720, 0);
        if (!_window)
            throw std::runtime_error(std::format("Failed to create window: {}", SDL_GetError()));
    }

    void Surface::Destroy()
    {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}
