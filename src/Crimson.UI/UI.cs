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

    private static List<Window> _windows;
    private static List<Window> _windowsToClose;
    
    public static Control BaseControl = null!;

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

        _windows = [];
        _windowsToClose = [];
        
        Theme = Theme.Light;
        // The content manager doesn't have the ability to load persistent resources yet so we have to manually do what
        // the content manager does.
        Theme.Font = options.DefaultFont != null
            ? Font.LoadResource(Content.Content.GetFullyQualifiedName(options.DefaultFont), Path.HasExtension(options.DefaultFont))
            : new Font(Resources.LoadEmbeddedResource("Crimson.UI.Roboto-Regular.ttf",
                Assembly.GetExecutingAssembly()));

        Clear();
    }

    public static void Destroy()
    {
        Theme.Font.Dispose();
    }

    public static void Clear()
    {
        foreach (Window window in _windows)
            window.Close();
        
        _windows.Clear();
        
        BaseControl = new AnchorLayout();
        BaseControl.CalculateLayout(_screenRegion, Scale);
    }

    public static void Update(float dt)
    {
        Vector2T<float> mPos = Input.Input.MousePosition;
        Vector2T<int> mousePos = new Vector2T<int>((int) mPos.X, (int) mPos.Y);
        bool mouseCaptured = false;
        BaseControl.Update(dt, ref mouseCaptured, mousePos);
        
        foreach (Window window in _windows)
            window.Update(dt);

        foreach (Window window in _windowsToClose)
        {
            window.Close();
            _windows.Remove(window);
        }
        _windowsToClose.Clear();
    }

    public static void Draw()
    {
        BaseControl.Draw();
        
        foreach (Window window in _windows)
            window.Draw();
    }

    public static void Resize(Rectangle<int> screenRegion)
    {
        _screenRegion = screenRegion;
        _scaleDirty = true;
        BaseControl.CalculateLayout(_screenRegion, Scale);
    }

    public static void OpenWindow(Window window)
    {
        _windows.Add(window);
        window.Initialize();
    }

    public static void CloseWindow(Window window)
    {
        _windowsToClose.Add(window);
    }
}