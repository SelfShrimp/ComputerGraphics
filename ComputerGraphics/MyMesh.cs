using System.Collections.Generic;
using SharpDX;

namespace ComputerGraphics
{
    public struct ConstBuff
    {
        public Matrix transform;
        public Vector4 lightDirection;
        public Vector4 lightColor;
    }
    public class MyMesh
    {
        public Vector3[] Vertices { get; private set; }
        public Vector2[] TexCoords { get; private set; }
        public Vector3[] Normals { get; private set; }
        public int[] Indices { get; private set; }

        public MyMesh(Vector3[] vertices, Vector2[] texCoords, Vector3[] normals, int[] indices)
        {
            Vertices = vertices;
            TexCoords = texCoords;
            Normals = normals;
            Indices = indices;
        }
    }
    public struct MyVertex
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector4 Color;

        public MyVertex(Vector3 position, Vector3 normal, Vector2 textureCoordinate, Vector4 color)
        {
            Position = new Vector4(position,1);
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Color = color;
        }
    }
    /*public struct PosTex {
        public Vector4 position;
        public Vector2 tex;
        public PosTex(Vector4 pos, Vector2 tex) {
            position = pos;
            this.tex = tex;
        }

        public void SetPosition(Vector4 position)
        {
            this.position = position;
        }
        public void SetTexture(Vector2 tex)
        {
            this.tex = tex;
        }
    }*/
    /*public struct MyMesh
    {
        public List<MyVertex> vertices;
        public List<int> indices;
    }*/

    /*public struct VertexColor
    {
        public Vector4 point;
        public Color4 color;
        
        public VertexColor(Vector4 position, Color4 color)
        {
            this.point = position;
            this.color = color;
        }

        public void setColor(Color4 color)
        {
            this.color = color;
        }
    }*/
}