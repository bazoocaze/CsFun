using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TesteSoftGl.util;

namespace TesteSoftGl.gl
{
    public class GlEngine3 : IDisposable
    {
        protected Control m_Control;
        protected Graphics m_Target;

        public Rectangle Viewport;
        protected Size ViewportMax;

        protected ColorBuffer BackBuffer;
        protected DepthBuffer DepthBuffer;

        protected PrimitiveContext m_PrimiteContext;

        public mat4 ModelMatrix;
        public mat4 ViewMatrix;
        public mat4 ProjectionMatrix;

        public bool bDrawWrireframe;
        public bool bDrawPrimitive;

        public Texture CurrentTexture;

        public float TextureBlendFactor;

        public GlEngine3()
        {
            Clear();
        }

        public void Clear()
        {
            ModelMatrix = mat4.Identity;
            ViewMatrix = mat4.Identity;
            ProjectionMatrix = mat4.Identity;

            DisposeBackBuffer();
            SetTarget(null);
            DepthBuffer = null;
            m_PrimiteContext = null;

            bDrawWrireframe = true;
            bDrawPrimitive = true;

            TextureBlendFactor = 0.75f;
        }

        public void SetTarget(Control control)
        {
            if (m_Target != null)
            {
                m_Target.Dispose();
                m_Target = null;
            }
            m_Control = control;
            if (m_Control != null)
            {
                m_Target = m_Control.CreateGraphics();
                ViewportMax = m_Control.ClientSize;
                SetViewport(0, 0, m_Control.ClientSize.Width, m_Control.ClientSize.Height);
            }
        }

        protected void DisposeBackBuffer()
        {
            if (BackBuffer != null)
            {
                BackBuffer.Dispose();
                BackBuffer = null;
            }
        }

        public void SetViewport(int x, int y, int width, int height)
        {
            x = glm.Clamp(x, 0, ViewportMax.Width - 1);
            y = glm.Clamp(y, 0, ViewportMax.Height - 1);
            width = glm.Clamp(width, 1, ViewportMax.Width - x);
            height = glm.Clamp(height, 1, ViewportMax.Height - y);

            DisposeBackBuffer();

            this.Viewport = new Rectangle(x, y, width, height);
            this.BackBuffer = new ColorBuffer(Viewport.Width, Viewport.Height);
            this.DepthBuffer = new DepthBuffer(Viewport.Width, Viewport.Height);
        }

        public void glClear(Color cor)
        {
            if (BackBuffer != null)
                BackBuffer.Fill(cor.ToArgb());
            DepthBuffer.Fill(float.PositiveInfinity);
        }

        public void FixedTransform(PrimitiveContext ctx, vec4[] in_vertices, int indexBase, int indexTo)
        {
            vec4 ret = ModelMatrix * in_vertices[indexBase];
            ret = ViewMatrix * ret;
            ret = ProjectionMatrix * ret;
            ctx.w[indexTo] = ret.w;
            if (ret.w != 0) ret = ret / ret.w;
            ret.x = (ret.x + 1f) * Viewport.Width * 0.5f;
            ret.y = (1f - ret.y) * Viewport.Height * 0.5f;
            ctx.v[indexTo] = ret;
        }

        public void glDrawPrimitives(int baseIndex, int numPrimitives, vec4[] in_vertices, vec4[] in_colors, vec2[] in_textCoords)
        {
            int primitiveSize = 3;
            int index = baseIndex;

            if (m_PrimiteContext == null) m_PrimiteContext = new PrimitiveContext(this);

            m_PrimiteContext.bverts = in_vertices;
            m_PrimiteContext.bcolors = in_colors;
            m_PrimiteContext.btc = in_textCoords;

            for (int primitive = 0; primitive < numPrimitives; primitive++)
            {
                m_PrimiteContext.Reset();
                m_PrimiteContext.baseIndex = index;

                for (int v = 0; v < primitiveSize; v++)
                    FixedTransform(m_PrimiteContext, in_vertices, index + v, v);

                if (bDrawPrimitive)
                    DrawTriangule(m_PrimiteContext);

                if (bDrawWrireframe)
                    DrawTrianguleWireframe(m_PrimiteContext);

                index += primitiveSize;
            }
        }

        private void DrawTriangule(PrimitiveContext ctx)
        {
            int itop = 0;
            if (ctx.v[ctx.iV1].y < ctx.v[itop].y) itop = ctx.iV1;
            if (ctx.v[ctx.iV2].y < ctx.v[itop].y) itop = ctx.iV2;

            ctx.iV0 = itop;
            ctx.iV1 = (itop + 1) % 3;
            ctx.iV2 = (itop + 2) % 3;

            ctx.Slope01 = GetSlope(ctx.v, ctx.iV0, ctx.iV1);
            ctx.Slope02 = GetSlope(ctx.v, ctx.iV0, ctx.iV2);
            ctx.Slope12 = GetSlope(ctx.v, ctx.iV1, ctx.iV2);
            ctx.Slope21 = GetSlope(ctx.v, ctx.iV2, ctx.iV1);

            if (ctx.Slope02 > ctx.Slope01)
            {
                int t = ctx.iV1;
                ctx.iV1 = ctx.iV2;
                ctx.iV2 = t;

                ctx.Slope01 = GetSlope(ctx.v, ctx.iV0, ctx.iV1);
                ctx.Slope02 = GetSlope(ctx.v, ctx.iV0, ctx.iV2);
                ctx.Slope12 = GetSlope(ctx.v, ctx.iV1, ctx.iV2);
                ctx.Slope21 = GetSlope(ctx.v, ctx.iV2, ctx.iV1);
            }

            int minY = (int)ctx.v[ctx.iV0].y;
            int maxY = (int)Math.Max(ctx.v[ctx.iV1].y, ctx.v[ctx.iV2].y);

            int startY = (minY < 0 ? 0 : minY);
            int endY = (maxY >= this.Viewport.Height ? this.Viewport.Height - 1 : maxY);

            float yv0 = ctx.v[ctx.iV0].y;
            float yv1 = ctx.v[ctx.iV1].y;
            float yv2 = ctx.v[ctx.iV2].y;

            float xv0 = ctx.v[ctx.iV0].x;
            float xv1 = ctx.v[ctx.iV1].x;
            float xv2 = ctx.v[ctx.iV2].x;

            bool bEqY01 = (yv0 == yv1);
            bool bEqY12 = (yv1 == yv2);
            bool bEqY20 = (yv2 == yv0);

            float dy12 = bEqY12 ? 0f : 1f / (yv1 - yv2);
            float dy20 = bEqY20 ? 0f : 1f / (yv2 - yv0);
            float dy21 = bEqY12 ? 0f : 1f / (yv2 - yv1);
            float dy10 = bEqY01 ? 0f : 1f / (yv1 - yv0);


            float xe;
            float xd;
            float delta;

            for (int y = startY; y <= endY; y++)
            {

                if ((!bEqY12) && (y > yv2 || bEqY20))
                {
                    xe = xv2 + ((y - yv2) * ctx.Slope21);
                    ctx.dfe021 = 1f;
                    delta = (y - yv2) * dy12;
                    ctx.dfe21 = glm.Clamp(delta, 0f, 1f);
                }
                else if (!bEqY20)
                {
                    xe = xv0 + ((y - yv0) * ctx.Slope02);
                    ctx.dfe021 = 0f;
                    delta = (y - yv0) * dy20;
                    ctx.dfe02 = glm.Clamp(delta, 0f, 1f);
                }
                else continue;


                if ((!bEqY12) && (y > yv1 || bEqY01))
                {
                    xd = xv1 + ((y - yv1) * ctx.Slope12);
                    ctx.dfd012 = 1f;
                    delta = (y - yv1) * dy21;
                    ctx.dfd12 = glm.Clamp(delta, 0f, 1f);
                }
                else if (!bEqY01)
                {
                    xd = xv0 + ((y - yv0) * ctx.Slope01);
                    ctx.dfd012 = 0f;
                    delta = (y - yv0) * dy10;
                    ctx.dfd01 = glm.Clamp(delta, 0f, 1f);
                }
                else continue;


                DrawScanLine(ctx, y, xe, xd);
            }
        }

        private float Linear2Homo(float a, float w0, float w1)
        { return (a * w0) / (w1 + (w0 - w1) * a); }

        private void DrawScanLine(PrimitiveContext ctx, int y, float xe, float xd)
        {
            int xmin = glm.Max((int)xe, 0);
            int xmax = glm.Min((int)xd, Viewport.Width);

            int ib = ctx.baseIndex;

            float w0 = ctx.w[ctx.iV0];
            float w1 = ctx.w[ctx.iV1];
            float w2 = ctx.w[ctx.iV2];

            vec4 C0 = ctx.bcolors[ib + ctx.iV0];
            vec4 C1 = ctx.bcolors[ib + ctx.iV1];
            vec4 C2 = ctx.bcolors[ib + ctx.iV2];

            vec2 T0 = ctx.btc[ib + ctx.iV0];
            vec2 T1 = ctx.btc[ib + ctx.iV1];
            vec2 T2 = ctx.btc[ib + ctx.iV2];

            float dn01 = Linear2Homo(ctx.dfd01, w0, w1);
            float dn12 = Linear2Homo(ctx.dfd12, w1, w2);
            float dn02 = Linear2Homo(ctx.dfe02, w0, w2);
            float dn21 = Linear2Homo(ctx.dfe21, w2, w1);

            float dr01 = 1f - dn01;
            float dr12 = 1f - dn12;
            float dr02 = 1f - dn02;
            float dr21 = 1f - dn21;

            float bn012 = ctx.dfd012;
            float bn021 = ctx.dfe021;
            float br012 = 1f - bn012;
            float br021 = 1f - bn021;

            float R0 = dr01 * br012;
            float R1 = dr02 * br021;
            float R23 = dn01 * br012 + dr12 * bn012;
            float R4 = dn21 * bn021;
            float R5 = dn12 * bn012;
            float R67 = dn02 * br021 + dr21 * bn021;

            float wLeft = w0 * R1 + w1 * R4 + w2 * R67;
            float wRight = w0 * R0 + w1 * R23 + w2 * R5;

            vec4 CE = C0 * R1 + C1 * R4 + C2 * R67;
            vec4 CD = C0 * R0 + C1 * R23 + C2 * R5;
            vec2 TE = T0 * R1 + T1 * R4 + T2 * R67;
            vec2 TD = T0 * R0 + T1 * R23 + T2 * R5;

            float zLeft = ctx.v[ctx.iV0].z * R1 + ctx.v[ctx.iV1].z * R4 + ctx.v[ctx.iV2].z * R67;
            float zRight = ctx.v[ctx.iV0].z * R0 + ctx.v[ctx.iV1].z * R23 + ctx.v[ctx.iV2].z * R5;

            vec4 CDE = CD - CE;
            vec2 TDE = TD - TE;

            float deltaDx = (xd != xe) ? 1f / (float)(xd - xe) : 0f;

            for (int x = xmin; x <= xmax; x++)
            {
                float dx = (float)(x - xe) * deltaDx;

                dx = Linear2Homo(dx, wLeft, wRight);

                float z = zLeft + (zRight - zLeft) * dx;
                if (z >= DepthBuffer.GetPoint(x, y)) continue;

                DepthBuffer.SetPoint(x, y, z);

                vec4 vertexColor = CE + (CDE * dx);
                vec2 textureCoord = TE + (TDE * dx);

                vec4 textureColor = CurrentTexture.GetTexel(textureCoord.x, textureCoord.y);

                vec4 color = vec4.Mix(vertexColor, textureColor, TextureBlendFactor);

                DrawPoint(x, y, Util.ToARGB(color));
            }
        }


        private float GetSlope(vec4[] v, int i0, int i1)
        {
            float dy = v[i1].y - v[i0].y;
            float dx = v[i1].x - v[i0].x;
            return glm.Clamp(dx / dy, -1000f, +1000f);
        }

        private void DrawTrianguleWireframe(PrimitiveContext context)
        {
            int[,] edges = { { 0, 1 }, { 1, 2 }, { 2, 0 } };

            vec4 cor = new vec4(0f, 1f, 0f, 1f);

            for (int e = 0; e < 3; e++)
            {
                vec4 v0 = context.v[edges[e, 0]];
                vec4 v1 = context.v[edges[e, 1]];
                vec4 diff = (v0 - v1);
                int maxPoints = glm.Max(10, (int)(Math.Abs(diff.x) + Math.Abs(diff.y) + 1));
                for (int valor = 0; valor < maxPoints; valor++)
                {
                    float alpha = ((float)valor / (float)maxPoints);
                    vec4 ponto = vec4.Mix(v0, v1, alpha);
                    DrawPoint(ponto, Util.ToARGB(cor));
                }
            }
        }

        private void DrawPoint(int x, int y, int color)
        {
            BackBuffer.SetPixel(x, y, color);
        }

        private void DrawPoint(vec4 ponto, int color)
        {
            int x = (int)ponto.x;
            int y = (int)ponto.y;
            if (x < 0 || y < 0 || x >= Viewport.Width || y >= Viewport.Height) return;
            BackBuffer.SetPixel(x, y, color);
        }

        public void SwapBuffers()
        {
            if (m_Target == null) return;
            glFinish();
            BackBuffer.DrawTo(m_Target, Viewport.X, Viewport.Y);
        }

        private void glFinish()
        {
        }

        public void Dispose()
        {
            DisposeBackBuffer();
        }

    }
}
