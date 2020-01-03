using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FamilyBudget.v3.App_CodeBase;

namespace FamilyBudget.v3.App_Helpers
{
    public static class HtmlExtentions
    {
        public static void EnablePartialViewValidation(this HtmlHelper helper)
        {
            if (helper.ViewContext.FormContext == null)
            {
                helper.ViewContext.FormContext = new FormContext();
            }
        }
    }

    public static class SelectExtensions
    {
        // DropDownList

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name)
        {
            return ExtendedDropDownList(htmlHelper, name, null /* selectList */, null /* optionLabel */, null
                /* htmlAttributes */);
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return ExtendedDropDownList(htmlHelper, name, null /* selectList */, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<ExtendedSelectListItem> selectList)
        {
            return ExtendedDropDownList(htmlHelper, name, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<ExtendedSelectListItem> selectList, object htmlAttributes)
        {
            return ExtendedDropDownList(htmlHelper, name, selectList, null /* optionLabel */,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<ExtendedSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return ExtendedDropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<ExtendedSelectListItem> selectList, string optionLabel)
        {
            return ExtendedDropDownList(htmlHelper, name, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return ExtendedDropDownList(htmlHelper, name, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ExtendedDropDownList(this HtmlHelper htmlHelper, string name,
            IEnumerable<ExtendedSelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, null, name, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList,
            object htmlAttributes)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList,
            string optionLabel)
        {
            return DropDownListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList,
            string optionLabel, object htmlAttributes)
        {
            return DropDownListFor(htmlHelper, expression, selectList, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList,
            string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            return DropDownListHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), selectList,
                optionLabel, htmlAttributes);
        }

        private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string expression,
            IEnumerable<ExtendedSelectListItem> selectList, string optionLabel,
            IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, metadata, optionLabel, expression, selectList, false, htmlAttributes);
        }

        // Helper methods

        private static IEnumerable<ExtendedSelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException("IEnumerable<SelectListItem>");
            }
            var selectList = o as IEnumerable<ExtendedSelectListItem>;
            if (selectList == null)
            {
                throw new InvalidOperationException("IEnumerable<SelectListItem>");
            }
            return selectList;
        }

        internal static string ListItemToOption(ExtendedSelectListItem item)
        {
            var builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            if (item.IsBold)
            {
                builder.Attributes["class"] = "bold";
            }
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
            return builder.ToString(TagRenderMode.Normal);
        }

        private static IEnumerable<ExtendedSelectListItem> GetSelectListWithDefaultValue(
            IEnumerable<ExtendedSelectListItem> selectList, object defaultValue, bool allowMultiple)
        {
            IEnumerable defaultValues;

            if (allowMultiple)
            {
                defaultValues = defaultValue as IEnumerable;
                if (defaultValues == null || defaultValues is string)
                {
                    throw new InvalidOperationException("expression");
                }
            }
            else
            {
                defaultValues = new[] {defaultValue};
            }

            IEnumerable<string> values = from object value in defaultValues
                select Convert.ToString(value, CultureInfo.CurrentCulture);
            var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
            var newSelectList = new List<ExtendedSelectListItem>();

            foreach (ExtendedSelectListItem item in selectList)
            {
                item.Selected = (item.Value != null)
                    ? selectedValues.Contains(item.Value)
                    : selectedValues.Contains(item.Text);
                newSelectList.Add(item);
            }
            return newSelectList;
        }

        private static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, ModelMetadata metadata,
            string optionLabel, string name, IEnumerable<ExtendedSelectListItem> selectList, bool allowMultiple,
            IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(name);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple)
                ? htmlHelper.GetModelStateValue(fullName, typeof (string[]))
                : htmlHelper.GetModelStateValue(fullName, typeof (string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData && defaultValue == null && !String.IsNullOrEmpty(name))
            {
                defaultValue = htmlHelper.ViewData.Eval(name);
            }

            if (defaultValue != null)
            {
                selectList = GetSelectListWithDefaultValue(selectList, defaultValue, allowMultiple);
            }

            // Convert each ListItem to an <option> tag
            var listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(
                    ListItemToOption(new ExtendedSelectListItem
                    {
                        Text = optionLabel,
                        Value = String.Empty,
                        Selected = false
                    }));
            }

            foreach (ExtendedSelectListItem item in selectList)
            {
                listItemBuilder.AppendLine(ListItemToOption(item));
            }

            var tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}