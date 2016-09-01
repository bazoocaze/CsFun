using MyRpc.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teste.Protos;

namespace MyRpc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MyServer server = new MyServer();
            server.AddService(new MySearchService());
            server.Start();

            var canalCliente = SimpleClient.Connect("localhost", 12345);
            MySearchClient cliente = new MySearchClient(canalCliente);

            SearchRequest req = new SearchRequest();
            req.FileName = "*.bat";
            var resp = cliente.Search(req);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormTeste());
        }
    }
}
