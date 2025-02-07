using SDL;
using static Euphoria.Graphics.SdlUtil;
using static SDL.SDL3;

namespace Euphoria.Graphics;

public unsafe class Renderer : IDisposable
{
    private readonly SDL_Window* _window;
    private readonly SDL_GPUDevice* _device;
    
    public Renderer(SDL_Window* window, in RendererInfo info)
    {
        _window = window;
        
        _device = Check(SDL_CreateGPUDevice(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV, info.Debug, (byte*) null),
            "Create GPU device");

        Check(SDL_ClaimWindowForGPUDevice(_device, window), "Claim window for device");
    }
    
    public void Render()
    {
        SDL_GPUCommandBuffer* cb = Check(SDL_AcquireGPUCommandBuffer(_device), "Get command buffer");

        SDL_GPUTexture* swapchainTexture;
        uint width, height;
        Check(SDL_WaitAndAcquireGPUSwapchainTexture(cb, _window, &swapchainTexture, &width, &height),
            "Acquire swapchain texture");

        if (swapchainTexture == null)
            return;

        SDL_GPUColorTargetInfo targetInfo = new SDL_GPUColorTargetInfo()
        {
            texture = swapchainTexture,
            clear_color = new SDL_FColor() { r = 1.0f, g = 0.5f, b = 0.25f },
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE
        };

        SDL_GPURenderPass* pass = Check(SDL_BeginGPURenderPass(cb, &targetInfo, 1, null), "Begin render pass");
        
        SDL_EndGPURenderPass(pass);

        Check(SDL_SubmitGPUCommandBuffer(cb), "Submit command buffer");
    }

    public void Dispose()
    {
        SDL_ReleaseWindowFromGPUDevice(_device, _window);
        SDL_DestroyGPUDevice(_device);
    }
}