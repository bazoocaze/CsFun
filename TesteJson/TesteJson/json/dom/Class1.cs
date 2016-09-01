using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TesteObjetoDinamico.json.dom
{
    public class Class1
    {
        public void Teste()
        {
            JsonObject obj = new JsonObject();
            obj.Add("nome", "jose");

            JsonVector vect = new JsonVector();
            vect.Add(10);
            vect.Add(11);

            obj.Add("vetor", vect);

            Debug.WriteLine("OBJ: {0}", obj, null);
        }
    }
}
