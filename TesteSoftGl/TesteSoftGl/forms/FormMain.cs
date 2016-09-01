using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using TesteSoftGl.gl;
using TesteSoftGl.my;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GlmSharp;
using TesteSoftGl.util;

namespace TesteSoftGl.forms
{
    public partial class FormMain : Form
    {
        private const int PRIMITIVE_COUNT = 3;

        GameLoop GameLoop;

        GlEngine3 m_Contexto1;
        GContext2 m_Contexto2;

        vec4[] assets_vertices;
        vec4[] assets_colors;
        vec2[] assets_textCoords;
        Texture assets_textura1;

        int rotY = 0;


        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            InitEngine();
        }

        private void InitEngine()
        {
            m_Contexto1 = new GlEngine3();
            m_Contexto1.SetTarget(panelCanvas);
            // m_contexto.SetViewport(10, 10, 100, 100);

            m_Contexto2 = new GContext2();
            m_Contexto2.SetTarget(panelCanvas);

            GameLoop = new TesteSoftGl.GameLoop();
            GameLoop.Init();
            GameLoop.GameUpdate = GameUpdate;
            GameLoop.GameRender = GameRender;
            GameLoop.GameInfo = GameInfo;

            CriarAssets();

            m_Contexto1.CurrentTexture = assets_textura1;
            m_Contexto2.SetTextura(0, assets_textura1);
        }

        private void CriarAssets()
        {
            assets_vertices = new vec4[] {
                new vec4(-5f, -2f, 5f, 1f),
                new vec4(5f, -2f, 5f, 1f),
                new vec4(5f, -2f, -5f, 1f),

                new vec4(-5f, -2f, 5f, 1f),
                new vec4(5f, -2f, -5f, 1f),
                new vec4(-5f, -2f, -5f, 1f),

                new vec4(-5f, -1f, 5f, 1f),
                new vec4(5f, -1f, -5f, 1f),
                new vec4(-5f, -1f, -5f, 1f),
            };

            assets_colors = new vec4[] {
                new vec4(1f, 0f, 0f, 1f),
                new vec4(0f, 1f, 0f, 1f),
                new vec4(0f, 0f, 1f, 1f),

                new vec4(1f, 0f, 0f, 1f),
                new vec4(0f, 0f, 1f, 1f),
                new vec4(1f, 1f, 0f, 1f),

                new vec4(1f, 0f, 0f, 1f),
                new vec4(0f, 0f, 1f, 1f),
                new vec4(1f, 1f, 0f, 1f),
            };

            assets_textCoords = new vec2[] {
                new vec2(0f, 0f),
                new vec2(1f, 0f),
                new vec2(1f, 1f),

                new vec2(0f, 0f),
                new vec2(1f, 1f),
                new vec2(0f, 1f),

                new vec2(0f, 0f),
                new vec2(1f, 1f),
                new vec2(0f, 1f),
            };

            int textureSize = 16;
            int textureStep = 2;
            int borderSize = 1;

            assets_textura1 = new Texture(textureSize, textureSize);
            for (int y = 0; y < assets_textura1.Height; y++)
            {
                for (int x = 0; x < assets_textura1.Width; x++)
                {
                    // quadriculado
                    int c = ((x / textureStep) ^ (y / textureStep)) % 2;

                    if (c != 0)
                        c = Util.ToARGB(255, 255, 255);
                    else
                        c = Util.ToARGB(0, 0, 0);

                    // bordas
                    if (y < borderSize || y >= (textureSize - borderSize) || x < borderSize || x >= (textureSize - borderSize))
                        c = Util.ToARGB(127, 127, 127);

                    if (y < borderSize) c = Util.ToARGB(255, 0, 0);
                    if (y >= (textureSize - borderSize)) c = Util.ToARGB(0, 0, 255);
                    if (x >= (textureSize - borderSize)) c = Util.ToARGB(255, 0, 255);

                    assets_textura1.SetPixel(x, y, c);
                }
            }
        }

        private void MyRender1()
        {
            m_Contexto1.glClear(Color.DarkGray);

            m_Contexto1.glDrawPrimitives(0, PRIMITIVE_COUNT, assets_vertices, assets_colors, assets_textCoords);

            // mat4 save = m_Contexto1.ModelMatrix;
            // m_Contexto1.ModelMatrix = mat4.Identity;
            // m_Contexto1.glDrawPrimitives(3, 1, assets_vertices, assets_colors, assets_textCoords);
            // m_Contexto1.ModelMatrix = save;

            m_Contexto1.SwapBuffers();
        }

        private void MyRender2()
        {
            m_Contexto2.Clear(Color.DarkGray, true);
            m_Contexto2.DrawPrimitives(0, PRIMITIVE_COUNT, assets_vertices, assets_colors, assets_textCoords);
            m_Contexto2.SwapBuffers();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            GameLoop.Enabled = true;
        }

        private void GameRender()
        {
            if (radioGContext1.Checked)
                MyRender1();
            if (radioGContext2.Checked)
                MyRender2();
        }

        private void GameUpdate()
        {
            if (radioGContext1.Checked)
                MyUpdater1();
            if (radioGContext2.Checked)
                MyUpdater2();
        }




        private void MyUpdater1()
        {
            if (checkRotate.Checked)
            {
                int maxRotY = 180;
                float frotY = ((float)rotY++ / maxRotY) * (float)(2f * Math.PI);
                if (rotY >= maxRotY) rotY = 0;
                m_Contexto1.ModelMatrix = mat4.RotateY(frotY);
            }

            vec3 eye = new vec3(0f, 5f, 10f);
            vec3 dir = new vec3(0f, -1f, -1f);
            m_Contexto1.ViewMatrix = mat4.LookAt(eye, eye + dir, vec3.UnitY);

            float ratio = (float)m_Contexto1.Viewport.Width / m_Contexto1.Viewport.Height;

            // m_Contexto1.ProjectionMatrix = mat4.Ortho(-10f * ratio, 10f * ratio, -10f, 10f, 0.1f, 100f);
            m_Contexto1.ProjectionMatrix = mat4.PerspectiveFov((float)Math.PI / 3f, panelCanvas.ClientSize.Width, panelCanvas.ClientSize.Height, 0.1f, 100f);
        }

        private void MyUpdater2()
        {
            if (checkRotate.Checked)
            {
                int maxRotY = 180;
                float frotY = ((float)rotY++ / maxRotY) * (float)(2f * Math.PI);
                if (rotY >= maxRotY) rotY = 0;
                m_Contexto2.ModelMatrix = mat4.RotateY(frotY);
            }

            vec3 eye = new vec3(0f, 5f, 10f);
            vec3 dir = new vec3(0f, -1f, -1f);
            m_Contexto2.ViewMatrix = mat4.LookAt(eye, eye + dir, vec3.UnitY);

            float ratio = (float)m_Contexto2.ViewPort.Width / m_Contexto2.ViewPort.Height;

            // m_Contexto1.ProjectionMatrix = mat4.Ortho(-10f * ratio, 10f * ratio, -10f, 10f, 0.1f, 100f);
            m_Contexto2.ProjectionMatrix = mat4.PerspectiveFov((float)Math.PI / 3f, panelCanvas.ClientSize.Width, panelCanvas.ClientSize.Height, 0.1f, 100f);


            // vec3 camPos = new vec3(0f, 0f, 10f);
            // vec3 camDir = new vec3(0f, 0f, -1f);
            // m_Contexto2.ViewMatrix = mat4.LookAt(camPos, camPos + camDir, vec3.UnitY);
            // m_Contexto2.ViewMatrix = mat4.Identity;
            // m_Contexto2.ViewMatrix = mat4.Translate(0.4f, 0.1f, 0);


            // m_Contexto2.ProjectionMatrix = mat4.Ortho(-1f, 1f, -1f, 1f);
            // m_Contexto2.ProjectionMatrix = mat4.PerspectiveFov((float)Math.PI / 1.5f, 320f, 200f, 0.1f, 100f);
            // m_Contexto2.ProjectionMatrix = mat4.Identity;
        }

        private void GameInfo()
        {
            float fps = GameLoop.FrameCount * 1000f / GameLoop.LastInfoMilis;
            float frameTime = (float)GameLoop.LastInfoMilis / Math.Max(GameLoop.FrameCount, 1);
            labelInfo.Text = String.Format("Frames:{0}  FPS:{1}  FrameTime:{2:0.00}ms  Elapsed:{3}ms  Events:{4}", GameLoop.FrameCount, fps, frameTime, GameLoop.LastInfoMilis, GameLoop.WinEvents);
            if (checkEnableFps.Checked)
                GameLoop.FpsTarget = 30;
            else
                GameLoop.FpsTarget = 0;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            GameLoop.Enabled = false;
        }

        private void buttonSwap_Click(object sender, EventArgs e)
        {
            if (radioGContext1.Checked)
                m_Contexto1.SwapBuffers();
            if (radioGContext2.Checked)
                m_Contexto2.SwapBuffers();
        }

        private void buttonOneFrame_Click(object sender, EventArgs e)
        {
            if (radioGContext1.Checked)
                MyRender1();
            if (radioGContext2.Checked)
                MyRender2();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void buttonTeste_Click(object sender, EventArgs e)
        {
            vec3 eye = new vec3(0f, 0f, 10f);
            vec3 target = new vec3(0f, 0f, -0.9f);
            vec3 eye1 = new vec3(0f, 0f, 9f);

            mat4 mc0 = mat4.LookAt(eye, eye + target, vec3.UnitY);
            mat4 mc1 = mat4.LookAt(eye1, eye1 + target, vec3.UnitY);

            mat4 mp = mat4.PerspectiveFov((float)Math.PI / 2f, 320, 200, 0.1f, 100f);

            vec4 v0 = new vec4(10f, 10f, -10f, 1f);
            vec4 v1 = new vec4(10f, 10f, -10f, 1f);
            vec4 v2 = new vec4(10f, 10f, -10f, 1f);
            vec4 v3 = new vec4(1f, 1f, -100f, 1f);
            vec4 v4 = new vec4(1f, 1f, -0.1f, 1f);

            vec4 r0 = mc1 * v0;
            vec4 r1 = mp * v1;
            vec4 r2 = mp * (mc1 * v2);
            vec4 r3 = mp * v3;
            vec4 r4 = mp * v4;

            vec4 rc0 = r0 / r0.w;
            vec4 rc1 = r1 / r1.w;
            vec4 rc2 = r2 / r2.w;
            vec4 rc3 = r3 / r3.w;
            vec4 rc4 = r4 / r4.w;

        }
    }
}
