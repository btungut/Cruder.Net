using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cruder.Helper
{
    public static class CruderInputExtensions
    {
        // CheckBox

        public static MvcHtmlString CheckBox(this CruderHtmlHelper cruderHtmlHelper, string name)
        {
            return CheckBox(cruderHtmlHelper, name, htmlAttributes: (object)null);
        }

        public static MvcHtmlString CheckBox(this CruderHtmlHelper cruderHtmlHelper, string name, bool isChecked)
        {
            return CheckBox(cruderHtmlHelper, name, isChecked, htmlAttributes: (object)null);
        }

        public static MvcHtmlString CheckBox(this CruderHtmlHelper cruderHtmlHelper, string name, bool isChecked, object htmlAttributes)
        {
            return CheckBox(cruderHtmlHelper, name, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBox(this CruderHtmlHelper cruderHtmlHelper, string name, object htmlAttributes)
        {
            return CheckBox(cruderHtmlHelper, name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBox(this CruderHtmlHelper cruderHtmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            return CheckBoxHelper(cruderHtmlHelper, metadata: null, name: name, isChecked: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString CheckBox(this CruderHtmlHelper cruderHtmlHelper, string name, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            return CheckBoxHelper(cruderHtmlHelper, metadata: null, name: name, isChecked: isChecked, htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString CheckBoxFor<TModel>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, bool>> expression)
        {
            return CheckBoxFor(cruderHtmlHelper, expression, htmlAttributes: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString CheckBoxFor<TModel>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            return CheckBoxFor(cruderHtmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString CheckBoxFor<TModel>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, cruderHtmlHelper.HtmlHelper.ViewData);
            bool? isChecked = null;
            if (metadata.Model != null)
            {
                bool modelChecked;
                if (Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
                {
                    isChecked = modelChecked;
                }
            }

            return CheckBoxHelper(cruderHtmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), isChecked, htmlAttributes);
        }

        private static MvcHtmlString CheckBoxHelper(CruderHtmlHelper cruderHtmlHelper, ModelMetadata metadata, string name, bool? isChecked, IDictionary<string, object> htmlAttributes)
        {
            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

            bool explicitValue = isChecked.HasValue;
            if (explicitValue)
            {
                attributes.Remove("checked"); // Explicit value must override dictionary
            }

            return InputHelper(cruderHtmlHelper,
                               InputType.CheckBox,
                               metadata,
                               name,
                               value: "true",
                               useViewData: !explicitValue,
                               isChecked: isChecked ?? false,
                               setId: true,
                               isExplicitValue: false,
                               format: null,
                               htmlAttributes: attributes);
        }

        // Password

        public static MvcHtmlString Password(this CruderHtmlHelper cruderHtmlHelper, string name)
        {
            return Password(cruderHtmlHelper, name, value: null);
        }

        public static MvcHtmlString Password(this CruderHtmlHelper cruderHtmlHelper, string name, object value)
        {
            return Password(cruderHtmlHelper, name, value, htmlAttributes: null);
        }

        public static MvcHtmlString Password(this CruderHtmlHelper cruderHtmlHelper, string name, object value, object htmlAttributes)
        {
            return Password(cruderHtmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString Password(this CruderHtmlHelper cruderHtmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return PasswordHelper(cruderHtmlHelper, metadata: null, name: name, value: value, htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString PasswordFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return PasswordFor(cruderHtmlHelper, expression, htmlAttributes: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString PasswordFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return PasswordFor(cruderHtmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString PasswordFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            string validationClass = cruderHtmlHelper.GenerateValidationEngineClass((expression.Body as MemberExpression).Member);

            if (htmlAttributes != null && htmlAttributes["class"] != null)
            {
                htmlAttributes["class"] = htmlAttributes["class"].ToString() + " " + validationClass;
            }
            else
            {
                if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
                htmlAttributes.Add("class", validationClass);
            }

            return PasswordHelper(cruderHtmlHelper,
                                  ModelMetadata.FromLambdaExpression(expression, cruderHtmlHelper.HtmlHelper.ViewData),
                                  ExpressionHelper.GetExpressionText(expression),
                                  value: null,
                                  htmlAttributes: htmlAttributes);
        }

        private static MvcHtmlString PasswordHelper(CruderHtmlHelper cruderHtmlHelper, ModelMetadata metadata, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return InputHelper(cruderHtmlHelper,
                               InputType.Password,
                               metadata,
                               name,
                               value,
                               useViewData: false,
                               isChecked: false,
                               setId: true,
                               isExplicitValue: true,
                               format: null,
                               htmlAttributes: htmlAttributes);
        }

        // RadioButton

        public static MvcHtmlString RadioButton(this CruderHtmlHelper cruderHtmlHelper, string name, object value)
        {
            return RadioButton(cruderHtmlHelper, name, value, htmlAttributes: (object)null);
        }

        public static MvcHtmlString RadioButton(this CruderHtmlHelper cruderHtmlHelper, string name, object value, object htmlAttributes)
        {
            return RadioButton(cruderHtmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RadioButton(this CruderHtmlHelper cruderHtmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            // Determine whether or not to render the checked attribute based on the contents of ViewData.
            string valueString = Convert.ToString(value, CultureInfo.CurrentCulture);
            bool isChecked = (!String.IsNullOrEmpty(name)) && (String.Equals(cruderHtmlHelper.EvalString(name), valueString, StringComparison.OrdinalIgnoreCase));
            // checked attributes is implicit, so we need to ensure that the dictionary takes precedence.
            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);
            if (attributes.ContainsKey("checked"))
            {
                return InputHelper(cruderHtmlHelper,
                                   InputType.Radio,
                                   metadata: null,
                                   name: name,
                                   value: value,
                                   useViewData: false,
                                   isChecked: false,
                                   setId: true,
                                   isExplicitValue: true,
                                   format: null,
                                   htmlAttributes: attributes);
            }

            return RadioButton(cruderHtmlHelper, name, value, isChecked, htmlAttributes);
        }

        public static MvcHtmlString RadioButton(this CruderHtmlHelper cruderHtmlHelper, string name, object value, bool isChecked)
        {
            return RadioButton(cruderHtmlHelper, name, value, isChecked, htmlAttributes: (object)null);
        }

        public static MvcHtmlString RadioButton(this CruderHtmlHelper cruderHtmlHelper, string name, object value, bool isChecked, object htmlAttributes)
        {
            return RadioButton(cruderHtmlHelper, name, value, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RadioButton(this CruderHtmlHelper cruderHtmlHelper, string name, object value, bool isChecked, IDictionary<string, object> htmlAttributes)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            // checked attribute is an explicit parameter so it takes precedence.
            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);
            attributes.Remove("checked");
            return InputHelper(cruderHtmlHelper,
                               InputType.Radio,
                               metadata: null,
                               name: name,
                               value: value,
                               useViewData: false,
                               isChecked: isChecked,
                               setId: true,
                               isExplicitValue: true,
                               format: null,
                               htmlAttributes: attributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, object value)
        {
            return RadioButtonFor(cruderHtmlHelper, expression, value, htmlAttributes: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes)
        {
            return RadioButtonFor(cruderHtmlHelper, expression, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, object value, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, cruderHtmlHelper.HtmlHelper.ViewData);
            return RadioButtonHelper(cruderHtmlHelper,
                                     metadata,
                                     metadata.Model,
                                     ExpressionHelper.GetExpressionText(expression),
                                     value,
                                     null /* isChecked */,
                                     htmlAttributes);
        }

        private static MvcHtmlString RadioButtonHelper(CruderHtmlHelper cruderHtmlHelper, ModelMetadata metadata, object model, string name, object value, bool? isChecked, IDictionary<string, object> htmlAttributes)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

            bool explicitValue = isChecked.HasValue;
            if (explicitValue)
            {
                attributes.Remove("checked"); // Explicit value must override dictionary
            }
            else
            {
                string valueString = Convert.ToString(value, CultureInfo.CurrentCulture);
                isChecked = model != null &&
                            !String.IsNullOrEmpty(name) &&
                            String.Equals(model.ToString(), valueString, StringComparison.OrdinalIgnoreCase);
            }

            return InputHelper(cruderHtmlHelper,
                               InputType.Radio,
                               metadata,
                               name,
                               value,
                               useViewData: false,
                               isChecked: isChecked ?? false,
                               setId: true,
                               isExplicitValue: true,
                               format: null,
                               htmlAttributes: attributes);
        }

        // TextBox

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name)
        {
            return TextBox(cruderHtmlHelper, name, value: null);
        }

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name, object value)
        {
            return TextBox(cruderHtmlHelper, name, value, format: null);
        }

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name, object value, string format)
        {
            return TextBox(cruderHtmlHelper, name, value, format, htmlAttributes: (object)null);
        }

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name, object value, object htmlAttributes)
        {
            return TextBox(cruderHtmlHelper, name, value, format: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name, object value, string format, object htmlAttributes)
        {
            return TextBox(cruderHtmlHelper, name, value, format, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            return TextBox(cruderHtmlHelper, name, value, format: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString TextBox(this CruderHtmlHelper cruderHtmlHelper, string name, object value, string format, IDictionary<string, object> htmlAttributes)
        {
            return InputHelper(cruderHtmlHelper,
                               InputType.Text,
                               metadata: null,
                               name: name,
                               value: value,
                               useViewData: (value == null),
                               isChecked: false,
                               setId: true,
                               isExplicitValue: true,
                               format: format,
                               htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return TextBoxFor(cruderHtmlHelper, expression, format: null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return TextBoxFor(cruderHtmlHelper, expression, format, (IDictionary<string, object>)null);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return TextBoxFor(cruderHtmlHelper, expression, format: null, htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return TextBoxFor(cruderHtmlHelper, expression, format: format, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return TextBoxFor(cruderHtmlHelper, expression, format: null, htmlAttributes: htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this CruderHtmlHelper<TModel> cruderHtmlHelper, Expression<Func<TModel, TProperty>> expression, string format, IDictionary<string, object> htmlAttributes)
        {
            string validationClass = cruderHtmlHelper.GenerateValidationEngineClass((expression.Body as MemberExpression).Member);

            if (htmlAttributes!=null && htmlAttributes["class"]!=null)
            {
                htmlAttributes["class"] = htmlAttributes["class"].ToString() + " " + validationClass;
            }
            else
            {
                if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
                htmlAttributes.Add("class", validationClass);
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, cruderHtmlHelper.HtmlHelper.ViewData);
            return TextBoxHelper(cruderHtmlHelper,
                                 metadata,
                                 metadata.Model,
                                 ExpressionHelper.GetExpressionText(expression),
                                 format,
                                 htmlAttributes);
        }


        private static MvcHtmlString TextBoxHelper(this CruderHtmlHelper cruderHtmlHelper, ModelMetadata metadata, object model, string expression, string format, IDictionary<string, object> htmlAttributes)
        {
            return InputHelper(cruderHtmlHelper,
                               InputType.Text,
                               metadata,
                               expression,
                               model,
                               useViewData: false,
                               isChecked: false,
                               setId: true,
                               isExplicitValue: true,
                               format: format,
                               htmlAttributes: htmlAttributes);
        }

        // Helper methods

        private static MvcHtmlString InputHelper(CruderHtmlHelper cruderHtmlHelper, InputType inputType, ModelMetadata metadata, string name, object value, bool useViewData, bool isChecked, bool setId, bool isExplicitValue, string format, IDictionary<string, object> htmlAttributes)
        {
            string fullName = cruderHtmlHelper.HtmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }

            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(inputType));
            tagBuilder.MergeAttribute("name", fullName, true);

            string valueParameter = cruderHtmlHelper.HtmlHelper.FormatValue(value, format);
            bool usedModelState = false;

            switch (inputType)
            {
                case InputType.CheckBox:
                    bool? modelStateWasChecked = cruderHtmlHelper.GetModelStateValue(fullName, typeof(bool)) as bool?;
                    if (modelStateWasChecked.HasValue)
                    {
                        isChecked = modelStateWasChecked.Value;
                        usedModelState = true;
                    }
                    goto case InputType.Radio;
                case InputType.Radio:
                    if (!usedModelState)
                    {
                        string modelStateValue = cruderHtmlHelper.GetModelStateValue(fullName, typeof(string)) as string;
                        if (modelStateValue != null)
                        {
                            isChecked = String.Equals(modelStateValue, valueParameter, StringComparison.Ordinal);
                            usedModelState = true;
                        }
                    }
                    if (!usedModelState && useViewData)
                    {
                        isChecked = cruderHtmlHelper.EvalBoolean(fullName);
                    }
                    if (isChecked)
                    {
                        tagBuilder.MergeAttribute("checked", "checked");
                    }
                    tagBuilder.MergeAttribute("value", valueParameter, isExplicitValue);
                    break;
                case InputType.Password:
                    if (value != null)
                    {
                        tagBuilder.MergeAttribute("value", valueParameter, isExplicitValue);
                    }
                    break;
                default:
                    string attemptedValue = (string)cruderHtmlHelper.GetModelStateValue(fullName, typeof(string));
                    tagBuilder.MergeAttribute("value", attemptedValue ?? ((useViewData) ? cruderHtmlHelper.EvalString(fullName, format) : valueParameter), isExplicitValue);
                    break;
            }

            if (setId)
            {
                tagBuilder.GenerateId(fullName);
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (cruderHtmlHelper.HtmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            //tagBuilder.MergeAttributes(cruderHtmlHelper.HtmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            if (inputType == InputType.CheckBox)
            {
                // Render an additional <input type="hidden".../> for checkboxes. This
                // addresses scenarios where unchecked checkboxes are not sent in the request.
                // Sending a hidden input makes it possible to know that the checkbox was present
                // on the page when the request was submitted.
                StringBuilder inputItemBuilder = new StringBuilder();
                inputItemBuilder.Append(tagBuilder.ToString(TagRenderMode.SelfClosing));

                TagBuilder hiddenInput = new TagBuilder("input");
                hiddenInput.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
                hiddenInput.MergeAttribute("name", fullName);
                hiddenInput.MergeAttribute("value", "false");
                inputItemBuilder.Append(hiddenInput.ToString(TagRenderMode.SelfClosing));
                return MvcHtmlString.Create(inputItemBuilder.ToString());
            }

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        private static RouteValueDictionary ToRouteValueDictionary(IDictionary<string, object> dictionary)
        {
            return dictionary == null ? new RouteValueDictionary() : new RouteValueDictionary(dictionary);
        }
    }

    internal static class Extension
    {
        public static object GetModelStateValue(this CruderHtmlHelper cruderHtmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (cruderHtmlHelper.HtmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        public static string EvalString(this CruderHtmlHelper cruderHtmlHelper, string key)
        {
            return Convert.ToString(cruderHtmlHelper.HtmlHelper.ViewData.Eval(key), CultureInfo.CurrentCulture);
        }

        public static string EvalString(this CruderHtmlHelper cruderHtmlHelper, string key, string format)
        {
            return Convert.ToString(cruderHtmlHelper.HtmlHelper.ViewData.Eval(key, format), CultureInfo.CurrentCulture);
        }

        public static bool EvalBoolean(this CruderHtmlHelper cruderHtmlHelper, string key)
        {
            return Convert.ToBoolean(cruderHtmlHelper.HtmlHelper.ViewData.Eval(key), CultureInfo.InvariantCulture);
        }
    }
}