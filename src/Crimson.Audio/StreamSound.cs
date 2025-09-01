using Silk.NET.OpenAL;
using StbVorbisSharp;

namespace Crimson.Audio;

public class StreamSound : IDisposable
{
    public event OnFinished Finished = delegate { };
    
    private readonly Vorbis _vorbis;
    private readonly uint _source;
    private Timer? _callbackTimer;
    
    private readonly uint[] _buffers;
    private uint _currentBuffer;

    /*public ulong PositionInSamples
    {
        get => (_totalAudioBuffers * _bufferSizeInSamples) / _vorbis.Channels + _source.Position;
        set => throw new NotImplementedException();
    }

    public double Position
    {
        get => PositionInSamples / (double) _format.SampleRate;
        set => throw new NotImplementedException();
    }*/
    
    public StreamSound(string path)
    {
        AL al = Audio.AL;
        _vorbis = Vorbis.FromMemory(File.ReadAllBytes(path));
        
        _buffers = new uint[2];
        for (int i = 0; i < _buffers.Length; i++)
        {
            _buffers[i] = al.GenBuffer();
            _vorbis.SubmitBuffer();
            al.BufferData(_buffers[i], _vorbis.Channels == 2 ? BufferFormat.Stereo16 : BufferFormat.Mono16,
                _vorbis.SongBuffer, _vorbis.SampleRate);
        }
        
        _source = al.GenSource();
        al.SourceQueueBuffers(_source, _buffers);
    }

    public void Play(float volume = 1.0f, double speed = 1.0)
    {
        AL al = Audio.AL;
        
        // Set the period to 1/4th the buffer size, to give ample room for regenerating buffers.
        _callbackTimer = new Timer(SourceOnBufferFinished, null, 0,
            (_vorbis.SongBuffer.Length * 1000) / _vorbis.Channels / _vorbis.SampleRate / 4);
        
        al.SetSourceProperty(_source, SourceFloat.Gain, volume);
        al.SetSourceProperty(_source, SourceFloat.Pitch, (float) speed);
        al.SourcePlay(_source);
    }

    public void Pause()
    {
        Audio.AL.SourcePause(_source);
    }

    public void Stop()
    {
        _callbackTimer?.Dispose();
        Audio.AL.SourceStop(_source);
        Finished();
    }
    
    public void Dispose()
    {
        AL al = Audio.AL;
        Finished = delegate { };
        _callbackTimer?.Dispose();
        al.DeleteBuffers(_buffers);
        _vorbis.Dispose();
    }
    
    private unsafe void SourceOnBufferFinished(object? obj)
    {
        AL al = Audio.AL;

        al.GetSourceProperty(_source, GetSourceInteger.SourceState, out int state);
        if (state == (int) SourceState.Stopped)
        {
            Stop();
            return;
        }

        al.GetSourceProperty(_source, GetSourceInteger.BuffersProcessed, out int processed);

        for (int i = 0; i < processed; i++)
        {
            uint buffer = _buffers[_currentBuffer];
            al.SourceUnqueueBuffers(_source, 1, &buffer);
            
            _vorbis.SubmitBuffer();
            short[] songBuffer = _vorbis.SongBuffer;
            
            if (_vorbis.Decoded < _vorbis.SongBuffer.Length / _vorbis.Channels)
            {
                if (_vorbis.Decoded == 0)
                    return;

                songBuffer = _vorbis.SongBuffer[..(_vorbis.Decoded * _vorbis.Channels)];
            }

            al.BufferData(buffer, _vorbis.Channels == 2 ? BufferFormat.Stereo16 : BufferFormat.Mono16, songBuffer,
                _vorbis.SampleRate);
            
            al.SourceQueueBuffers(_source, 1, &buffer);

            _currentBuffer++;
            if (_currentBuffer >= _buffers.Length)
                _currentBuffer = 0;
        }
    }

    public delegate void OnFinished();
}