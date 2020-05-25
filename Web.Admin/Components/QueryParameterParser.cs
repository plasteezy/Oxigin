using Web.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Data.Repository;

namespace Web.Admin.Components
{
    public class QueryParameterParser
    {
        public static readonly string UI_AUTOCOMPLETE_QUERY_PARAMETER = "_term_";

        public static readonly string UI_DATA_PARAMETER = "data";

        public static readonly string UI_DATASOURCE_PARAMETER = "query";

        public static bool TryParseQueryParameter(string paramValue, QueryParameterType paramType,
            out string parsedParamValue)
        {
            if (paramValue == null)
            {
                if (paramType == QueryParameterType.BOOLEAN)
                {
                    parsedParamValue = "0";
                    return true;
                }
                else
                {
                    parsedParamValue = null;
                    return false;
                }
            }

            string[] dateFormats = { "yyyy-MM-dd", "yyyy-M-d" };
            string[] timeFormats = { "HH:mm:ss", "HH:mm", "hh:mm:ss tt",
                "hh:mm tt", "H:m:ss", "H:m", "h:m:s tt",
                "h:m tt" };
            string[] dateTimeFormats = dateFormats.SelectMany(
                df => timeFormats.Select(tf => df + ' ' + tf)).ToArray();
            DateTime parsedDate = DateTime.MinValue;
            bool parsedBoolean = false;
            try
            {
                // Validate param value.
                switch (paramType)
                {
                    case QueryParameterType.INT:
                        Int32.Parse(paramValue);
                        break;

                    case QueryParameterType.LONG:
                        Int64.Parse(paramValue);
                        break;

                    case QueryParameterType.TIME:
                        parsedDate = DateTime.ParseExact(paramValue, timeFormats, null,
                            System.Globalization.DateTimeStyles.AssumeUniversal);
                        break;

                    case QueryParameterType.DATE:
                        parsedDate = DateTime.ParseExact(paramValue,
                            dateFormats.Union(dateTimeFormats).ToArray(), null,
                            System.Globalization.DateTimeStyles.AssumeUniversal);
                        break;

                    case QueryParameterType.DATETIME:
                        parsedDate = DateTime.ParseExact(paramValue, dateTimeFormats, null,
                            System.Globalization.DateTimeStyles.AssumeUniversal);
                        break;

                    case QueryParameterType.DOUBLE:
                        Double.Parse(paramValue);
                        break;

                    case QueryParameterType.FLOAT:
                        Single.Parse(paramValue);
                        break;

                    case QueryParameterType.DECIMAL:
                        Decimal.Parse(paramValue);
                        break;

                    case QueryParameterType.BOOLEAN:
                        parsedBoolean = Boolean.Parse(paramValue);
                        break;

                    case QueryParameterType.GUID:
                        Guid.Parse(paramValue);
                        break;

                    default: // for string.
                        break;
                }
            }
            catch (FormatException)
            {
                parsedParamValue = null;
                return false;
            }
            catch (OverflowException)
            {
                parsedParamValue = null;
                return false;
            }

            // Get param value for TSQL
            switch (paramType)
            {
                case QueryParameterType.INT:
                case QueryParameterType.LONG:
                case QueryParameterType.DOUBLE:
                case QueryParameterType.FLOAT:
                case QueryParameterType.DECIMAL:
                    parsedParamValue = paramValue;
                    break;

                case QueryParameterType.BOOLEAN:
                    parsedParamValue = parsedBoolean ? "1" : "0";
                    break;

                case QueryParameterType.TIME:
                    parsedParamValue = EscapeAndQuoteSqlData(parsedDate.ToString("HH:mm:ss"));
                    break;

                case QueryParameterType.DATE:
                    parsedParamValue = EscapeAndQuoteSqlData(parsedDate.ToString("yyyy-MM-dd"));
                    break;

                case QueryParameterType.DATETIME:
                    parsedParamValue = EscapeAndQuoteSqlData(parsedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    break;

                default:
                    parsedParamValue = EscapeAndQuoteSqlData(paramValue.Replace("'", "''"));
                    break;
            }
            return true;
        }

        /// <summary>
        /// Performs a one-time replacement of various search strings in a text with their corresponding replacement
        /// strings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is to be preferred to repeated calls to System.String.Replace() for each search string because if
        /// </para>
        /// <para>
        /// the replacement for a search string is also a search string, then when the latter search string is being
        /// </para>
        /// <para>
        /// replaced, the former's replacement would be affected.
        /// </para>
        /// <para>
        /// For example, suppose we have a text such as "This is good! Keep it up." and we want to replace "good"
        /// </para>
        /// <para>
        /// with "it" and replace "it" with "your eyes" so that we have "This is it! Keep your eyes up.". A first
        /// </para>
        /// <para>
        /// call to Replace with arguments being "good" and "it" will yield "This is it! Keep it up.". This is where
        /// </para>
        /// <para>
        /// the problem comes. A second call to Replace with arguments being "it" and "your eyes" will yield
        /// </para>
        /// <para>
        /// "This is your eyes! Keep your eyes up.". If this behaviour is not desired, then this method comes as
        /// </para>
        /// <para>
        /// a suitable substitute.
        /// </para>
        /// <para>
        /// Search strings which are empty strings are not found, except when the input string itself is empty,
        /// </para>
        /// <para>
        /// in which it's replaced by a string corresponding to an empty search string specified in the
        /// </para>
        /// <para>
        /// search string table. If no empty search string is found, the input is returned ummodified.
        /// </para>
        /// <para>
        /// Also if there are two search strings with one being the prefix of the other, the longer search
        /// </para>
        /// <para>
        /// string is preferred.
        /// </para>
        /// </remarks>
        /// <param name="input">The text in which the replacement is to take place.</param>
        /// <param name="searchStringTable">
        /// The strings to search for and their corresponding
        /// replacements.
        /// </param>
        /// <returns>Text with all search strings replaced with their corrresponding replacements.</returns>
        public static string Replace(string input, Dictionary<string, string> searchStringTable)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (searchStringTable == null)
            {
                throw new ArgumentNullException("searchStringTable");
            }
            foreach (KeyValuePair<string, string> kvp in searchStringTable)
            {
                if (kvp.Value == null)
                    throw new ArgumentException("Search string table contains null value.");
            }
            if (input.Length == 0)
            {
                if (searchStringTable.ContainsKey(input))
                {
                    return searchStringTable[input];
                }
                return input;
            }

            // Skip over empty search strings and order the rest in order of decreasing length.
            //
            // Empty search strings would cause infinite looping in the foreach loop below from
            // the implementation perspective, and aren't to be found anyway from the interface
            // perspective.
            //
            // By ordering remaining search strings in order of decreasing length, fulfil the
            // condition that if one search string happens to be the prefix of another, it'll be
            // deferred in preference to the other (longer) search string.
            IEnumerable<string> searchStrings = from searchString in searchStringTable.Keys
                                                where searchString.Length > 0
                                                orderby searchString.Length descending
                                                select searchString;

            // Note the positions of occurence of all search strings in the input string.
            // If a search string occurs at a position where a previous search string also
            // occurs, then skip to the next position of occurence because it will mean that the previous search
            // string has the current search string as its prefix and so the previous search string is to be
            // preferred.
            // Use dictionary to store positions and their corresponding search strings so as to check the
            // correctness of the algorithm with regards to the property that no position is to be added
            // more than once (since dictionaries don't allow duplicate keys).
            Dictionary<int, string> searchStringPositions = new Dictionary<int, string>();
            foreach (string searchString in searchStrings)
            {
                int position = 0;
                while ((position = input.IndexOf(searchString, position)) != -1)
                {
                    if (searchStringPositions.ContainsKey(position) == false)
                    {
                        searchStringPositions.Add(position, searchString);
                    }
                    position += searchString.Length;
                }
            }

            // Starting from smallest position to the biggest, use position to get the search string and the
            // search string to get the replacement string.
            IEnumerable<int> positions = from position in searchStringPositions.Keys
                                         orderby position ascending
                                         select position;
            StringBuilder output = new StringBuilder(input);
            // Because search strings and their replacements generally differ in length and replacement is to be
            // done progressively, positions aside the first one will generally not be correct with respect to
            // the original input string.
            // E.g. if input = "I am so glad that my Father", positionTable = { "8" : "glad", "18" : "my" } and
            // searchStringTable = { "glad" : "happy", "my", "our" }, then first replacement will work as follows:
            // 1. "glad" is replaced by "happy" at position 8 and input becomes "I am so happy that my Father".
            // Position 18 is now no longer at "my" but just after "that". Position 18 should now become 19.
            // Because "happy" is longer than "glad" by one character, position 18 was incorrect by 1 character.
            // offset keeps track of all changes in length of original input string so that it will be used
            // to correct subsequent positions during replacements.
            int offset = 0;
            foreach (int position in positions)
            {
                string searchString = searchStringPositions[position];
                string replacementString = searchStringTable[searchString];
                output.Replace(searchString, replacementString, position + offset, searchString.Length);
                offset += (replacementString.Length - searchString.Length);
            }
            return output.ToString();
        }

        public static string EscapeCsvData(object data)
        {
            StringBuilder csv = new StringBuilder("\"");
            if (data != null) csv.Append(data.ToString().Replace("\"", "\"\""));
            csv.Append("\"");
            return csv.ToString();
        }

        public static string EscapeJavascriptString(string jstr, char quoteChar)
        {
            return jstr.Replace("" + quoteChar, "\\" + quoteChar);
        }

        public static SelectListItem[] CreateDropDownData(int queryId, string selected)
        {
            var data = new List<SelectListItem>();
            var savedQueriesRepo = new ReportRepository();
            var savedQuery = savedQueriesRepo.Find(queryId);
            using (var conn = new SqlConnection(savedQuery.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(savedQuery.Definition, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sel = new SelectListItem
                        {
                            Value = reader[0].ToString()
                        };
                        if (reader.VisibleFieldCount > 1)
                            sel.Text = reader[1].ToString();
                        else
                            sel.Text = sel.Value;
                        if (sel.Value == selected) sel.Selected = true;
                        data.Add(sel);
                    }
                }
            }
            return data.ToArray();
        }

        public static SelectListItem[] CreateDropDownData(string data, string selected)
        {
            var dropDownData = new List<SelectListItem>();
            var queryString = HttpUtility.ParseQueryString(data);
            foreach (var key in queryString.AllKeys)
            {
                var sel = new SelectListItem
                {
                    Value = key,
                    Text = queryString[key]
                };
                if (sel.Text.Length == 0) sel.Text = sel.Value;
                if (sel.Value == selected) sel.Selected = true;
                dropDownData.Add(sel);
            }
            return dropDownData.ToArray();
        }

        public static string EscapeAndQuoteSqlData(string data)
        {
            return '\'' + data.Replace("'", "''") + '\'';
        }
    }
}