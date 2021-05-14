using System.Collections.Generic;
using System.Windows;
using DiagramDesigner.BaseClass;

namespace DiagramDesigner.Interface
{
    public interface IPathFinder
    {
        List<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine);
        List<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation);
    }
}
