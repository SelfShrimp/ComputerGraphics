using System;
using System.Collections.Generic;
using System.Globalization;
using SharpDX;
using System.IO;
using System.Linq;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using BitmapSource = SharpDX.WIC.BitmapSource;

namespace ComputerGraphics;

//I like it name
public class ObjObject : GameComponent
{
    private MyMesh mesh;

    public ObjObject(Game game, string objFile, string textureFile) : base(game)
    {
        //ReadObj(objFile);
        mesh = ReadObjFile(objFile);
        var imagingFactory = new SharpDX.WIC.ImagingFactory2();
        var bitmap = LoadBitmap(imagingFactory, textureFile);
        texture = CreateTexture2DFromBitmap(game.d3dDevice, bitmap);

        Init();
    }

    public static BitmapSource LoadBitmap(SharpDX.WIC.ImagingFactory2 factory, string filename)
    {
        var bitmapDecoder = new SharpDX.WIC.BitmapDecoder(
            factory,
            filename,
            SharpDX.WIC.DecodeOptions.CacheOnDemand
        );

        var formatConverter = new SharpDX.WIC.FormatConverter(factory);

        formatConverter.Initialize(
            bitmapDecoder.GetFrame(0),
            SharpDX.WIC.PixelFormat.Format32bppPRGBA,
            SharpDX.WIC.BitmapDitherType.None,
            null,
            0.0,
            SharpDX.WIC.BitmapPaletteType.Custom);

        return formatConverter;
    }

    public static SharpDX.Direct3D11.Texture2D CreateTexture2DFromBitmap(SharpDX.Direct3D11.Device device,
        SharpDX.WIC.BitmapSource bitmapSource)
    {
        // Allocate DataStream to receive the WIC image pixels
        int stride = bitmapSource.Size.Width * 4;
        using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true))
        {
            // Copy the content of the WIC to the buffer
            bitmapSource.CopyPixels(stride, buffer);
            return new SharpDX.Direct3D11.Texture2D(device, new SharpDX.Direct3D11.Texture2DDescription()
            {
                Width = bitmapSource.Size.Width,
                Height = bitmapSource.Size.Height,
                ArraySize = 1,
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                Usage = SharpDX.Direct3D11.ResourceUsage.Immutable,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            }, new SharpDX.DataRectangle(buffer.DataPointer, stride));
        }
    }

    MyMesh ReadObjFile(string filePath)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> texCoords = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    continue;

                switch (parts[0])
                {
                    case "v":
                        vertices.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)));
                        break;

                    case "vt":
                        texCoords.Add(new Vector2(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture)));
                        break;

                    case "vn":
                        normals.Add(new Vector3(
                            float.Parse(parts[1], CultureInfo.InvariantCulture),
                            float.Parse(parts[2], CultureInfo.InvariantCulture),
                            float.Parse(parts[3], CultureInfo.InvariantCulture)));
                        break;

                    case "f":
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] indicesParts = parts[i].Split('/');
                            if (indicesParts.Length < 1)
                                throw new Exception("Invalid face data");
                            int vertexIndex = int.Parse(indicesParts[0]) - 1;
                            int normalIndex = int.Parse(indicesParts[2]) - 1;
                            int texIndex = int.Parse(indicesParts[1]) - 1;
                            this.indices.Add(vertexIndex);
                            points.Add(new MyVertex(vertices[vertexIndex],normals[normalIndex],texCoords[texIndex]));
                        }

                        break;
                }
            }
        }

        Vector3 center = new Vector3();
        for (int i = 0; i < vertices.Count; i++)
        {
            center += vertices[i];
        }
        center /= vertices.Count;
        for (int i = 0; i < vertices.Count; i++)
        {
            float distance = Vector3.Distance(center, vertices[i]);
            if (distance > radius)
            {
                radius = distance;
            }
        }

        radius += 0.005f;
        //radius *= scale;
        return new MyMesh(vertices.ToArray(), texCoords.ToArray(), normals.ToArray(), indices.ToArray());
    }
    /*List<MyVertex> ReadObjFile(string filePath)
    {
        List<MyVertex> vertices = new List<MyVertex>();
        CultureInfo culture = CultureInfo.InvariantCulture;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            MyVertex lastVertex = new MyVertex();
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(' ');

                if (parts[0] == "v")
                {
                    float x = float.Parse(parts[1], culture);
                    float y = float.Parse(parts[2], culture);
                    float z = float.Parse(parts[3], culture);
                    vertices.Add(new MyVertex(new Vector4(x, y, z, 1), Vector3.Zero, Vector2.Zero));
                    lastVertex = vertices[vertices.Count - 1];
                }


                else if (parts[0] == "vn")
                {
                    if (vertices.Count > 0)
                    {
                        float x = float.Parse(parts[1], culture);
                        float y = float.Parse(parts[2], culture);
                        float z = float.Parse(parts[3], culture);
                        lastVertex.normal = new Vector3(x, y, z);
                        vertices[vertices.Count - 1] = lastVertex;
                    }
                }
                else if (parts[0] == "vt")
                {
                    float u = float.Parse(parts[1], culture);
                    float v = float.Parse(parts[2], culture);
                    lastVertex.tex = new Vector2(u, v);
                    vertices[vertices.Count - 1] = lastVertex;
                }
                else if (parts[0] == "f")
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string[] indice = parts[i].Split('/');
                        int vertexIndex = int.Parse(indice[0], culture) - 1;
                        /*int texIndex = int.Parse(indice[1], culture) - 2;
                        int normalIndex = int.Parse(indice[2], culture) - 1;#1#
                        indices.Add(vertexIndex);
                        /*vertices[vertexIndex].SetNormal(vertices[normalIndex].normal);
                        vertices[vertexIndex].SetTexture(vertices[texIndex].tex);#1#
                    }
                }
            }
        }

        return vertices;
    }*/
    /*private void ReadObj(string objFile)
    {
        using (var reader = new StreamReader(objFile))
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
                    vc.color = Color4.White;
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
            /*points.AddRange(points);
            var a = indices;
            indices.Reverse();
            indices.AddRange(a);#1#
        }
    }*/
}