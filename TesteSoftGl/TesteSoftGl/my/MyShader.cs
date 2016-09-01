using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteSoftGl.gl;
using TesteSoftGl.util;

namespace TesteSoftGl.my
{
    public class MyShader : ShaderProgram
    {
        vec4 corVertice;

        public override void VertexShader()
        {
            gl_Position = GetAtributevec4(0);
            VaryingOut[0] = (vec4)GetAtributevec2(2);
            corVertice = GetAtributevec4(1); 
        }

        public override bool PixelShader()
        {
            vec2 textCoord = (vec2)VaryingIn[0];
            vec4 corTextura = GetTextura(0).GetTexel(ref textCoord);
            // gl_FragColor = new vec4(0f, 0f, 0f, 0f);
            gl_FragColor = Util.ToARGB(vec4.Mix(corVertice, corTextura, 0.5f));
            return true;
        }
    }
}
