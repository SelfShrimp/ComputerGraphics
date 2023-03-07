
using System;
using ComputerGraphics.Components;
using SharpDX;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using ComputerGraphics;

namespace ComputerGraphics
{
    internal class Programm
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game game = new Game();
            //Test test = new Test(game);
            ObjObject variable = new ObjObject(game,"../../assets/Ball_obj.obj");
            game.components.Add(variable);
            //game.components.Add(test);
            game.Run();
        }
    }
}
