using System.Collections.Generic;
using System.Windows;
using DiagramDesigner.BaseClass;

namespace DiagramDesigner.Interface
{
    public interface IPathFinder
    {
        IList<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine);
        IList<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation);
    }
}
