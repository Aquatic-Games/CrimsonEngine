using System.Diagnostics;
using System.Runtime.InteropServices;
using Crimson.Core;
using grabs.Graphics;
using grabs.ShaderCompiler;
using SDL3;

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
    
    public static void LoadGraphicsShader(IntPtr device, string name, out IntPtr? vertexShader, out IntPtr? pixelShader)
    {
        Logger.Trace($"Compiling shader '{name}'.");

        vertexShader = null;
        pixelShader = null;

        string fullPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Content", "Shaders", $"{name}.hlsl"));
        string includeDir = Path.GetDirectoryName(fullPath);
        string hlsl = File.ReadAllText(fullPath);
        
        string? vertexEntryPoint = null;
        string? pixelEntryPoint = null;

        uint vertexUniforms = 0;
        uint pixelUniforms = 0;
        uint vertexSamplers = 0;
        uint pixelSamplers = 0;

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
                    vertexUniforms = uint.Parse(splitLine[3]);
                    vertexSamplers = uint.Parse(splitLine[4]);
                    break;
                case "pixel":
                    pixelEntryPoint = splitLine[2];
                    pixelUniforms = uint.Parse(splitLine[3]);
                    pixelSamplers = uint.Parse(splitLine[4]);
                    break;
            }

            i = j;
        }
        
        SDL.GPUShaderFormat shaderFormat = SDL.GetGPUShaderFormats(device);

        if (vertexEntryPoint != null)
        {
            Logger.Trace("Creating vertex shader.");
            vertexShader = CreateShader(device, ShaderStage.Vertex, shaderFormat, hlsl, vertexEntryPoint, includeDir,
                true, vertexUniforms, vertexSamplers);
        }

        if (pixelEntryPoint != null)
        {
            Logger.Trace("Creating pixel shader.");
            pixelShader = CreateShader(device, ShaderStage.Pixel, shaderFormat, hlsl, pixelEntryPoint, includeDir, true,
                pixelUniforms, pixelSamplers);
        }
    }

    private static unsafe IntPtr CreateShader(IntPtr device, ShaderStage stage, SDL.GPUShaderFormat shaderFormat,
        string hlsl, string entryPoint, string? includeDir, bool debug, uint numUniforms, uint numSamplers)
    {
        ShaderFormat grabsFormat;

        if (shaderFormat.HasFlag(SDL.GPUShaderFormat.SPIRV))
            grabsFormat = ShaderFormat.Spirv;
        else if (shaderFormat.HasFlag(SDL.GPUShaderFormat.DXIL))
            grabsFormat = ShaderFormat.Dxil;
        else if (shaderFormat.HasFlag(SDL.GPUShaderFormat.DXBC))
            grabsFormat = ShaderFormat.Dxbc;
        else
            throw new NotSupportedException();

        SDL.GPUShaderStage sdlStage = stage switch
        {
            ShaderStage.Vertex => SDL.GPUShaderStage.Vertex,
            ShaderStage.Pixel => SDL.GPUShaderStage.Fragment,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        
        byte[] compiled = Compiler.CompileHlsl(stage, grabsFormat, hlsl, entryPoint, includeDir, debug);
            
        fixed (byte* pData = compiled)
        {
            SDL.GPUShaderCreateInfo shaderInfo = new()
            {
                Stage = sdlStage,
                Format = shaderFormat,
                Code = (nint) pData,
                CodeSize = (nuint) compiled.Length,
                NumUniformBuffers = numUniforms,
                NumSamplers = numSamplers,
                Entrypoint = entryPoint
            };

            Logger.Trace("Creating shader.");
            return SDL.CreateGPUShader(device, in shaderInfo).Check("Create GPU shader");
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