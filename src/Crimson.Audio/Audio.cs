using MixrSharp;
using MixrSharp.Devices;

namespace Crimson.Audio;

public static unsafe class Audio
{
    private static Device _device;

    internal static Context Context;
    
    public static void Create()
    {
        _device = new SdlDevice(44100);
        Context = _device.Context;
    }

    public static void Destroy()
    {
        _device.Dispose();
    }
}