using System.Collections.Generic;
using SharpDX;

namespace ComputerGraphics
{
    /*public struct MyVertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 tex;
        public MyVertex(Vector3 pos, Vector3 nor, Vector2 tex) {
            position = pos;
            normal = nor;
            this.tex = tex;
        }
    }
    public struct MyMesh
    {
        public List<MyVertex> vertices;
        public List<int> indices;
    }*/

    public struct VertexColor
    {
        public Vector4 point;
        public Color4 color = Color4.White;
        
        public VertexColor(Vector4 position, Color4 color)
        {
            this.point = position;
            this.color = color;
        }

        public void setColor(Color4 color)
        {
            this.color = color;
        }
    }
}