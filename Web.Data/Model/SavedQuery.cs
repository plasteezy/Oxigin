using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Data.Model
{
    public class SavedQuery
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Definition { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Connection")]
        public string ConnectionString { get; set; }

        public DateTime Date { get; set; }

        [DataType(DataType.MultilineText)]
        public string Config { get; set; }
    }
}