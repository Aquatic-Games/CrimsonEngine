using System.Numerics;
using System.Reflection;
using Crimson.Content;
using Crimson.Graphics;
using Crimson.Math;
using Crimson.Platform;
using Crimson.UI.Controls;
using Crimson.UI.Controls.Layouts;

namespace Crimson.UI;

public static class UI
{
    private static Rectangle<int> _screenRegion;
    private static ScaleMethod _scaleMethod;
    private static Size<int> _referenceSize;
    private static bool _scaleDirty;
    private static float _calculatedSize;
    
    public static Control? Control;

    public static Theme Theme;

    public static float ScaleMultiplier;

    public static ScaleMethod ScaleMethod
    {
        get => _scaleMethod;
        set
        {
            _scaleMethod = value;
            _scaleDirty = true;
        }
    }

    public static Size<int> ReferenceSize
    {
        get => _referenceSize;
        set
        {
            _referenceSize = value;
            _scaleDirty = true;
        }
    }

    public static float Scale
    {
        get
        {
            if (_scaleDirty)
            {
                _scaleDirty = false;
                
                switch (ScaleMethod)
                {
                    case ScaleMethod.Manual:
                        _calculatedSize = 1.0f;
                        break;
                    case ScaleMethod.ReferenceSize:
                    {
                        Size<int> surfaceSize = Surface.Size;
                        _calculatedSize = (surfaceSize.As<float>().Height / ReferenceSize.As<float>().Height);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return _calculatedSize * ScaleMultiplier;
        }
    }

    public static void Create(Rectangle<int> screenRegion, UIOptions options)
    {
        _screenRegion = screenRegion;
        _scaleDirty = true;
        ScaleMultiplier = 1.0f;
        ScaleMethod = ScaleMethod.Manual;
        ReferenceSize = new Size<int>(1280, 720);
        
        Theme = Theme.Light;
        // The content manager doesn't have the ability to load persistent resources yet so we have to manually do what
        // the content manager does.
        Theme.Font = options.DefaultFont != null
            ? Font.LoadResource(Content.Content.GetFullyQualifiedName(options.DefaultFont), Path.HasExtension(options.DefaultFont))
            : new Font(Resources.LoadEmbeddedResource("Crimson.UI.Roboto-Regular.ttf",
                Assembly.GetExecutingAssembly()));
    }

    public static void Destroy()
    {
        Theme.Font.Dispose();
    }

    public static void Clear()
    {
        Control = null;
    }

    public static void Update(float dt)
    {
        Vector2T<int> mPos = Input.Input.MousePosition.As<int>();
        bool mouseCaptured = false;
        Control?.Update(dt, ref mouseCaptured, mPos);
    }

    public static void Draw()
    {
        Control?.Draw();
    }

    public static void Resize(Rectangle<int> screenRegion)
    {
        _screenRegion = screenRegion;
        _scaleDirty = true;
        Control?.CalculateLayout(_screenRegion, Scale);
    }
}