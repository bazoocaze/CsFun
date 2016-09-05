using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Teste.Protos;

namespace TesteGrpc
{
    public class MyTeste
    {
        public void Teste()
        {
            Server server = new Server();
            server.Ports.Add("localhost", 12345, ServerCredentials.Insecure);
            server.Services.Add(SearchService.BindService(new MyService()));
            server.Start();
        }
    }
}
