using Euphoria.Engine.Launch;
using SDL;
using static SDL.SDL_InitFlags;
using static SDL.SDL3;

namespace Euphoria.Engine;

public sealed class Window : IDisposable
{
    private readonly unsafe SDL_Window* _window;
    
    public Window(in WindowInfo info)
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS))
            throw new Exception($"Failed to initialize SDL: {SDL_GetError()}");
        
        
    }

    public void Dispose()
    {
        
    }
}