using System;
using System.Globalization;
using System.Windows.Data;

namespace DiagramDesigner.Converters
{
    public abstract class ConverterBase : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) => true;


        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => true;

    }
}
