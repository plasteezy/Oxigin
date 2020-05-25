#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

#endregion

namespace Web.Admin.Helpers
{
    public static class HtmlHelpers
    {
        private const string SCRIPT_INCLUDE_FORMAT = "<script src=\"{0}\"></script>";
        private const string IMAGE_TAG = "<img src=\"{0}\" alt=\"{1}\" class=\"{2}\"  />";

        /// <summary>
        ///   Used to include a script.
        /// </summary>
        /// <param name = "h"></param>
        /// <param name = "virtualPath">The virtual path to the script</param>
        /// <returns></returns>
        public static MvcHtmlString IncludeScript(this HtmlHelper h, string virtualPath)
        {
            string clientPath = VirtualPathUtility.ToAbsolute(virtualPath);
            return MvcHtmlString.Create(string.Format(SCRIPT_INCLUDE_FORMAT, clientPath));
        }

        /// <summary>
        ///   Renders an html image tag.
        /// </summary>
        /// <param name = "h"></param>
        /// <param name = "path">The virtual path to the image</param>
        /// <param name = "alt"></param>
        /// <param name = "classes"></param>
        /// <returns></returns>
        public static MvcHtmlString Image(this HtmlHelper h, string path, string alt, params string[] classes)
        {
            path = VirtualPathUtility.ToAbsolute(path);
            string classAttr = string.Join(" ", classes);
            return MvcHtmlString.Create(string.Format(IMAGE_TAG, path, alt, classAttr));
        }

        public static void Flash(this HtmlHelper h)
        {
            h.RenderPartial("_FlashPartial");
            h.ViewContext.TempData.Remove("flash");
            h.ViewContext.TempData.Remove("flashmode");
        }

        public static MvcHtmlString CustomLabelFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                              Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttributes = new RouteValueDictionary();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            var tag = new TagBuilder("label");

            if (metadata.IsRequired)
            {
                if (metadata.ModelType != typeof(bool))
                    htmlAttributes.Add("class", "field-is-required");
            }

            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            var span = new TagBuilder("span");
            span.SetInnerText(labelText);

            // assign <span> to <label> inner html
            tag.InnerHtml = span.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static HtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> items)
        {
            var output = new StringBuilder();
            output.Append(@"<div class=""checkboxList"">");

            foreach (var item in items)
            {
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(item.Value);
                output.Append("\"");

                if (item.Selected)
                    output.Append(@" checked=""checked""");

                output.Append(" /> ");
                output.Append(item.Text);
                output.Append("<br />");
            }

            output.Append("</div>");

            return new HtmlString(output.ToString());
        }

        public static MvcHtmlString CheckBoxList<T>(this HtmlHelper h, string name, IEnumerable<T> baseList, IEnumerable<T> selectedList)
        {
            var output = new StringBuilder();
            output.Append(@"<div class=""checkboxList"">");
            foreach (var o in baseList)
            {
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(o);
                output.Append("\"");

                if (selectedList != null)
                    if (selectedList.Contains(o))
                        output.Append(@" checked=""checked""");

                output.Append(" /> ");
                output.Append(o);
                output.Append("<br />");
            }
            output.Append("</div>");
            return new MvcHtmlString(output.ToString());
        }
    }
}