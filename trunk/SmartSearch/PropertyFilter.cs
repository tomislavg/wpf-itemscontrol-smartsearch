#region

using System;
using System.Windows;
using System.Windows.Data;

#endregion

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    /// <summary>
    ///   Define an object property on wich filter should be applied
    /// </summary>
    public sealed class PropertyFilter : FrameworkElement
    {
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register("TextFormat", typeof (string), typeof (PropertyFilter),
                                        new UIPropertyMetadata(string.Empty, OnTextFormatChanged));

        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register("ValueConverter", typeof (IValueConverter), typeof (PropertyFilter),
                                        new UIPropertyMetadata(null, OnValueConverterChanged));

        private readonly Func<object, string> converReturn;


        private ValueTransform transformMode = ValueTransform.None;

        public PropertyFilter()
        {
            converReturn =
                (Func<object, string>)
                Delegate.CreateDelegate(typeof (Func<object, string>), this, "ReturnConvert", false);
        }

        /// <summary>
        ///   Constructor with monitor parameter
        /// </summary>
        /// <param name = "fieldName"></param>
        /// <param name = "monitor"></param>
        public PropertyFilter(string fieldName, bool monitor)
        {
            FieldName = fieldName;
            MonitorPropertyChanged = monitor;
        }

        /// <summary>
        ///   Constructor with property name only
        /// </summary>
        /// <param name = "fieldName"></param>
        public PropertyFilter(string fieldName)
        {
            FieldName = fieldName;
        }

        /// <summary>
        ///   Property name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        ///   Does the smart search component has to monitor property changes on that property
        /// </summary>
        public bool MonitorPropertyChanged { get; set; }


        private ValueTransform TransformMode
        {
            get { return transformMode; }
            set { transformMode = value; }
        }


        public string TextFormat
        {
            get { return (string) GetValue(TextFormatProperty); }
            set { SetValue(TextFormatProperty, value); }
        }


        public IValueConverter ValueConverter
        {
            get { return (IValueConverter) GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }


        private static void OnTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetTransform(d, ValueTransform.TextFormat);
        }

        private static void OnValueConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetTransform(d, ValueTransform.ValueConverter);
        }

        private static void SetTransform(DependencyObject d, ValueTransform transformation)
        {
            var ssvc = (PropertyFilter) d;
            if (ssvc != null)
            {
                if (ssvc.TransformMode != ValueTransform.None)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "ValueTransform has already been set to {0} for this PropertyFilter [FielName = {1}]",
                            ssvc.TransformMode, ssvc.FieldName));
                }
                ssvc.TransformMode = transformation;
            }
        }

        public string Convert(object value)
        {
            return converReturn(value);
        }

        /// <summary>
        ///   Decide which formating to apply given the value transform value
        /// </summary>
        /// <param name = "value">Value to format</param>
        /// <returns>Formated value</returns>
        private string ReturnConvert(object value)
        {
            if (value != null)
            {
                switch (TransformMode)
                {
                    case ValueTransform.None:
                        return value.ToString();
                    case ValueTransform.TextFormat:
                        return TextFormating(value);
                    case ValueTransform.ValueConverter:
                        return ValueConverter.Convert(value, null, null, null).ToString();
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        ///   Format value type values with a given string formating mask
        /// </summary>
        /// <param name = "value">Original Value to format</param>
        /// <returns>String formated value</returns>
        private string TextFormating(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                    return ((SByte) value).ToString(TextFormat);
                case TypeCode.Byte:
                    return ((Byte) value).ToString(TextFormat);
                case TypeCode.Int16:
                    return ((Int16) value).ToString(TextFormat);
                case TypeCode.UInt16:
                    return ((UInt16) value).ToString(TextFormat);
                case TypeCode.Int32:
                    return ((Int32) value).ToString(TextFormat);
                case TypeCode.UInt32:
                    return ((UInt32) value).ToString(TextFormat);
                case TypeCode.Int64:
                    return ((Int64) value).ToString(TextFormat);
                case TypeCode.UInt64:
                    return ((UInt64) value).ToString(TextFormat);
                case TypeCode.Single:
                    return ((Single) value).ToString(TextFormat);
                case TypeCode.Double:
                    return ((Double) value).ToString(TextFormat);
                case TypeCode.Decimal:
                    return ((Decimal) value).ToString(TextFormat);
                case TypeCode.DateTime:
                    return ((DateTime) value).ToString(TextFormat);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Nested type: ValueTransform

        private enum ValueTransform
        {
            None,
            TextFormat,
            ValueConverter,
        }

        #endregion
    }
}