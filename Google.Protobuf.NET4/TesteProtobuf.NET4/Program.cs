using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teste.Protos;

namespace TesteProtobufNet4
{
    static class Program
    {
        private const string P_FILE_NAME = "c:/temp/teste.dat";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var output = File.Create(P_FILE_NAME))
            {
                MyMessage m = new MyMessage();
                m.Id = 123;
                m.Nome = "José";
                CodedOutputStream outs = new CodedOutputStream(output);
                m.WriteTo(outs);
                outs.Flush();
            }

            using (var input = File.OpenRead(P_FILE_NAME))
            {
                MyMessage m = new MyMessage();
                m.MergeFrom(new CodedInputStream(input));
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
