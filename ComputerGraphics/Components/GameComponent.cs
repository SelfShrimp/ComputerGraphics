using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics
{
    public abstract class GameComponent : IDisposable
    {
        protected D3D11.Buffer vertexBuffer;
        protected D3D11.VertexShader vertexShader;
        protected D3D11.PixelShader pixelShader;
        public Vector4[] vectors;
        protected Game game;

        public GameComponent(Game game)
        {
            this.game = game;
        }

        public virtual void Draw() {
            game.d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>(), 0));
            game.d3dDeviceContext.Draw(vectors.Count(), 0);
            vertexBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.VertexBuffer, vectors);
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
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
        }
    }
}
