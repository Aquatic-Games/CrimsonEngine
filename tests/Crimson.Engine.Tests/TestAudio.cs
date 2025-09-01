using Crimson.Audio;
using Crimson.Engine.Entities;

namespace Crimson.Engine.Tests;

public class TestAudio : Scene
{
    public override void Initialize()
    {
        /*StreamSound sound = new StreamSound("/home/aqua/Music/excite.ogg");
        sound.Play();*/
        Audio.Audio.FireStream("excite", persistent: true);
        App.SetScene(new TestDDS());
    }
}