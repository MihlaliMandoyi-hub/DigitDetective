using System.Globalization;

namespace DigitDetective;

public class BoolToBorderColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isUnlocked = value is bool b && b;
        return isUnlocked ? Color.FromArgb("#FFD23F") : Color.FromArgb("#55FFFFFF");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}