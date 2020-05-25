using System.Data.SqlClient;
using Web.Data.Model;

namespace Web.Admin.Models
{
    public class QueryResultsModel
    {
        public SavedQuery Query { get; set; }

        public SqlDataReader QueryResults { get; set; }

        public string SqlQuery { get; set; }

        public string SqlCountQuery { get; set; }

        public QueryParameterCollection QueryParameters { get; set; }

        public bool Download { get; set; }

        public bool Debug { get; set; }

        public bool Execute { get; set; }
    }
}