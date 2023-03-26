using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct2D1;
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
        Texture2D texture = new Texture2D(device, new Texture2DDescription()
        {
            Width = width,
            Height = height,
            ArraySize = 1,
            BindFlags = BindFlags.RenderTarget,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            Format = Format.R8G8B8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            OptionFlags = ResourceOptionFlags.None,
            MipLevels = 1,
        });
        RenderTargetView renderTargetView = new RenderTargetView(device, texture);
        float orthoWidth = 20.0f; 
        float orthoHeight = 20.0f; 
        float nearPlane = 1.0f;
        float farPlane = 50.0f;
        Matrix projectionMatrix = Matrix.OrthoOffCenterRH(-orthoWidth, orthoWidth, -orthoHeight, orthoHeight, nearPlane, farPlane);
        Vector3 lightDirection = new Vector3(1, 1, 1);
        lightDirection.Normalize(); 
        Vector3 lightPosition = new Vector3(0, 0, -farPlane / 2);

        Vector3 lightTarget = lightPosition + lightDirection;
        Vector3 upVector = new Vector3(0, 1, 0);

        Matrix viewMatrix = Matrix.LookAtRH(lightPosition, lightTarget, upVector);
    }
}