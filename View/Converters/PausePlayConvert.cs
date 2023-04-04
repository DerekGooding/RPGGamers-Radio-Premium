using System;
using System.Globalization;
using System.Windows.Data;

namespace Radio_Leech.View.Converters
{
    public class PausePlayConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "■";
            else
                return "▶";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

