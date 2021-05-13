using System.Windows;
using DiagramDesigner.BaseClass.ConnectorClass;

namespace DiagramDesigner.BaseClass
{
    public class PointHelper
    {
        public static Point GetPointForConnector(FullyCreatedConnectorInfo connector)
        {
            Point point = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Top:
                    point = new Point(connector.DesignerItem.Left + (connector.DesignerItem.ItemWidth / 2), connector.DesignerItem.Top - (ConnectorInfoBase.ConnectorHeight));
                    break;
                case ConnectorOrientation.Bottom:
                    point = new Point(connector.DesignerItem.Left + (connector.DesignerItem.ItemWidth / 2), (connector.DesignerItem.Top + connector.DesignerItem.ItemHeight) + (ConnectorInfoBase.ConnectorHeight / 2));
                    break;
                case ConnectorOrientation.Right:
                    point = new Point(connector.DesignerItem.Left + connector.DesignerItem.ItemWidth + (ConnectorInfoBase.ConnectorWidth), connector.DesignerItem.Top + (connector.DesignerItem.ItemHeight / 2));
                    break;
                case ConnectorOrientation.Left:
                    point = new Point(connector.DesignerItem.Left - ConnectorInfoBase.ConnectorWidth, connector.DesignerItem.Top + (connector.DesignerItem.ItemHeight / 2));
                    break;
            }

            return point;
        }


    }
}
