// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#include "Crimson/Graphics/Renderer.h"

#include <format>
#include <stdexcept>
#include <SDL3/SDL_gpu.h>

#include "Crimson/Platform/Surface.h"
#include "Crimson/Util/Logger.h"

namespace Crimson
{
    static SDL_GPUDevice* _device;
    static SDL_Window* _window;

    void Renderer::Create()
    {
        const SDL_PropertiesID props = SDL_CreateProperties();
        SDL_SetBooleanProperty(props, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_SPIRV_BOOLEAN, true);
#ifndef NDEBUG
        CS_DEBUG("Debugging enabled, enabling graphics debugging.");
        SDL_SetBooleanProperty(props, SDL_PROP_GPU_DEVICE_CREATE_DEBUGMODE_BOOLEAN, true);
        SDL_SetBooleanProperty(props, SDL_PROP_GPU_DEVICE_CREATE_PREFERLOWPOWER_BOOLEAN, true);
#endif

#ifdef CS_WINDOWS
        SDL_SetBooleanProperty(props, SDL_PROP_GPU_DEVICE_CREATE_SHADERS_DXBC_BOOLEAN, true);
#endif

        CS_TRACE("Creating GPU device.");
        _device = SDL_CreateGPUDeviceWithProperties(props);
        if (!_device)
            CS_FATAL("Failed to create GPU device: {}", SDL_GetError());

        _window = static_cast<SDL_Window*>(Surface::GetHandle());
        SDL_ClaimWindowForGPUDevice(_device, _window);
    }

    void Renderer::Destroy()
    {
        SDL_ReleaseWindowFromGPUDevice(_device, _window);
        SDL_DestroyGPUDevice(_device);
    }

    void Renderer::Present()
    {
        SDL_GPUCommandBuffer* cb = SDL_AcquireGPUCommandBuffer(_device);

        SDL_GPUTexture* swapchainTexture;
        SDL_WaitAndAcquireGPUSwapchainTexture(cb, _window, &swapchainTexture, nullptr, nullptr);

        if (!swapchainTexture)
            return;

        const SDL_GPUColorTargetInfo target
        {
            .texture = swapchainTexture,
            .clear_color = { 1.0f, 0.5f, 0.25f, 1.0f },
            .load_op = SDL_GPU_LOADOP_CLEAR,
            .store_op = SDL_GPU_STOREOP_STORE
        };

        SDL_GPURenderPass* pass = SDL_BeginGPURenderPass(cb, &target, 1, nullptr);
        SDL_EndGPURenderPass(pass);

        SDL_SubmitGPUCommandBuffer(cb);
    }
}
