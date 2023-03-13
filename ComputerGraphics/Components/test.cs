/*namespace ComputerGraphics;

// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using Windows.UI.Core;

namespace MiniCubeTexture
{
    /// <summary>
    /// Simple renderer of a colored rotating cube.
    /// </summary>
    public class CubeTextureRenderer : Component
    {
        private SharpDX.Direct3D11.Buffer constantBuffer;
        private InputLayout layout;
        private VertexBufferBinding vertexBufferBinding;
        private Stopwatch clock;
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private ShaderResourceView textureView;
        private SamplerState sampler;

        /// <summary>
        /// Initializes a new instance of <see cref="CubeTextureRenderer"/>
        /// </summary>
        public CubeTextureRenderer()
        {
            Scale = 1.0f;
            ShowCube = true;
            EnableClear = true;
        }

        public bool EnableClear { get; set; }

        public bool ShowCube { get; set; }

        public float Scale { get; set; }

        public virtual void Initialize(DeviceManager devices)
        {
            // Remove previous buffer
            RemoveAndDispose(ref constantBuffer);

            // Setup local variables
            var d3dDevice = devices.DeviceDirect3D;
            var d3dContext = devices.ContextDirect3D;

            var path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

            // Loads vertex shader bytecode
            var vertexShaderByteCode = NativeFile.ReadAllBytes(path + "\\MiniCubeTexture_VS.fxo");
            vertexShader = new VertexShader(d3dDevice, vertexShaderByteCode);

            // Loads pixel shader bytecode
            pixelShader = new PixelShader(d3dDevice, NativeFile.ReadAllBytes(path + "\\MiniCubeTexture_PS.fxo"));

            // Layout from VertexShader input signature
            layout = new InputLayout(d3dDevice, vertexShaderByteCode, new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                    });

            // Instantiate Vertex buiffer from vertex data
            var vertices = SharpDX.Direct3D11.Buffer.Create(d3dDevice, BindFlags.VertexBuffer, new[]
                                  {
                                      // 3D coordinates              UV Texture coordinates
                                      -1.0f, -1.0f, -1.0f, 1.0f,     0.0f, 1.0f, // Front
                                      -1.0f,  1.0f, -1.0f, 1.0f,     0.0f, 0.0f,
                                       1.0f,  1.0f, -1.0f, 1.0f,     1.0f, 0.0f,
                                      -1.0f, -1.0f, -1.0f, 1.0f,     0.0f, 1.0f,
                                       1.0f,  1.0f, -1.0f, 1.0f,     1.0f, 0.0f,
                                       1.0f, -1.0f, -1.0f, 1.0f,     1.0f, 1.0f,

                                      -1.0f, -1.0f,  1.0f, 1.0f,     1.0f, 0.0f, // BACK
                                       1.0f,  1.0f,  1.0f, 1.0f,     0.0f, 1.0f,
                                      -1.0f,  1.0f,  1.0f, 1.0f,     1.0f, 1.0f,
                                      -1.0f, -1.0f,  1.0f, 1.0f,     1.0f, 0.0f,
                                       1.0f, -1.0f,  1.0f, 1.0f,     0.0f, 0.0f,
                                       1.0f,  1.0f,  1.0f, 1.0f,     0.0f, 1.0f,

                                      -1.0f, 1.0f, -1.0f,  1.0f,     0.0f, 1.0f, // Top
                                      -1.0f, 1.0f,  1.0f,  1.0f,     0.0f, 0.0f,
                                       1.0f, 1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
                                      -1.0f, 1.0f, -1.0f,  1.0f,     0.0f, 1.0f,
                                       1.0f, 1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
                                       1.0f, 1.0f, -1.0f,  1.0f,     1.0f, 1.0f,

                                      -1.0f,-1.0f, -1.0f,  1.0f,     1.0f, 0.0f, // Bottom
                                       1.0f,-1.0f,  1.0f,  1.0f,     0.0f, 1.0f,
                                      -1.0f,-1.0f,  1.0f,  1.0f,     1.0f, 1.0f,
                                      -1.0f,-1.0f, -1.0f,  1.0f,     1.0f, 0.0f,
                                       1.0f,-1.0f, -1.0f,  1.0f,     0.0f, 0.0f,
                                       1.0f,-1.0f,  1.0f,  1.0f,     0.0f, 1.0f,

                                      -1.0f, -1.0f, -1.0f, 1.0f,     0.0f, 1.0f, // Left
                                      -1.0f, -1.0f,  1.0f, 1.0f,     0.0f, 0.0f,
                                      -1.0f,  1.0f,  1.0f, 1.0f,     1.0f, 0.0f,
                                      -1.0f, -1.0f, -1.0f, 1.0f,     0.0f, 1.0f,
                                      -1.0f,  1.0f,  1.0f, 1.0f,     1.0f, 0.0f,
                                      -1.0f,  1.0f, -1.0f, 1.0f,     1.0f, 1.0f,

                                       1.0f, -1.0f, -1.0f, 1.0f,     1.0f, 0.0f, // Right
                                       1.0f,  1.0f,  1.0f, 1.0f,     0.0f, 1.0f,
                                       1.0f, -1.0f,  1.0f, 1.0f,     1.0f, 1.0f,
                                       1.0f, -1.0f, -1.0f, 1.0f,     1.0f, 0.0f,
                                       1.0f,  1.0f, -1.0f, 1.0f,     0.0f, 0.0f,
                                       1.0f,  1.0f,  1.0f, 1.0f,     0.0f, 1.0f,
                            });

            vertexBufferBinding = new VertexBufferBinding(vertices, sizeof(float) * 6, 0);

            // Create Constant Buffer
            constantBuffer = ToDispose(new SharpDX.Direct3D11.Buffer(d3dDevice, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));

            // Load texture and create sampler
            using (var bitmap = TextureLoader.LoadBitmap(devices.WICFactory, "GeneticaMortarlessBlocks.jpg"))
            using (var texture2D = TextureLoader.CreateTexture2DFromBitmap(d3dDevice, bitmap))
                textureView = new ShaderResourceView(d3dDevice, texture2D);

            sampler = new SamplerState(d3dDevice, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod =-float.MaxValue,
                MaximumLod = float.MaxValue
            });

            clock = new Stopwatch();
            clock.Start();
        }

        public virtual void Render(TargetBase render)
        {
            var d3dContext = render.DeviceManager.ContextDirect3D;

            float width = (float)render.RenderTargetSize.Width;
            float height = (float)render.RenderTargetSize.Height;

            // Prepare matrices
            var view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, width / (float)height, 0.1f, 100.0f);
            var viewProj = Matrix.Multiply(view, proj);

            var time = (float)(clock.ElapsedMilliseconds / 1000.0);


            // Set targets (This is mandatory in the loop)
            d3dContext.OutputMerger.SetTargets(render.DepthStencilView, render.RenderTargetView);

            // Clear the views
            d3dContext.ClearDepthStencilView(render.DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            if (EnableClear)
            {
                d3dContext.ClearRenderTargetView(render.RenderTargetView, Color.Black);
            }

            if (ShowCube)
            {
                // Calculate WorldViewProj
                var worldViewProj = Matrix.Scaling(Scale) * Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f) * viewProj;
                worldViewProj.Transpose();

                // Setup the pipeline
                d3dContext.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
                d3dContext.InputAssembler.InputLayout = layout;
                d3dContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                d3dContext.VertexShader.SetConstantBuffer(0, constantBuffer);
                d3dContext.VertexShader.Set(vertexShader);
                d3dContext.PixelShader.SetShaderResource(0, textureView);
                d3dContext.PixelShader.SetSampler(0, sampler);
                d3dContext.PixelShader.Set(pixelShader);

                // Update Constant Buffer
                d3dContext.UpdateSubresource(ref worldViewProj, constantBuffer, 0);

                // Draw the cube
                d3dContext.Draw(36, 0);
            }
        }
    }
}

/*using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Windows;
using System.IO;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDXExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the window
            var form = new RenderForm("SharpDX Example");

            // Create the device and swap chain
            var device = new D3D11.Device(DriverType.Hardware, DeviceCreationFlags.None);
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            var swapChain = new SwapChain(device.Fac, device, swapChainDesc);

            // Create the vertex shader
            var vertexShaderByteCode = File.ReadAllBytes("VertexShader.cso");
            var vertexShader = new VertexShader(device, vertexShaderByteCode);

            // Create the pixel shader
            var pixelShaderByteCode = File.ReadAllBytes("PixelShader.cso");
            var pixelShader = new PixelShader(device, pixelShaderByteCode);

            // Load the OBJ file
            var objLoader = new ObjLoader();
            var objModel = objLoader.Load("Model.obj");

            // Create the vertex buffer
            var vertexBufferDesc = new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = objModel.Vertices.Length * sizeof(Vertex),
                Usage = ResourceUsage.Default
            };
            var vertexBuffer = Buffer.Create(device, objModel.Vertices, vertexBufferDesc);

            // Create the index buffer
            var indexBufferDesc = new BufferDescription()
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = objModel.Indices.Length * sizeof(int),
                Usage = ResourceUsage.Default
            };
            var indexBuffer = Buffer.Create(device, objModel.Indices, indexBufferDesc);

            // Create the texture
            var texture = Texture2D.FromFile(device, "Texture.png");
            var textureView = new ShaderResourceView(device, texture);

            // Create the sampler state
            var samplerStateDesc = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagMipLinear
            };
            var samplerState = new SamplerState(device, samplerStateDesc);

            // Create the input layout
            var inputLayout = new InputLayout(device, vertexShaderByteCode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12)
            });

            // Set up the viewport
            var viewport = new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f);

            // Set up the constant buffer
            var constantBufferDesc = new BufferDescription()
            {
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 16 * sizeof(float),
                Usage = ResourceUsage.Dynamic
            };
            var constantBuffer = new Buffer(device, constantBufferDesc);

            // Set up the world matrix
            var worldMatrix = Matrix.Identity;

            // Set up the view matrix
            var eye = new Vector3(0.0f, 0.0f, -5.0f);
            var target = new Vector3(0.0f, 0.0f, 0.0f);
            var up = new Vector3(0.0f, 1.0f, 0.0f);
            var viewMatrix = Matrix.LookAtLH(eye, target, up);

            // Set up the projection matrix
            var aspectRatio = (float)form.ClientSize.Width / (float)form.ClientSize.Height;
            var fieldOfView = (float)Math.PI / 4.0f;
            var nearPlane = 0.1f;
            var farPlane = 100.0f;
            var projectionMatrix = Matrix.PerspectiveFovLH(fieldOfView, aspectRatio, nearPlane, farPlane);

            // Set up the world-view-projection matrix
            var wvpMatrix = worldMatrix * viewMatrix * projectionMatrix;

            // Set up the device context
            var deviceContext = device.ImmediateContext;
            deviceContext.InputAssembler.InputLayout = inputLayout;
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            deviceContext.VertexShader.Set(vertexShader);
            deviceContext.PixelShader.Set(pixelShader);
            deviceContext.PixelShader.SetShaderResource(0, textureView);
            deviceContext.PixelShader.SetSampler(0, samplerState);
            deviceContext.Rasterizer.SetViewport(viewport);
            deviceContext.VertexShader.SetConstantBuffer(0, constantBuffer);

            // Render loop
            RenderLoop.Run(form, () =>
            {
                // Clear the render target
                var clearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
                deviceContext.ClearRenderTargetView(swapChain.GetBackBuffer<RenderTargetView>(0), clearColor);

                // Update the constant buffer
                var dataStream = constantBuffer.Map(MapMode.WriteDiscard);
                dataStream.Write(wvpMatrix);
                constantBuffer.Unmap();

                // Draw the model
                deviceContext.DrawIndexed(objModel.Indices.Count, 0, 0);

                // Present the swap chain
                swapChain.Present(1, PresentFlags.None);
            });

            // Clean up
            textureView.Dispose();
            texture.Dispose();
            constantBuffer.Dispose();
            inputLayout.Dispose();
            samplerState.Dispose();
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
            pixelShader.Dispose();
            vertexShader.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }

    struct Vertex
    {
        public Vector3 Position;
        public Vector2 TexCoord;
    }

    class ObjLoader
    {
        public ObjModel Load(string filename)
        {
            var model = new ObjModel();

            using (var streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine().Trim();

                    if (line.StartsWith("v "))
                    {
                        var parts = line.Split(' ');
                        var x = float.Parse(parts[1]);
                        var y = float.Parse(parts[2]);
                        var z = float.Parse(parts[3]);
                        model.Vertices.Add(new Vertex() { Position = new Vector3(x, y, z) });
                    }
                    else if (line.StartsWith("vt "))
                    {
                        var parts = line.Split(' ');
                        var u = float.Parse(parts[1]);
                        var v = float.Parse(parts[2]);
                        model.Vertices[model.Vertices.Count - 1].TexCoord = new Vector2(u, v);
                    }
                    else if (line.StartsWith("f "))
                    {
                        var parts = line.Split(' ');

                        for (int i = 1; i <= 3; i++)
                        {
                            var indices = parts[i].Split('/');
                            var vertexIndex = int.Parse(indices[0]) - 1;
                            var texCoordIndex = int.Parse(indices[1]) - 1;
                            model.Vertices[vertexIndex].TexCoord = model.Vertices[vertexIndex].TexCoord;
                            model.Indices.Add(vertexIndex);
                        }
                    }
                }
            }

            return model;
        }
    }

    class ObjModel
    {
        public List<Vertex> Vertices { get; } = new List<Vertex>();
        public List<int> Indices { get; } = new List<int>();
    }
}#1#*/