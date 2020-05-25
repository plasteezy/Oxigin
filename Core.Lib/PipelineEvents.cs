namespace Core.Lib
{
    public class PipelineEvents
    {
        public PipelineDelegate<PipelineEventArgs> OnExtractKnowledge { get; set; }
        public PipelineDelegate<PipelineEventArgs> OnErrorOccurred { get; set; }
    }
}