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
    /// Wrapper that encapsulate filter property informations as well as precompiled value getter
    /// </summary>
    internal sealed class PropertyFilterValueGetter
    {
        /// <summary>
        ///   Precompiled value getter
        /// </summary>
        private readonly Func<object, object> _propertyValueGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilterValueGetter"/> class.
        /// </summary>
        /// <param name="propertyFilter">
        /// Associated property Filter descriptor
        /// </param>
        /// <param name="type">
        /// Underlying type
        /// </param>
        public PropertyFilterValueGetter(PropertyFilter propertyFilter, Type type)
        {
            PropertyFilterDescriptor = propertyFilter;
            _propertyValueGetter = CompileValueGetter(propertyFilter.FieldName, type);
        }

        /// <summary>
        ///   Property filter descriptor
        /// </summary>
        public PropertyFilter PropertyFilterDescriptor { get; private set; }

        /// <summary>
        /// Return the string value to test against
        /// </summary>
        /// <param name="candidate">
        /// Candidate for which retreive value
        /// </param>
        /// <returns>
        /// Candidate formated values
        /// </returns>
        public string GetValue(object candidate)
        {
            object oValue = _propertyValueGetter(candidate);

            if (oValue != null)
            {
                return PropertyFilterDescriptor.Convert(oValue);
            }

            return string.Empty;
        }


        /// <summary>
        /// Return a precompiled propertyValue getter
        /// </summary>
        /// <param name="propertyName">
        /// Property name for which to generate the delegate
        /// </param>
        /// <param name="type">
        /// Container type
        /// </param>
        /// <returns>
        /// Compiled delegate
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