using MixrSharp.Stream;

namespace Crimson.Audio;

internal static class StreamUtils
{
    public static AudioStream CreateStreamFromFile(string path)
    {
        string extension = Path.GetExtension(path);

        // Quite basic. New mixr will probably have this built in.
        return extension switch
        {
            ".wav" => new Wav(path),
            ".ogg" => new Vorbis(path),
            ".mp3" => new Mp3(path),
            ".flac" => new Flac(path),
            _ => throw new NotSupportedException(extension)
        };
    }
}