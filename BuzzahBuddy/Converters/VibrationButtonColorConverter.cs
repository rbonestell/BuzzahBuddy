using System.Globalization;

namespace BuzzahBuddy.Converters;

/// <summary>
/// Converter that returns appropriate button color based on vibration state.
/// </summary>
public class VibrationButtonColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isVibrating)
        {
            // Return color resource key - use pastel danger for gentler stop button
            return isVibrating ? Application.Current?.Resources["PastelDanger"] : Application.Current?.Resources["Success"];
        }
        return Application.Current?.Resources["Primary"];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
