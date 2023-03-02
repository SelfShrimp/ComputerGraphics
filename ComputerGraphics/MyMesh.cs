using System.Collections.Generic;
using SharpDX;

namespace ComputerGraphics
{
    public struct MyVertex {
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
    }
}