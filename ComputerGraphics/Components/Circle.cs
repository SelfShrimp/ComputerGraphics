using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputerGraphics.Components
{
    internal class Circle : GameComponent
    {
        float speedX = 0.5f;
        float speedY = 0.5f;
        private int[] count= new int[] {0,0};
        private Random rnd = new Random();
        public Circle(Game game) : base(game)
        {
            getVectors();
            Init();
        }

        private void getVectors()
        {
            speedY = rnd.NextFloat(-0.7f, 0.7f);
            points = new Vector4[30];
            int n = 10;
            float deltaTheta = (float)(2 * Math.PI / n);
            for (int i = 0; i < n; i++)
            {
                float theta = i * deltaTheta; // Theta is the angle for that triangle
                int index = 3 * i;
                points[index + 0] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                points[index + 1] = new Vector4((float)Math.Cos(theta) / 40, (float)Math.Sin(theta) / 40, 0.0f, 0.0f);
                points[index + 2] = new Vector4((float)Math.Cos(theta + deltaTheta) / 40, (float)Math.Sin(theta + deltaTheta) / 40, 0.0f, 0.0f);
            }
        }
        public override void Update()
        {
            checkTouch();
            Move();
            base.Update();

        }

        public override void Draw()
        {
            base.Draw();

        }

        private void checkTouch()
        {
            if ((position.Y >= 1f && speedY > 0.0f) || (position.Y <= -1f && speedY < 0.0f))
            {
                speedY *= -1;
            }

            if (speedX < 0.0f && position.X < game.components[0].points[0].X)
            {
                count[1]++;
                speedX = 0.5f;
                position = new Vector3();
                Console.WriteLine(count[0] + ":" + count[1]);
            }
            else if (speedX > 0.0f && position.X > game.components[1].points[1].X)
            {
                count[0]++;
                speedX = 0.5f;
                position = new Vector3();
                Console.WriteLine(count[0] + ":" + count[1]);
            }

            if ((speedX < 0.0f && (position.X <= game.components[0].points[1].X && position.X >= game.components[0].points[0].X &&
            ((position.Y <= game.components[0].points[1].Y && position.Y >= game.components[0].points[3].Y) ||
            (position.Y <= game.components[0].points[1].Y && position.Y >= game.components[0].points[3].Y)))
            )
            ||
            (speedX > 0.0f && (position.X >= game.components[1].points[0].X && position.X <= game.components[1].points[1].X &&
            ((position.Y <= game.components[1].points[0].Y && position.Y >= game.components[1].points[2].Y) ||
            (position.Y <= game.components[1].points[0].Y && position.Y >= game.components[1].points[2].Y))
            )))
            {
                speedY = rnd.NextFloat(-0.7f, 0.7f);
                speedX *= -1.1f;
            }
            /*foreach (var vector in points)
            {
                if ((vector.Y >= 1.0f && speedY>0.0f) || (vector.Y <= -1.0f && speedY<0.0f))
                {
                    speedY *= -1;
                }

                if(speedX < 0.0f && vector.X < game.components[0].points[0].X-0.1f){
                    count[1]++;
                    speedX = 0.5f;
                    getVectors();
                    Console.WriteLine(count[0] + ":" + count[1]);
                    break;
                } else if (speedX > 0.0f && vector.X > game.components[1].points[1].X+0.1f)
                {
                    count[0]++;
                    speedX = 0.5f;
                    getVectors();
                    Console.WriteLine(count[0] + ":" + count[1]);
                    break;
                }

                if ((speedX<0.0f && (vector.X <= game.components[0].points[1].X && vector.X >= game.components[0].points[0].X &&
                ((vector.Y <= game.components[0].points[1].Y && vector.Y >= game.components[0].points[3].Y) ||
                (vector.Y <= game.components[0].points[1].Y && vector.Y >= game.components[0].points[3].Y)))
                )
                ||
                (speedX > 0.0f && (vector.X >= game.components[1].points[0].X && vector.X <= game.components[1].points[1].X &&
                ((vector.Y <= game.components[1].points[0].Y && vector.Y >= game.components[1].points[2].Y) ||
                (vector.Y <= game.components[1].points[0].Y && vector.Y >= game.components[1].points[2].Y))
                )))
                {
                    speedY = rnd.NextFloat(-0.7f, 0.7f);
                    speedX *= -1.1f;
                }


            }*/
        }

        public override void Move()
        {
            position.X = position.X + game.deltaTime * speedX;
            position.Y = position.Y + game.deltaTime * speedY;
            /*for (int i = 0; i < points.Length; i++)
            {
                points[i].X = points[i].X + game.deltaTime * speedX;
                points[i].Y = points[i].Y + game.deltaTime * speedY;
            }*/
        }
    }
}
