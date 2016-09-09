using System;
using System.IO;
using System.Threading;

namespace TesteThrift
{
    internal class SearchServiceHandler : SearchService.Iface
    {
        public void Delay1(int millis)
        {
            Thread.Sleep(millis);
        }

        public void Delay2(int millis)
        {
            Thread.Sleep(millis);
        }

        public SearchResult Search(SearchRequest request)
        {
            // throw new InvalidOperation();
            SearchResult ret = new SearchResult();
            ret.FileName = LocateFile(request, request.StartPath);
            ret.Found = !String.IsNullOrWhiteSpace(ret.FileName);
            return ret;
        }

        private string LocateFile(SearchRequest request, string startPath)
        {
            try
            {
                var files = Directory.GetFiles(startPath, request.FileMask);
                foreach (var file in files)
                {
                    return file;
                }

                var directories = Directory.GetDirectories(startPath);
                foreach (var directory in directories)
                {
                    string ret = LocateFile(request, Path.Combine(startPath, directory));
                    if (!String.IsNullOrWhiteSpace(ret)) return ret;
                }                
            }
            catch (IOException) when (request.IgnoreErrors) { }
            catch (UnauthorizedAccessException) when (request.IgnoreErrors) { }

            return null;
        }
    }
}