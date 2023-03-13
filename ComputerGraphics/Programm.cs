
using System;

namespace ComputerGraphics
{
    internal class Programm
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game game = new Game();
            ObjObject plane = new ObjObject(game,"../../assets/Plane_obj.obj", "../../assets/textures/grass_tex.png");
            plane.rotateX = (float)Math.PI/2;
            plane.position.Y = -1f;
            ObjObject ball = new ObjObject(game,"../../assets/Ball_obj.obj", "../../assets/textures/colors.jpg");
            ObjObject ball2 = new ObjObject(game,"../../assets/Ball_obj.obj", "../../assets/textures/ball_tex.jpg");
            ball2.position.X = 20f;
            ball2.positionStock = ball2.position;
            ball2.scale = 0.7f;
            ObjObject ball3 = new ObjObject(game,"../../assets/Ball_obj.obj", "../../assets/textures/ball_tex.jpg");
            ball3.position.X = 10f;
            ball3.position.Z = 5f;
            ball3.positionStock = ball3.position;
            ball3.scale = 0.2f;
            ObjObject ball4 = new ObjObject(game,"../../assets/Ball_obj.obj", "../../assets/textures/ball_tex.jpg");
            ball4.position.X = -17f;
            ball4.positionStock = ball4.position;
            ball4.scale = 2f;


            game.components.Add(plane);
            game.components.Add(ball);
            game.components.Add(ball2);
            game.components.Add(ball3);
            game.components.Add(ball4);
            
            //game.components.Add(test);
            game.Run();
        }
    }
}
