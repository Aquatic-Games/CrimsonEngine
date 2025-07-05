using Silk.NET.OpenAL;

namespace Crimson.Audio;

public class StreamSound : IDisposable
{
    private readonly uint[] _buffers;
    private uint _currentBuffer;
    
    private readonly uint _source;
    
    public StreamSound(string path)
    {
        AL al = Audio.Al;

        _source = al.GenSource();
    }
    
    public void Dispose()
    {
        
    }
}