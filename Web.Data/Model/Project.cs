using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Data.Model
{
    public class Project
    {
        public Project()
        {
            ProjectDataSets = new List<ProjectDataSet>();
        }

        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%&*()\s?'"":;.,_+={}\[\]\\/-]+$", ErrorMessage = "Please enter alphanumeric characters only")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@#$%&*()\s?'"":;.,_+={}\[\]\\/-]+$", ErrorMessage = "Please enter alphanumeric characters only")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ICollection<ProjectDataSet> ProjectDataSets { get; set; }
    }

    public class ProjectDataSet
    {
        public ProjectDataSet()
        {
            EmotionDataSets = new List<EmotionDataSet>();
            SentimentDataSets = new List<SentimentDataSet>();
            KeywordDataSets = new List<KeywordDataSet>();
            CategoryDataSets = new List<CategoryDataSet>();
        }

        public int ProjectDataSetId { get; set; }

        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Entry is required")]
        [Display(Name = "Entry")]
        public string Entry { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<EmotionDataSet> EmotionDataSets { get; set; }
        public virtual ICollection<SentimentDataSet> SentimentDataSets { get; set; }
        public virtual ICollection<KeywordDataSet> KeywordDataSets { get; set; }
        public virtual ICollection<CategoryDataSet> CategoryDataSets { get; set; }
    }

    public class CategoryDataSet
    {
        public int CategoryDataSetId { get; set; }
        public int ProjectDataSetId { get; set; }
        public string Label { get; set; }
        public double Score { get; set; }
        public virtual ProjectDataSet ProjectDataSet { get; set; }

    }

    public class EmotionDataSet
    {
        public int EmotionDataSetId { get; set; }
        public int ProjectDataSetId { get; set; }
        public double AngerScore { get; set; }
        public double JoyScore { get; set; }
        public double DisgustScore { get; set; }
        public double SadnessScore { get; set; }
        public double FearScore { get; set; }
        public virtual ProjectDataSet ProjectDataSet { get; set; }
    }

    public class SentimentDataSet
    {
        public int SentimentDataSetId { get; set; }
        public int ProjectDataSetId { get; set; }
        public string SentimentLabel { get; set; }
        public double SentimentScore { get; set; }
        public virtual ProjectDataSet ProjectDataSet { get; set; }
    }

    public class KeywordDataSet
    {
        public int KeywordDataSetId { get; set; }
        public int ProjectDataSetId { get; set; }
        public string Text { get; set; }
        public double Relevance { get; set; }
        public virtual ProjectDataSet ProjectDataSet { get; set; }
    }

    public class ApiCredential
    {
        public int ApiCredentialId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%&*()\s?'"":;.,_+={}\[\]\\/-]+$", ErrorMessage = "Please enter alphanumeric characters only")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
