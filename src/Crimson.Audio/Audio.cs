using Silk.NET.OpenAL;

namespace Crimson.Audio;

public static unsafe class Audio
{
    private static ALContext _alc;
    private static Device* _device;
    private static Context* _context;
    
    internal static AL AL;
    
    public static void Create()
    {
        _alc = ALContext.GetApi(true);
        _device = _alc.OpenDevice(null);
        _context = _alc.CreateContext(_device, null);
        _alc.MakeContextCurrent(_context);
        AL = AL.GetApi(true);
    }

    public static void Destroy()
    {
        AL.Dispose();
        _alc.DestroyContext(_context);
        _alc.CloseDevice(_device);
        _alc.Dispose();
    }

    public static StreamSound FireStream(string name, float volume = 1.0f, double speed = 1.0)
    {
        StreamSound sound = new StreamSound(name);
        sound.Finished += () => sound.Dispose();
        sound.Play(volume, speed);
        return sound;
    }
}