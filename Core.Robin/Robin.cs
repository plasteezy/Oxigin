using System;
using System.ServiceProcess;
using Core.Backbone.Broker;
using Core.Backbone.Contract;
using Core.Backbone.Pipeline;
using Core.Config;
using Core.Config.Contract;
using Global.Models;
using NLog;
using Unity;

namespace Core.Robin
{
    public partial class Robin : ServiceBase
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly ITextAnalysisMonitor monitor;

        public Robin()
        {
            InitializeComponent();

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ITextAnalysisMonitor, TextAnalysisMonitor>().
                RegisterType<ITextAnalysisPipeline, TextAnalysisPipeline>().
                RegisterType<IConfigurationFactory, ConfigurationFactory>();

            monitor = container.Resolve<ITextAnalysisMonitor>();
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                monitor.Listen = new Listen
                {
                    RoutingKey = RoutingKey.TextAnalysis,
                    Queue = RabbitQueue.TextAnalysisQueue,
                };

                monitor.Start("Robin");
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                log.Info("robin is stopping...");
                monitor.Kill();
                log.Info("robin has stopped");
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
            }
        }
    }
}
