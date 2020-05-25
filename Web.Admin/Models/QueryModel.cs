using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Admin.Models
{
    [Bind(Exclude = "Id, Date")]
    public class QueryModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Definition { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(50, ErrorMessage = "Too long")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(150, ErrorMessage = "Too long")]
        public string ConnectionString { get; set; }

        [StringLength(1000, ErrorMessage = "Too long")]
        public string Config { get; set; }
    }
}