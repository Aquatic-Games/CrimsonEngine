using System.Numerics;
using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Input.Bindings;

public struct KeyBinding : IInputBinding
{
    public Key Key;

    public bool Enabled { get; set; }
    
    public bool Pressed => Enabled && Input.IsKeyDown(Key);

    public float Value => Pressed ? 1 : 0;

    public Vector2T<float> Value2D => Pressed ? Vector2T<float>.One : Vector2T<float>.Zero;
}