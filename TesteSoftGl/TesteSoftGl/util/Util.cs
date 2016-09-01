using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteSoftGl.util
{
    public static class Util
    {
        public static int Clamp(int valor, int minimo, int maximo)
        { return (valor > maximo ? maximo : (valor < minimo ? minimo : valor)); }

        public static float Clamp(float valor, float minimo, float maximo)
        { return (valor > maximo ? maximo : (valor < minimo ? minimo : valor)); }

        public static float Clamp(float valor)
        { return (valor > 1f ? 1f : (valor < 0f ? 0f : valor)); }

        public static int Max(int v1, int v2)
        { return (v2 > v1 ? v2 : v1); }

        public static int Min(int v1, int v2)
        { return (v2 < v1 ? v2 : v1); }

        public static float Max(float v1, float v2)
        { return (v2 > v1 ? v2 : v1); }

        public static float Min(float v1, float v2)
        { return (v2 < v1 ? v2 : v1); }






        public static int ToARGB(byte r, byte g, byte b)
        { return (255 << 24) | (r << 16) | (g << 8) | b; }

        public static int ToARGB(byte r, byte g, byte b, byte a)
        { return (a << 24) | (r << 16) | (g << 8) | b; }

        public static vec4 ToARGB(int cor)
        {
            float r = (((uint)(cor >> 16)) & 0x000000FF) / 255f;
            float g = (((uint)(cor >> 8)) & 0x000000FF) / 255f;
            float b = (((uint)(cor >> 0)) & 0x000000FF) / 255f;
            float a = (((uint)(cor >> 24)) & 0x000000FF) / 255f;
            return new vec4(r, g, b, a);
        }

        public static int ToARGB(vec4 color)
        {
            return ToARGB((byte)(color.x * 255), (byte)(color.y * 255), (byte)(color.z * 255), (byte)(color.w * 255));
        }

        public static int ToARGB(vec3 color)
        {
            return ToARGB((byte)(color.x * 255), (byte)(color.y * 255), (byte)(color.z * 255), 255);
        }



        public static vec4 FromARGB(int color)
        {
            return new vec4(
                ((color >> 16) & 0x00FF) / 255f,
                ((color >> 8) & 0x00FF) / 255f,
                ((color >> 0) & 0x00FF) / 255f,
                ((color >> 24) & 0x00FF) / 255f);
        }

        public static vec3 FromARGBv3(int color)
        {
            return new vec3(
                ((color >> 16) & 0x00FF) / 255f,
                ((color >> 8) & 0x00FF) / 255f,
                ((color >> 0) & 0x00FF) / 255f);
        }




    }
}
