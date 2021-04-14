using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GuiVersion.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    class FilePathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                var converter = new ImageSourceConverter();
                var imageSource = converter.ConvertFromString(path) as ImageSource;
                return imageSource;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
