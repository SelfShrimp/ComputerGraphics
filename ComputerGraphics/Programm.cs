
using SharpDX;
using System.Collections.Generic;

namespace ComputerGraphics
{
    internal class Programm
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            GameComponent triangle1 = new Triangle(game, new Vector4[] {
                new Vector4(-0.5f, 0.5f, 0.0f, 0.0f), new Vector4(0.5f, 0.5f, 0.0f, 0.0f),
                new Vector4(-0.5f, -0.5f, 0.0f, 0.0f), new Vector4(0.5f, -0.5f, 0.0f, 0.0f),
            }
                );
            GameComponent triangle2 = new Triangle(game, new Vector4[] {
                new Vector4(0.5f, 0.5f, 0.0f, 0.0f), new Vector4(0.5f, -0.5f, 0.0f, 0.0f),
                new Vector4(-0.5f, -0.5f, 0.0f, 0.0f), //new Vector4(0.5f, -0.5f, 0.0f, 0.0f),
            }
                );
            game.components.Add(triangle1);
            //game.components.Add(triangle2);
            game.Run();
        }
    }
}
