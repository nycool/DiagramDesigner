using System.Windows;
using DiagramDesigner.BaseClass.Connectors;

namespace DiagramDesigner.BaseClass
{
    public class PointHelper
    {
        public static Point GetPointForConnector(Connectors.ConnectInfo connector)
        {
            Point point = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Top:
                    point = new Point(connector.DesignerItem.Left + (connector.DesignerItem.ItemWidth / 2), connector.DesignerItem.Top - (ConnectBaseInfo.ConnectorHeight));
                    break;
                case ConnectorOrientation.Bottom:
                    point = new Point(connector.DesignerItem.Left + (connector.DesignerItem.ItemWidth / 2), connector.DesignerItem.Top + connector.DesignerItem.ItemHeight + ConnectBaseInfo.ConnectorHeight / 2);
                    break;
                case ConnectorOrientation.Right:
                    point = new Point(connector.DesignerItem.Left + connector.DesignerItem.ItemWidth + ConnectBaseInfo.ConnectorWidth, connector.DesignerItem.Top + connector.DesignerItem.ItemHeight / 2);
                    break;
                case ConnectorOrientation.Left:
                    point = new Point(connector.DesignerItem.Left - ConnectBaseInfo.ConnectorWidth, connector.DesignerItem.Top + (connector.DesignerItem.ItemHeight / 2));
                    break;
            }

            return point;
        }


    }
}
