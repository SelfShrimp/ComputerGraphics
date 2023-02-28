using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.AxHost;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics
{
    public abstract class GameComponent : IDisposable
    {
        protected D3D11.Buffer vertexBuffer;
        protected CompilationResult vertexShaderByteCode;
        protected D3D11.VertexShader vertexShader;
        protected D3D11.PixelShader pixelShader;
        protected InputLayout layout;
        protected Game game;
        public Vector4[] points;

        public Vector3 position;
        public D3D11.Buffer constBuffer;


        public GameComponent(Game game)
        {
            this.game = game;

        }

        public virtual void Update()
        {

            /*var worldViewProj = Matrix.Translation(position);
            worldViewProj.M32 = +10f;
            worldViewProj.M31 = +10f;
            worldViewProj.M11 = +10f;*/
            //Random rnd = new Random();
            /*position.Y = rnd.NextFloat(-1, 1);
            position.X = rnd.NextFloat(-1, 1);*/
            //var matrix = Matrix.Translation(position);
            //position.X  = position.Y;
            //Matrix matrix = Matrix.Translation(0,0,0);
            Matrix matrix = Matrix.Scaling(1) * Matrix.RotationX(0) * Matrix.RotationY(0) * Matrix.RotationZ(0) * Matrix.Translation(position) * 1;
            game.d3dContext.UpdateSubresource(ref matrix, constBuffer);
        }

        public virtual void Draw()
        {
            SetContext();
            game.d3dContext.Draw(points.Count(), 0);
            //vertexBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.VertexBuffer, points);
            //constBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.ConstantBuffer, vectors);

        }

        public virtual void Move() { }
        public virtual void Move(float pos) { }
        protected virtual void CompileShaders()
        {

            vertexShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "VSmain", "vs_4_0", ShaderFlags.Debug);
            vertexShader = new D3D11.VertexShader(game.d3dDevice, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "PSmain", "ps_4_0", ShaderFlags.Debug);
            pixelShader = new D3D11.PixelShader(game.d3dDevice, pixelShaderByteCode);


        }
        protected virtual void InitLayout()
        {
            layout = new InputLayout(
                game.d3dDevice,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new D3D11.InputElement[]
                {
                    new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0)
                }
            );
        }

        protected virtual void InitConstBuff()
        {
            constBuffer = new D3D11.Buffer(game.d3dDevice, Utilities.SizeOf<Matrix>(), ResourceUsage.Default,
                BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

        protected virtual void Init()
        {
            vertexBuffer = D3D11.Buffer.Create(game.d3dDevice, points, new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default
            });

            CompileShaders();
            InitLayout();
            InitConstBuff();
        }

        protected virtual void SetContext()
        {
            game.d3dContext.InputAssembler.InputLayout = layout;
            game.d3dContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            game.d3dContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>(), 0));
            game.d3dContext.VertexShader.SetConstantBuffer(0, constBuffer);

            game.d3dContext.VertexShader.Set(vertexShader);
            game.d3dContext.PixelShader.Set(pixelShader);
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
