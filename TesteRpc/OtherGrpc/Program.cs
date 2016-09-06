using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teste.Protos;
using static Teste.Protos.SearchService;

namespace OtherGrpc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TesteRpc();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormTeste());
        }

        public static void TesteRpc()
        {
            Server server = new Server();
            server.AddPort(new ServerPort("localhost", 12345));
            server.Services.Add(SearchService.BindService(new SearchServiceImpl()));
            server.Start();

            Channel channel = new Channel("localhost", 12345);
            SearchServiceClient cliente = new SearchServiceClient(channel);

            SearchRequest req = new SearchRequest();
            req.FileMask = "*.txt";

            var result = cliente.Search(req);
        }
    }
}
