using System.Numerics;
using System.Runtime.CompilerServices;
using Euphoria.Math;
using grabs;
using grabs.Core;
using grabs.ShaderCompiler;
using SDL;
using static Euphoria.Graphics.SdlUtil;
using static SDL.SDL3;

namespace Euphoria.Graphics;

public unsafe class TextureBatcher : IDisposable
{
    public const uint MaxBatches = 4096;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxBatches;
    private const uint MaxIndices = NumIndices * MaxBatches;
    
    private readonly SDL_GPUDevice* _device;
    
    private readonly List<DrawItem> _drawList;

    private readonly Vertex[] _vertices;
    private readonly ushort[] _indices;

    private readonly SDL_GPUBuffer* _vertexBuffer;
    private readonly SDL_GPUBuffer* _indexBuffer;

    private readonly SDL_GPUTransferBuffer* _transferBuffer;

    private readonly SDL_GPUGraphicsPipeline* _pipeline;

    public TextureBatcher(SDL_GPUDevice* device)
    {
        _device = device;

        _drawList = new List<DrawItem>();

        _vertices = new Vertex[MaxVertices];
        _indices = new ushort[MaxIndices];

        SDL_GPUBufferCreateInfo vertexBufferInfo = new SDL_GPUBufferCreateInfo()
        {
            size = (uint) (MaxVertices * sizeof(Vertex)),
            usage = SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX
        };

        _vertexBuffer = Check(SDL_CreateGPUBuffer(_device, &vertexBufferInfo), "Create vertex buffer");

        SDL_GPUBufferCreateInfo indexBufferInfo = new SDL_GPUBufferCreateInfo()
        {
            size = MaxIndices * sizeof(ushort),
            usage = SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX
        };

        _indexBuffer = Check(SDL_CreateGPUBuffer(_device, &indexBufferInfo), "Create index buffer");

        SDL_GPUTransferBufferCreateInfo transferBufferInfo = new SDL_GPUTransferBufferCreateInfo()
        {
            size = vertexBufferInfo.size + indexBufferInfo.size,
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD
        };

        _transferBuffer = Check(SDL_CreateGPUTransferBuffer(_device, &transferBufferInfo), "Create transfer buffer");

        string shaderCode = File.ReadAllText("EngineContent/Shaders/TextureBatcher.hlsl");
        byte[] vertexShaderSpirv = Compiler.CompileHlsl(ShaderStage.Vertex, shaderCode, "VSMain");
        byte[] pixelShaderSpirv = Compiler.CompileHlsl(ShaderStage.Pixel, shaderCode, "PSMain");

        SDL_GPUShader* vertexShader;
        
        fixed (byte* pSpirv = vertexShaderSpirv)
        {
            using PinnedString entryPoint = "VSMain";
            
            SDL_GPUShaderCreateInfo vertexShaderInfo = new SDL_GPUShaderCreateInfo()
            {
                format = SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV,
                stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX,
                code_size = (nuint) vertexShaderSpirv.Length,
                code = pSpirv,
                entrypoint = entryPoint,
                num_uniform_buffers = 1
            };

            vertexShader = Check(SDL_CreateGPUShader(_device, &vertexShaderInfo), "Create vertex shader");
        }

        SDL_GPUShader* pixelShader;

        fixed (byte* pSpirv = pixelShaderSpirv)
        {
            using PinnedString entryPoint = "PSMain";
            
            SDL_GPUShaderCreateInfo pixelShaderInfo = new SDL_GPUShaderCreateInfo()
            {
                format = SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV,
                stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT,
                code_size = (nuint) pixelShaderSpirv.Length,
                code = pSpirv,
                entrypoint = entryPoint
            };

            pixelShader = Check(SDL_CreateGPUShader(_device, &pixelShaderInfo), "Create pixel shader");
        }

        SDL_GPUColorTargetDescription targetDesc = new SDL_GPUColorTargetDescription()
        {
            format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM
        };

        SDL_GPUVertexBufferDescription vertexBufferDesc = new SDL_GPUVertexBufferDescription()
        {
            input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX,
            instance_step_rate = 1,
            pitch = (uint) sizeof(Vertex),
            slot = 0
        };

        SDL_GPUVertexAttribute* attributes = stackalloc SDL_GPUVertexAttribute[]
        {
            // Position
            new SDL_GPUVertexAttribute()
            {
                buffer_slot = 0,
                location = 0,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT2,
                offset = 0
            },
            // TexCoord
            new SDL_GPUVertexAttribute()
            {
                buffer_slot = 0,
                location = 1,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT2,
                offset = 2 * sizeof(float)
            },
            // Tint
            new SDL_GPUVertexAttribute()
            {
                buffer_slot = 0,
                location = 2,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT4,
                offset = 4 * sizeof(float)
            }
        };

        SDL_GPUGraphicsPipelineCreateInfo pipelineInfo = new SDL_GPUGraphicsPipelineCreateInfo()
        {
            vertex_shader = vertexShader,
            fragment_shader = pixelShader,
            target_info = new SDL_GPUGraphicsPipelineTargetInfo()
            {
                color_target_descriptions = &targetDesc,
                num_color_targets = 1
            },
            vertex_input_state = new SDL_GPUVertexInputState()
            {
                num_vertex_buffers = 1,
                vertex_buffer_descriptions = &vertexBufferDesc,
                num_vertex_attributes = 3,
                vertex_attributes = attributes
            },
            primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST
        };

        _pipeline = Check(SDL_CreateGPUGraphicsPipeline(device, &pipelineInfo), "Create graphics pipeline");
    }

    public void Draw(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color tint)
    {
        _drawList.Add(new DrawItem(topLeft, topRight, bottomLeft, bottomRight, tint));
    }

    internal void RenderDrawList(SDL_GPUCommandBuffer* cb, SDL_GPUTexture* texture, Size<uint> renderSize)
    {
        CameraMatrices matrices =
            new CameraMatrices(Matrix4x4.CreateOrthographicOffCenter(0, renderSize.Width, renderSize.Height, 0, -1, 1),
                Matrix4x4.Identity);
        SDL_PushGPUVertexUniformData(cb, 0, new IntPtr(&matrices), (uint) sizeof(CameraMatrices));

        uint numDraws = 0;
        foreach (DrawItem item in _drawList)
        {
            if (numDraws >= MaxBatches)
                Flush(cb, texture, numDraws);
            
            uint vOffset = numDraws * NumVertices;
            uint iOffset = numDraws * NumIndices;

            _vertices[vOffset + 0] = new Vertex((Vector2T<float>) item.TopLeft, new Vector2T<float>(0, 0), item.Tint);
            _vertices[vOffset + 1] = new Vertex((Vector2T<float>) item.TopRight, new Vector2T<float>(1, 0), item.Tint);
            _vertices[vOffset + 2] = new Vertex((Vector2T<float>) item.BottomRight, new Vector2T<float>(1, 1), item.Tint);
            _vertices[vOffset + 3] = new Vertex((Vector2T<float>) item.BottomLeft, new Vector2T<float>(0, 1), item.Tint);

            _indices[iOffset + 0] = (ushort) (0 + vOffset);
            _indices[iOffset + 1] = (ushort) (1 + vOffset);
            _indices[iOffset + 2] = (ushort) (3 + vOffset);
            _indices[iOffset + 3] = (ushort) (1 + vOffset);
            _indices[iOffset + 4] = (ushort) (2 + vOffset);
            _indices[iOffset + 5] = (ushort) (3 + vOffset);

            numDraws++;
        }
        
        Flush(cb, texture, numDraws);
        
        _drawList.Clear();
    }

    private void Flush(SDL_GPUCommandBuffer* cb, SDL_GPUTexture* texture, uint numDraws)
    {
        if (numDraws == 0)
            return;

        void* map = (void*) SDL_MapGPUTransferBuffer(_device, _transferBuffer, true);

        uint numVertexBytes = numDraws * NumVertices * Vertex.SizeInBytes;
        uint numIndexBytes = numDraws * NumIndices * sizeof(ushort);
        
        fixed (Vertex* pVertices = _vertices)
            Unsafe.CopyBlock(map, pVertices, numVertexBytes);
        
        fixed (ushort* pIndices = _indices)
            Unsafe.CopyBlock((byte*) map + numVertexBytes, pIndices, numIndexBytes);
        
        SDL_GPUCopyPass* copyPass = SDL_BeginGPUCopyPass(cb);
        
        UploadTransferBuffer(copyPass, _transferBuffer, _vertexBuffer, numVertexBytes, 0);
        UploadTransferBuffer(copyPass, _transferBuffer, _indexBuffer, numIndexBytes, numVertexBytes);
        
        SDL_EndGPUCopyPass(copyPass);

        SDL_GPUColorTargetInfo targetInfo = new SDL_GPUColorTargetInfo()
        {
            texture = texture,
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_LOAD,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
        };

        SDL_GPURenderPass* pass = Check(SDL_BeginGPURenderPass(cb, &targetInfo, 1, null), "Begin render pass");

        SDL_BindGPUGraphicsPipeline(pass, _pipeline);

        SDL_GPUBufferBinding vertexBinding = new SDL_GPUBufferBinding()
        {
            offset = 0,
            buffer = _vertexBuffer
        };
        SDL_BindGPUVertexBuffers(pass, 0, &vertexBinding, 1);

        SDL_GPUBufferBinding indexBinding = new SDL_GPUBufferBinding()
        {
            offset = 0,
            buffer = _indexBuffer
        };
        SDL_BindGPUIndexBuffer(pass, &indexBinding, SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_16BIT);

        SDL_DrawGPUIndexedPrimitives(pass, numDraws * NumIndices, 1, 0, 0, 0);
        
        SDL_EndGPURenderPass(pass);
    }
    
    public void Dispose()
    {
        SDL_ReleaseGPUGraphicsPipeline(_device, _pipeline);
        SDL_ReleaseGPUTransferBuffer(_device, _transferBuffer);
        SDL_ReleaseGPUBuffer(_device, _indexBuffer);
        SDL_ReleaseGPUBuffer(_device, _vertexBuffer);
    }

    private readonly struct Vertex
    {
        public readonly Vector2T<float> Position;

        public readonly Vector2T<float> TexCoord;

        public readonly Color Tint;

        public Vertex(Vector2T<float> position, Vector2T<float> texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }

        public const uint SizeInBytes = 32;
    }

    private readonly ref struct CameraMatrices
    {
        public readonly Matrix4x4 Projection;
        public readonly Matrix4x4 Transform;

        public CameraMatrices(Matrix4x4 projection, Matrix4x4 transform)
        {
            Projection = projection;
            Transform = transform;
        }
    }

    private readonly struct DrawItem
    {
        public readonly Vector2 TopLeft;
        public readonly Vector2 TopRight;
        public readonly Vector2 BottomLeft;
        public readonly Vector2 BottomRight;
        public readonly Color Tint;

        public DrawItem(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color tint)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Tint = tint;
        }
    }
}