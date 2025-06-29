using Crimson.Core;
using Crimson.Graphics.RHI.Vulkan.Vma;
using Crimson.Math;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly VmaAllocator_T* _allocator;

    private readonly VmaAllocation_T* _allocation;
    private readonly Image _image;
    
    public readonly ImageView ImageView;
    public readonly bool IsSwapchainTexture;

    public VulkanTexture(Vk vk, VkDevice device, VmaAllocator_T* allocator, in TextureInfo info) : base(new Size<uint>(info.Width, info.Height))
    {
        _vk = vk;
        _device = device;
        _allocator = allocator;

        ImageType type;
        ImageViewType viewType;
        ImageCreateFlags flags = ImageCreateFlags.None;
        ImageUsageFlags usage = ImageUsageFlags.TransferDstBit;
        uint arrayLayers = 1;
        Extent3D extent = new Extent3D(info.Width, info.Height, 1);

        switch (info.Type)
        {
            case TextureType.Texture2D:
                type = ImageType.Type2D;
                viewType = ImageViewType.Type2D;
                break;
            case TextureType.TextureCube:
                type = ImageType.Type2D;
                viewType = ImageViewType.TypeCube;
                flags |= ImageCreateFlags.CreateCubeCompatibleBit;
                arrayLayers = 6;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if ((info.Usage & TextureUsage.ShaderResource) != 0)
            usage |= ImageUsageFlags.SampledBit;
        if ((info.Usage & TextureUsage.ColorAttachment) != 0)
            usage |= ImageUsageFlags.ColorAttachmentBit;
        if ((info.Usage & TextureUsage.DepthStencil) != 0)
            usage |= ImageUsageFlags.DepthStencilAttachmentBit;
        if ((info.Usage & TextureUsage.GenerateMips) != 0)
            usage |= ImageUsageFlags.ColorAttachmentBit;

        VmaAllocationCreateInfo allocInfo = new()
        {
            usage = VmaMemoryUsage.VMA_MEMORY_USAGE_AUTO
        };
        
        ImageCreateInfo imageInfo = new()
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = type,
            Flags = flags,
            Usage = usage,
            Extent = extent,
            ArrayLayers = arrayLayers,
            MipLevels = 1,
            Format = info.Format.ToVk(),
            InitialLayout = ImageLayout.Undefined,
            Tiling = ImageTiling.Optimal,
            Samples = SampleCountFlags.Count1Bit,
            SharingMode = SharingMode.Exclusive
        };

        Logger.Trace("Creating image.");
        Vma.Vma.CreateImage(_allocator, &imageInfo, &allocInfo, out _image, out _allocation, out _)
            .Check("Create image");

        ImageViewCreateInfo viewInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = _image,
            Format = imageInfo.Format,
            ViewType = viewType,
            Components = new ComponentMapping()
            {
                R = ComponentSwizzle.Identity,
                G = ComponentSwizzle.Identity,
                B = ComponentSwizzle.Identity,
                A = ComponentSwizzle.Identity
            },
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                LayerCount = imageInfo.ArrayLayers,
                BaseArrayLayer = 0,
                LevelCount = imageInfo.MipLevels,
                BaseMipLevel = 0
            }
        };
        
        Logger.Trace("Creating image view.");
        _vk.CreateImageView(_device, &viewInfo, null, out ImageView).Check("Create image view");
    }

    public VulkanTexture(Vk vk, VkDevice device, Image swapchainImage, VkFormat format, Size<uint> size) : base(size)
    {
        _vk = vk;
        _device = device;
        _image = swapchainImage;
        IsSwapchainTexture = true;

        ImageViewCreateInfo viewInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = swapchainImage,
            Format = format,
            ViewType = ImageViewType.Type2D,
            Components = new ComponentMapping()
            {
                R = ComponentSwizzle.Identity,
                G = ComponentSwizzle.Identity,
                B = ComponentSwizzle.Identity,
                A = ComponentSwizzle.Identity
            },
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                BaseMipLevel = 0,
                LevelCount = 1,
                BaseArrayLayer = 0,
                LayerCount = 1
            }
        };
        
        Logger.Trace("Creating swapchain image view.");
        _vk.CreateImageView(_device, &viewInfo, null, out ImageView).Check("Create image view");
    }
    
    public override void Dispose()
    {
        _vk.DestroyImageView(_device, ImageView, null);
        
        if (!IsSwapchainTexture)
            Vma.Vma.DestroyImage(_allocator, _image, _allocation);
    }

    public void Transition(CommandBuffer cb, ImageLayout oldLayout, ImageLayout newLayout)
    {
        ImageMemoryBarrier barrier = new()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = _image,
            OldLayout = oldLayout,
            NewLayout = newLayout,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit,
            SubresourceRange = new ImageSubresourceRange()
            {
                AspectMask = ImageAspectFlags.ColorBit,
                LayerCount = 1,
                BaseArrayLayer = 0,
                LevelCount = 1,
                BaseMipLevel = 0
            }
        };

        _vk.CmdPipelineBarrier(cb, PipelineStageFlags.ColorAttachmentOutputBit,
            PipelineStageFlags.ColorAttachmentOutputBit, 0, 0, null, 0, null, 1, &barrier);
    }
}