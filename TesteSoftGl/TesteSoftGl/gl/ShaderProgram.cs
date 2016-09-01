using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteSoftGl.gl
{
    public class ShaderProgram
    {
        public GContext1 internal_context;
        public int internal_vertexIndex;

        public vec4 gl_Position;
        public int gl_FragColor;
        public float gl_FragDepth;

        public vec4[] VaryingOut;
        public vec4[] VaryingIn;

        public vec4 GetAtributevec4(int indexAtributo)
        {
            return internal_context.Atributosvec4[indexAtributo][internal_vertexIndex];
        }

        public vec2 GetAtributevec2(int indexAtributo)
        {
            return internal_context.Atributosvec2[indexAtributo][internal_vertexIndex];
        }

        public Texture GetTextura(int indexTextura)
        {
            return internal_context.UniformTexturas[indexTextura];
        }

        public virtual void VertexShader()
        {
        }

        public virtual bool PixelShader()
        {
            return true;
        }
    }
}
