using System;
using System.Windows.Data;

namespace KagEditor.WpfCore.Utils
{
    public class EnumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.GetValues(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
