using System.Globalization;
using System.Windows.Data;

namespace GamerRadio.Helpers;

internal class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            throw new ArgumentException("Value must be a boolean.", nameof(value));
        }

        // If the parameter is "Invert", reverse the logic
        var invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) ?? false;

        return (boolValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
        {
            throw new ArgumentException("Value must be a Visibility.", nameof(value));
        }

        var invert = parameter?.ToString()?.Equals("Invert", StringComparison.OrdinalIgnoreCase) ?? false;

        return (visibility == Visibility.Visible) ^ invert;
    }
}
