﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Windows;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using System.Drawing;
using static SharpDX.Windows.RenderLoop;
using SharpDX.D3DCompiler;

namespace ComputerGraphics
{

    public class Game : IDisposable
    {
        private RenderForm renderForm;

        private const int Width = 800;
        private const int Height = 800;

        public D3D11.Device d3dDevice;
        public D3D11.DeviceContext d3dDeviceContext;

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
        
        public Game()
        {
            renderForm = new RenderForm("My first SharpDX game");
            InitializeDeviceResources();
        }
        private void Init()
        {
            //InitializeDeviceResources();
            foreach (var component in components)
            {
                component.Init();
            }
            InitLayout();
            
        }

        public void Run()
        {
            Init();
            RenderLoop.Run(renderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            Draw();
        }

        private void InitLayout()
        {
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("shaders.hlsl", "VSmain", "vs_4_0", ShaderFlags.Debug);
            inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
            inputLayout = new D3D11.InputLayout(d3dDevice, inputSignature, inputElements);
            d3dDeviceContext.InputAssembler.InputLayout = inputLayout;
        }



        private void InitializeDeviceResources()
        {
            //60,1-refresh rate
            ModeDescription backBufferDesc = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
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
            d3dDeviceContext = d3dDevice.ImmediateContext;

            viewport = new Viewport(0, 0, Width, Height);
            d3dDeviceContext.Rasterizer.SetViewport(viewport);

            D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0);
            renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
        }

        private void Draw()
        {
            //set back buffer as current render target view
            d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);

            //clear the screen
            d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(1, 100, 1));

            foreach (var component in components)
            {
                component.Draw();
            }

            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            renderTargetView.Dispose();
            swapChain.Dispose();
            d3dDevice.Dispose();
            d3dDeviceContext.Dispose();
            renderForm.Dispose();
            inputLayout.Dispose();
            inputSignature.Dispose();
            inputLayout.Dispose();
        }
    }
}