using System.Numerics;
using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Input.Bindings;

public struct KeyBinding : IInputBinding
{
    public Key Key;

    public bool Enabled { get; set; }

    public InputSource Source => InputSource.Relative;
    
    public bool Active => Enabled && Input.IsKeyDown(Key);

    public float Value => Active ? 1 : 0;

    public Vector2T<float> Value2D => Active ? Vector2T<float>.One : Vector2T<float>.Zero;

    public KeyBinding(Key key)
    {
        Key = key;
    }
}