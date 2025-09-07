using Crimson.Core;
using Graphite;
using Graphite.ShaderTools;

namespace Crimson.Graphics.Utils;

internal static class ShaderUtils
{
    /*public static void LoadGraphicsShader(Device device, string name, out ShaderModule? vertex, out ShaderModule? pixel)
    {
        Logger.Trace($"Compiling shader '{name}'.");
        
        vertex = null;
        pixel = null;
        
        string hlsl = File.ReadAllText(Path.Combine("Shaders", $"{name}.hlsl"));

        string? vertexEntryPoint = null;
        string? pixelEntryPoint = null;

        int i = 0;
        while ((i = hlsl.IndexOf("#pragma", i)) >= 0)
        {
            int j = hlsl.IndexOf('\n', i);

            string line = hlsl[i..j];

            // Yes i know this is inefficient but it works so I don't care
            string[] splitLine = line.Split(' ');

            switch (splitLine[1])
            {
                case "vertex":
                    vertexEntryPoint = splitLine[2];
                    break;
                case "pixel":
                    pixelEntryPoint = splitLine[2];
                    break;
            }

            i = j;
        }

        if (vertexEntryPoint != null)
        {
            Logger.Trace("    Compiling vertex...");
            byte[] spirv = Compiler.CompileHlsl(ShaderStage.Vertex, hlsl, vertexEntryPoint);
            vertex = device.CreateShaderModule(ShaderStage.Vertex, spirv, vertexEntryPoint);
        }

        if (pixelEntryPoint != null)
        {
            Logger.Trace("    Compiling pixel...");
            byte[] spirv = Compiler.CompileHlsl(ShaderStage.Pixel, hlsl, pixelEntryPoint);
            pixel = device.CreateShaderModule(ShaderStage.Pixel, spirv, pixelEntryPoint);
        }
    }*/

    public static void LoadGraphicsShader(Device device, string name, out ShaderModule? vertexShader,
        out ShaderModule? pixelShader)
    {
        Logger.Trace($"Compiling shader '{name}'.");

        vertexShader = null;
        pixelShader = null;

        string fullPath = Path.GetFullPath(Path.Combine("Content", "Shaders", $"{name}.hlsl"));
        string includeDir = Path.GetDirectoryName(fullPath);
        string hlsl = File.ReadAllText(fullPath);

        string? vertexEntryPoint = null;
        string? pixelEntryPoint = null;

        int i = 0;
        while ((i = hlsl.IndexOf("#pragma", i)) >= 0)
        {
            int j = hlsl.IndexOf('\n', i);

            string line = hlsl[i..j];

            // Yes i know this is inefficient but it works so I don't care
            string[] splitLine = line.Split(' ');

            switch (splitLine[1])
            {
                case "vertex":
                    vertexEntryPoint = splitLine[2].TrimEnd();
                    break;
                case "pixel":
                    pixelEntryPoint = splitLine[2].TrimEnd();
                    break;
            }

            i = j;
        }

        if (vertexEntryPoint != null)
        {
            Logger.Trace("Creating vertex shader.");
            vertexShader =
                device.CreateShaderModuleFromHLSL(ShaderStage.Vertex, hlsl, vertexEntryPoint, includeDir, true);
        }

        if (pixelEntryPoint != null)
        {
            Logger.Trace("Creating pixel shader.");
            pixelShader = device.CreateShaderModuleFromHLSL(ShaderStage.Pixel, hlsl, pixelEntryPoint, includeDir, true);
        }
    }

    /*public static unsafe IntPtr LoadGraphicsShader(IntPtr device, SDL.GPUShaderStage stage, string name, string entryPoint, uint numUniforms, uint numSamplers)
    {
        string basePath = Path.Combine("Shaders", $"{name}");

        string path = stage switch
        {
            SDL.GPUShaderStage.Vertex => basePath + "_v",
            SDL.GPUShaderStage.Fragment => basePath + "_p",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        SDL.GPUShaderFormat shaderFormat = SDL.GetGPUShaderFormats(device);

        if (shaderFormat.HasFlag(SDL.GPUShaderFormat.DXBC))
            path += ".dxil";
        else if (shaderFormat.HasFlag(SDL.GPUShaderFormat.SPIRV))
            path += ".spv";
        else
            throw new NotSupportedException(shaderFormat.ToString());

        byte[] data = File.ReadAllBytes(path);

        fixed (byte* pData = data)
        {
            SDL.GPUShaderCreateInfo shaderInfo = new()
            {
                Stage = stage,
                Format = shaderFormat,
                Code = (nint) pData,
                CodeSize = (nuint) data.Length,
                NumUniformBuffers = numUniforms,
                NumSamplers = numSamplers,
                Entrypoint = Marshal.StringToCoTaskMemAnsi(entryPoint)
            };

            Logger.Trace("Creating shader.");
            return SDL.CreateGPUShader(device, in shaderInfo).Check("Create GPU shader");
        }
    }*/
}