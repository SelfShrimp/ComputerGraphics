using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics
{
    public abstract class GameComponent : IDisposable
    {
        protected D3D11.Buffer vertexBuffer;
        protected D3D11.VertexShader vertexShader;
        protected D3D11.PixelShader pixelShader;
        protected Game game;
        public Vector4[] vectors;

        public Vector3 position;
        public D3D11.Buffer constBuffer;


        public GameComponent(Game game)
        {
            this.game = game;

        }

        public virtual void Update()
        {
            Random rnd = new Random();

            var worldViewProj = Matrix.Translation(position);// * game.camera.GetProjectionMatrixOrhographic();
            worldViewProj.M32 = +10f;
            worldViewProj.M31 = +10f;
            //worldViewProj.M11 = +10f;
            Matrix matrix = Matrix.Translation(1,  1, 0);
            game.d3dDeviceContext.UpdateSubresource(ref matrix, constBuffer);
        }

        public virtual void Draw() {
            game.d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>(), 0));
            game.d3dDeviceContext.Draw(vectors.Count(), 0);
            //vertexBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.VertexBuffer, vectors);
            //constBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.ConstantBuffer, vectors);
            //game.d3dDeviceContext.VertexShader.SetConstantBuffer(0, constBuffer);
        }

        public virtual void Move() { }
        public virtual void Move(float pos) { }

        protected virtual void InitShaders() {

            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "VSmain", "vs_4_0", ShaderFlags.Debug);
            vertexShader = new D3D11.VertexShader(game.d3dDevice, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "PSmain", "ps_4_0", ShaderFlags.Debug);
            pixelShader = new D3D11.PixelShader(game.d3dDevice, pixelShaderByteCode);

            game.d3dDeviceContext.VertexShader.Set(vertexShader);
            game.d3dDeviceContext.PixelShader.Set(pixelShader);

            //builds shapes from triangles, must not intersect with the previous parts, else ignore
            game.d3dDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            vertexBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.VertexBuffer, vectors);
            constBuffer = new D3D11.Buffer(game.d3dDevice, Utilities.SizeOf<Matrix>(), ResourceUsage.Default,
                BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            //constBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.ConstantBuffer, vectors);
            game.d3dDeviceContext.VertexShader.SetConstantBuffer(0, constBuffer);
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            constBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
        }
    }
}
