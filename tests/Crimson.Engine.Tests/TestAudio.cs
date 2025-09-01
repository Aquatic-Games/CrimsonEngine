using Crimson.Audio;
using Crimson.Engine.Entities;

namespace Crimson.Engine.Tests;

public class TestAudio : Scene
{
    public override void Initialize()
    {
        /*StreamSound sound = new StreamSound("/home/aqua/Music/excite.ogg");
        sound.Play();*/
        Audio.Audio.FireStream("/home/aqua/Music/excite.ogg", speed: 2);
    }
}