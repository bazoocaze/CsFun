using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteSoftGl.util;

namespace TesteSoftGl.gl
{
    public class Texture
    {
        public int Width;
        public int Height;
        public int Stride;

        public vec4[] Pixels;

        public Texture(int width, int height, int stride)
        {
            this.Stride = stride;
            this.Width = width;
            this.Height = height;
            Pixels = new vec4[Stride * Height];
        }

        public Texture(int width, int height) : this(width, height, width) { }

        public void SetPixel(int x, int y, int color)
        { Pixels[y * Stride + x] = Util.ToARGB(color); }

        public void SetPixel(int x, int y, vec4 color)
        { Pixels[y * Stride + x] = color; }

        public vec4 GetTexel(ref vec2 pos)
        { return GetTexel(pos.x, pos.y); }

        public vec4 GetTexel(float u, float v)
        {
            int iu = (int)(u * (Width));
            int iv = (int)(v * (Height));
            iu = glm.Clamp(iu, 0, Width - 1);
            iv = glm.Clamp(iv, 0, Height - 1);
            return Pixels[iv * Stride + iu];
        }

        public vec4 GetTexelDebug(float u, float v)
        {
            float ru = u * (float)(Width - 1);
            float rv = v * (float)(Height - 1);

            int err = 0;
            if (ru < 0f || ru >= (Width - 1)) err += 1;
            if (rv < 0f || rv >= (Height - 1)) err += 2;
            if (err == 1) return new vec4(0.5f, 0.5f, 0f, 1f);
            if (err == 2) return new vec4(0f, 0.5f, 0.5f, 1f);
            if (err == 3) return new vec4(0.5f, 0f, 0.5f, 1f);

            int iu = (int)Util.Clamp(0f, Width - 1, ru);
            int iv = (int)Util.Clamp(0f, Height - 1, rv);

            return Pixels[iv * Stride + iu];
        }

    }
}
