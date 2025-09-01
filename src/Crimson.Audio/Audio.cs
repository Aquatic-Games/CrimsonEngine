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

    /// <summary>
    /// Load and play a Stream Sound once.
    /// </summary>
    /// <param name="name">The name of the sound. This is loaded through the <see cref="Content.Content"/> manager, so
    /// the same rules apply.</param>
    /// <param name="volume">The volume of the sound.</param>
    /// <param name="speed">The speed and pitch of the sound.</param>
    /// <param name="persistent">Whether the sound should continue playing, even on scene change.</param>
    /// <returns>A <see cref="StreamSound"/> instance. You should not normally need to use or store this value.</returns>
    /// <remarks>When using this method, the audio manager will handle disposing of the sound automatically. There is no
    /// need to manually call dispose, although you can.</remarks>
    public static StreamSound FireStream(string name, float volume = 1.0f, double speed = 1.0, bool persistent = false)
    {
        StreamSound sound = Content.Content.Load<StreamSound>(name, persistent);
        sound.Finished += () => sound.Dispose();
        sound.Play(volume, speed);
        return sound;
    }
}