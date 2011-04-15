// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyFilterValueGetter.cs" company="dotnetexplorer.blog.com">
//   2011
// </copyright>
// <summary>
//   Wrapper that encapsulate filter property informations as well as precompiled value getter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///   Wrapper that encapsulate filter property informations as well as precompiled value getter
    /// </summary>
    internal sealed class PropertyFilterValueGetter
    {
        /// <summary>
        ///   Precompiled value getter
        /// </summary>
        private readonly Func<object, object> _propertyValueGetter;

        private readonly string fieldName = string.Empty;

        private readonly bool isNativeType;
        private readonly bool monitorPropertyChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PropertyFilterValueGetter" /> class.
        /// </summary>
        /// <param name = "valueFilter">
        ///   Associated property Filter descriptor
        /// </param>
        /// <param name = "type">
        ///   Underlying type
        /// </param>
        public PropertyFilterValueGetter(PropertyFilter valueFilter, Type type)
        {
            ValueFilterDescriptor = valueFilter;
            fieldName = valueFilter.FieldName;
            monitorPropertyChanged = valueFilter.MonitorPropertyChanged;
            _propertyValueGetter = CompileValueGetter(valueFilter.FieldName, type);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PropertyFilterValueGetter" /> class for value type or string candidates
        /// </summary>
        /// <param name = "valueFilter">
        ///   The property filter.
        /// </param>
        public PropertyFilterValueGetter(ValueFilter valueFilter)
        {
            ValueFilterDescriptor = valueFilter;
            isNativeType = true;
        }

        /// <summary>
        ///   Property filter descriptor
        /// </summary>
        private ValueFilter ValueFilterDescriptor { get; set; }

        public bool MonitorPropertyChanged
        {
            get { return monitorPropertyChanged; }
        }

        public string FieldName
        {
            get { return fieldName; }
        }


        /// <summary>
        ///   Return the string value to test against
        /// </summary>
        /// <param name = "candidate">
        ///   Candidate for which retreive value
        /// </param>
        /// <returns>
        ///   Candidate formated values
        /// </returns>
        public string GetValue(object candidate)
        {
            object oValue = isNativeType ? candidate : _propertyValueGetter(candidate);

            if (oValue != null)
            {
                return ValueFilterDescriptor.Convert(oValue);
            }

            return string.Empty;
        }


        /// <summary>
        ///   Return a precompiled propertyValue getter
        /// </summary>
        /// <param name = "propertyName">
        ///   Property name for which to generate the delegate
        /// </param>
        /// <param name = "type">
        ///   Container type
        /// </param>
        /// <returns>
        ///   Compiled delegate
        /// </returns>
        private static Func<object, object> CompileValueGetter(string propertyName, Type type)
        {
            ParameterExpression param = Expression.Parameter(typeof (object), "Candidate");
            LambdaExpression func = Expression.Lambda(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(
                            param,
                            type
                            ),
                        propertyName
                        ),
                    typeof (object)
                    ),
                param
                );
            return (Func<object, object>) func.Compile();
        }
    }
}
