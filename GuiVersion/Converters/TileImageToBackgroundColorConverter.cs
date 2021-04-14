using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SweeperCore;

namespace GuiVersion.Converters
{
    [ValueConversion(typeof(TileImage), typeof(SolidColorBrush))]
    class TileImageToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TileImage image)
            {
                return image switch
                {
                    TileImage.Hidden => new SolidColorBrush(Colors.LightGray),
                    TileImage.Exploded => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.White)

                };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
