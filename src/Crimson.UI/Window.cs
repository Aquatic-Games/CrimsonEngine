namespace Crimson.UI;

public abstract class Window : IDisposable
{
    private bool _isClosing;

    public virtual void Initialize()
    {
        _isClosing = false;
    }

    public virtual void Update(float dt) { }

    public virtual void Draw() { }

    public virtual void Dispose() { }

    public virtual void OnClose() { }

    public void Close()
    {
        if (_isClosing)
            return;
        
        _isClosing = true;
        UI.CloseWindow(this);
        OnClose();
    }
}