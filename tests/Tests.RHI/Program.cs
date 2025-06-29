using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.RHI;
using Crimson.Graphics.RHI.D3D11;
using Crimson.Graphics.RHI.Vulkan;
using Crimson.Math;
using Crimson.Platform;
using grabs.Graphics;
using grabs.ShaderCompiler;
using Buffer = Crimson.Graphics.RHI.Buffer;
using BufferUsage = Crimson.Graphics.RHI.BufferUsage;
using ColorAttachmentInfo = Crimson.Graphics.RHI.ColorAttachmentInfo;
using CommandList = Crimson.Graphics.RHI.CommandList;
using Device = Crimson.Graphics.RHI.Device;
using Format = Crimson.Graphics.RHI.Format;
using GraphicsPipelineInfo = Crimson.Graphics.RHI.GraphicsPipelineInfo;
using InputElementDescription = Crimson.Graphics.RHI.InputElementDescription;
using Pipeline = Crimson.Graphics.RHI.Pipeline;
using ShaderModule = Crimson.Graphics.RHI.ShaderModule;
using ShaderStage = Crimson.Graphics.RHI.ShaderStage;
using Surface = Crimson.Platform.Surface;
using Texture = Crimson.Graphics.RHI.Texture;

const string Shader = """
                      struct VSOutput
                      {
                          float4 Position: SV_Position;
                      };

                      struct PSOutput
                      {
                          float4 Color: SV_Target0;
                      };

                      VSOutput VSMain(const in uint vertex: SV_VertexID)
                      {
                          float2 vertices[] =
                          {
                              float2(-0.5, -0.5),
                              float2(+0.5, -0.5),
                              float2(+0.5, +0.5),
                              float2(-0.5, +0.5)
                          };
                          
                          uint indices[] =
                          {
                              0, 1, 3,
                              1, 2, 3
                          };

                          VSOutput output;
                          
                          output.Position = float4(vertices[indices[vertex]], 0.0, 1.0);
                          
                          return output;
                      }

                      PSOutput PSMain()
                      {
                          PSOutput output;
                          
                          output.Color = float4(1.0, 0.5, 0.25, 1.0);
                          
                          return output;
                      }
                      """;

/*const string Shader = """
                      struct VSInput
                      {
                          float2 Position: POSITION0;
                          float3 Color:    COLOR0;
                      };
                      
                      struct VSOutput
                      {
                          float4 Position: SV_Position;
                          float3 Color:    COLOR0;
                      };

                      struct PSOutput
                      {
                          float4 Color: SV_Target0;
                      };

                      VSOutput VSMain(const in VSInput input)
                      {
                          VSOutput output;
                          
                          output.Position = float4(input.Position, 0.0, 1.0);
                          output.Color = input.Color;
                          
                          return output;
                      }

                      PSOutput PSMain(const in VSOutput input)
                      {
                          PSOutput output;
                          
                          output.Color = float4(input.Color, 1.0);
                          
                          return output;
                      }
                      """;*/

Logger.EnableConsole();

WindowOptions options = new()
{
    Title = "Tests.RHI (Vulkan)",
    //Title = "Tests.RHI (D3D11)",
    Size = new Size<int>(1280, 720),
    Resizable = true
};

bool alive = true;

Events.Create();
Events.WindowClose += () => alive = false;

Surface.Create(in options);

Device device = new VulkanDevice("Tests.RHI", Surface.Info.Handle, options.Size.As<uint>(), true);
//Device device = new D3D11Device(Surface.Info.Handle, options.Size.As<uint>(), true);
Events.SurfaceSizeChanged += size => device.Resize(size.As<uint>());
CommandList cl = device.CreateCommandList();

ReadOnlySpan<float> vertices =
[
    -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,
    +0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
    +0.5f, +0.5f, 0.0f, 0.0f, 1.0f,
    -0.5f, +0.5f, 0.0f, 0.0f, 0.0f
];

ReadOnlySpan<uint> indices =
[
    0, 1, 3,
    1, 2, 3
];

uint verticesSize = (uint) (vertices.Length * sizeof(float));
uint indicesSize = (uint) (indices.Length * sizeof(uint));

Buffer vertexBuffer = device.CreateBuffer(BufferUsage.VertexBuffer | BufferUsage.TransferDst, verticesSize);
Buffer indexBuffer = device.CreateBuffer(BufferUsage.IndexBuffer | BufferUsage.TransferDst, indicesSize);

Buffer transferBuffer = device.CreateBuffer(BufferUsage.TransferSrc, verticesSize + indicesSize);
nint mapBuffer = device.MapBuffer(transferBuffer);
unsafe
{
    fixed (float* pVertices = vertices)
        Unsafe.CopyBlock((byte*) mapBuffer, pVertices, verticesSize);
    fixed (uint* pIndices = indices)
        Unsafe.CopyBlock((byte*) mapBuffer + verticesSize, pIndices, indicesSize);
}
device.UnmapBuffer(transferBuffer);

cl.Begin();
cl.CopyBufferToBuffer(transferBuffer, 0, vertexBuffer, 0, verticesSize);
cl.CopyBufferToBuffer(transferBuffer, verticesSize, indexBuffer, 0, indicesSize);
cl.End();
device.ExecuteCommandList(cl);

transferBuffer.Dispose();

ShaderModule vertexShader = device.CreateShaderModule(ShaderStage.Vertex,
    Compiler.CompileHlsl(grabs.Graphics.ShaderStage.Vertex, ShaderFormat.Spirv, Shader, "VSMain"), "VSMain");
ShaderModule pixelShader = device.CreateShaderModule(ShaderStage.Pixel,
    Compiler.CompileHlsl(grabs.Graphics.ShaderStage.Pixel, ShaderFormat.Spirv, Shader, "PSMain"), "PSMain");

Pipeline pipeline = device.CreateGraphicsPipeline(new GraphicsPipelineInfo()
{
    VertexShader = vertexShader,
    PixelShader = pixelShader,
    ColorTargets = [device.SwapchainFormat],
    /*InputLayout =
    [
        new InputElementDescription(Format.R32G32_Float, 0, 0, 0),
        new InputElementDescription(Format.R32G32B32_Float, 8, 1, 0)
    ]*/
});

pixelShader.Dispose();
vertexShader.Dispose();

while (alive)
{
    Events.ProcessEvents();

    Texture texture = device.GetNextSwapchainTexture();
    
    cl.Begin();
    
    cl.BeginRenderPass([new ColorAttachmentInfo(texture, Color.CornflowerBlue)]);
    
    cl.SetGraphicsPipeline(pipeline);
    //cl.SetVertexBuffer(0, vertexBuffer, 5 * sizeof(float));
    //cl.SetIndexBuffer(indexBuffer, Format.R32_UInt);
    
    cl.Draw(6);
    //cl.DrawIndexed(6);
    
    cl.EndRenderPass();
    
    cl.End();
    
    device.ExecuteCommandList(cl);
    device.Present();
}

pipeline.Dispose();
indexBuffer.Dispose();
vertexBuffer.Dispose();
cl.Dispose();
device.Dispose();
Surface.Destroy();
Events.Destroy();