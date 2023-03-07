using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SharpDX;

namespace ComputerGraphics;

//I like it name
public class ObjObject : GameComponent
{
    public ObjObject(Game game, string fileName) : base(game)
    {
        Read(fileName);
        Init();
    }

    private void Read(string fileName)
    {
        using (var reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(' ');
                    var vertex = new Vector4(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture),
                        1);
                    VertexColor vc = new VertexColor();
                    vc.point = vertex;
                    points.Add(vc);
                }
                else if (line.StartsWith("f "))
                {
                    var parts = line.Split(' ');
                    var index1 = int.Parse(parts[1].Split('/')[0]) - 1;
                    var index2 = int.Parse(parts[2].Split('/')[0]) - 1;
                    var index3 = int.Parse(parts[3].Split('/')[0]) - 1;
                    indices.Add(index1);
                    indices.Add(index2);
                    indices.Add(index3);
                }
            }
        }
    }
}