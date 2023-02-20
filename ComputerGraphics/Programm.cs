
using ComputerGraphics.Components;
using SharpDX;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

namespace ComputerGraphics
{
    internal class Programm
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            GameComponent triangle1 = new Simple2dFigure(game, new Vector4[] {
                new Vector4(-0.9f, 0.2f, 0.0f, 0.0f), new Vector4(-0.85f, 0.2f, 0.0f, 0.0f),
                new Vector4(-0.9f, -0.2f, 0.0f, 0.0f), new Vector4(-0.85f, -0.2f, 0.0f, 0.0f),
            }
                );
            GameComponent triangle2 = new Simple2dFigure(game, new Vector4[] {
                new Vector4(0.85f, 0.2f, 0.0f, 0.0f), new Vector4(0.9f, 0.2f, 0.0f, 0.0f),
                new Vector4(0.85f, -0.2f, 0.0f, 0.0f), new Vector4(0.9f, -0.2f, 0.0f, 0.0f),

            }
                );
            GameComponent circle = new Circle(game);
            game.components.Add(triangle1);
            game.components.Add(triangle2);
            game.components.Add(circle);
            game.Run();
        }
    }
}
