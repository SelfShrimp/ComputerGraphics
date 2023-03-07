
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
            Sphere sun = new Sphere(game,5f, 0f);
            Sphere mercury = new Sphere(game,0.5f, 0.4f);
            mercury.position.X = 10f;
            mercury.distance = 10;
            Sphere venus = new Sphere(game,0.5f,0.3f);
            venus.position.X = 15f;
            venus.distance = 15;
            Sphere earth = new Sphere(game,1f,0.24f);
            earth.position.X = 20f;
            earth.distance = 20;
            earth.AddMoon(180, -2);
            Sphere mars = new Sphere(game,1f,0.28f);
            mars.position.X = 25f;
            mars.distance = 25;
            Sphere jupiter = new Sphere(game,2f, 0.2f);
            jupiter.position.X = 30f;
            jupiter.distance = 30;
            Sphere saturn = new Sphere(game,1.5f, 0.35f);
            saturn.position.X = 35F;
            saturn.distance = 35;
            saturn.AddMoon(200, -4);
            saturn.AddMoon(180, -2);
            Sphere uranus = new Sphere(game,0.7f,0.15f);
            uranus.position.X = 40f;
            uranus.distance = 40;
            Sphere neptune = new Sphere(game,0.6f,0.1f);
            neptune.position.X = 45f;
            neptune.distance = 45;
            
            /*Sphere moon = new Sphere(game,0.1f,0.1f);
            moon.position.X = 20f;
            moon.distance = 5;*/
            game.components.Add(sun);
            game.components.Add(mercury);
            game.components.Add(venus);
            game.components.Add(earth);
            game.components.Add(mars);
            game.components.Add(jupiter);
            game.components.Add(saturn);
            game.components.Add(uranus);
            game.components.Add(neptune);
            //game.components.Add(moon);
            game.Run();
        }
    }
}
