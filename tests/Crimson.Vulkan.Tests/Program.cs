using Crimson.Core;
using Crimson.Graphics.Vulkan;
using SDL3;

Logger.LogToConsole = true;

if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
{
    Console.WriteLine($"Failed to initialize SDL: {SDL.GetError()}");
    return;
}

IntPtr window = SDL.CreateWindow("Crimson.Vulkan.Tests", 1280, 720, SDL.WindowFlags.Vulkan);
if (window == IntPtr.Zero)
{
    Console.WriteLine($"Failed to create window: {SDL.GetError()}");
    return;
}

GraphicsDevice device = new GraphicsDevice("Crimson.Vulkan.Tests", window);

device.Dispose();

SDL.DestroyWindow(window);
SDL.Quit();