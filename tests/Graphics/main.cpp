// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.

#include <Crimson/Platform/Surface.h>
#include <Crimson/Graphics/Renderer.h>

#include <SDL3/SDL.h>

using namespace Crimson;

int main(int argc, const char* argv[])
{
    const SurfaceInfo info
    {
        .Size = { 1280, 720 },
        .Title = "Graphics Tests"
    };

    Surface::Create(info);
    Renderer::Create();

    bool alive = true;
    while (alive)
    {
        SDL_Event event;
        while (SDL_PollEvent(&event))
        {
            switch (event.type)
            {
                case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    alive = false;
                    break;
            }
        }

        Renderer::Present();
    }

    Renderer::Destroy();
    Surface::Destroy();

    return 0;
}