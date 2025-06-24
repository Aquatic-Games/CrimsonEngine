using Crimson.Core;
using Crimson.Math;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;

    private VulkanTexture? _currentSwapchainTexture;

    public readonly CommandBuffer Buffer;
    
    public VulkanCommandList(Vk vk, VkDevice device, CommandPool pool)
    {
        _vk = vk;
        _device = device;
        _pool = pool;

        CommandBufferAllocateInfo allocateInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _pool,
            CommandBufferCount = 1,
            Level = CommandBufferLevel.Primary
        };
        
        Logger.Trace("Allocating command buffer.");
        _vk.AllocateCommandBuffers(_device, &allocateInfo, out Buffer).Check("Allocate command buffer");
    }

    public override void Begin()
    {
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo
        };
        
        _vk.BeginCommandBuffer(Buffer, &beginInfo).Check("Begin command buffer");
    }
    
    public override void End()
    {
        if (_currentSwapchainTexture != null)
        {
            _currentSwapchainTexture.Transition(Buffer, ImageLayout.ColorAttachmentOptimal, ImageLayout.PresentSrcKhr);
            _currentSwapchainTexture = null;
        }
        
        _vk.EndCommandBuffer(Buffer).Check("End command buffer");
    }

    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        RenderingAttachmentInfo* colorRenderAttachments = stackalloc RenderingAttachmentInfo[colorAttachments.Length];

        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            VulkanTexture texture = (VulkanTexture) attachment.Texture;
            ref readonly Color clearColor = ref attachment.ClearColor;

            if (texture.IsSwapchainTexture)
            {
                texture.Transition(Buffer, ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal);
                _currentSwapchainTexture = texture;
            }

            colorRenderAttachments[i] = new RenderingAttachmentInfo()
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = texture.ImageView,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                ClearValue =
                    new ClearValue(new ClearColorValue(clearColor.R, clearColor.G, clearColor.B, clearColor.A)),
                LoadOp = attachment.LoadOp.ToVk(),
                StoreOp = attachment.StoreOp.ToVk()
            };
        }

        Texture firstTexture = colorAttachments[0].Texture;

        RenderingInfo renderingInfo = new()
        {
            SType = StructureType.RenderingInfo,
            ColorAttachmentCount = (uint) colorAttachments.Length,
            PColorAttachments = colorRenderAttachments,
            LayerCount = 1,
            RenderArea = new Rect2D(new Offset2D(0, 0), new Extent2D(firstTexture.Size.Width, firstTexture.Size.Height))
        };
        
        _vk.CmdBeginRendering(Buffer, &renderingInfo);
    }
    
    public override void EndRenderPass()
    {
        _vk.CmdEndRendering(Buffer);
    }

    public override void Dispose()
    {
        _vk.FreeCommandBuffers(_device, _pool, 1, in Buffer);
    }
}