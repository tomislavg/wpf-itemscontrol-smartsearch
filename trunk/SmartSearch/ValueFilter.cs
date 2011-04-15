

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SmartSearch
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    ///   Define an object property on wich filter should be applied
    /// </summary>
    public class ValueFilter : FrameworkElement
    {
        /// <summary>
        ///   The text format property.
        /// </summary>
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register("TextFormat", typeof (string), typeof (ValueFilter),
                                        new UIPropertyMetadata(string.Empty, OnTextFormatChanged));

        /// <summary>
        ///   The value converter property.
        /// </summary>
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register("ValueConverter", typeof (IValueConverter), typeof (ValueFilter),
                                        new UIPropertyMetadata(null, OnValueConverterChanged));

        /// <summary>
        ///   The conver return.
        /// </summary>
        private readonly Func<object, string> converReturn;


        /// <summary>
        ///   The transform mode.
        /// </summary>
        private ValueTransform transformMode = ValueTransform.None;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ValueFilter" /> class.
        /// </summary>
        public ValueFilter()
        {
            converReturn =
                (Func<object, string>)
                Delegate.CreateDelegate(typeof (Func<object, string>), this, "ReturnConvert", false);
        }


        /// <summary>
        ///   Gets or sets TransformMode.
        /// </summary>
        private ValueTransform TransformMode
        {
            get { return transformMode; }
            set { transformMode = value; }
        }


        /// <summary>
        ///   Gets or sets TextFormat.
        /// </summary>
        public string TextFormat
        {
            get { return (string) GetValue(TextFormatProperty); }
            set { SetValue(TextFormatProperty, value); }
        }


        /// <summary>
        ///   Gets or sets ValueConverter.
        /// </summary>
        public IValueConverter ValueConverter
        {
            get { return (IValueConverter) GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }


        /// <summary>
        ///   The on text format changed.
        /// </summary>
        /// <param name = "d">
        ///   The d.
        /// </param>
        /// <param name = "e">
        ///   The e.
        /// </param>
        private static void OnTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetTransform(d, ValueTransform.TextFormat);
        }

        /// <summary>
        ///   The on value converter changed.
        /// </summary>
        /// <param name = "d">
        ///   The d.
        /// </param>
        /// <param name = "e">
        ///   The e.
        /// </param>
        private static void OnValueConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetTransform(d, ValueTransform.ValueConverter);
        }

        /// <summary>
        ///   The set transform.
        /// </summary>
        /// <param name = "d">
        ///   The d.
        /// </param>
        /// <param name = "transformation">
        ///   The transformation.
        /// </param>
        /// <exception cref = "InvalidOperationException">
        /// </exception>
        private static void SetTransform(DependencyObject d, ValueTransform transformation)
        {
            var ssvc = (ValueFilter) d;
            if (ssvc != null)
            {
                if (ssvc.TransformMode != ValueTransform.None)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "ValueTransform has already been set to {0} for this ValueFilter", ssvc.TransformMode));
                }

                ssvc.TransformMode = transformation;
            }
        }

        /// <summary>
        ///   The convert.
        /// </summary>
        /// <param name = "value">
        ///   The value.
        /// </param>
        /// <returns>
        ///   The convert.
        /// </returns>
        public string Convert(object value)
        {
            return converReturn(value);
        }

        /// <summary>
        ///   Decide which formating to apply given the value transform value
        /// </summary>
        /// <param name = "value">
        ///   Value to format
        /// </param>
        /// <returns>
        ///   Formated value
        /// </returns>
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
        /// <param name = "value">
        ///   Original Value to format
        /// </param>
        /// <returns>
        ///   String formated value
        /// </returns>
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

        /// <summary>
        ///   The value transform.
        /// </summary>
        protected enum ValueTransform
        {
            /// <summary>
            ///   The none.
            /// </summary>
            None,

            /// <summary>
            ///   The text format.
            /// </summary>
            TextFormat,

            /// <summary>
            ///   The value converter.
            /// </summary>
            ValueConverter,
        }

        #endregion
    }
}