using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using TesteSoftGl.util;
using GlmSharp;

namespace TesteSoftGl.gl
{
    public class GContext2
    {
        private const int G_MAX_TEXTURAS = 8;


        public ColorBuffer BackBuffer;


        public Control TargetControl;
        public Graphics TargetGraphics;

        public Rectangle ViewPort;

        public Texture[] Texturas;

        public bool bWireframe = true;



        protected mat4 m_ModelMatrix;
        protected mat4 m_ViewMatrix;
        protected mat4 m_ProjectionMatrix;

        protected mat4 m_MV;
        protected mat4 m_MVP;

        protected bool mb_MV;
        protected bool mb_MVP;


        public mat4 ModelMatrix { get { return m_ModelMatrix; } set { m_ModelMatrix = value; mb_MV = false; mb_MVP = false; } }
        public mat4 ViewMatrix { get { return m_ViewMatrix; } set { m_ViewMatrix = value; mb_MV = false; mb_MVP = false; } }
        public mat4 ProjectionMatrix { get { return m_ProjectionMatrix; } set { m_ProjectionMatrix = value; mb_MV = false; mb_MVP = false; } }


        public void SetTarget(Control control)
        {
            TargetControl = control;
            TargetGraphics = control.CreateGraphics();

            TargetGraphics.CompositingMode = CompositingMode.SourceCopy;
            TargetGraphics.CompositingQuality = CompositingQuality.HighSpeed;
            TargetGraphics.PixelOffsetMode = PixelOffsetMode.None;
            TargetGraphics.SmoothingMode = SmoothingMode.None;
            TargetGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            // TargetGraphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            SetViewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);
        }


        public void SetViewport(int x, int y, int width, int height)
        {
            ViewPort = new Rectangle(x, y, width, height);
            if (BackBuffer == null)
                BackBuffer = new ColorBuffer(width, height);
            else
                BackBuffer.SetSize(width, height);
        }


        public void SetTextura(int i, Texture textura)
        {
            if (Texturas == null) Texturas = new Texture[G_MAX_TEXTURAS];
            Texturas[i] = textura;
        }


        public void Clear(Color color, bool clearDepthBuffer)
        {
            BackBuffer.Fill((int)((uint)color.ToArgb() | (uint)0xFF000000));
        }


        public void SwapBuffers()
        {
            glFinish();

            BackBuffer.DrawTo(TargetGraphics, ViewPort.X, ViewPort.Y);
            TargetGraphics.Flush();
        }


        private void glFinish()
        {

        }

        public mat4 GetMV()
        {
            if (!mb_MV)
            {
                m_MV = m_ViewMatrix * m_ModelMatrix;
                mb_MV = true;
            }
            return m_MV;
        }

        public mat4 GetMVP()
        {
            if (!mb_MVP)
            {
                m_MVP = m_ProjectionMatrix * (m_ViewMatrix * m_ModelMatrix);
                mb_MVP = true;
            }
            return m_MVP;
        }

        public vec4 FixedTransform(int index, vec4[] in_vertices)
        {
            mat4 mvpMatrix = GetMVP();
            // vec4 ret = mvpMatrix * in_vertices[index];

            vec4 ret = m_ModelMatrix * in_vertices[index];
            ret = m_ViewMatrix * ret;
            ret = m_ProjectionMatrix * ret;

            if (ret.w != 0f)
            {
                float rw = ret.w;
                ret = ret / rw;
                ret.w = rw;
            }

            ret.x = (float)(ret.x + 1f) * (float)(ViewPort.Width - 1) * 0.5f;
            ret.y = (float)(ret.y + 1f) * (float)(ViewPort.Height - 1) * 0.5f;

            return ret;
        }


        public void DrawPrimitives(int index, int numPrimitives, vec4[] in_vertices, vec4[] in_colors, vec2[] in_textCoords)
        {
            int primitiveSize = 3;
            vec4[] transVertices = new vec4[3];
            for (int p = 0; p < numPrimitives; p++)
            {
                int vertBase = (primitiveSize * p) + index;
                for (int i = 0; i < 3; i++)
                    transVertices[i] = FixedTransform(vertBase + i, in_vertices);

                DrawTri(vertBase, transVertices, in_colors, in_textCoords);

                if (bWireframe)
                    DrawTriWireframe(vertBase, transVertices, in_colors, in_textCoords);
            }
        }


        protected void DrawTriWireframe(int baseIndex, vec4[] V, vec4[] in_colors, vec2[] in_textCoords)
        {
            int maxStep = 100;
            int step = 1;
            int color = Util.ToARGB(0, 255, 0);

            int[,] edges = { { 0, 1 }, { 0, 2 }, { 2, 1 } };
            for (int e = 0; e < 3; e++)
            {
                vec4 p0 = V[edges[e, 0]];
                vec4 p1 = V[edges[e, 1]];
                vec4 dif = vec4.Abs(p1 - p0);

                maxStep = (int)glm.Max(10, dif.x + dif.y) + 1;
                step = 1;
                float factor = 1f / (float)maxStep;

                for (int n = 0; n < maxStep; n += step)
                {
                    float a = (float)n * factor;
                    vec4 p = vec4.Mix(p0, p1, a);
                    DrawPixel((int)p.x, (int)p.y, color);
                }
            }
        }


        protected void DrawTri(int baseIndex, vec4[] V, vec4[] in_colors, vec2[] in_textCoords)
        {
            int vBaseI = 0;
            int itop = 0;

            if (V[vBaseI + 1].y < V[vBaseI + itop].y) itop = 1;
            if (V[vBaseI + 2].y < V[vBaseI + itop].y) itop = 2;

            int vi0 = vBaseI + itop;
            int vi1 = vBaseI + ((itop + 1) % 3);
            int vi2 = vBaseI + ((itop + 2) % 3);

            float dSlope02;
            float dSlope01;
            float dSlope21;
            float dSlope12;

            do
            {
                dSlope02 = GetSlope(V[vi0].x, V[vi2].x, V[vi0].y, V[vi2].y);
                dSlope01 = GetSlope(V[vi0].x, V[vi1].x, V[vi0].y, V[vi1].y);
                dSlope21 = GetSlope(V[vi2].x, V[vi1].x, V[vi2].y, V[vi1].y);
                dSlope12 = GetSlope(V[vi1].x, V[vi2].x, V[vi1].y, V[vi2].y);

                if (dSlope02 > dSlope01)
                {
                    return;
                    // BACK_FACE_CULLING
                    int tmp = vi2;
                    vi2 = vi1;
                    vi1 = tmp;
                    continue;
                }

                break;
            } while (true);

            /*
            if (float.IsInfinity(dSlope02) || float.IsNaN(dSlope02)) return;
            if (float.IsInfinity(dSlope01) || float.IsNaN(dSlope01)) return;
            if (float.IsInfinity(dSlope21) || float.IsNaN(dSlope21)) return;
            if (float.IsInfinity(dSlope12) || float.IsNaN(dSlope12)) return;
            */

            int minY = (int)V[vi0].y;
            int maxY = (int)Math.Max(V[vi2].y, V[vi1].y);

            int yDif20 = (int)(V[vi2].y - V[vi0].y);
            int yDif10 = (int)(V[vi1].y - V[vi0].y);
            int yDif21 = (int)(V[vi2].y - V[vi1].y);
            int yDif12 = (int)(V[vi1].y - V[vi2].y);

            float pe_x;
            float pd_x;

            int ny = 0;

            float delta02;
            float delta01;
            float delta21;
            float delta12;

            for (int y = minY; y <= maxY; y++)
            {

                // esquerda
                if (y < V[vi2].y)
                {
                    pe_x = V[vi0].x + (ny * dSlope02);
                    delta02 = (float)(y - minY) / (float)(yDif20);
                    delta21 = 0f;
                }
                else
                {
                    pe_x = V[vi2].x + (ny - yDif20) * dSlope21;
                    delta02 = 0f;
                    delta21 = (float)(y - V[vi2].y) / (float)(yDif12);
                }

                // direita
                if (y < V[vi1].y)
                {
                    pd_x = V[vi0].x + (ny * dSlope01);
                    delta01 = (float)(y - minY) / (float)(yDif10);
                    delta12 = 0f;
                }
                else
                {
                    pd_x = V[vi1].x + ((ny - yDif10) * dSlope12);
                    delta01 = 0f;
                    delta12 = (float)(y - V[vi1].y) / (float)(yDif21);
                }

                // DrawTriScanLine(y, pe_x, pd_x, color);
                DrawTriScanLine(y, pe_x, pd_x, vi0, vi1, vi2, delta02, delta01, delta21, delta12, baseIndex, in_colors, in_textCoords, V);

                ny++;
            }
        }

        private void DrawTriScanLine(int y, float pe_x, float pd_x, int cor)
        {
            int minX = (int)pe_x;
            int maxX = (int)pd_x;

            for (int x = minX; x <= maxX; x++)
                DrawPixel(x, y, cor);
        }

        private void DrawTriScanLine(int y, float pe_x, float pd_x, int vi0, int vi1, int vi2, float delta02, float delta01, float delta21, float delta12, int vertBaseInput, vec4[] colors, vec2[] textCords, vec4[] in_vertices)
        {
            int minX = (int)pe_x;
            int maxX = (int)pd_x;

            vec4 CE;
            vec4 CD;

            vec2 TE;
            vec2 TD;

            vec4 C0 = colors[vertBaseInput + vi0];
            vec4 C1 = colors[vertBaseInput + vi1];
            vec4 C2 = colors[vertBaseInput + vi2];

            float z0 = 1f / in_vertices[vi0].z;
            float z1 = 1f / in_vertices[vi1].z;
            float z2 = 1f / in_vertices[vi2].z;

            float ZE;
            float ZD;

            vec2 T0 = textCords[vertBaseInput + vi0];
            vec2 T1 = textCords[vertBaseInput + vi1];
            vec2 T2 = textCords[vertBaseInput + vi2];

            if (delta21 >= delta02)
            {
                CE = vec4.Mix(C2, C1, delta21);
                TE = vec2.Mix(T2, T1, delta21);
                ZE = glm.Mix(z2, z1, delta21);
            }
            else
            {
                CE = vec4.Mix(C0, C2, delta02);
                TE = vec2.Mix(T0, T2, delta02);
                ZE = glm.Mix(z0, z2, delta02);
            }

            if (delta12 >= delta01)
            {
                CD = vec4.Mix(C1, C2, delta12);
                TD = vec2.Mix(T1, T2, delta12);
                ZD = glm.Mix(z1, z2, delta12);
            }
            else
            {
                CD = vec4.Mix(C0, C1, delta01);
                TD = vec2.Mix(T0, T1, delta01);
                ZD = glm.Mix(z0, z1, delta01);
            }

            vec4 cPoint;
            vec4 cText;
            vec2 tText;

            int startX = minX;
            int endX = maxX;

            if (startX < 0) startX = 0;
            if (startX >= ViewPort.Width) return;

            if (endX < 0) return;
            if (endX >= ViewPort.Width) endX = ViewPort.Width - 1;

            for (int x = startX; x <= endX; x++)
            {
                float deltaX = 0;
                if (minX != maxX) deltaX = (float)(x - minX) / (float)(maxX - minX);

                cPoint = vec4.Mix(CE, CD, deltaX);

                float intZ = glm.Mix(ZE, ZD, deltaX);
                float invZ = glm.Mix(1f / ZE, 1f / ZD, deltaX);
                tText = vec2.Mix(TE * ZE, TD * ZD, deltaX);
                tText = tText * invZ;

                cText = Texturas[0].GetTexel(tText.x, tText.y);

                int c = Util.ToARGB(vec4.Mix(cPoint, cText, 0.33f));

                DrawPixel(x, y, c);
            }
        }

        private void DrawPixel(int x, int y, int color)
        {
            if (x >= 0 && x < ViewPort.Width && y >= 0 && y < ViewPort.Height)
                BackBuffer.Pixels[y * BackBuffer.Stride + x] = color;
        }

        private void GetSlope(float x1, float x2, float y1, float y2, out float dx, out float dy)
        {
            float rx = (x1 - x2) * (x1 - x2);
            float ry = (y1 - y2) * (y1 - y2);
            if (ry > rx)
            {
                dy = (y2 - y1) / Math.Abs(x1 - x2);
                dx = 1f;
            }
            else
            {
                dx = (x2 - x1) / Math.Abs(y1 - y2);
                dy = 1f;
            }
        }

        private float GetSlope(float x1, float x2, float y1, float y2)
        {
            float rx = (x1 - x2) * (x1 - x2);
            float ry = (y1 - y2) * (y1 - y2);
            if (ry > rx)
            {
                return (x2 - x1) / (y2 - y1);
            }
            else
            {
                return (x2 - x1) / Math.Abs(y1 - y2);
            }
        }

    }
}
