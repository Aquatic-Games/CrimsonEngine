global using VkPipeline = Silk.NET.Vulkan.Pipeline;
using Crimson.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal sealed unsafe class VulkanPipeline : Pipeline
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public readonly PipelineLayout Layout;
    public readonly VkPipeline Pipeline;

    public VulkanPipeline(Vk vk, VkDevice device, in GraphicsPipelineInfo info)
    {
        _vk = vk;
        _device = device;

        PipelineLayoutCreateInfo layoutInfo = new()
        {
            SType = StructureType.PipelineLayoutCreateInfo
        };
        
        Logger.Trace("Creating pipeline layout.");
        _vk.CreatePipelineLayout(_device, &layoutInfo, null, out Layout).Check("Creating pipeline layout");

        VulkanShaderModule vertexShader = (VulkanShaderModule) info.VertexShader;
        nint vertexShaderEntryPoint = SilkMarshal.StringToPtr(vertexShader.EntryPoint);
        
        VulkanShaderModule pixelShader = (VulkanShaderModule) info.PixelShader;
        nint pixelShaderEntryPoint = SilkMarshal.StringToPtr(pixelShader.EntryPoint);

        PipelineShaderStageCreateInfo* stages = stackalloc PipelineShaderStageCreateInfo[]
        {
            new PipelineShaderStageCreateInfo
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.VertexBit,
                Module = vertexShader.Module,
                PName = (byte*) vertexShaderEntryPoint
            },
            new PipelineShaderStageCreateInfo
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.FragmentBit,
                Module = pixelShader.Module,
                PName = (byte*) pixelShaderEntryPoint
            }
        };

        PipelineVertexInputStateCreateInfo vertexInputState = new()
        {
            SType = StructureType.PipelineVertexInputStateCreateInfo
        };

        PipelineInputAssemblyStateCreateInfo inputAssemblyState = new()
        {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            Topology = PrimitiveTopology.TriangleList
        };

        PipelineViewportStateCreateInfo viewportState = new()
        {
            SType = StructureType.PipelineViewportStateCreateInfo,
            ViewportCount = 1,
            ScissorCount = 1
        };

        PipelineRasterizationStateCreateInfo rasterizationState = new()
        {
            SType = StructureType.PipelineRasterizationStateCreateInfo,
            CullMode = CullModeFlags.None,
            LineWidth = 1
        };

        PipelineMultisampleStateCreateInfo multisampleState = new()
        {
            SType = StructureType.PipelineMultisampleStateCreateInfo,
            RasterizationSamples = SampleCountFlags.Count1Bit
        };

        PipelineDepthStencilStateCreateInfo depthStencilState = new()
        {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,
        };
        
        DynamicState* pStates = stackalloc DynamicState[]
        {
            DynamicState.Viewport,
            DynamicState.Scissor,
            DynamicState.VertexInputBindingStride
        };

        PipelineDynamicStateCreateInfo dynamicState = new()
        {
            SType = StructureType.PipelineDynamicStateCreateInfo,
            DynamicStateCount = 3,
            PDynamicStates = pStates
        };

        PipelineColorBlendAttachmentState* blendAttachments =
            stackalloc PipelineColorBlendAttachmentState[info.ColorTargets.Length];
        VkFormat* colorAttachmentFormats = stackalloc VkFormat[info.ColorTargets.Length];

        for (int i = 0; i < info.ColorTargets.Length; i++)
        {
            blendAttachments[i] = new PipelineColorBlendAttachmentState()
            {
                BlendEnable = false
            };
            
            colorAttachmentFormats[i] = info.ColorTargets[i].ToVk();
        }

        PipelineColorBlendStateCreateInfo blendState = new()
        {
            SType = StructureType.PipelineColorBlendStateCreateInfo,
            AttachmentCount = (uint) info.ColorTargets.Length,
            PAttachments = blendAttachments
        };

        PipelineRenderingCreateInfo renderingInfo = new()
        {
            SType = StructureType.PipelineRenderingCreateInfo,
            ColorAttachmentCount = (uint) info.ColorTargets.Length,
            PColorAttachmentFormats = colorAttachmentFormats
        };

        GraphicsPipelineCreateInfo pipelineInfo = new()
        {
            SType = StructureType.GraphicsPipelineCreateInfo,
            Layout = Layout,
            
            StageCount = 2,
            PStages = stages,
            
            PVertexInputState = &vertexInputState,
            PInputAssemblyState = &inputAssemblyState,
            PViewportState = &viewportState,
            PRasterizationState = &rasterizationState,
            PMultisampleState = &multisampleState,
            PDepthStencilState = &depthStencilState,
            PColorBlendState = &blendState,
            PDynamicState = &dynamicState,
            
            PNext = &renderingInfo
        };
        
        Logger.Trace("Creating graphics pipeline");
        _vk.CreateGraphicsPipelines(_device, new PipelineCache(), 1, &pipelineInfo, null, out Pipeline)
            .Check("Create graphics pipeline");
    }
        
    
    public override void Dispose()
    {
        _vk.DestroyPipeline(_device, Pipeline, null);
        _vk.DestroyPipelineLayout(_device, Layout, null);
    }
}