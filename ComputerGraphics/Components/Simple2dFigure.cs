
/*
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
        private const float height = 0.4f;
        private const float width = 0.05f;
        public Simple2dFigure(ref Game game, Vector4[] vectors) : base(game)
        {
            //this.position = position;
            //GetPoints();
            this.points = vectors;
            Init();
        }

        public override void Update()
        {
            //GetPoints();
            base.Update();
        }

        public override void Draw()
        {
            //GetPoints();
            base.Draw();
            /*game.d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vector4>(), 0));
            game.d3dDeviceContext.Draw(vectors.Count(), 0);
            vertexBuffer = D3D11.Buffer.Create<Vector4>(game.d3dDevice, D3D11.BindFlags.VertexBuffer, vectors);#1#
        }

        private void GetPoints()
        {
            points = new Vector4[]{
                    new Vector4(position.X, position.Y, 0.0f, 0.0f),
                    new Vector4(position.X + width, position.Y, 0.0f, 0.0f),
                    new Vector4(position.X, position.Y - height, 0.0f, 0.0f),
                    new Vector4(position.X + width, position.Y - height, 0.0f, 0.0f)
            };
        }

        public override void Move(float pos)
        {
            //foreach only read:(

            if (pos > 0 && points[0].Y < 1f)
            {
                position.Y = position.Y + game.deltaTime * pos;
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].Y = points[i].Y + game.deltaTime * pos;
                }
            }
            else if (pos < 0 && points[3].Y > -1f)
            {
                position.Y= position.Y + game.deltaTime * pos;
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].Y = points[i].Y + game.deltaTime * pos;
                }
            }

        }
    }
}
*/
