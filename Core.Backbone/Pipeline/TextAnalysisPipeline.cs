using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Backbone.Contract;
using Core.Lib;
using Core.Config.Contract;
using Global.Models;
using NLog;

namespace Core.Backbone.Pipeline
{
    public class TextAnalysisPipeline : ITextAnalysisPipeline
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        public HttpClient HttpClient { get; set; }

        private readonly List<string> moduleCollection;
        private readonly PipelineEvents events;

        public TextAnalysisPipeline(IConfigurationFactory configurationFactory)
        {
            events = configurationFactory.GetEvents();
            moduleCollection = configurationFactory.GetModules();
        }

        public async Task Invoke(Job job)
        {
            var args = new PipelineEventArgs(job, events, moduleCollection, HttpClient);
            var errorModuleExists = false;

            try
            {
                log.Debug("job received: {0}", job.UniqueId);

                if (events.OnExtractKnowledge == null) return;
                await events.OnExtractKnowledge(args);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (events.OnErrorOccurred != null) errorModuleExists = true;
            }

            if (errorModuleExists) await events.OnErrorOccurred(args);
        }
    }
}