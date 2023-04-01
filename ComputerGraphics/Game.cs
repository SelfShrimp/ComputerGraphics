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
using ComputerGraphics.shadows;
using System.Runtime.Remoting.Contexts;

namespace ComputerGraphics
{
    public class Game : IDisposable
    {
        private readonly RenderForm renderForm;

        private const int Width = 800;
        private const int Height = 800;

        public float deltaTime;

        public D3D11.Device d3dDevice;
        public D3D11.DeviceContext d3dContext;

        private SwapChain swapChain;

        private D3D11.RenderTargetView renderTargetView;
        private D3D11.DepthStencilView depthStencilView;
        private Viewport viewport;

        public ShadowMap shadow;
        public D3D11.Buffer lightConstBuffer;
        public LightConstBuff lightConstBuff;

        public readonly List<GameComponent> components = new List<GameComponent>();

        private Stopwatch clock;
        private TimeSpan totalTime;

        public Camera camera;
        private bool bindCamera = false;

        public Game()
        {
            renderForm = new RenderForm("Katamari");
            renderForm.Width = Width;
            renderForm.Height = Height;

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
                    /*var a = Vector3.Transform(camera.position - camera.target, Quaternion.RotationAxis(new Vector3(0, 1, 0), (float)-Math.PI / 180)) ;
                    camera.position = new Vector3(a.X,a.Y,a.Z)+ components[1].position;*/
                    camera.RotateAroundTarget((float)-Math.PI / 180, new Vector3(0, 1, 0));
                    components[1].quaternion = Quaternion.RotationAxis(new Vector3(0, 1, 0), (float)-Math.PI / 180) * components[1].quaternion;
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    /*var a = Vector3.Transform(camera.position - camera.target, Quaternion.RotationAxis(new Vector3    (0, 1, 0), (float)Math.PI / 180)) ;
                    camera.position = new Vector3(a.X,a.Y,a.Z)+ components[1].position;*/
                    camera.RotateAroundTarget((float)Math.PI / 180, new Vector3(0, 1, 0));
                    components[1].quaternion = Quaternion.RotationAxis(new Vector3(0, 1, 0), (float)Math.PI / 180) * components[1].quaternion;
                }

                if (Keyboard.IsKeyDown(Key.Up))
                {
                    /*var a = Vector3.Transform(camera.position - camera.target, Quaternion.RotationAxis(new Vector3(1, 0, 0), (float)Math.PI / 180)) ;
                    camera.position = new Vector3(a.X,a.Y,a.Z)+ components[1].position;*/
                    camera.RotateAroundTarget((float)Math.PI / 180, new Vector3(1, 0, 0));
                }
                    
                if (Keyboard.IsKeyDown(Key.Down))
                {
                    /*var a = Vector3.Transform(camera.position - camera.target, Quaternion.RotationAxis(new Vector3(1, 0, 0), (float)-Math.PI / 180)) ;
                    camera.position = new Vector3(a.X,a.Y,a.Z)+ components[1].position;*/
                    camera.RotateAroundTarget((float)-Math.PI / 180, new Vector3(1, 0, 0));
                }

                if (Keyboard.IsKeyDown(Key.W))
                {
                    Vector3 direction = camera.target - camera.position;
                    direction.Y = 0;
                    direction.Normalize();
                    Vector3 velocity = direction * 0.1f;
                    components[1].position += velocity;
                    //components[1].rotateX += 0.1f;
                    var inverMatrix = Matrix.Invert(camera.viewMatrix);
                    
                    components[1].quaternion = Quaternion.RotationAxis(inverMatrix.Right, (float)-Math.PI / 120) * components[1].quaternion;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position += velocity;
                        var a = Vector3.Transform(gameComponent.position - components[1].position, Matrix.RotationX(0.1f));
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z) + components[1].position;
                    }

                    camera.position += velocity;
                    camera.target += velocity;
                }
                if (Keyboard.IsKeyDown(Key.A))
                {
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Up) * 0.1f;
                    //Vector3 velocity = new Vector3(0.1f,0,0f);
                    components[1].position += velocity;
                    //components[1].rotateY += 0.1f;
                    components[1].quaternion = Quaternion.RotationAxis(new Vector3(0, 1, 0), (float)-Math.PI / 120) * components[1].quaternion;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position += velocity;
                        var a = Vector3.Transform(gameComponent.position - components[1].position, Matrix.RotationZ(0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                    }

                    camera.position += velocity;
                    camera.target += velocity;
                }

                if (Keyboard.IsKeyDown(Key.S))
                {
                    Vector3 direction = camera.target - camera.position;
                    direction.Y = 0;
                    direction.Normalize();
                    Vector3 velocity = direction * 0.1f;
                    //Vector3 velocity = new Vector3(0,0,0.1f);
                    components[1].position -= velocity;
                    //components[1].rotateX -= 0.1f;
                    components[1].quaternion = Quaternion.RotationAxis(new Vector3(1, 0, 0), (float)Math.PI / 120) * components[1].quaternion;

                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position -= velocity;
                        var a = Vector3.Transform(gameComponent.position -  components[1].position, Matrix.RotationX(-0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                    }

                    camera.position -= velocity;
                    camera.target -= velocity;
                }

                if (Keyboard.IsKeyDown(Key.D))
                {
                    Vector3 direction = camera.target - camera.position;
                    direction.Normalize();
                    Vector3 velocity = Vector3.Cross(direction, Vector3.Up) * 0.1f;
                    //Vector3 velocity = new Vector3(0.1f,0,0f);
                    components[1].position -= velocity;
                    //components[1].rotateY -= 0.1f;
                    components[1].quaternion = Quaternion.RotationAxis(new Vector3(0, 1, 0), (float)Math.PI / 120) * components[1].quaternion;
                    foreach (var gameComponent in components[1].gameComponents)
                    {
                        gameComponent.position -= velocity;
                        var a = Vector3.Transform(gameComponent.position - components[1].position, Matrix.RotationZ(-0.1f )) ;
                        gameComponent.position = new Vector3(a.X, a.Y, a.Z)+ components[1].position;
                    }
                    //camera.position = components[1].position + new Vector3(0,15,-15);;
                    camera.position -= velocity;
                    camera.target -= velocity;
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
            var backBufferDesc =
                new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            //descriptor for the swap chain
            var swapChainDesc = new SwapChainDescription()
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

            viewport = new Viewport(0, 0, Width, Height)
            {
                MinDepth = 0.0f,
                MaxDepth = 1.0f
            };
            
            var depthBuffer = new Texture2D(d3dDevice, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 0,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            depthStencilView = new DepthStencilView(d3dDevice, depthBuffer);

            var backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0);
            renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);

            shadow = new ShadowMap(d3dDevice);
            lightConstBuffer = new D3D11.Buffer(d3dDevice, Utilities.SizeOf<LightConstBuff>(), ResourceUsage.Default,
                BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);


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
                var indicesForDelete = new List<int>();
                for (var i = 2; i < components.Count; i++)
                {
                    if (components[1].boundingSphere.Intersects(components[i].boundingSphere) &&
                        components[1].boundingSphere.Radius > components[i].boundingSphere.Radius)
                    {
                        /*var distance = components[1].position - components[i].position;
                        components[i].positionStock = distance;*/
                        components[1].gameComponents.Add(components[i]);
                        components[1].radius += components[i].radius;
                        indicesForDelete.Add(i);
                    }
                }
                indicesForDelete.Sort();
                for (var i = indicesForDelete.Count - 1; i >= 0; i--)
                {
                    components.RemoveAt(indicesForDelete[i]);
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            components.ForEach(component => { component.Update(); });
        }
        
        private void SomeDoWithLight()
        {
            lightConstBuff = new LightConstBuff();
            lightConstBuff.ambientColor = new Vector3(0.4f, 0.4f, 0.4f);
            lightConstBuff.diffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
            lightConstBuff.specularColor = new Vector3(0.4f, 0.4f, 0.4f);
            lightConstBuff.position = camera.position;
            lightConstBuff.direction = new Vector3(1f, -3f, -10f);
            Matrix lightView = Matrix.LookAtRH(new Vector3(100f,100f,100f), lightConstBuff.direction + new Vector3(100f, 100f, 100f), Vector3.UnitY);
            Matrix lightProjection = Matrix.OrthoRH(200, 200, 0.1f, 1000);
            Matrix lightViewProjection = Matrix.Multiply(lightView, lightProjection);
            lightConstBuff.lightMatrix = lightViewProjection;
            d3dContext.UpdateSubresource(ref lightConstBuff, lightConstBuffer);
        }
        private void Draw()
        {
            Viewport shadowViewport = new Viewport(0, 0, 512, 512, 0.0f, 1.0f);
            d3dContext.Rasterizer.SetViewport(shadowViewport);
            SomeDoWithLight();
            d3dContext.OutputMerger.SetTargets(shadow.shadowDepthView, Array.Empty<RenderTargetView>());

            d3dContext.ClearDepthStencilView(shadow.shadowDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            
            foreach (var component in components)
            {
                component.UpdateForLight();
                //d3dContext.PixelShader.SetConstantBuffer(1, lightConstBuffer);
                component.DrawForLight();
            }


            d3dContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
            d3dContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(0, 0, 0));
            d3dContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);

            d3dContext.Rasterizer.SetViewport(viewport);

            d3dContext.PixelShader.SetShaderResource(1, shadow.shadowMapSRV);
            d3dContext.PixelShader.SetConstantBuffer(1, lightConstBuffer);

            Update();
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
        }
    }
}