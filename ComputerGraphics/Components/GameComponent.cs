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
        protected D3D11.Buffer indexBuffer;
        protected CompilationResult vertexShaderByteCode;
        protected D3D11.VertexShader vertexShader;
        protected D3D11.PixelShader pixelShader;
        protected InputLayout layout;
        protected Game game;
        public List<Vector4> points = new List<Vector4>();
        public List<int> indices = new List<int>();

        private RasterizerState rasterizerState;
        //public MyMesh meshes;

        public Vector3 position;
        public D3D11.Buffer constBuffer;


        public GameComponent(Game game)
        {
            this.game = game;

        }

        public virtual void Update()
        {
            Matrix matrix = Matrix.Scaling(1) * Matrix.RotationX(0) * Matrix.RotationY(0) * Matrix.RotationZ(0) * Matrix.Translation(position) * game.camera.viewProjectionMatrix;
            //Matrix matrix = Matrix.Identity;
            //matrix.M34 = -1;
            game.d3dContext.UpdateSubresource(ref matrix, constBuffer);
        }

        public virtual void Draw()
        {
            SetContext();
            game.d3dContext.DrawIndexed(indices.Count(), 0,0);

        }

        public virtual void Move() { }
        public virtual void Move(float pos) { }
        protected virtual void CompileShaders()
        {

            vertexShaderByteCode = ShaderBytecode.CompileFromFile("../../hlsl/shaders.hlsl", "VSmain", "vs_4_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization);
            vertexShader = new D3D11.VertexShader(game.d3dDevice, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("../../hlsl/shaders.hlsl", "PSmain", "ps_4_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization);
            pixelShader = new D3D11.PixelShader(game.d3dDevice, pixelShaderByteCode);


        }
        protected virtual void InitLayout()
        {
            layout = new InputLayout(
                game.d3dDevice,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new D3D11.InputElement[]
                {
                    new D3D11.InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0)
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
            /*vertexBuffer = D3D11.Buffer.Create(game.d3dDevice, meshes.vertices.ToArray(), new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default
            });*/
            vertexBuffer = D3D11.Buffer.Create(game.d3dDevice, BindFlags.VertexBuffer, points.ToArray());
            indexBuffer = D3D11.Buffer.Create(game.d3dDevice, BindFlags.IndexBuffer, indices.ToArray());

            CompileShaders();
            InitLayout();
            InitConstBuff();

            rasterizerState = new RasterizerState(game.d3dDevice, new RasterizerStateDescription()
            {
                FillMode = SharpDX.Direct3D11.FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                IsScissorEnabled = false,
                IsDepthClipEnabled = true
            });
        }

        protected virtual void SetContext()
        {
            game.d3dContext.InputAssembler.InputLayout = layout;
            game.d3dContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            game.d3dContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>(), 0));
            game.d3dContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            game.d3dContext.VertexShader.SetConstantBuffer(0, constBuffer);

            game.d3dContext.VertexShader.Set(vertexShader);
            game.d3dContext.PixelShader.Set(pixelShader);

            game.d3dDevice.ImmediateContext.Rasterizer.State = rasterizerState;

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
