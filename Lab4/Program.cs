using System;
using System.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Color = System.Drawing.Color;

namespace Lab4
{
    class Program
    {
        internal class Window3D : GameWindow
        {
            bool showShape = true;
            KeyboardState lastKeyPress;
            private const int XYZ_SIZE = 75;
            private bool axesControl = true;
            private int transStep = 0;
            private int radStep = 0;
            private int attStep = 0;

            private bool newStatus = false;
            private bool gradientColor = false;
            private bool resetColor = false;

            Random r = new Random();
            Color randomColor;
            Color c1, c2, c3;

            public void options()
            {
                Console.WriteLine("Exit: Escape key");
                Console.WriteLine("Mutati obiectul in plan: Up - Down - Left - Right");
                Console.WriteLine("Mutati obiectul pe inaltime: Shift - Ctrl");
                Console.WriteLine("Ascunde obiectul: P");
                Console.WriteLine("Creati o copie a obiectului: Z");
                Console.WriteLine("Schimba culoare fetei de sus: C");
                Console.WriteLine("Gradient culoare triunghi: V");
                Console.WriteLine("Resetare obiect: X");

            }

            private int[,] objVertices = {
            {5, 10, 5,
                10, 5, 10,
                5, 10, 5,
                10, 5, 10,
                5, 5, 5,
                5, 5, 5,
                5, 10, 5,
                10, 10, 5,
                10, 10, 10,
                10, 10, 10,
                5, 10, 5,
                10, 10, 5},
            {5, 5, 12,
                5, 12, 12,
                5, 5, 5,
                5, 5, 5,
                5, 12, 5,
                12, 5, 12,
                12, 12, 12,
                12, 12, 12,
                5, 12, 5,
                12, 5, 12,
                5, 5, 12,
                5, 12, 12},
            {6, 6, 6,
                6, 6, 6,
                6, 6, 12,
                6, 12, 12,
                6, 6, 12,
                6, 12, 12,
                6, 6, 12,
                6, 12, 12,
                6, 6, 12,
                6, 12, 12,
                12, 12, 12,
                12, 12, 12}};
            private Color[] colorVertices = { Color.White, Color.LawnGreen, Color.WhiteSmoke, Color.Tomato, Color.Turquoise, Color.OldLace, Color.DarkCyan, Color.PaleVioletRed, Color.IndianRed, Color.PeachPuff, Color.OrangeRed, Color.Gray };

            private Window3D() : base(800, 600, new GraphicsMode(32, 24, 0, 8))
            {
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                GL.ClearColor(Color.MidnightBlue);
                GL.Enable(EnableCap.DepthTest);
                GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                GL.Viewport(0, 0, Width, Height);

                double aspect_ratio = Width / (double)Height;

                Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref perspective);

                Matrix4 lookat = Matrix4.LookAt(30, 30, 30, 0, 0, 0, 0, 1, 0);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref lookat);

                showShape = true;
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);

                KeyboardState keyboard = Keyboard.GetState();

                //Exit
                if (keyboard[Key.Escape])
                {
                    Exit();
                    return;
                }

                //Ascunde obiect
                if (keyboard[Key.P] && !keyboard.Equals(lastKeyPress))
                {
                    if (showShape)
                    {
                        showShape = false;
                    }
                    else
                    {
                        showShape = true;
                    }
                }
                
                //Copiaza obiect
                if (keyboard[Key.Z] && !keyboard.Equals(lastKeyPress))
                {
                    if (newStatus)
                    {
                        newStatus = false;
                    }
                    else
                    {
                        newStatus = true;
                    }
                }

                //Schimba culuarea fetei de sus a cubului
                if (keyboard[Key.C] && !keyboard.Equals(lastKeyPress))
                {
                    randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                    colorVertices[6] = randomColor;
                    randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                    colorVertices[7] = randomColor;
                }

                //Schimba culuarea gradient
                if (keyboard[Key.V] && !keyboard.Equals(lastKeyPress))
                {
                    if (gradientColor == true)
                        gradientColor = false;
                    else
                        gradientColor = resetColor = true;
                }

                //Misca obiectul din sageti shift si control
                if (keyboard[Key.Up])
                {
                    transStep--;
                }
                if (keyboard[Key.Down])
                {
                    transStep++;
                }

                if (keyboard[Key.Right])
                {
                    radStep--;
                }
                if (keyboard[Key.Left])
                {
                    radStep++;
                }

                if (keyboard[Key.ShiftLeft])
                {
                    attStep++;
                }
                if (keyboard[Key.ControlLeft])
                {
                    attStep--;
                }

                //reseteaza obiectul
                if(keyboard[Key.X])
                {
                    attStep = 0;
                    radStep = -1;
                    transStep = 0;
                }
                lastKeyPress = keyboard;
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                base.OnRenderFrame(e);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (newStatus)
                {
                    DrawShape();
                }

                if (axesControl)
                {
                    DrawAxes();
                }

                if (showShape == true)
                {
                    GL.PushMatrix();
                    GL.Translate(transStep, attStep, radStep);
                    DrawShape();
                    GL.PopMatrix();
                }

                //GL.Flush();
                SwapBuffers();
            }

            private void DrawAxes()
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.Red);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(XYZ_SIZE, 0, 0);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.Yellow);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, XYZ_SIZE, 0); ;
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.Green);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, XYZ_SIZE);
                GL.End();
            }

            private void DrawShape()
            {
                
       
                if (gradientColor == false)
                {
                    Console.Clear();
                    options();
                    GL.Begin(PrimitiveType.Triangles);
                    for (int i = 0; i < 35; i = i + 3)
                    {
                        GL.Color3(colorVertices[i / 3]);
                        GL.Vertex3(objVertices[0, i], objVertices[1, i], objVertices[2, i]);
                        GL.Vertex3(objVertices[0, i + 1], objVertices[1, i + 1], objVertices[2, i + 1]);
                        GL.Vertex3(objVertices[0, i + 2], objVertices[1, i + 2], objVertices[2, i + 2]);
                    }
                    GL.End();
                    //Console.Clear();
                    
                }
                else
                {
                    if (resetColor == true)
                    {
                        GL.Begin(PrimitiveType.Triangles);

                        for (int i = 0; i < 32; i = i + 3)
                        {
                            GL.Color3(colorVertices[i / 3]);
                            GL.Vertex3(objVertices[0, i], objVertices[1, i], objVertices[2, i]);
                            GL.Vertex3(objVertices[0, i + 1], objVertices[1, i + 1], objVertices[2, i + 1]);
                            GL.Vertex3(objVertices[0, i + 2], objVertices[1, i + 2], objVertices[2, i + 2]);
                        }

                        Console.Clear();
                        options();
                        randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                        Console.WriteLine("Vertex1 \nR:" + randomColor.R + " G:" + randomColor.G + " B:" + randomColor.B);
                        GL.Color3(randomColor);
                        GL.Vertex3(objVertices[0, 32], objVertices[1, 32], objVertices[2, 32]);
                        c1 = randomColor;

                        randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                        Console.WriteLine("Vertex2 \nR:" + randomColor.R + " G:" + randomColor.G + " B:" + randomColor.B);
                        GL.Color3(randomColor);
                        GL.Vertex3(objVertices[0, 33], objVertices[1, 33], objVertices[2, 33]);
                        c2 = randomColor;

                        randomColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                        Console.WriteLine("Vertex3 \nR:" + randomColor.R + " G:" + randomColor.G + " B:" + randomColor.B);
                        GL.Color3(randomColor);
                        GL.Vertex3(objVertices[0, 34], objVertices[1, 34], objVertices[2, 34]);
                        c3 = randomColor;

                        GL.End();
                        resetColor = false;
                    }
                    else
                    {
                        GL.Begin(PrimitiveType.Triangles);

                        for (int i = 0; i < 32; i = i + 3)
                        {
                            GL.Color3(colorVertices[i / 3]);
                            GL.Vertex3(objVertices[0, i], objVertices[1, i], objVertices[2, i]);
                            GL.Vertex3(objVertices[0, i + 1], objVertices[1, i + 1], objVertices[2, i + 1]);
                            GL.Vertex3(objVertices[0, i + 2], objVertices[1, i + 2], objVertices[2, i + 2]);
                        }

                        

                        GL.Color3(c1);
                        GL.Vertex3(objVertices[0, 32], objVertices[1, 32], objVertices[2, 32]);

                        GL.Color3(c2);
                        GL.Vertex3(objVertices[0, 33], objVertices[1, 33], objVertices[2, 33]);

                        GL.Color3(c3);
                        GL.Vertex3(objVertices[0, 34], objVertices[1, 34], objVertices[2, 34]);

                        GL.End();
                        
                    }
                }
            }

            static void Main(string[] args)
            {
                
                using (Window3D example = new Window3D())
                {
                    example.Run(30.0, 0.0);

                }

            }
        }
    }
}