using System.Threading.Tasks;

namespace Core.Lib
{
    public delegate Task PipelineDelegate<in T>(T e);
}