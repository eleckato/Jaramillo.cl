using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace jaramillo.cl.Common.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Arregla los prefix para agregar partial view a editores de child entities en ViewModels
        /// </summary>
        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, string partialViewName)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new System.Web.Mvc.TemplateInfo
                {
                    HtmlFieldPrefix = name
                }
            };

            return helper.Partial(partialViewName, model, viewData);

        }
    }
}