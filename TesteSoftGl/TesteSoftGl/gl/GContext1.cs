using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using TesteSoftGl.util;
using GlmSharp;

namespace TesteSoftGl.gl
{
    public class GContext1
    {
        public Control TargetControl;
        public Graphics TargetGraphics;

        public ColorBuffer BackBuffer;
        public DepthBuffer DepthBuffer;

        public Rectangle Viewport;
        public Size ViewportMax;

        public ShaderProgram ShaderProgram;

        public long CounterFragments;
        public long CounterTriangules;

        public vec4[][] Atributosvec4;
        public vec2[][] Atributosvec2;
        public Texture[] UniformTexturas;
        public bool[] VaryingEnable;
        public vec4[][] VaryingOut;
        public vec4[] VaryingIn;

        public GContext1()
        {
            ClearContext();
        }

        public const int G_MAX_VARYINGS = 16;
        public const int G_MAX_ATRIBUTOS = 16;
        public const int G_MAX_TEXTURAS = 16;
        public const int G_MAX_UNIFORMS = 16;

        public void EnableVarying(int index, bool enabled)
        {
            if (VaryingEnable == null)
            {
                VaryingEnable = new bool[G_MAX_VARYINGS];
                VaryingIn = new vec4[G_MAX_VARYINGS];
                VaryingOut = new vec4[3][];
                for (int i = 0; i < 3; i++)
                    VaryingOut[i] = new vec4[G_MAX_VARYINGS];
            }
            VaryingEnable[index] = enabled;
        }

        public void ClearContext()
        {
            Viewport = new Rectangle();
            TargetControl = null;
            TargetGraphics = null;
            BackBuffer = null;
            DepthBuffer = null;
        }

        public void SetViewport(int width, int height)
        {
            SetViewport(0, 0, width, height);
        }

        public void SetViewport(int x, int y, int width, int height)
        {
            x = Util.Clamp(x, 0, ViewportMax.Width - 2);
            y = Util.Clamp(y, 0, ViewportMax.Height - 2);
            width = Util.Clamp(width, 1, ViewportMax.Width - x);
            height = Util.Clamp(height, 1, ViewportMax.Height - y);

            Viewport.X = x;
            Viewport.Y = y;
            Viewport.Width = width;
            Viewport.Height = height;

            if (BackBuffer == null) BackBuffer = new ColorBuffer(width, height);
            else BackBuffer.SetSize(width, height);

            if (DepthBuffer == null) DepthBuffer = new gl.DepthBuffer(width, height);
            else DepthBuffer.SetSize(width, height);
        }

        public void SetControl(Control control)
        {
            TargetControl = control;
            if (control == null)
            {
                TargetGraphics = null;
                return;
            }

            TargetGraphics = control.CreateGraphics();
            ViewportMax = control.ClientSize;
            SetViewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);
        }

        public void Clear(Color clearColor, bool clearDepthBuffer)
        {
            int width = Viewport.Width;
            int height = Viewport.Height;

            if (BackBuffer != null)
                BackBuffer.Fill(clearColor.ToArgb());

            if (clearDepthBuffer && DepthBuffer != null)
                DepthBuffer.Fill(float.MaxValue);
        }

        public void glFlush()
        {
        }

        public void glFinish()
        {
        }

        public void wglSwapBuffers()
        {
            if (!IsValid) return;

            BackBuffer.DrawTo(TargetGraphics, Viewport.X, Viewport.Y);

            TargetGraphics.Flush();
        }

        public bool IsValid { get { return TargetGraphics != null; } }

        public void SetAttributeV4(int indexAtributo, vec4[] dados)
        {
            if (Atributosvec4 == null) Atributosvec4 = new vec4[G_MAX_ATRIBUTOS][];
            Atributosvec4[indexAtributo] = dados;
        }

        public void SetAttributeV2(int indexAtributo, vec2[] dados)
        {
            if (Atributosvec2 == null) Atributosvec2 = new vec2[G_MAX_ATRIBUTOS][];
            Atributosvec2[indexAtributo] = dados;
        }

        public void SetTextura(int indexTextura, Texture textura)
        {
            if (UniformTexturas == null) UniformTexturas = new Texture[G_MAX_TEXTURAS];
            UniformTexturas[indexTextura] = textura;
        }

        public void DrawTriangles(int baseIndex, int numTriangulos)
        {
            if (ShaderProgram == null) return;
            ShaderProgram.internal_context = this;

            vec4[] vertResult = new vec4[3];

            int vertexIndex = baseIndex;
            for (int t = 0; t < numTriangulos; t++)
            {
                for (int vert = 0; vert < 3; vert++)
                {
                    ShaderProgram.VaryingOut = VaryingOut[vert];
                    ShaderProgram.internal_vertexIndex = vertexIndex++;
                    ShaderProgram.VertexShader();
                    vertResult[vert] = ShaderProgram.gl_Position;
                }
                ProcessTriangle(vertResult);
            }

            ShaderProgram.internal_context = null;
        }

        private void ProcessTriangle(vec4[] vertices)
        {
            float mX = 0.5f * Viewport.Width;
            float mY = 0.5f * Viewport.Height;

            for (int v = 0; v < 3; v++)
            {
                vertices[v].x = (vertices[v].x + 1f) * mX;
                vertices[v].y = (vertices[v].y + 1f) * mY;
                // vertices[v].z = (vertices[v].x + 0.5f) / 2f * Viewport.Width;
            }
            // RasterizeBox(vertices);
            RasterizeTri(vertices);
        }

        private void RasterizeBox(vec4[] vertices)
        {
            CounterTriangules++;

            ShaderProgram.VaryingIn = VaryingIn;

            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            for (int v = 0; v < 3; v++)
            {
                minX = Util.Min(minX, vertices[v].x);
                minY = Util.Min(minY, vertices[v].y);
                maxX = Util.Max(maxX, vertices[v].x);
                maxY = Util.Max(maxY, vertices[v].y);
            }

            minX = Util.Clamp(minX, 0, Viewport.Width - 1);
            minY = Util.Clamp(minY, 0, Viewport.Height - 1);
            maxX = Util.Clamp(maxX, 0, Viewport.Width - 1);
            maxY = Util.Clamp(maxY, 0, Viewport.Height - 1);

            for (int y = (int)minY; y <= maxY; y++)
            {
                for (int x = (int)minX; x <= maxX; x++)
                {
                    float dpz = vertices[0].z;
                    if (dpz < DepthBuffer.GetPoint(x, y))
                    {
                        VaryingIn[0].x = ((float)x - minX) / (maxX - minX);
                        VaryingIn[0].y = ((float)y - minY) / (maxY - minY);

                        if (ShaderProgram.PixelShader())
                        {
                            CounterFragments++;
                            BackBuffer.SetPixel(x, y, ShaderProgram.gl_FragColor);
                            DepthBuffer.SetPoint(x, y, dpz);
                        }
                    }
                }
            }

        }

        // drawing line between 2 points from left to right
        // papb -> pcpd
        // pa, pb, pc, pd must then be sorted before
        void ProcessScanLine(int y, vec4 pa, vec4 pb, vec4 pc, vec4 pd)
        {
            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.y == pb.y or pc.y == pd.y, gradient is forced to 1
            var gradient1 = pa.y != pb.y ? (y - pa.y) / (pb.y - pa.y) : 1;
            var gradient2 = pc.y != pd.y ? (y - pc.y) / (pd.y - pc.y) : 1;

            int sx = (int)glm.Mix(pa.x, pb.x, gradient1);
            int ex = (int)glm.Mix(pc.x, pd.x, gradient2);

            if (ex < sx)
            {
                var temp = ex;
                ex = sx;
                sx = temp;
            }

            // starting Z & ending Z
            float z1 = glm.Mix(pa.z, pb.z, gradient1);
            float z2 = glm.Mix(pc.z, pd.z, gradient2);

            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);

                float dpz = glm.Mix(z1, z2, gradient);
                if (dpz < DepthBuffer.GetPoint(x, y))
                {
                    VaryingIn[0].x = 0;
                    VaryingIn[0].y = 0;
                    if (ShaderProgram.PixelShader())
                    {
                        CounterFragments++;
                        BackBuffer.SetPixel(x, y, ShaderProgram.gl_FragColor);
                        DepthBuffer.SetPoint(x, y, dpz);
                    }
                }
            }
        }


        public void RasterizeTri(vec4[] ver)
        {
            CounterTriangules++;

            ShaderProgram.VaryingIn = VaryingIn;

            // Sorting the points in order to always have this order on screen ver[0], ver[1] & ver[2]
            // with ver[0] always up (thus having the Y the lowest possible to be near the top screen)
            // then ver[1] between ver[0] & ver[2]
            if (ver[0].y > ver[1].y)
            {
                var temp = ver[1];
                ver[1] = ver[0];
                ver[0] = temp;
            }

            if (ver[1].y > ver[2].y)
            {
                var temp = ver[1];
                ver[1] = ver[2];
                ver[2] = temp;
            }

            if (ver[0].y > ver[1].y)
            {
                var temp = ver[1];
                ver[1] = ver[0];
                ver[0] = temp;
            }

            // inverse slopes
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing inverse slopes
            if (ver[1].y - ver[0].y > 0)
                dP1P2 = (ver[1].x - ver[0].x) / (ver[1].y - ver[0].y);
            else
                dP1P2 = 0;

            if (ver[2].y - ver[0].y > 0)
                dP1P3 = (ver[2].x - ver[0].x) / (ver[2].y - ver[0].y);
            else
                dP1P3 = 0;

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                for (var y = (int)ver[0].y; y <= (int)ver[2].y; y++)
                {
                    if (y < ver[1].y)
                    {
                        ProcessScanLine(y, ver[0], ver[2], ver[0], ver[1]);
                    }
                    else
                    {
                        ProcessScanLine(y, ver[0], ver[2], ver[1], ver[2]);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                for (var y = (int)ver[0].y; y <= (int)ver[2].y; y++)
                {
                    if (y < ver[1].y)
                    {
                        ProcessScanLine(y, ver[0], ver[1], ver[0], ver[2]);
                    }
                    else
                    {
                        ProcessScanLine(y, ver[1], ver[2], ver[0], ver[2]);
                    }
                }
            }
        }



    }
}
