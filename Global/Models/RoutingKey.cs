namespace Global.Models
{
    public class RoutingKey
    {
        public static string TextAnalysis = "TextAnalysis";
    }

    public class RabbitQueue
    {
        public static string TextAnalysisQueue = "TextAnalysisQueue";
    }

    public class Job
    {
        public string UniqueId { get; set; }
        public int ProjectId { get; set; }
        public string Body { get; set; }
    }

    public class Listen
    {
        public string Queue { get; set; }

        public string RoutingKey { get; set; }
    }

}