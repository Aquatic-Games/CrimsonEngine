// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#include "TextureBatcher.h"

#include "Crimson/Util/Logger.h"

namespace Crimson
{
    TextureBatcher::TextureBatcher(SDL_GPUDevice* device, SDL_GPUTextureFormat outFormat)
    {
        _device = device;

        // 4096 sprites initial capacity.
        constexpr uint32 initialCapacity = 4096;

        _vertices.reserve(initialCapacity * NumVertices);
        _indices.reserve(initialCapacity * NumIndices);

        constexpr SDL_GPUBufferCreateInfo vertexBufferInfo
        {
            .usage = SDL_GPU_BUFFERUSAGE_VERTEX,
            .size = initialCapacity * NumVertices * sizeof(Vertex)
        };

        _vertexBuffer = SDL_CreateGPUBuffer(_device, &vertexBufferInfo);
        if (!_vertexBuffer)
            CS_FATAL("Failed to create vertex buffer: {}", SDL_GetError());

        constexpr SDL_GPUBufferCreateInfo indexBufferInfo
        {
            .usage = SDL_GPU_BUFFERUSAGE_INDEX,
            .size = initialCapacity * NumIndices * sizeof(IndexType),
        };

        _indexBuffer = SDL_CreateGPUBuffer(_device, &indexBufferInfo);
        if (!_indexBuffer)
            CS_FATAL("Failed to create index buffer: {}", SDL_GetError());

        SDL_GPUTransferBufferCreateInfo transBufferInfo
        {
            .usage = SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
            .size = vertexBufferInfo.size + indexBufferInfo.size,
        };

        _transferBuffer = SDL_CreateGPUTransferBuffer(_device, &transBufferInfo);
        if (!_transferBuffer)
            CS_FATAL("Failed to create transfer buffer: {}", SDL_GetError());
    }

    TextureBatcher::~TextureBatcher()
    {
        SDL_ReleaseGPUTransferBuffer(_device, _transferBuffer);
        SDL_ReleaseGPUBuffer(_device, _indexBuffer);
        SDL_ReleaseGPUBuffer(_device, _vertexBuffer);
    }
}
