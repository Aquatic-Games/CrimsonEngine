using Silk.NET.OpenAL;

namespace Crimson.Audio;

public static unsafe class Audio
{
    private static ALContext _alc;

    private static Device* _device;
    private static Context* _context;
    
    internal static AL Al;
    
    public static void Create()
    {
        Al = AL.GetApi(true);
        _alc = ALContext.GetApi(true);

        _device = _alc.OpenDevice(null);
        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
    }

    public static void Destroy()
    {
        _alc.DestroyContext(_context);
        _alc.CloseDevice(_device);
        _alc.Dispose();
        Al.Dispose();
    }
}