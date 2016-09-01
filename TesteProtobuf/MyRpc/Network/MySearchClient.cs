using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teste.Protos;

namespace MyRpc.Network
{
    public class MySearchClient : MyClientBase
    {
        public MySearchClient(SimpleClient canal) : base("search_service", canal)
        {
            AddMethod(MyMethodDefinition<SearchRequest, SearchResponse>.Create("search"));
            this.Start();
        }

        public SearchResponse Search(SearchRequest request)
        {
            MyRequestHandler<SearchRequest, SearchResponse> handler = new MyRequestHandler<SearchRequest, SearchResponse>();
            this.AddPendingRequest(handler);
            return handler.Send(this.m_Canal, this.ServiceId, "search", request);
        }
    }
}
