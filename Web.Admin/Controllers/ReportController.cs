#region

using Web.Admin.Components;
using Web.Admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Web.Data.Contract;
using Web.Data.Model;
using Web.Data.Repository;
using static System.Int32;

#endregion

namespace Web.Admin.Controllers
{
    public class ReportController : RootController
    {
        private readonly IReportRepository _reportRepository;

        // If you are using Dependency Injection, you can delete the following constructor
        public ReportController() : this(new ReportRepository()) { }

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        //
        // GET: /Reports/

        public ViewResult Index(string description, string sort, string sortDir)
        {
            var savedQueries = _reportRepository.All;

            // Filtering
            if (!string.IsNullOrWhiteSpace(description))
            {
                savedQueries = from q in savedQueries
                               where q.Description.Contains(description.Trim())
                               select q;
            }
            // Sorting
            if (sort == "Id")
            {
                savedQueries = sortDir == "ASC" ? savedQueries.OrderBy(q => q.Id) :
                    savedQueries.OrderByDescending(q => q.Id);
            }
            else
            {
                savedQueries = sortDir == "ASC" ? savedQueries.OrderBy(q => q.Description) :
                    savedQueries.OrderByDescending(q => q.Description);
            }

            ViewData["Description"] = description;
            return View(savedQueries);
        }

        //
        // GET: /Reports/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Reports/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Exclude = "Date, ConnectionString")]SavedQuery tsqlquery)
        {
            tsqlquery.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            if (string.IsNullOrWhiteSpace(tsqlquery.Config)) tsqlquery.Config = null;
            else
            {
                ValidateQueryConfig(tsqlquery.Config);
            }
            if (!ModelState.IsValid) return View();
            if (!ModelState.IsValid) return View(tsqlquery);

            tsqlquery.Date = DateTime.Now;

            _reportRepository.InsertOrUpdate(tsqlquery);
            _reportRepository.Save();

            FlashSuccess($"Query: {tsqlquery.Description} has been successfully added.");
            return RedirectToAction("Index", new { sort = "Id", sortDir = "DESC" });
        }

        private void ValidateQueryConfig(string config)
        {
            var d = new XmlSerializer(typeof(QueryParameterCollection));
            List<QueryParameter> qpc;
            try
            {
                qpc = ((QueryParameterCollection)d.Deserialize(
                         new StringReader(config))).QueryParameters;
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("Config", GetInnermostException(exc).Message);
                return;
            }
            for (int i = 0; i < qpc.Count; i++)
            {
                try
                {
                    var qp = qpc[i];
                    if (qp.Id == null)
                    {
                        throw new ApplicationException("Id not specified.");
                    }
                    if (!Regex.IsMatch(qp.Id, @"^_\w+_$"))
                    {
                        throw new ApplicationException('"' + qp.Id + "\" is not valid HTML element id. "
                            + "Id must match pattern \"^_\\w+_$\"");
                    }
                    if (qp.Id.Length > 50)
                    {
                        throw new ApplicationException("Id's length > 50.");
                    }
                    if (qp.Name != null && qp.Name.Length > 50)
                    {
                        throw new ApplicationException("Name's length > 50.");
                    }
                    if (qp.RawType != null)
                    {
                        QueryParameterType qpType;
                        if (!Enum.TryParse(qp.RawType,
                            true, out qpType))
                        {
                            throw new ApplicationException('"' + qp.RawType + "\" is an " +
                                "invalid query parameter type.");
                        }
                    }
                    if (qp.Default != null)
                    {
                        string validDefaultValue;
                        if (!QueryParameterParser.TryParseQueryParameter(qp.Default, qp.Type,
                            out validDefaultValue))
                        {
                            throw new ApplicationException('"' + qp.Default + "\" is an " +
                                "invalid default value.");
                        }
                    }
                    if (qp.UserInterfaceElement != null)
                    {
                        if (qp.UserInterfaceElement.RawType == null)
                        {
                            throw new ApplicationException("Type not specified for ui.");
                        }
                        UserInterfaceElementType qpUiType;
                        if (!Enum.TryParse(qp.UserInterfaceElement.RawType,
                            true, out qpUiType))
                        {
                            throw new ApplicationException('"' + qp.UserInterfaceElement.RawType + "\" is an " +
                                "invalid ui type.");
                        }
                        switch (qpUiType)
                        {
                            case UserInterfaceElementType.DROP_DOWN:
                                ValidateDropDownUi(qp.UserInterfaceElement.CustomAttributes);
                                break;

                            case UserInterfaceElementType.DROP_DOWN_DB:
                                ValidateDropDownDbUi(qp.UserInterfaceElement.CustomAttributes);
                                break;

                            case UserInterfaceElementType.AUTOCOMPLETE:
                                ValidateAutocompleteUi(qp.UserInterfaceElement.CustomAttributes);
                                break;
                        }
                    }
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("Config", "param " + i + ": " +
                        GetInnermostException(exc).Message);
                    break;
                }
            }
            if (ModelState.IsValid)
            {
                if (qpc.Select(e => e.Id.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count()
                    != qpc.Count)
                {
                    ModelState.AddModelError("Config", "Ids are not distinct.");
                }
            }
        }

        public static void ValidateDropDownUi(XmlAttribute[] uiConfig)
        {
            XmlAttribute dataAttr = null;
            if (uiConfig != null) dataAttr = uiConfig.SingleOrDefault(attr => attr.Name ==
                 QueryParameterParser.UI_DATA_PARAMETER);
            if (dataAttr == null)
                throw new ApplicationException("Could not find data attribute for drop down ui.");
        }

        public static void ValidateDropDownDbUi(XmlAttribute[] uiConfig)
        {
            XmlAttribute queryAttr = null;
            if (uiConfig != null)
            {
                queryAttr = uiConfig.SingleOrDefault(attr => attr.Name ==
                    QueryParameterParser.UI_DATASOURCE_PARAMETER);
            }
            if (queryAttr == null)
                throw new ApplicationException("Could not find query attribute for drop down (db driven) ui.");
            int queryId;
            if (!TryParse(queryAttr.Value, out queryId))
                throw new ApplicationException("Invalid query id for drop down (db driven) ui.");
        }

        public static void ValidateAutocompleteUi(XmlAttribute[] uiConfig)
        {
            XmlAttribute queryAttr = null;
            if (uiConfig != null)
            {
                queryAttr = uiConfig.SingleOrDefault(attr => attr.Name ==
                    QueryParameterParser.UI_DATASOURCE_PARAMETER);
            }
            if (queryAttr == null)
                throw new ApplicationException("Could not find query attribute for autocomplete ui.");
            int queryId;
            if (!TryParse(queryAttr.Value, out queryId))
                throw new ApplicationException("Invalid query id for autocomplete ui.");
        }

        //
        // GET: /Reports/Edit/5

        public ActionResult Edit(int id)
        {
            return View(_reportRepository.Find(id));
        }

        //
        // POST: /Reports/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Exclude = "ConnectionString")]SavedQuery tsqlquery)
        {
            tsqlquery.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            if (string.IsNullOrWhiteSpace(tsqlquery.Config)) tsqlquery.Config = null;
            else
            {
                ValidateQueryConfig(tsqlquery.Config);
            }
            if (!ModelState.IsValid) return View(tsqlquery);

            _reportRepository.InsertOrUpdate(tsqlquery);
            _reportRepository.Save();
            FlashSuccess($"Query: {tsqlquery.Description} has been successfully updated.");
            return RedirectToAction("Index");
        }

        //
        // GET: /Reports/Delete/5

        public ActionResult Delete(int id)
        {
            _reportRepository.Delete(id);
            _reportRepository.Save();
            FlashSuccess("Query was successfully deleted.");
            return RedirectToAction("index");
        }

        public ActionResult Execute(int id)
        {
            var query = _reportRepository.Find(id);
            var model = new QueryResultsModel
            {
                Query = query,
                Download = Request.QueryString["-download-"] != null,
                Debug = Request.QueryString["-debug-"] != null,
                Execute = Request.QueryString["-execute-"] != null
            };
            if (query.Config != null)
                model.QueryParameters = ParseQueryParameters(query.Config);
            if (!model.Execute) return View(model);

            var queryParameterValues = new Dictionary<string, string>();
            if (model.QueryParameters != null)
            {
                foreach (var queryParameter in model.QueryParameters.QueryParameters)
                {
                    var queryParameterValue = Request.QueryString[queryParameter.Id];
                    if (string.IsNullOrEmpty(queryParameterValue))
                        queryParameterValue = queryParameter.Default;
                    queryParameter.Value = queryParameterValue;
                    string validQueryParameterValue;
                    if (!QueryParameterParser.TryParseQueryParameter(queryParameterValue,
                        queryParameter.Type, out validQueryParameterValue))
                    {
                        if (queryParameterValue == null)
                            ModelState.AddModelError("", queryParameter.Name + " is required.");
                        else
                            ModelState.AddModelError("", queryParameter.Name + " is invalid.");
                    }
                    else
                    {
                        queryParameterValues.Add(queryParameter.Id, validQueryParameterValue);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                FlashValidationError();
                return View(model);
            }

            var sqlQuery = QueryParameterParser.Replace(query.Definition, queryParameterValues);

            try
            {
                if (model.Download)
                {
                    if (model.Debug)
                    {
                        model.SqlQuery = sqlQuery;
                    }
                    Download(query.ConnectionString, sqlQuery);
                    return new EmptyResult();
                }
            }
            catch (Exception exc)
            {
                FlashError("Error (SQL): " + GetInnermostException(exc).Message);
                return View(model);
            }

            if (model.Debug)
            {
                model.SqlQuery = sqlQuery;
            }

            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = new SqlConnection(query.ConnectionString);
                connection.Open();
                var command = new SqlCommand(sqlQuery, connection);
                reader = command.ExecuteReader(CommandBehavior.SingleResult |
                                                   CommandBehavior.CloseConnection);
                model.QueryResults = reader;
            }
            catch (Exception exc)
            {
                FlashError("Error (SQL): " + GetInnermostException(exc).Message);
                reader?.Close();
                connection?.Close();
            }
            return View(model);
        }

        private static QueryParameterCollection ParseQueryParameters(string queryConfig)
        {
            var d = new XmlSerializer(typeof(QueryParameterCollection));
            var queryParameters = (QueryParameterCollection)d.Deserialize(
                new StringReader(queryConfig));
            foreach (var queryParameter in queryParameters.QueryParameters)
            {
                if (queryParameter.Name == null)
                    queryParameter.Name = queryParameter.Id;
                if (queryParameter.RawType == null)
                    queryParameter.Type = QueryParameterType.STRING;
                else queryParameter.Type = (QueryParameterType)Enum.Parse(typeof(QueryParameterType),
                    queryParameter.RawType, true);
                if (queryParameter.UserInterfaceElement != null)
                {
                    queryParameter.UserInterfaceElement.Type = (UserInterfaceElementType)Enum.Parse(
                        typeof(UserInterfaceElementType), queryParameter.UserInterfaceElement.RawType, true);
                }
            }
            return queryParameters;
        }

        private void Download(string connectionString, string query)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        Response.AddHeader("Content-Disposition", "attachment; filename=report" +
                            new Random().Next() + ".csv");
                        Response.ContentType = "application/octet-stream";
                        for (var i = 0; i < reader.VisibleFieldCount; i++)
                        {
                            Response.Write(QueryParameterParser.EscapeCsvData(reader.GetName(i)));
                            Response.Write(',');
                        }
                        Response.Write(Environment.NewLine);
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.VisibleFieldCount; i++)
                            {
                                Response.Write(QueryParameterParser.EscapeCsvData(reader[i]));
                                Response.Write(',');
                            }
                            Response.Write(Environment.NewLine);
                        }
                    }
                }
            }
        }

        private static Exception GetInnermostException(Exception exc)
        {
            var innermostExc = exc;
            while (innermostExc.InnerException != null) innermostExc = innermostExc.InnerException;
            return innermostExc;
        }

        [HttpGet]
        public ActionResult GetAutocompleteData(int savedQueryId, string query)
        {
            var data = new List<string>();
            var savedQuery = _reportRepository.Find(savedQueryId);
            using (var conn = new SqlConnection(savedQuery.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(savedQuery.Definition.Replace(
                    QueryParameterParser.UI_AUTOCOMPLETE_QUERY_PARAMETER,
                    QueryParameterParser.EscapeAndQuoteSqlData(query ?? "")), conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader[0].ToString());
                    }
                }
            }
            return new JsonResult
            {
                ContentType = "text/plain",
                Data = new
                {
                    query,
                    suggestions = data.ToArray(),
                    data = data.ToArray()
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}