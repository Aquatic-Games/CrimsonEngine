using Crimson.Audio;
using Crimson.Engine.Entities;

namespace Crimson.Engine.Tests;

public class TestAudio : Scene
{
    public override void Initialize()
    {
        Audio.Audio.MasterVolume = 0.5f;
        Audio.Audio.SoundEffectsVolume = 7f;
        Audio.Audio.MusicVolume = 0.2f;
        
        /*StreamSound sound = new StreamSound("/home/aqua/Music/excite.ogg");
        sound.Play();*/
        Audio.Audio.FireStream("excite", type: AudioType.SoundEffect, persistent: true);
        App.SetScene(new TestDDS());
    }
}