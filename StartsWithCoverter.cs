using System;
using System.Globalization;
using System.Windows.Data;

namespace ChatSystem
{
    public class StartsWithConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text && parameter is string prefix)
            {
                return text.StartsWith(prefix);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}