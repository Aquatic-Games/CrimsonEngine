namespace Crimson.Graphics.RHI;

public ref struct GraphicsPipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<Format> ColorTargets;

    public GraphicsPipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader, in ReadOnlySpan<Format> colorTargets)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorTargets = colorTargets;
    }
}