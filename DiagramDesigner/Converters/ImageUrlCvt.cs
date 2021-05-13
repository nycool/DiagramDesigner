using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagramDesigner.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class ImageUrlCvt : ConverterBase
    {

        private static ImageUrlCvt _instance;
        public static ImageUrlCvt Instance => _instance ??= new ImageUrlCvt();


        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is string url)
            {
                Uri imagePath = new Uri(url, UriKind.RelativeOrAbsolute);

                return new BitmapImage(imagePath);
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
