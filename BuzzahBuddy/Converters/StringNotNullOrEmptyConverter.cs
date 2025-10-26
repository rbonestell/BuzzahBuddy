using System.Globalization;

namespace BuzzahBuddy.Converters;

/// <summary>
/// Converter that returns true if the string value is not null or empty.
/// </summary>
public class StringNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
