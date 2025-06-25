using Crimson.Core;
using Crimson.Graphics.RHI.Vulkan;
using Crimson.Math;
using Crimson.Platform;
using grabs.Graphics;
using grabs.ShaderCompiler;
using ColorAttachmentInfo = Crimson.Graphics.RHI.ColorAttachmentInfo;
using CommandList = Crimson.Graphics.RHI.CommandList;
using Device = Crimson.Graphics.RHI.Device;
using GraphicsPipelineInfo = Crimson.Graphics.RHI.GraphicsPipelineInfo;
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

                          VSOutput output;
                          
                          output.Position = float4(vertices[vertex], 0.0, 1.0);
                          
                          return output;
                      }

                      PSOutput PSMain()
                      {
                          PSOutput output;
                          
                          output.Color = float4(1.0, 0.5, 0.25, 1.0);
                          
                          return output;
                      }
                      """;

Logger.EnableConsole();

WindowOptions options = new()
{
    Title = "Tests.RHI (Vulkan)",
    Size = new Size<int>(1280, 720),
    Resizable = true
};

bool alive = true;

Events.Create();
Events.WindowClose += () => alive = false;

Surface.Create(in options);

Device device = new VulkanDevice("Tests.RHI", Surface.Info.Handle, true);
CommandList cl = device.CreateCommandList();

ShaderModule vertexShader = device.CreateShaderModule(ShaderStage.Vertex,
    Compiler.CompileHlsl(grabs.Graphics.ShaderStage.Vertex, ShaderFormat.Spirv, Shader, "VSMain"), "VSMain");
ShaderModule pixelShader = device.CreateShaderModule(ShaderStage.Pixel,
    Compiler.CompileHlsl(grabs.Graphics.ShaderStage.Pixel, ShaderFormat.Spirv, Shader, "PSMain"), "PSMain");

Pipeline pipeline =
    device.CreateGraphicsPipeline(new GraphicsPipelineInfo(vertexShader, pixelShader, [device.SwapchainFormat]));

pixelShader.Dispose();
vertexShader.Dispose();

while (alive)
{
    Events.ProcessEvents();

    Texture texture = device.GetNextSwapchainTexture();
    
    cl.Begin();
    
    cl.BeginRenderPass([new ColorAttachmentInfo(texture, Color.CornflowerBlue)]);
    cl.EndRenderPass();
    
    cl.End();
    
    device.ExecuteCommandList(cl);
    device.Present();
}

pipeline.Dispose();
cl.Dispose();
device.Dispose();
Surface.Destroy();
Events.Destroy();