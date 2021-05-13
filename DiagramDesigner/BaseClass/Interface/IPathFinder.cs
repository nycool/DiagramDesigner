using System.Collections.Generic;
using System.Windows;

namespace DiagramDesigner.BaseClass.Interface
{
    public interface IPathFinder
    {
        List<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine);
        List<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation);
    }
}
