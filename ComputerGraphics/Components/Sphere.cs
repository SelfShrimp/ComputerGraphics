using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Mathematics;

namespace ComputerGraphics.Components
{
    public class Sphere : GameComponent
    {
        public float radius = 0.1f;
        public int slices = 20;
        public int stacks = 20;
        public int distance;
        float angle = 0;
        private bool moon = false;
        private List<Sphere> moons = new List<Sphere>();

        public Sphere(Game game, float radius, float speed, bool moon=false) : base(game)
        {
            this.moon = moon;
            Random rnd = new Random();
            speedRotateY = rnd.NextFloat(0.01f, 0.02f);
            this.speed = speed;
            this.radius = radius;
            float phiStep = MathUtil.Pi / stacks;
            float thetaStep = MathUtil.Pi * 2 / slices;
            /*points = new List<VertexColor>
            {
                new VertexColor(new Vector4(-0.5f, 0.5f, 0.0f, 1), SharpDX.Color.Red),
                new VertexColor(new Vector4(0.5f, 0.5f, 0.0f, 1), SharpDX.Color.Green),
                new VertexColor(new Vector4(0.0f, -0.5f, 0.0f, 1), SharpDX.Color.Blue)
            };
            indices = new List<int>()
            {
                0,2,1
            };*/
            for (int i = 0; i <= stacks; i++)
            {
                float phi = i * phiStep;

                // Vertices of ring
                for (int j = 0; j < slices; j++)
                {
                    float theta = j * thetaStep;

                    VertexColor v = new VertexColor();
                    v.point.X = (float)(radius * Math.Sin(phi) * Math.Cos(theta));
                    v.point.Y = -(float)(radius * Math.Cos(phi));
                    v.point.Z = (float)(radius * Math.Sin(phi) * Math.Sin(theta));
                    v.point.W = 1;
                    if (j < slices / 2)
                    {
                        v.color = Color.Green;
                    }
                    else
                    {
                        v.color = Color.Yellow;
                    }
                    //v.color = new Color4(rnd.NextFloat(1,255),rnd.NextFloat(1,255),rnd.NextFloat(1,255),0);
                    points.Add(v);
                }
            }
            // Connect the vertices between the rings
            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    indices .Add(i * (slices) + j);
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

        public void AddMoon(int distance, int distanceForVector)
        {
            Sphere moon = new Sphere(game,0.2f,0.1f, true);
            moon.position = this.position - new Vector3(0, 0, distanceForVector);
            moon.distance = distance;
            moons.Add(moon);
        }

        public override void Draw()
        {
            foreach (var moon in moons)
            {
                moon.Draw();
            }
            base.Draw();
        }
        
        public override void Update()
        {
            foreach (var moon in moons)
            {
                var a = Vector3.Transform(moon.position - this.position, Matrix.RotationY((float)(Math.PI/ 18000) * moon.distance)) ;
                moon.position = new Vector3(a.X,a.Y,a.Z)+this.position;
                moon.Update();
                //moon.Move(this.position - new Vector3(0, 0, -2));
            }
            
            if (rotateY >= 6f)
                rotateY = 0f;
            rotateY += speedRotateY;
            /*if (position.X == 0f)
            {
                speedZ *= -1;
            }*/

            if (position.Z == 0f)
            {
                speed *= -1;
                /*if (speedX < 0)
                    speedX -= 0.1f;
                else
                    speedX += 0.1f;*/
            }

            /*if (Math.Abs(position.X) == Math.Abs(position.Z))
            {
                if (speedX < 0)
                    speedX += 0.1f;
                else
                    speedX -= 0.1f;
            }*/
            /*position.X = (float)Math.Round(position.X+speedX,2);
            position.Z = (float)Math.Round(position.Z + speedZ,2);*/
            //Matrix position = new Vector3(radius * Math.Cos(angle), 0, radius * Math.Sin(angle));

            if (!moon)
            {
                position.X = distance * (float)Math.Cos(angle);
                position.Z = distance * (float)Math.Sin(angle);
                angle += game.deltaTime * speed;
            }
            base.Update();
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