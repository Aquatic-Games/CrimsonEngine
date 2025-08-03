using MixrSharp;
using MixrSharp.Stream;

namespace Crimson.Audio;

public class StreamSound : IDisposable
{
    private readonly AudioStream _stream;
    private readonly AudioFormat _format;
    private readonly uint _bufferSizeInSamples;
    private readonly byte[] _buffer;
    
    private readonly AudioSource _source;
    
    private readonly AudioBuffer[] _buffers;
    private uint _currentBuffer;
    private uint _totalAudioBuffers;

    public ulong PositionInSamples
    {
        get => (_totalAudioBuffers * _bufferSizeInSamples) / _format.Channels + _source.Position;
        set => throw new NotImplementedException();
    }

    public double Position
    {
        get => PositionInSamples / (double) _format.SampleRate;
        set => throw new NotImplementedException();
    }
    
    public StreamSound(string path)
    {
        _stream = StreamUtils.CreateStreamFromFile(path);
        _format = _stream.Format;
        _bufferSizeInSamples = _format.SampleRate;
        
        _buffer = new byte[_bufferSizeInSamples * _format.BytesPerSample];
        _buffers = new AudioBuffer[2];
        
        Context context = Audio.Context;
        _source = context.CreateSource(new SourceDescription(SourceType.Pcm, _format));
        _source.BufferFinished += SourceOnBufferFinished;

        for (int i = 0; i < _buffers.Length; i++)
        {
            _stream.GetBuffer(_buffer);
            _buffers[i] = context.CreateBuffer(_buffer);
            _source.SubmitBuffer(_buffers[i]);
        }
    }

    public void Play(float volume = 1.0f, double speed = 1.0)
    {
        _source.Volume = volume;
        _source.Speed = speed;
        _source.Play();
    }

    public void Pause()
    {
        _source.Pause();
    }

    public void Stop()
    {
        _source.Stop();
    }
    
    public void Dispose()
    {
        foreach (AudioBuffer buffer in _buffers)
            buffer.Dispose();
        
        _source.Dispose();
        _stream.Dispose();
    }
    
    private void SourceOnBufferFinished()
    {
        _totalAudioBuffers++;
        _stream.GetBuffer(_buffer);
        _buffers[_currentBuffer].Update(_buffer);
        _source.SubmitBuffer(_buffers[_currentBuffer]);

        _currentBuffer++;
        if (_currentBuffer >= _buffers.Length)
            _currentBuffer = 0;
    }
}