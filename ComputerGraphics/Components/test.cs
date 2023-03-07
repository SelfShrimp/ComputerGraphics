using System.Collections.Generic;
using SharpDX;

namespace ComputerGraphics.Components;

public class Test : GameComponent
{
    public Test(Game game) : base(game)
    {
        points = new List<VertexColor>
        {
            new VertexColor(new Vector4(-0.5f, 0.5f, 0.0f, 1), SharpDX.Color.Red),
            new VertexColor(new Vector4(0.5f, 0.5f, 0.0f, 1), SharpDX.Color.Green),
            new VertexColor(new Vector4(0.0f, -0.5f, 0.0f, 1), SharpDX.Color.Blue)
        };
        indices = new List<int>()
        {
            0, 2, 1
        };
        Init();
    }
}