using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Backbone.Contract;
using Core.Backbone.Pipeline;
using Core.Config.Contract;
using Global;
using Global.Models;
using NLog;
using RabbitMQ.Client;
using ServiceStack;

namespace Core.Backbone.Broker
{
    public class TextAnalysisMonitor : ITextAnalysisMonitor
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public Thread WorkerThread { get; set; }

        protected internal ITextAnalysisPipeline TextAnalysisPipeline;
        private readonly IConfigurationFactory configurationFactory;

        public TextAnalysisMonitor(IConfigurationFactory configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        public Listen Listen { get; set; }

        public void Start(string consumer)
        {

            log.Info("Oxigin Text Analyzer Server Version: [" + Assembly.GetExecutingAssembly().GetName().Version + "]");
            log.Info("Platform: [" + Environment.OSVersion.Platform + "]");
            log.Info("Version: [" + Environment.OSVersion.Version + "]");
            log.Info("OS: [" + Environment.OSVersion + "]");

            log.Info("Text Analyzer is booting");
            log.Info("Worker: {0}", consumer);

            log.Info("Running startup diagnosis");
            Utility.StartupCheck();
            log.Info("Start up diagnosis complete");

            WorkerThread = new Thread(async () => await InitMessenger()) { Name = consumer + "-thread" };
            WorkerThread.Start();
        }

        public void Kill()
        {
            WorkerThread.Join(TimeSpan.FromSeconds(10.0));
        }

        public async Task InitMessenger()
        {
            try
            {
                TextAnalysisPipeline = new TextAnalysisPipeline(configurationFactory);
                await SubscribeToChannel(Listen);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        public async Task SubscribeToChannel(Listen listen)
        {
            log.Info("Subscribing to rabbit mq channel");

            var exchange = ConfigurationManager.AppSettings["Rabbit::Exchange"];
            var host = ConfigurationManager.AppSettings["Rabbit::Host"];
            var user = ConfigurationManager.AppSettings["Rabbit::User"];
            var pwd = ConfigurationManager.AppSettings["Rabbit::Pwd"];

            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pwd,
                AutomaticRecoveryEnabled = true
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, "direct");
                    var qName = channel.QueueDeclare(listen.Queue, true, false, false, null);

                    channel.QueueBind(qName, exchange, listen.RoutingKey);

                    //Todo read on: EventingBasicConsumer
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(qName, true, consumer);

                    log.Info("Subscription to rabbit mq channel complete");
                    log.Info("Initialization complete. Text analyzer is up and running");

                    while (true)
                    {
                        log.Debug("waiting for a job");
                        var obj = consumer.Queue.Dequeue();

                        if (obj == null) return;
                        var body = obj.Body;

                        var callbackObj = Encoding.UTF8.GetString(body).FromJson<Job>();
                        await TextAnalysisPipeline.Invoke(callbackObj);
                    }
                }
            }
        }
    }
}