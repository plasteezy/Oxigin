using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Core.Lib;
using Core.Lib.Contracts;
using Core.Lib.Models;
using Global;
using NLog;
using Web.Data.Contract;
using Web.Data.Model;
using Web.Data.Repository;

namespace Core.Module
{
    public class ModWatsonTextAnalyzer : IPipelineModule
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly IAiActivity<WatsonTextAnalysisObject> watsonAiActivity;
        private readonly IProjectDataSetRepository projectDataSetRepository;

        public ModWatsonTextAnalyzer() : this(new WatsonAiActivity(), new ProjectDataSetRepository()){}

        public ModWatsonTextAnalyzer(IAiActivity<WatsonTextAnalysisObject> watsonAiActivity, IProjectDataSetRepository projectDataSetRepository)
        {
            this.watsonAiActivity = watsonAiActivity;
            this.projectDataSetRepository = projectDataSetRepository;
        }

        public void Init(PipelineEvents events, NameValueCollection configElement)
        {
            events.OnExtractKnowledge += OnExtractKnowledge;
        }

        private async Task OnExtractKnowledge(PipelineEventArgs e)
        {
            try
            {
                var obj = await watsonAiActivity.AnalyzeText(e.Job.Body, e.Client);

                if (obj.Success)
                {
                    var projDataSet = new ProjectDataSet
                    {
                        ProjectId = e.Job.ProjectId,
                        DateCreated = DateTime.Now,
                        Entry = e.Job.Body,
                    }
                }
                else
                {
                   // Utility.RabbitEnqueue();
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                log.Error(ex.Message);
            }
        }
    }
}