using System;
using System.Linq.Expressions;
using System.Web.Mvc;


namespace jaramillo.cl.Common.Extensions
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// Extension to render line breaks in text. Use it like @Html.DisplayWithBreaksFor(m => m.value)
        /// </summary>
        public static MvcHtmlString DisplayWithBreaksFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var model = html.Encode(metadata.Model).Replace("\n", "<br />\n");

            if (String.IsNullOrEmpty(model))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(model);
        }
    }
}