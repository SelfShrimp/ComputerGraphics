
using System;
using ComputerGraphics.Components;
using SharpDX;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

namespace ComputerGraphics
{
    internal class Programm
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game game = new Game();
            Sphere sun = new Sphere(game,5f);
            Sphere mercury = new Sphere(game,0.5f);
            mercury.position.X = 10f;
            Sphere venus = new Sphere(game,1f);
            
            Sphere earth = new Sphere(game,1f);
            Sphere mars = new Sphere(game,1f);
            Sphere jupiter = new Sphere(game,1f);
            Sphere saturn = new Sphere(game,1f);
            Sphere uranus = new Sphere(game,1f);
            Sphere neptune = new Sphere(game,1f);
            game.components.Add(sun);
            game.components.Add(mercury);
            game.components.Add(venus);
            game.components.Add(earth);
            game.components.Add(mars);
            game.components.Add(jupiter);
            game.components.Add(saturn);
            game.components.Add(uranus);
            game.components.Add(neptune);
            game.Run();
        }
    }
}
