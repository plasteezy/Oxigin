using System.Collections.Generic;

namespace Core.Lib.Models
{
    public class WatsonTextAnalysisObject
    {
        public WatsonTextAnalysisResponse WatsonTextAnalysisResponse { get; set; }
        public bool Success { get; set; }

    }

    public class WatsonTextAnalysisResponse
    {
        public Usage usage { get; set; }
        public string language { get; set; }
        public Sentiment sentiment { get; set; }
        public List<Category> categories { get; set; }
        public List<Keyword> keywords { get; set; }
        public Emotion emotion { get; set; }
    }

    public class Usage
    {
        public int text_units { get; set; }
        public int text_characters { get; set; }
        public int features { get; set; }
    }

    public class Document
    {
        public double score { get; set; }
        public string label { get; set; }
    }

    public class Sentiment
    {
        public Document document { get; set; }
    }

    public class Keyword
    {
        public string text { get; set; }
        public double relevance { get; set; }
        public int count { get; set; }
    }

    public class Category
    {
        public string label { get; set; }
        public double score { get; set; }
    }

    public class Emotion2
    {
        public double sadness { get; set; }
        public double joy { get; set; }
        public double fear { get; set; }
        public double disgust { get; set; }
        public double anger { get; set; }
    }

    public class Document2
    {
        public Emotion2 emotion { get; set; }
    }

    public class Emotion
    {
        public Document2 document { get; set; }
    }
}
