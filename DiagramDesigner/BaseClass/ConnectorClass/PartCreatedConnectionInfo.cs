using System.Windows;

namespace DiagramDesigner.BaseClass.ConnectorClass
{
    /// <summary>
    /// 部分连接上的点
    /// </summary>
    public class PartCreatedConnectionInfo : ConnectorInfoBase
    {
        public Point CurrentLocation { get; private set; }

        public PartCreatedConnectionInfo(Point currentLocation)
            : base(ConnectorOrientation.None)
        {
            this.CurrentLocation = currentLocation;
        }
    }
}
