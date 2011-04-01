using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WPFCommons.SmartSearch
{
    /// <summary>
    /// Wrapper that encapsulate filter property informations as well as precompiled value getter
    /// </summary>
    internal class PropertyFilterValueGetter
    {
        /// <summary>
        /// Precompiled value getter
        /// </summary>
        private readonly Func<object, object> propertyValueGetter;

        public PropertyFilterValueGetter(PropertyFilter propertyFilter, Type type)
        {
            PropertyFilerDescriptor = propertyFilter;
            propertyValueGetter = CompileValueGetter(propertyFilter.FieldName, type);
        }

        /// <summary>
        /// Property filter descriptor
        /// </summary>
        public PropertyFilter PropertyFilerDescriptor { get; private set; }

        /// <summary>
        /// Property filter weight
        /// </summary>
        public int PropertyWeight { get; set; }

        /// <summary>
        /// Return the string value to test against
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public string GetValue(object candidate)
        {
            object oValue = propertyValueGetter(candidate);

            if (oValue != null)
            {
                return PropertyFilerDescriptor.Convert(oValue);
            }

            return string.Empty;
        }


        /// <summary>
        /// Return a precompiled propertyValue getter
        /// </summary>
        /// <param name="propertyInfo">Property info for which to generate the delegate</param>
        /// <param name="type">Container type</param>
        /// <returns>Compiled delegate</returns>
        private static Func<object, object> CompileValueGetter(string propertyName, Type type)
        {
            ParameterExpression param = Expression.Parameter(typeof(object), "Candidate");
            LambdaExpression func = Expression.Lambda(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(
                            param,
                            type
                            ),
                        propertyName
                        ),
                    typeof(object)
                    ),
                param
                );
            return (Func<object, object>)func.Compile();
        }
    }
}