using System;
using System.Collections.Generic;

using SharpDX.Windows;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Input;
using SharpDX.Direct3D11;
using ComputerGraphics.camera;

namespace ComputerGraphics
{

    public class Game : IDisposable
    {
        private RenderForm renderForm;

        public int width = 800;
        public int height = 800;

        public float deltaTime;

        public D3D11.Device d3dDevice;
        public D3D11.DeviceContext d3dContext;

        private SwapChain swapChain;

        private D3D11.RenderTargetView renderTargetView;

        public ShaderSignature inputSignature;
        private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0)
        };
        private D3D11.InputLayout inputLayout;

        private Viewport viewport;

        public List<GameComponent> components = new List<GameComponent>();
        
        private Stopwatch _clock;
        private TimeSpan _totalTime;

        public Camera camera;

        public Game()
        {
            renderForm = new RenderForm("Galaxy");
            renderForm.Width = width;
            renderForm.Height = height;

            InitializeDeviceResources();
        }
        private void Init()
        {
         //   InitLayout();

        }
        public void Run()
        {
            Init();
            Press();
            camera = new Camera();


            _clock = new Stopwatch();
            _clock.Start();
            _totalTime = _clock.Elapsed;

            RenderLoop.Run(renderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            var curTime = _clock.Elapsed;
            deltaTime = (float)(curTime - _totalTime).TotalSeconds;
            _totalTime = curTime;
            Update();
            Draw();
        }

        private void Press()
        {
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.S) { camera.position.Z-=0.5f; camera.target.Z -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.W) { camera.position.Z+=0.5f; camera.target.Z += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.D) { camera.position.X+=0.5f; camera.target.X += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.A) { camera.position.X-=0.5f; camera.target.X -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Up) { camera.target.Y -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Down) {camera.target.Y += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Right) { camera.target.X += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Left) { camera.target.X -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Escape) { renderForm.Close(); } };

        }

        /*private void InitLayout()
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "VSmain", "vs_4_0", ShaderFlags.Debug);
            inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
            inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);
            d3dDeviceContext.InputAssembler.InputLayout = inputLayout;
        }*/



        private void InitializeDeviceResources()
        {
            //60,1-refresh rate, BUT IT DOESNT WORK
            ModeDescription backBufferDesc = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            //descriptor for the swap chain
            SwapChainDescription swapChainDesc = new SwapChainDescription()
            {
                ModeDescription = backBufferDesc,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = renderForm.Handle,
                IsWindowed = true
            };
            //create device and swap chain
            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDesc, out d3dDevice, out swapChain);
            d3dContext = d3dDevice.ImmediateContext;

            viewport = new Viewport(0, 0, width, height);
            d3dContext.Rasterizer.SetViewport(viewport);

            D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0);
            renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            //set back buffer as current render target view
            d3dContext.OutputMerger.SetRenderTargets(renderTargetView);
        }

        private void Update()
        {
            camera.Update();
            components.ForEach(component => { component.Update(); });
        }

        private void Draw()
        {
            

            //clear the screen
            d3dContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(0, 0, 0));
            //var paddle = new Mesh(d3dDevice, new Vector3(0.1f, 0.2f, 0.1f));
            foreach (var component in components)
            {
                component.Draw();
            }

            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            components.ForEach(component => { component.Dispose();});
            renderTargetView.Dispose();
            swapChain.Dispose();
            d3dDevice.Dispose();
            d3dContext.Dispose();
            renderForm.Dispose();
            inputLayout.Dispose();
            inputSignature.Dispose();
            inputLayout.Dispose();
        }

    }
}