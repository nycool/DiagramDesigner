using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DiagramDesigner.Converters
{
    [ValueConversion(typeof(List<Point>), typeof(PathSegmentCollection))]
    public class ConnectionPathCvt : ConverterBase
    {
        private static ConnectionPathCvt _instance;

        public static ConnectionPathCvt Instance => _instance ??= new ConnectionPathCvt();


        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<Point> points)
            {
                var pointCollection = new PointCollection();

                foreach (Point point in points)
                {
                    pointCollection.Add(point);
                }
                return pointCollection;
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
