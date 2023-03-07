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

        public D3D11.RenderTargetView renderTargetView;
        private Texture2D depthStencil;
        public D3D11.DepthStencilView depthStencilView;
        private Texture2D depthBuffer;

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
        private bool bindCamera = false;

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
            Press();
            var curTime = _clock.Elapsed;
            deltaTime = (float)(curTime - _totalTime).TotalSeconds;
            _totalTime = curTime;
            Update();
            Draw();
        }

        private void Press()
        {
            //renderForm.KeyDown DONT SET IN LOOP
            /*renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.S) { camera.position.Z-=0.5f; camera.target.Z -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.W) { camera.position.Z+=0.5f; camera.target.Z += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.D) { camera.position.X+=0.5f; camera.target.X += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.A) { camera.position.X-=0.5f; camera.target.X -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Up) { camera.target.Y -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Down) {camera.target.Y += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Right) { camera.target.X += 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Left) { camera.target.X -= 0.5f; } };
            renderForm.KeyDown += (sender, args) => { if (args.KeyCode == Keys.Escape) { renderForm.Close(); } };*/
            if (Keyboard.IsKeyDown(Key.NumPad9))
            {
                camera.position = components[4].position - new Vector3(0, 0, -10);
                bindCamera = true;
            }

            if (Keyboard.IsKeyDown(Key.NumPad0))
            {
                bindCamera = false;
                camera.position = new Vector3(0, 10, -50);
                camera.target = new Vector3(0, 0, 0);
                camera.up = new Vector3(0, 1, 0);
            }

            if (!bindCamera)
            {
                if (Keyboard.IsKeyDown(Key.Left))
                {
                    //this yaw around
                    //return vector4 AAAAAAAAAAAAAAAAAAAAAA
                    /*var a = Vector3.Transform(camera.position, Matrix.RotationY(MathUtil.PiOverFour / 60.0f));
                    camera.position = new Vector3(a.X,a.Y,a.Z);
                    a = Vector3.Transform(camera.up, Matrix.RotationY(MathUtil.PiOverFour / 60.0f));
                    camera.up = new Vector3(a.X,a.Y,a.Z);*/
                    //this fps
                    float yaw = (float)-(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(yaw, 0, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    /*var a = Vector3.Transform(camera.position - camera.target, Matrix.RotationY(-MathUtil.PiOverFour / 60.0f));
                    camera.position = new Vector3(a.X, a.Y, a.Z);// + camera.target;
                    a = Vector3.Transform(camera.up, Matrix.RotationY(-MathUtil.PiOverFour / 60.0f));
                    camera.up = new Vector3(a.X,a.Y,a.Z);*/
                    float yaw = (float)(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(yaw, 0, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.Up))
                {
                    /*var a = Vector3.Transform(camera.position , Matrix.RotationX(MathUtil.PiOverFour / 60.0f));
                    camera.position = new Vector3(a.X,a.Y,a.Z) ;
                    a = Vector3.Transform(camera.up, Matrix.RotationX(MathUtil.PiOverFour / 60.0f));
                    camera.up = new Vector3(a.X,a.Y,a.Z);*/

                    /*var angle = deltaTime * 100;
                    Matrix rotationMatrix = Matrix.RotationY((float)(Math.PI / 180) * angle);
                    var a = Vector3.Transform(camera.position, rotationMatrix);
                    camera.position = new Vector3(a.X,a.Y,a.Z);
                    a = Vector3.Transform(camera.target, rotationMatrix);
                    camera.target = new Vector3(a.X,a.Y,a.Z);*/

                    float pitch = (float)-(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(0, pitch, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.Down))
                {
                    /*var a = Vector3.Transform(camera.position - camera.target, Matrix.RotationX(-MathUtil.PiOverFour / 60.0f));
                    camera.position = new Vector3(a.X,a.Y,a.Z) + camera.target;
                    a = Vector3.Transform(camera.up, Matrix.RotationX(-MathUtil.PiOverFour / 60.0f));
                    camera.up = new Vector3(a.X,a.Y,a.Z);*/
                    float pitch = (float)(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(0, pitch, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.W))
                {
                    //Vector3 velocity = (Vector3.Normalize(camera.target - camera.position)+Vector3.ForwardLH) * 0.5f;
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = direction * 0.5f;
                    camera.position += velocity;
                    camera.target += velocity;
                    //camera.up = Vector3.TransformCoordinate(camera.up, velocity);
                }

                if (Keyboard.IsKeyDown(Key.A))
                {
                    //Vector3 velocity = (Vector3.Normalize(camera.target - camera.position)+Vector3.Left) * -0.5f;
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Up) * 0.5f;
                    camera.position += velocity;
                    camera.target += velocity;
                }

                if (Keyboard.IsKeyDown(Key.S))
                {
                    //Vector3 velocity = (Vector3.Normalize(camera.target - camera.position)+Vector3.BackwardLH) * -0.5f;
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = direction * 0.5f;
                    camera.position -= velocity;
                    camera.target -= velocity;
                }

                if (Keyboard.IsKeyDown(Key.D))
                {
                    //Vector3 velocity = (Vector3.Normalize(camera.target - camera.position)+Vector3.Right) * 0.5f;
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Up) * 0.5f;
                    camera.position -= velocity;
                    camera.target -= velocity;
                }

                if (Keyboard.IsKeyDown(Key.Q))
                {
                    //Vector3 velocity =(Vector3.Normalize(camera.target - camera.position)+Vector3.Up) * 0.5f;
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Right) * 0.5f;
                    camera.position += velocity;
                    camera.target += velocity;
                }

                if (Keyboard.IsKeyDown(Key.E))
                {
                    //Vector3 velocity = (Vector3.Normalize(camera.target - camera.position)+Vector3.Down) * -0.5f;
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Right) * 0.5f;
                    camera.position -= velocity;
                    camera.target -= velocity;
                }
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                renderForm.Close();
            }
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
            ModeDescription backBufferDesc =
                new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
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
            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDesc,
                out d3dDevice, out swapChain);
            d3dContext = d3dDevice.ImmediateContext;

            viewport = new Viewport(0, 0, width, height);
            viewport.MinDepth = 0.0f;
            viewport.MaxDepth = 1.0f;
            d3dContext.Rasterizer.SetViewport(viewport);

            Texture2D depthBuffer = new Texture2D(d3dDevice, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            depthStencilView = new DepthStencilView(d3dDevice, depthBuffer);

            D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0);
            renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            //set back buffer as current render target view
            d3dContext.OutputMerger.SetRenderTargets(renderTargetView);
            d3dContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
        }

        private void Update()
        {
            if (bindCamera)
            {
                camera.target = components[4].position;
                var a = Vector3.Transform(camera.position - camera.target, Matrix.RotationY((float)(Math.PI/ 18000) * 180)) ;
                camera.position = new Vector3(a.X,a.Y,a.Z)+ components[4].position;
                //camera.position = (camera.target - new Vector3(0f, 0f, 10f));

                /*var a = Vector3.Transform(camera.position, Matrix.RotationY(-MathUtil.PiOverFour / 60.0f));
                camera.position = new Vector3(a.X,a.Y,a.Z);
                a = Vector3.Transform(camera.up, Matrix.RotationY(-MathUtil.PiOverFour / 60.0f));
                camera.up = new Vector3(a.X,a.Y,a.Z);*/
            }

            camera.Update();
            components.ForEach(component => { component.Update(); });
        }

        private void Draw()
        {
            //clear the screen
            d3dContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(0, 0, 0));

            d3dContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);

            //d3dContext.OutputMerger.SetTargets(depthStencilView);


            //var paddle = new Mesh(d3dDevice, new Vector3(0.1f, 0.2f, 0.1f));
            foreach (var component in components)
            {
                component.Draw();
            }


            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            components.ForEach(component => { component.Dispose(); });
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