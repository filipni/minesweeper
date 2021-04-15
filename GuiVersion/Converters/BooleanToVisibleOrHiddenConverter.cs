using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GuiVersion.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BooleanToVisibleOrHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool visible && visible)
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
