using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Core.Lib;
using Core.Lib.Contracts;
using Core.Lib.Models;
using Global;
using Global.Models;
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
                        CategoryDataSets = GetCategoryDataSet(obj.WatsonTextAnalysisResponse),
                        EmotionDataSets = GetEmotionDataSet(obj.WatsonTextAnalysisResponse),
                        KeywordDataSets = GetKeywordDataSet(obj.WatsonTextAnalysisResponse),
                        SentimentDataSets = GetSentimentDataSet(obj.WatsonTextAnalysisResponse)
                    };

                    projectDataSetRepository.InsertOrUpdate(projDataSet);
                    projectDataSetRepository.Save();
                }
                else Utility.RabbitEnqueue(e.Job, RoutingKey.TextAnalysis);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                log.Error(ex.Message);
            }
        }

        public List<EmotionDataSet> GetEmotionDataSet(WatsonTextAnalysisResponse obj)
        {
            var list = new List<EmotionDataSet>
            {
                new EmotionDataSet
                {
                    AngerScore = obj.emotion.document.emotion.anger,
                    DisgustScore = obj.emotion.document.emotion.disgust,
                    FearScore = obj.emotion.document.emotion.fear,
                    JoyScore = obj.emotion.document.emotion.joy,
                    SadnessScore = obj.emotion.document.emotion.sadness,
                }
            };

            return list;
        }

        public List<SentimentDataSet> GetSentimentDataSet(WatsonTextAnalysisResponse obj)
        {
            var list = new List<SentimentDataSet>
            {
                new SentimentDataSet
                {
                    SentimentLabel = obj.sentiment.document.label,
                    SentimentScore = obj.sentiment.document.score
                }
            };

            return list;
        }

        public List<KeywordDataSet> GetKeywordDataSet(WatsonTextAnalysisResponse obj)
        {
            var list = new List<KeywordDataSet>();
            foreach (var item in obj.keywords)
            {
                list.Add(new KeywordDataSet
                {
                    Text = item.text,
                    Relevance = item.relevance                   
                });
            }

            return list;
        }

        public List<CategoryDataSet> GetCategoryDataSet(WatsonTextAnalysisResponse obj)
        {
            var list = new List<CategoryDataSet>();
            foreach (var item in obj.categories)
            {
                list.Add(new CategoryDataSet
                {
                    Label = item.label,
                    Score = item.score
                });
            }

            return list;
        }
    }
}