using System;
using System.Windows;
using System.Windows.Data;

namespace DiagramDesigner.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityCvt : ConverterBase
    {

        private static BoolToVisibilityCvt _instance;

        public static BoolToVisibilityCvt Instance => _instance ??= new BoolToVisibilityCvt();


        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isBool)
            {
                return isBool ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
