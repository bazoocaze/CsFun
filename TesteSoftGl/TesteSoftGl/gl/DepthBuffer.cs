using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteSoftGl.gl
{
    public class DepthBuffer
    {
        protected float[] Points;

        public int Width;
        public int Height;
        public int Stride;

        public DepthBuffer(int width, int height)
        {
            SetSize(width, height);
        }

        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Stride = width;

            Points = new float[height * Stride];
        }

        public void Fill(float valor)
        {
            for (int x = 0; x < Width; x++)
                Points[x] = valor;
            for (int y = 1; y < Height; y++)
                Array.Copy(Points, 0, Points, y * Stride, Width);
        }

        public float GetPoint(int x, int y)
        {
            return Points[y * Stride + x];
        }

        public void SetPoint(int x, int y, float dpz)
        {
            Points[y * Stride + x] = dpz;
        }
    }
}
