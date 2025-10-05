using System.Runtime.InteropServices;
using Crimson.Graphics.RHI;
using Crimson.Graphics.RHI.OpenGL;
using Crimson.Math;
using SDL3;

if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
{
    Console.WriteLine($"Failed to initialize SDL: {SDL.GetError()}");
    return;
}

SDL.GLSetAttribute(SDL.GLAttr.ContextMajorVersion, 4);
SDL.GLSetAttribute(SDL.GLAttr.ContextMinorVersion, 3);
SDL.GLSetAttribute(SDL.GLAttr.ContextProfileMask, (int) SDL.GLProfile.Core);

const int width = 1280;
const int height = 720;
IntPtr window = SDL.CreateWindow("Crimson.Graphics.RHI.Tests", width, height, SDL.WindowFlags.OpenGL);

if (window == IntPtr.Zero)
{
    Console.WriteLine($"Failed to create window: {SDL.GetError()}");
    return;
}

IntPtr context = SDL.GLCreateContext(window);
SDL.GLMakeCurrent(window, context);

GraphicsDevice device = new GLGraphicsDevice(new GLContext(s => Marshal.GetFunctionPointerForDelegate(SDL.GLGetProcAddress(s)),
i =>
{
    SDL.GLSetSwapInterval(i);
    SDL.GLSwapWindow(window);
}));

bool running = true;
while (running)
{
    while (SDL.PollEvent(out SDL.Event winEvent))
    {
        switch ((SDL.EventType) winEvent.Type)
        {
            case SDL.EventType.WindowCloseRequested:
                running = false;
                break;
        }
    }
    
    device.BeginRendering(Color.CornflowerBlue);
    device.EndRendering();
    
    device.Present();
}

device.Dispose();
SDL.GLDestroyContext(context);
SDL.DestroyWindow(window);
SDL.Quit();