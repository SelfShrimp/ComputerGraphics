using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics.shadows;



public class ShadowMap
{
    public RenderTargetView RenderTargetView { get; private set; }
    public ShaderResourceView Texture { get; private set; }

    public void Initialize(D3D11.Device device, int width, int height)
    {
        // Create the render target texture
        var desc = new Texture2DDescription()
        {
            Width = width,
            Height = height,
            ArraySize = 1,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            Format = Format.R32_Typeless,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default
        };
        var texture = new Texture2D(device, desc);

        // Create the depth stencil view
        var depthDesc = new Texture2DDescription()
        {
            Width = width,
            Height = height,
            ArraySize = 1,
            BindFlags = BindFlags.DepthStencil,
            CpuAccessFlags = CpuAccessFlags.None,
            Format = Format.D32_Float,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default
        };
        var depthTexture = new Texture2D(device, depthDesc);
        var depthView = new DepthStencilView(device, depthTexture);

        // Create the render target view
        var rtvDesc = new RenderTargetViewDescription()
        {
            Format = Format.R32_Float,
            Dimension = RenderTargetViewDimension.Texture2D
        };
        var rtv = new RenderTargetView(device, texture, rtvDesc);

        // Create the shader resource view
        var srvDesc = new ShaderResourceViewDescription()
        {
            Format = Format.R32_Float,
            Dimension = ShaderResourceViewDimension.Texture2D
        };
        var srv = new ShaderResourceView(device, texture, srvDesc);

        // Set the render target and depth stencil views
        RenderTargetView = rtv;
        Texture = srv;
    }
}