using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Lib.Contracts
{
    public interface IPipelineModule
    {
        void Init(PipelineEvents events, NameValueCollection configElement);
    }

    public interface IAiActivity<T> where T : class
    {
        Task<T> AnalyzeText(string term, HttpClient client);
    }
}