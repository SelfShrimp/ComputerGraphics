using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGraphics.Components
{
    internal class Circle : GameComponent
    {
        float speedX = 0.005f;
        float speedY = 0.005f;
        private int[] count= new int[] {0,0};
        private Random rnd = new Random();
        public Circle(Game game) : base(game)
        {
            getVectors();
            /*vectors = new Vector4[]
            {
                new Vector4(-0.0375f, 0.05f, 0.0f, 0.0f), new Vector4(0.0375f, 0.05f, 0.0f, 0.0f), 
                new Vector4(-0.0375f, -0.05f, 0.0f, 0.0f), new Vector4(0.0375f, -0.05f, 0.0f, 0.0f), 
            };*/
            InitShaders();
        }

        private void getVectors()
        {
            speedY = rnd.NextFloat(-0.007f, 0.007f);
            vectors = new Vector4[30];
            int n = 10;
            float deltaTheta = (float)(2 * Math.PI / n);
            for (int i = 0; i < n; i++)
            {
                float theta = i * deltaTheta; // Theta is the angle for that triangle
                int index = 3 * i;
                vectors[index + 0] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                vectors[index + 1] = new Vector4((float)Math.Cos(theta) / 40, (float)Math.Sin(theta) / 40, 0.0f, 0.0f);
                vectors[index + 2] = new Vector4((float)Math.Cos(theta + deltaTheta) / 40, (float)Math.Sin(theta + deltaTheta) / 40, 0.0f, 0.0f);
            }
        }

        public override void Draw()
        {
            checkTouch();
            Move();
            base.Draw();

        }

        private void checkTouch()
        {
            foreach (var vector in vectors)
            {
                if ((vector.Y >= 1.0f && speedY>0.0f) || (vector.Y <= -1.0f && speedY<0.0f))
                {
                    speedY *= -1;
                }

                if(speedX < 0.0f && vector.X < game.components[0].vectors[0].X-0.1f){
                    count[1]++;
                    speedX = 0.005f;
                    getVectors();
                    Console.WriteLine(count[0] + ":" + count[1]);
                    break;
                } else if (speedX > 0.0f && vector.X > game.components[1].vectors[1].X+0.1f)
                {
                    count[0]++;
                    speedX = 0.005f;
                    getVectors();
                    Console.WriteLine(count[0] + ":" + count[1]);
                    break;
                }

                if ((speedX<0.0f && (vector.X <= game.components[0].vectors[1].X && vector.X >= game.components[0].vectors[0].X &&
                ((vector.Y <= game.components[0].vectors[1].Y && vector.Y >= game.components[0].vectors[3].Y) ||
                (vector.Y <= game.components[0].vectors[1].Y && vector.Y >= game.components[0].vectors[3].Y)))
                )
                ||
                (speedX > 0.0f && (vector.X >= game.components[1].vectors[0].X && vector.X <= game.components[1].vectors[1].X &&
                ((vector.Y <= game.components[1].vectors[0].Y && vector.Y >= game.components[1].vectors[2].Y) ||
                (vector.Y <= game.components[1].vectors[0].Y && vector.Y >= game.components[1].vectors[2].Y))
                )))
                {
                    speedY = rnd.NextFloat(-0.007f, 0.007f);
                    speedX *= -1.1f;
                }


            }
        }

        public override void Move()
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i].X += speedX;
                vectors[i].Y += speedY;
            }
        }

        protected override void InitShaders()
        {
            base.InitShaders();
        }
    }
}
