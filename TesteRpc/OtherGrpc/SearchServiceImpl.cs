using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Teste.Protos;
using static Teste.Protos.SearchService;

namespace OtherGrpc
{
    public class SearchServiceImpl : SearchServiceBase
    {
        public override Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            var task = new Task<SearchResponse>(delegate
            {
                SearchResponse ret = new SearchResponse();
                return ret;
            });
            task.Start();
            return task;
        }
    }
}
