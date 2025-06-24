using Crimson.Core;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    
    private readonly Image _image;
    
    public readonly ImageView ImageView;

    public VulkanTexture(Vk vk, VkDevice device, Image swapchainImage, Format format)
    {
        _vk = vk;
        _device = device;

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
        
        if (_image.Handle != 0)
            _vk.DestroyImage(_device, _image, null);
    }
}