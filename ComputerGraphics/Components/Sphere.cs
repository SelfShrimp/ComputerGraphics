using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Mathematics;

namespace ComputerGraphics.Components
{
    public class Sphere : GameComponent
    {
        public float radius = 0.1f;
        public int slices = 20;
        public int stacks = 20;

        public Sphere(Game game, float radius) : base(game)
        {
            this.radius = radius;
            /*points.Add(new Vector4(0,0,2f,1));
            points.Add(new Vector4(1,0,2f,1));
            points.Add(new Vector4(0,1,2f,1));
            points.Add(new Vector4(1,1,0,1));
            indices.Add(0);
            indices.Add(1);
            indices.Add(2);
            indices.Add(1);
            indices.Add(3);
            indices.Add(2);
            Init();
            return;*/
            float phiStep = MathUtil.Pi / stacks;
            float thetaStep = MathUtil.Pi * 2 / slices;

            for (int i = 0; i <= stacks; i++)
            {
                float phi = i * phiStep;

                // Vertices of ring
                for (int j = 0; j < slices; j++)
                {
                    float theta = j * thetaStep;

                    Vector4 v = new Vector4();
                    v.X = (float)(radius * Math.Sin(phi) * Math.Cos(theta));
                    v.Y = -(float)(radius * Math.Cos(phi));
                    v.Z = (float)(radius * Math.Sin(phi) * Math.Sin(theta));
                    v.W = 1;
                    
                    points.Add(v);
                }
            }

            // Connect the vertices between the rings
            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    indices.Add(i * (slices) + j);
                    indices.Add(i * (slices) + (j + 1) % slices);
                    indices.Add((i + 1) * (slices) + j);

                    indices.Add((i + 1) * (slices) + j);
                    indices.Add(i * (slices) + (j + 1) % slices);
                    indices.Add((i + 1) * (slices) + (j + 1) % slices);
                }
            }

            //GenerateSphere(out meshes);
            Init();
        }

        /*void GenerateSphere(out MyMesh sphereMesh)
        {
            sphereMesh = new MyMesh();
            sphereMesh.vertices = new List<MyVertex>();
            sphereMesh.indices = new List<int>();
            float dTheta = 2.0f * (float)Math.PI / slices;
            float dPhi = (float)Math.PI / stacks;
            float Phi = 0;
            var tempVertex = sphereMesh.vertices;
            tempVertex.Add(new MyVertex() {
                position = new Vector3(0, -radius, 0),
                normal = new Vector3(0, -1, 0),
            });
            
            for (int i = 1; i < stacks; i++) {
                float y = -radius * (float)Math.Cos(Phi + i * dPhi);
                float r = radius * (float)Math.Sin(Phi + i * dPhi);
                for (int j = 0; j <= slices; j++) {
                    float c = (float)Math.Cos(j * dTheta);
                    float s = (float)Math.Sin(j * dTheta);
                    MyVertex myVertex = new MyVertex();
                    myVertex.position = new Vector3(r * c, y, r * s);
                    myVertex.normal = new Vector3(0, 1f, 0);
                    tempVertex.Add(myVertex);
                }
                
            }
            tempVertex.Add(new MyVertex() {
                position = new Vector3(0, radius, 0),
                normal = new Vector3(0, 1, 0),
            });
            sphereMesh.vertices = tempVertex;
                
            int numsPerRing = slices + 1;
            List<int> tempIndice = sphereMesh.indices;
            tempIndice.Clear();
            int number = 0;
            for (int i = 0; i < slices; i++) {
                tempIndice.Add(0);
                tempIndice.Add(number + 1);
                tempIndice.Add(number + 2);
                number += 1;
            }
            for (int i = 0; i < stacks - 2; i++) {
                for (int j = 1; j <= slices; j++) {
                    tempIndice.Add(i * numsPerRing + j);
                    tempIndice.Add((i + 1) * numsPerRing + j);
                    tempIndice.Add((i + 1) * numsPerRing + j + 1);
                    tempIndice.Add((i + 1) * numsPerRing + j + 1);
                    tempIndice.Add(i * numsPerRing + j + 1);
                    tempIndice.Add(i * numsPerRing + j);
                }
            }
            number = 0;
            for (int i = 0; i < slices; i++) {
                tempIndice.Add(sphereMesh.vertices.Count - 1);
                tempIndice.Add(sphereMesh.vertices.Count - 2 - number);
                tempIndice.Add(sphereMesh.vertices.Count - 3 - number);
                number += 1;
            }
            sphereMesh.indices = tempIndice;
        }*/
    }
}