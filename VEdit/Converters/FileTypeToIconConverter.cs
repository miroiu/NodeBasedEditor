using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using VEdit.UI;

namespace VEdit.Converters
{
    [ValueConversion(typeof(ProjectFile), typeof(BitmapImage))]
    public class FileTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProjectFile file)
            {
                return new BitmapImage(new Uri($"pack://application:,,,/Resources/Icons/{file.Type}.png"));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
