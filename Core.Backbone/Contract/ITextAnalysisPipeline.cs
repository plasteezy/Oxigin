using System.Net.Http;
using System.Threading.Tasks;
using Global.Models;

namespace Core.Backbone.Contract
{
    public interface ITextAnalysisPipeline
    {
        Task Invoke(Job job);
        HttpClient HttpClient { get; set; }
    }
}