namespace Crimson.Graphics.RHI.OpenGL;

public class GLContext
{
    public Func<string, nint> GetProcAddressFunc;

    public Action<int> PresentFunc;

    public GLContext(Func<string, IntPtr> getProcAddressFunc, Action<int> presentFunc)
    {
        GetProcAddressFunc = getProcAddressFunc;
        PresentFunc = presentFunc;
    }
}