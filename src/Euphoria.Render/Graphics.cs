using Euphoria.Core;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.Vulkan;

namespace Euphoria.Render;

public static class Graphics
{
    private static Surface _surface;
    private static Swapchain _swapchain;
    
    internal static Instance Instance;
    internal static Device Device;
    internal static CommandList CommandList;
    
    public static void Create(in SurfaceInfo info)
    {
        GrabsLog.LogMessage += (severity, type, message, file, line) =>
        {
            Logger.Severity eSeverity = severity switch
            {
                GrabsLog.Severity.Verbose => Logger.Severity.Trace,
                GrabsLog.Severity.Info => Logger.Severity.Debug,
                GrabsLog.Severity.Warning => Logger.Severity.Warning,
                GrabsLog.Severity.Error => Logger.Severity.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };

            Logger.Log(eSeverity, message, line, file);
        };
        
        // TODO: Enable D3D11 when it actually works properly
        //if (OperatingSystem.IsWindows())
        //    Instance.RegisterBackend<D3D11Backend>();
        Instance.RegisterBackend<VulkanBackend>();

        InstanceInfo instanceInfo = new InstanceInfo("Euphoria", true);

        Instance = Instance.Create(in instanceInfo);
    }

    public static void Destroy()
    {
        
    }
}