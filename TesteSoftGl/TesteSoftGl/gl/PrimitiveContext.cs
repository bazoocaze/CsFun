using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteSoftGl.gl
{
    public class PrimitiveContext
    {
        public vec4[] bverts;
        public vec4[] bcolors;
        public vec2[] btc;

        public vec4[] v;
        public float[] w;

        public int baseIndex;

        public int iV0;
        public int iV1;
        public int iV2;

        public GlEngine3 Engine;

        public float Slope01;
        public float Slope02;
        public float Slope12;
        public float Slope21;

        public float dfd01;
        public float dfd12;
        public float dfd012;

        public float dfe02;
        public float dfe21;
        public float dfe021;

        public PrimitiveContext(GlEngine3 glEngine)
        {
            this.Engine = glEngine;
            v = new vec4[3];
            w = new float[4];
        }

        public void Reset()
        {
            iV0 = 0;
            iV1 = 1;
            iV2 = 2;
        }


    }
}
