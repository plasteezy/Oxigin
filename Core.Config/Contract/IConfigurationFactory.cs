using System.Collections.Generic;
using Core.Lib;

namespace Core.Config.Contract
{
    public interface IConfigurationFactory
    {
        List<string> GetModules();

        PipelineEvents GetEvents();
    }
}