using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using D3D11 = SharpDX.Direct3D11;
using System;
using System.Runtime.Remoting.Contexts;

namespace ComputerGraphics.shadows;



public class ShadowMap : IDisposable
{
    Texture2D shadowMap;
    public DepthStencilView shadowDepthView;
    public ShaderResourceView shadowMapSRV;

    public ShadowMap(D3D11.Device device)
    {
        shadowMap = new Texture2D(device, new Texture2DDescription()
        {
            Format = Format.R32_Typeless,
            ArraySize = 1,
            MipLevels = 1,
            Width = 1024,
            Height = 1024,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        });

        shadowDepthView = new DepthStencilView(device, shadowMap, new DepthStencilViewDescription()
        {
            Format = Format.D32_Float,
            Dimension = DepthStencilViewDimension.Texture2D,
            Flags = DepthStencilViewFlags.None,
            Texture2D = new DepthStencilViewDescription.Texture2DResource()
            {
                MipSlice = 0
            }
        });

        shadowMapSRV = new ShaderResourceView(device, shadowMap, new ShaderResourceViewDescription()
        {
            Format = Format.R32_Float,
            Dimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new ShaderResourceViewDescription.Texture2DResource()
            {
                MipLevels = 1,
                MostDetailedMip = 0
            }
        });
    }

    public void Draw(D3D11.DeviceContext context, RenderTargetView renderTargetView)
    {
        context.OutputMerger.SetTargets(shadowDepthView, renderTargetView);
        context.ClearDepthStencilView(shadowDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
    }
    public void SetShader(D3D11.DeviceContext context)
    {
        context.PixelShader.SetShaderResource(1, shadowMapSRV);
    }

    public void Dispose()
    {
        shadowMap.Dispose();
        shadowDepthView.Dispose();
        shadowMapSRV.Dispose();
    }
}