using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teste.Protos;

namespace MyRpc.Network
{
    public class MySearchService : MyServiceDefinition
    {
        public MySearchService() : base("search_service")
        {
            this.AddMethod(MyMethodDefinition<SearchRequest, SearchResponse>.Create("search", Search));
        }

        public SearchResponse Search(SearchRequest request)
        {
            SearchResponse resp = new SearchResponse();
            resp.FilePath = "ducker";
            resp.Found = false;
            return resp;
        }

    }
}
