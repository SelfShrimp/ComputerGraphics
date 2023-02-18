using SharpDX;
using System;
using System.Collections.Generic;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics
{
    public abstract class GameComponent : IDisposable
    {
        protected D3D11.Buffer vertexBuffer;
        protected D3D11.VertexShader vertexShader;
        protected D3D11.PixelShader pixelShader;
        protected Vector4[] vectors;
        protected Game game;

        public GameComponent(Game game)
        {
            this.game = game;
        }

        public virtual void Draw() { }
        public virtual void Init() { }
        protected virtual void InitShaders() { }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
        }
    }
}
