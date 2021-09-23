using System.Windows;
using DiagramDesigner.BaseClass.Connectors;

namespace DiagramDesigner.BaseClass
{
    public class PointHelper
    {
        public static Point GetPointForConnector(Connector connector)
        {
            Point point = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Top:
                    point = new Point(connector.DesignerItem.Left + (connector.DesignerItem.ItemWidth / 2), connector.DesignerItem.Top - (ConnectorBase.ConnectorHeight));
                    break;
                case ConnectorOrientation.Bottom:
                    point = new Point(connector.DesignerItem.Left + (connector.DesignerItem.ItemWidth / 2), connector.DesignerItem.Top + connector.DesignerItem.ItemHeight + ConnectorBase.ConnectorHeight / 2);
                    break;
                case ConnectorOrientation.Right:
                    point = new Point(connector.DesignerItem.Left + connector.DesignerItem.ItemWidth + ConnectorBase.ConnectorWidth, connector.DesignerItem.Top + connector.DesignerItem.ItemHeight / 2);
                    break;
                case ConnectorOrientation.Left:
                    point = new Point(connector.DesignerItem.Left - ConnectorBase.ConnectorWidth, connector.DesignerItem.Top + (connector.DesignerItem.ItemHeight / 2));
                    break;
            }

            return point;
        }


    }
}
