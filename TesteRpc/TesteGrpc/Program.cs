using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grpc.Core;
using Teste.Protos;
using static Teste.Protos.SearchService;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TesteGrpc
{
    static class Program
    {
        public const int P_PORTA_RPC = 12345;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TesteChannel();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormTeste());
        }

        public static void TesteChannel()
        {
            Server server = new Server();

            var serverPort = new ServerPort("0.0.0.0", P_PORTA_RPC, ServerCredentials.Insecure);
            server.Ports.Add(serverPort);

            var searchService = SearchService.BindService(new MyService());
            server.Services.Add(searchService);
            server.Start();

            Channel channel = new Channel("localhost", P_PORTA_RPC, ChannelCredentials.Insecure);
            channel.ConnectAsync(DateTime.UtcNow.AddSeconds(4)).Wait();
            var client = new SearchServiceClient(channel);

            SearchRequest req = new SearchRequest();
            req.FileMask = "*.bat";
            req.StartDir = @"C:\";
            req.Recursive = true;
            req.IgnoreErrors = true;
            var result = client.Search(req);

        }

        public static void TesteSerializeException()
        {
            // var target = new MyTeste();
            // target.Id = 123;
            // target.Nome = "José";

            // var target = new FileNotFoundException(null, "teste.txt");

            var target = new Dictionary<string, int>();
            target.Add("alfa", 1);
            target.Add("charlie", 3);

            DebugFormatter fmt = new DebugFormatter();
            MemoryStream ms = new MemoryStream();
            fmt.Serialize(ms, target);
        }
    }

    public class MyService : SearchServiceBase
    {
        public MyService()
        {

        }

        public override Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            return Util.RpcReturnAsync(SearchSync, request, context);
        }

        private SearchResponse SearchSync(SearchRequest request, ServerCallContext context)
        {
            SearchResponse resp = new SearchResponse();
            string ret = Localizar(request, request.StartDir);
            if (ret != null)
            {
                resp.Found = true;
                resp.FileName = ret;
            }
            return resp;
        }

        private string Localizar(SearchRequest req, string currentDir)
        {
            try
            {
                string[] aFile = Directory.GetFiles(currentDir, req.FileMask, SearchOption.TopDirectoryOnly);
                foreach (string file in aFile)
                {
                    return Path.Combine(currentDir, file);
                }
            }
            catch (UnauthorizedAccessException) when (req.IgnoreErrors) { }
            catch (DirectoryNotFoundException) when (req.IgnoreErrors) { return null; }

            if (!req.Recursive) return null;

            try
            {
                string[] aDir = Directory.GetDirectories(currentDir);
                foreach (string dir in aDir)
                {
                    string ret = Localizar(req, Path.Combine(currentDir, dir));
                    if (ret != null) return ret;
                }
            }
            catch (UnauthorizedAccessException) when (req.IgnoreErrors) { }

            return null;
        }
    }
}
