using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace dotnetexplorer.blog.com.WPFIcRtSandFc.SampleApp
{
    public class SampleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
            {
                var str = value.ToString();


                str = str.Insert(2, ".");
                str = str.Insert(4, ".");
                str = str.Insert(8, ".");
                return str;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
