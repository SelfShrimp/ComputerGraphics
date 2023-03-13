using System;
using System.Collections.Generic;
using SharpDX.Windows;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using System.Diagnostics;
using System.Windows.Input;
using SharpDX.Direct3D11;
using ComputerGraphics.camera;

namespace ComputerGraphics
{
    public class Game : IDisposable
    {
        private RenderForm renderForm;

        private int width = 800;
        private int height = 800;

        public float deltaTime;

        public D3D11.Device d3dDevice;
        public D3D11.DeviceContext d3dContext;

        private SwapChain swapChain;

        private D3D11.RenderTargetView renderTargetView;
        private D3D11.DepthStencilView depthStencilView;
        private Viewport viewport;

        public List<GameComponent> components = new List<GameComponent>();

        private Stopwatch clock;
        private TimeSpan totalTime;

        public Camera camera;
        private bool bindCamera = false;
        DataStream stream;

        public Game()
        {
            renderForm = new RenderForm("Katamari");
            renderForm.Width = width;
            renderForm.Height = height;

            InitializeDeviceResources();
        }

        public void Run()
        {
            camera = new Camera();
            
            clock = new Stopwatch();
            clock.Start();
            totalTime = clock.Elapsed;

            RenderLoop.Run(renderForm, RenderCallback);
        }

        private void RenderCallback()
        {
            Press();
            var curTime = clock.Elapsed;
            deltaTime = (float)(curTime - totalTime).TotalSeconds;
            totalTime = curTime;
            Update();
            Draw();
        }

        
        Vector3 ballVelocity = Vector3.Zero;
        Vector3 ballAcceleration = Vector3.Zero;
        Vector3 ballRotation = Vector3.Zero;
        Vector3 ballAngularVelocity = Vector3.Zero;
        Vector3 ballAngularAcceleration = Vector3.Zero;
        private void Press()
        {
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
                    float yaw = (float)-(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(yaw, 0, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    float yaw = (float)(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(yaw, 0, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.Up))
                {
                    float pitch = (float)-(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(0, pitch, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.Down))
                {
                    float pitch = (float)(MathUtil.Pi / 180.0);
                    Matrix rotationMatrix = Matrix.RotationYawPitchRoll(0, pitch, 0);
                    Vector3 rotatedTarget =
                        Vector3.TransformCoordinate(camera.target - camera.position, rotationMatrix);
                    camera.target = camera.position + rotatedTarget;
                    camera.up = Vector3.TransformCoordinate(camera.up, rotationMatrix);
                }

                if (Keyboard.IsKeyDown(Key.W))
                {
                    /*Vector3 direction = camera.target - camera.position;
                    direction.Normalize();*/
                    //Vector3 velocity = direction * 0.5f;
                    Vector3 velocity = new Vector3(0,0,0.1f);
                    components[1].position += velocity;
                    components[1].rotateX += 0.1f;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position += velocity;
                        var a = Vector3.Transform(gameComponent.position - components[1].position, Matrix.RotationX(0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                        
                    }
                    camera.position = components[1].position + new Vector3(0,15,-15);
                    camera.target += velocity;
                    
                }

                if (Keyboard.IsKeyDown(Key.A))
                {
                    /*Vector3 direction = camera.target - camera.position;
                    direction.Normalize();*/
                    //Vector3 velocity = Vector3.Cross(direction, Vector3.Up) * 0.5f;
                    Vector3 velocity = new Vector3(0.1f,0,0f);
                    components[1].position -= velocity;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position -= velocity;
                        var a = Vector3.Transform(gameComponent.position - components[1].position, Matrix.RotationZ(0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                    }
                    camera.position = components[1].position + new Vector3(0,15,-15);;
                    camera.target -= velocity;
                }

                if (Keyboard.IsKeyDown(Key.S))
                {
                    /*Vector3 direction = camera.target - camera.position;
                    direction.Normalize();*/
                    //Vector3 velocity = direction * 0.5f;
                    Vector3 velocity = new Vector3(0,0,0.1f);
                    components[1].position -= velocity;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position -= velocity;
                        var a = Vector3.Transform(gameComponent.position - + components[1].position, Matrix.RotationX(-0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                    }
                    camera.position = components[1].position + new Vector3(0,15,-15);;
                    camera.target -= velocity;
                }

                if (Keyboard.IsKeyDown(Key.D))
                {
                    /*Vector3 direction = camera.target - camera.position;
                    direction.Normalize();*/
                    //Vector3 velocity = Vector3.Cross(direction, Vector3.Up) * 0.5f;
                    Vector3 velocity = new Vector3(0.1f,0,0f);
                    components[1].position += velocity;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position += velocity;
                        var a = Vector3.Transform(gameComponent.position - + components[1].position, Matrix.RotationZ(-0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                    }
                    camera.position = components[1].position + new Vector3(0,15,-15);;
                    camera.target += velocity;
                }

                if (Keyboard.IsKeyDown(Key.Q))
                {
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Right) * 0.5f;
                    camera.position += velocity;
                    camera.target += velocity;
                }

                if (Keyboard.IsKeyDown(Key.E))
                {
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
            //d3dContext.OutputMerger.SetRenderTargets(renderTargetView);
            
        }

        private void Update()
        {
            if (bindCamera)
            {
                camera.target = components[4].position;
                var a = Vector3.Transform(camera.position - camera.target, Matrix.RotationY((float)(Math.PI/ 18000) * 180)) ;
                camera.position = new Vector3(a.X,a.Y,a.Z)+ components[4].position;
            }

            camera.Update();
            try
            {
                List<int> indecesForDelete = new List<int>();
                for (int i = 2; i < components.Count; i++)
                {
                    if (components[1].boundingSphere.Intersects(components[i].boundingSphere) &&
                        components[1].boundingSphere.Radius > components[i].boundingSphere.Radius)
                    {
                        /*var distance = components[1].position - components[i].position;
                        components[i].positionStock = distance;*/
                        components[1].gameComponents.Add(components[i]);
                        components[1].radius += components[i].radius;
                        indecesForDelete.Add(i);
                    }
                }
                indecesForDelete.Sort();
                for (int i = indecesForDelete.Count - 1; i >= 0; i--)
                {
                    components.RemoveAt(indecesForDelete[i]);
                }
            }
            catch (Exception e)
            {
            }
            components.ForEach(component => { component.Update(); });
        }

        private void Draw()
        {
            d3dContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
            //clear the screen
            d3dContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(0, 0, 0));

            d3dContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            //d3dContext.UnmapSubresource(renderTargetView, 0);
            foreach (var component in components)
            {
                //d3dContext.MapSubresource(component.texture, 0, MapMode.WriteDiscard, D3D11.MapFlags.None, out stream);
                component.Draw();
                //d3dContext.UnmapSubresource(component.texture, 0);
            }


            swapChain.Present(1, PresentFlags.None);
        }

        //TODO: finish dispose
        public void Dispose()
        {
            components.ForEach(component => { component.Dispose(); });
            renderTargetView.Dispose();
            swapChain.Dispose();
            d3dDevice.Dispose();
            d3dContext.Dispose();
            renderForm.Dispose();
        }
    }
}