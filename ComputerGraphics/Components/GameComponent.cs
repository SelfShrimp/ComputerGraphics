using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using D3D11 = SharpDX.Direct3D11;

namespace ComputerGraphics
{
    public abstract class GameComponent : IDisposable
    {
        private D3D11.Buffer vertexBuffer;
        private D3D11.Buffer indexBuffer;
        private D3D11.Buffer constBuffer;
        private CompilationResult vertexShaderByteCode;
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout layout;
        private readonly Game game;

        //protected ObjModel objectModel = new ObjModel();
        protected readonly List<MyVertex> points = new();
        public List<int> indices = new();

        private RasterizerState rasterizerState;
        //public MyMesh meshes;

        public Vector3 position;
        public Quaternion quaternion = Quaternion.Identity;
        public float scale = 1f;
        //protected float speedZ = 0f;

        private ShaderResourceView shaderResourceView;
        protected Texture2D texture;
        private SamplerState sampler;

        public BoundingSphere boundingSphere;
        public float radius = 0f;

        public List<GameComponent> gameComponents = new();


        public GameComponent(Game game)
        {
            this.game = game;
        }

        public virtual void Update()
        {
            gameComponents.ForEach(component =>
            {
                component.Update();
            });
            boundingSphere.Center = position;
            boundingSphere.Radius = radius*scale;
            Console.WriteLine(quaternion);
            Matrix matrix = Matrix.Scaling(scale) * Matrix.RotationQuaternion(quaternion) * 
                            Matrix.Translation(position) *game.camera.viewProjectionMatrix;
            /*ConstBuff constBuff = new ConstBuff();
            constBuff.transform = matrix;*/
            game.d3dContext.UpdateSubresource(ref matrix, constBuffer);
        }

        public virtual void Draw()
        {
            gameComponents.ForEach(component => { component.Draw(); });
            SetContext();
            game.d3dContext.Draw(points.Count(), 0);
            //game.d3dContext.DrawIndexed(indices.Count(),0, 0);
        }

        public virtual void Move() { }
        public virtual void Move(float pos) { }
        protected virtual void CompileShaders()
        {

            vertexShaderByteCode = ShaderBytecode.CompileFromFile("../../hlsl/shaders.hlsl", "VSmain", "vs_5_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization);
            vertexShader = new D3D11.VertexShader(game.d3dDevice, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("../../hlsl/shaders.hlsl", "PSmain", "ps_5_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization);
            pixelShader = new D3D11.PixelShader(game.d3dDevice, pixelShaderByteCode);
            
        }
        protected virtual void InitLayout()
        {
            //slot params in hlsl
            layout = new InputLayout(
                game.d3dDevice,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new D3D11.InputElement[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0),
                    //new D3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, D3D11.InputClassification.PerVertexData, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0),
                    new InputElement("DIF", 0, Format.R32G32B32_Float, 0),
                }
            );
        }

        protected virtual void InitConstBuff()
        {
            constBuffer = new D3D11.Buffer(game.d3dDevice, Utilities.SizeOf<ConstBuff>(), ResourceUsage.Default,
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
            

            boundingSphere = new BoundingSphere(new Vector3(0,0,0),radius);
            vertexBuffer = D3D11.Buffer.Create(game.d3dDevice, BindFlags.VertexBuffer, points.ToArray());
            //indexBuffer = D3D11.Buffer.Create(game.d3dDevice, BindFlags.IndexBuffer, indices.ToArray());

            CompileShaders();
            InitLayout();
            InitConstBuff();
            
            rasterizerState = new RasterizerState(game.d3dDevice, new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                IsScissorEnabled = false,
                IsDepthClipEnabled = true
            });
            /*var shaderResourceViewDesc = new ShaderResourceViewDescription()
            {
                Format = Format.R32G32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
            };
            shaderResourceViewDesc.Texture2D.MipLevels = 1;
            shaderResourceViewDesc.Texture2D.MostDetailedMip = 0;*/
            shaderResourceView = new ShaderResourceView(game.d3dDevice, texture);
            sampler = new SamplerState(game.d3dDevice, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 1,
                MipLodBias = 0,
                MinimumLod = 0,
                MaximumLod = float.MaxValue,
            });
        }

        protected virtual void SetContext()
        {
            game.d3dContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<MyVertex>(), 0));
            game.d3dContext.InputAssembler.InputLayout = layout;
            game.d3dContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            game.d3dContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            
            game.d3dContext.VertexShader.SetConstantBuffer(0, constBuffer);
            game.d3dContext.VertexShader.Set(vertexShader);
            
            game.d3dContext.PixelShader.SetShaderResource(0,shaderResourceView);
            game.d3dContext.PixelShader.SetSampler(0, sampler);
            game.d3dContext.PixelShader.Set(pixelShader);
            /*var dataBox = game.d3dContext.MapSubresource(vertexBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None);
            Utilities.Write(dataBox.DataPointer, points.ToArray(), 0, points.Count);
            game.d3dContext.UnmapSubresource(vertexBuffer, 0);*/
            
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
