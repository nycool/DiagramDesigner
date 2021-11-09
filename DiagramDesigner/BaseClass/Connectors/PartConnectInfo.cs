using System.Windows;

namespace DiagramDesigner.BaseClass.Connectors
{
    /// <summary>
    /// 部分连接上的点
    /// </summary>
    public class PartConnectInfo : ConnectBaseInfo
    {
        public Point CurrentLocation { get; private set; }

        public PartConnectInfo(Point currentLocation)
            : base(ConnectorOrientation.None)
        {
            this.CurrentLocation = currentLocation;
        }
    }
}