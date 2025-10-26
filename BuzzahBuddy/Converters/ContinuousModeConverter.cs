using System.Globalization;

namespace BuzzahBuddy.Converters;

/// <summary>
/// Converter that converts a boolean IsContinuous property to a display string.
/// </summary>
public class ContinuousModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isContinuous)
        {
            return isContinuous ? "Continuous" : "Pulsed";
        }
        return "Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
