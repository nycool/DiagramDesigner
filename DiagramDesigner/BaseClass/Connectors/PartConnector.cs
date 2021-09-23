using System.Windows;

namespace DiagramDesigner.BaseClass.Connectors
{
    /// <summary>
    /// 部分连接上的点
    /// </summary>
    public class PartConnector : ConnectorBase
    {
        public Point CurrentLocation { get; private set; }

        public PartConnector(Point currentLocation)
            : base(ConnectorOrientation.None)
        {
            this.CurrentLocation = currentLocation;
        }
    }
}