
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics
{
    public class Simple2dFigure : GameComponent
    {
        public Simple2dFigure (Game game, Vector4[] vectors) : base(game)
        {
            this.vectors = vectors;
            InitShaders();
        }

        public override void Draw()
        {
            base.Draw();
            /*game.d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>(), 0));
            game.d3dDeviceContext.Draw(vectors.Count(), 0);
            vertexBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.VertexBuffer, vectors);*/
        }

        protected override void InitShaders()
        {
            base.InitShaders();

            /*var vertexShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "VSmain", "vs_4_0", ShaderFlags.Debug);
            vertexShader = new D3D11.VertexShader(game.d3dDevice, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "PSmain", "ps_4_0", ShaderFlags.Debug);
            pixelShader = new D3D11.PixelShader(game.d3dDevice, pixelShaderByteCode);

            game.d3dDeviceContext.VertexShader.Set(vertexShader);
            game.d3dDeviceContext.PixelShader.Set(pixelShader);

            //builds shapes from triangles, must not intersect with the previous parts, else ignore
            game.d3dDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;*/
        }

        public override void Move(float pos)
        {
            //foreach only read:(
            
                if(pos>0 && vectors[0].Y < 1f)
                {
                for (int i = 0; i < vectors.Length; i++)
                {
                    vectors[i].Y += pos;
                }
                }
                else if(pos < 0 && vectors[3].Y > -1f)
                {
                for (int i = 0; i < vectors.Length; i++)
                {
                    vectors[i].Y += pos;
                }
                }
            
        }
    }
}
