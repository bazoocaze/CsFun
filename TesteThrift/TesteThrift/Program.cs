using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

namespace TesteThrift
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            StartServer();
            TestClient();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        public static Task serverTask;

        public static void StartServer()
        {
            SearchServiceHandler handler = new SearchServiceHandler();
            SearchService.Processor processor = new SearchService.Processor(handler);

            TMultiplexedProcessor multiProcessor = new TMultiplexedProcessor();
            multiProcessor.RegisterProcessor("search_service", processor);

            TServerTransport serverTransport = new TServerSocket(9090);
            var a = new TFramedTransport.Factory();
            TServer server = new TThreadPoolServer(multiProcessor, serverTransport, new TFramedTransport.Factory(), new TBinaryProtocol.Factory());
            serverTask = Task.Factory.StartNew(server.Serve);
        }

        public static void TestClient()
        {
            TTransport transport = new TFramedTransport(new TSocket("localhost", 9090));
            TProtocol protocol = new TBinaryProtocol(transport);
            TMultiplexedProtocol mp = new TMultiplexedProtocol(protocol, "search_service");
            SearchService.Client client = new SearchService.Client(mp);

            transport.Open();

            SearchRequest req = new SearchRequest();
            req.FileMask = "*.bat";
            req.StartPath = "c:\\";
            req.IgnoreErrors = true;
            var result = client.Search(req);

            Debug.WriteLine("P10");

            client.Delay1(2000);
            client.Delay2(1000);

            Task t1 = Task.Factory.StartNew(() => { Debug.WriteLine("T1:begin"); client.Delay1(2000); Debug.WriteLine("T1:end"); });
            Task t2 = Task.Factory.StartNew(() => { Debug.WriteLine("T2:begin"); client.Delay2(1000); Debug.WriteLine("T2:end"); });

            Debug.WriteLine("P20");

            t2.Wait();

            Debug.WriteLine("P30");

            t1.Wait();

            Debug.WriteLine("P40");
        }
    }
}
