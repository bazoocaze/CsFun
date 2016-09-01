using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TesteSoftGl.util;
using GlmSharp;

namespace TesteSoftGl.gl
{
    /*
     * 
     * http://www.codeproject.com/Tips/66909/Rendering-fast-with-GDI-What-to-do-and-what-not-to
     * 
    Rendering FAST:
pGraphics->SetCompositingMode( CompositingModeSourceCopy );
pGraphics->SetCompositingQuality( CompositingQualityHighSpeed );
pGraphics->SetPixelOffsetMode( PixelOffsetModeNone );
pGraphics->SetSmoothingMode( SmoothingModeNone );
pGraphics->SetInterpolationMode( InterpolationModeDefault );

pGraphics->DrawImage( RenderBitmap, 0, 0 );
     * */
    public class ColorBuffer : IDisposable
    {
        public int Width;
        public int Height;
        public int Stride;

        public int[] Pixels;

        protected Bitmap m_BitmapBuffer;
        protected Rectangle m_BitmapBufferSize;

        protected const PixelFormat c_PixelFormat = PixelFormat.Format32bppPArgb;
        protected const int PixelStride = 4;

        public ColorBuffer(int width, int height)
        {
            SetSize(width, height);
        }

        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            DisposeBitmapBuffer();

            m_BitmapBufferSize = new Rectangle(0, 0, Width, Height);
            m_BitmapBuffer = new Bitmap(width, height, c_PixelFormat);
            BitmapData data = m_BitmapBuffer.LockBits(m_BitmapBufferSize, ImageLockMode.ReadOnly, c_PixelFormat);
            m_BitmapBuffer.UnlockBits(data);
            this.Stride = data.Stride / PixelStride;
            Pixels = new int[Height * Stride];
        }

        private void DisposeBitmapBuffer()
        {
            if (m_BitmapBuffer != null)
            {
                m_BitmapBuffer.Dispose();
                m_BitmapBuffer = null;
            }
        }

        public void Fill(int color)
        {
            for (int x = 0; x < Width; x++)
                Pixels[x] = color;
            for (int y = 1; y < Height; y++)
                Array.Copy(Pixels, 0, Pixels, y * Stride, Width);
        }

        public void DrawTo(Graphics dest, int x, int y)
        {
            UpdateBitmapBuffer();
            Gdi32.DrawImage(dest, x, y, m_BitmapBuffer);
            // dest.DrawImageUnscaled(m_BitmapBuffer, x, y);
        }

        private void UpdateBitmapBuffer()
        {
            if (m_BitmapBuffer == null)
            {
                m_BitmapBuffer = new Bitmap(Width, Height, c_PixelFormat);
                m_BitmapBufferSize = new Rectangle(0, 0, Width, Height);
            }

            BitmapData data = m_BitmapBuffer.LockBits(m_BitmapBufferSize, ImageLockMode.WriteOnly, c_PixelFormat);
            if (Stride == data.Stride)
                Marshal.Copy(Pixels, 0, data.Scan0, this.Height * this.Stride);
            else
                for (int y = 0; y < Height; y++)
                    Marshal.Copy(Pixels, y * this.Stride, data.Scan0 + (y * data.Stride), data.Width);
            m_BitmapBuffer.UnlockBits(data);
        }

        public void SetPixel(int x, int y, int color)
        { Pixels[y * Stride + x] = color; }

        public int GetPixel(int x, int y)
        { return Pixels[y * Stride + x]; }

        public void Dispose()
        {
            DisposeBitmapBuffer();
        }
    }
}
