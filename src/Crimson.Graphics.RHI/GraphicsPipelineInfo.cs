namespace Crimson.Graphics.RHI;

public ref struct GraphicsPipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<Format> ColorTargets;

    public ReadOnlySpan<InputElementDescription> InputLayout;

    public GraphicsPipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader, ReadOnlySpan<Format> colorTargets,
        ReadOnlySpan<InputElementDescription> inputLayout)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorTargets = colorTargets;
        InputLayout = inputLayout;
    }
}