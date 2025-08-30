// Copyright (c) Aquatic Games 2025. This file is licensed under the MIT license.
#pragma once

#include <vector>
#include <SDL3/SDL.h>

#include "Crimson/Math/Math.h"

namespace Crimson
{
    class CanvasRenderer final
    {
        using IndexType = uint32;
        struct Vertex;

        static constexpr uint32 NumVertices = 4;
        static constexpr uint32 NumIndices = 6;

        SDL_GPUDevice* _device;

        std::vector<Vertex> _vertices;
        std::vector<IndexType> _indices;

        SDL_GPUBuffer* _vertexBuffer;
        SDL_GPUBuffer* _indexBuffer;
        SDL_GPUTransferBuffer* _transferBuffer;

        SDL_GPUGraphicsPipeline* _pipeline;

    public:
        CanvasRenderer(SDL_GPUDevice* device, SDL_GPUTextureFormat outFormat);
        ~CanvasRenderer();

    private:
        struct Vertex
        {
            Vector2f Position;
            Vector2f TexCoord;
        };
    };
}
