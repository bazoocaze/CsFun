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
using System.Diagnostics;
using System.Threading;

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

        public static async void TesteChannel()
        {
            Server server = new Server();

            var serverPort = new ServerPort("127.0.0.1", P_PORTA_RPC, ServerCredentials.Insecure);
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

            Debug.WriteLine("[p1]");

            var t1 = client.Delay1Async(new DelayDesc() { MilliSeconds = 2000 });

            Debug.WriteLine("[p2]");

            Debug.WriteLine("[p3]");

            var t2 = client.Delay2Async(new DelayDesc() { MilliSeconds = 1000 });

            Debug.WriteLine("[p4]");

            Empty v1 = await t1;

            Debug.WriteLine("[p5]");

            Empty v2 = await t2;

            Debug.WriteLine("[p6]");

            ProcessStartInfoPB psi = new ProcessStartInfoPB();
            psi.FileName = "c:/temp/teste.bat";
            var p = client.RunCmd(psi);
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

        public override Task<Empty> Delay1(DelayDesc request, ServerCallContext context)
        {
            return Task.Run<Empty>(delegate ()
            {
                Debug.WriteLine("server:Delay1 begin");
                Thread.Sleep(request.MilliSeconds);
                Debug.WriteLine("server:Delay1 end");
                return new Empty();
            });
        }

        public override Task<Empty> Delay2(DelayDesc request, ServerCallContext context)
        {
            return Task.Run<Empty>(delegate ()
            {
                Debug.WriteLine("server:Delay2 begin");
                Thread.Sleep(request.MilliSeconds);
                Debug.WriteLine("server:Delay2 end");
                return new Empty();
            });
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

        public override Task<ProcessPB> RunCmd(ProcessStartInfoPB request, ServerCallContext context)
        {
            return Task.Run<ProcessPB>(delegate ()
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = request.FileName;
                psi.Arguments = (String.Join(" ", request.Arguments.ToArray())).Trim();
                ProcessPB ret = new ProcessPB();
                ret.Started = false;
                try
                {
                    using (Process p = Process.Start(psi))
                    {
                        ret.Pid = p.Id;
                        ret.Started = true;
                    }
                }
                catch (FileNotFoundException) { }
                return ret;
            });
        }
    }
}
