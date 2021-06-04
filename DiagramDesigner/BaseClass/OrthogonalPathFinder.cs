using DiagramDesigner.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiagramDesigner.BaseClass
{
    // Note: I couldn't find a useful open source library that does
    // orthogonal routing so started to write something on my own.
    // Categorize this as a quick and dirty short term solution.
    // I will keep on searching.

    // Helper class to provide an orthogonal connection path
    public class OrthogonalPathFinder : IPathFinder
    {
        private const int Margin = 20;

        public IList<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine)
        {
            Rect rectSource = GetRectWithMargin(source, Margin);

            Rect rectSink = GetRectWithMargin(sink, Margin);

            var points = GetPoints(source, sink, rectSource, rectSink).ToList();

            OptimizeLinePoints(points.ToList(), source.Orientation, sink.Orientation, out var resultPoints, rectSource, rectSink);

            CheckPathEnd(source, sink, showLastLine, resultPoints);

            return resultPoints;
        }

        public IList<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation)
        {
            List<Point> linePoints = new List<Point>();

            Rect rectSource = GetRectWithMargin(source, 10);

            Point startPoint = GetOffsetPoint(source, rectSource);

            Point endPoint = sinkPoint;

            linePoints.Add(startPoint);

            Point currentPoint = startPoint;

            if (!rectSource.Contains(endPoint))
            {
                while (true)
                {
                    if (IsPointVisible(currentPoint, endPoint, rectSource))
                    {
                        linePoints.Add(endPoint);
                        break;
                    }

                    Point n = GetNearestNeighborSource(source, endPoint, rectSource, out var sideFlag);

                    linePoints.Add(n);

                    currentPoint = n;

                    if (IsPointVisible(currentPoint, endPoint, rectSource))
                    {
                        linePoints.Add(endPoint);
                        break;
                    }

                    GetOppositeCorners(source.Orientation, rectSource, out var n1, out var n2);

                    linePoints.Add(sideFlag ? n1 : n2);

                    linePoints.Add(endPoint);
                    break;
                }
            }
            else
            {
                linePoints.Add(endPoint);
            }

            if (preferredOrientation != ConnectorOrientation.None)
            {
                OptimizeLinePoints(linePoints, source.Orientation, preferredOrientation, out linePoints, rectSource);
            }
            else
            {
                OptimizeLinePoints(linePoints, source.Orientation, GetOpositeOrientation(source.Orientation), out linePoints, rectSource);
            }

            return linePoints;
        }

        private static IEnumerable<Point> FilterPoint(IList<Point> points, params Rect[] rects)
        {
            int cut = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i >= cut)
                {
                    for (int k = points.Count - 1; k > i; k--)
                    {
                        if (IsPointVisible(points[i], points[k], rects))
                        {
                            cut = k;
                            break;
                        }
                    }

                    yield return points[i];
                }
            }
        }

        private IEnumerable<Point> GetPoints(ConnectorInfo source, ConnectorInfo sink, Rect rectSource, Rect rectSink)
        {
            Point startPoint = GetOffsetPoint(source, rectSource);

            Point endPoint = GetOffsetPoint(sink, rectSink);

            yield return startPoint;

            Point currentPoint = startPoint;

            if (!rectSink.Contains(currentPoint) && !rectSource.Contains(endPoint))
            {
                while (true)
                {
                    #region source algorithm

                    if (IsPointVisible(currentPoint, endPoint, rectSource, rectSink))
                    {
                        yield return endPoint;
                        currentPoint = endPoint;
                        break;
                    }

                    Point neighbor = GetNearestVisibleNeighborSink(currentPoint, endPoint, sink, rectSource, rectSink);
                    if (!double.IsNaN(neighbor.X))
                    {
                        yield return neighbor;
                        yield return endPoint;
                        currentPoint = endPoint;
                        break;
                    }

                    if (currentPoint == startPoint)
                    {
                        Point n = GetNearestNeighborSource(source, endPoint, rectSource, rectSink, out var flag);

                        yield return n;

                        currentPoint = n;

                        if (!IsRectVisible(currentPoint, rectSink, rectSource))
                        {
                            GetOppositeCorners(source.Orientation, rectSource, out var n1, out var n2);
                            if (flag)
                            {
                                yield return n1;
                                currentPoint = n1;
                            }
                            else
                            {
                                yield return n2;
                                currentPoint = n2;
                            }
                            if (!IsRectVisible(currentPoint, rectSink, rectSource))
                            {
                                if (flag)
                                {
                                    yield return n2;
                                    currentPoint = n2;
                                }
                                else
                                {
                                    yield return n1;
                                    currentPoint = n1;
                                }
                            }
                        }
                    }

                    #endregion source algorithm

                    #region sink algorithm

                    else // from here on we jump to the sink algorithm
                    {
                        GetNeighborCorners(sink.Orientation, rectSink, out var s1, out var s2);

                        GetOppositeCorners(sink.Orientation, rectSink, out var n1, out var n2);

                        bool n1Visible = IsPointVisible(currentPoint, n1, rectSource, rectSink);

                        bool n2Visible = IsPointVisible(currentPoint, n2, rectSource, rectSink);

                        if (n1Visible && n2Visible)
                        {
                            if (rectSource.Contains(n1))
                            {
                                yield return n2;
                                if (rectSource.Contains(s2))
                                {
                                    yield return n1;
                                    yield return s1;
                                }
                                else
                                {
                                    yield return s2;
                                }

                                yield return endPoint;
                                currentPoint = endPoint;
                                break;
                            }

                            if (rectSource.Contains(n2))
                            {
                                yield return n1;
                                if (rectSource.Contains(s1))
                                {
                                    yield return n2;
                                    yield return s2;
                                }
                                else
                                {
                                    yield return s1;
                                }

                                yield return endPoint;
                                currentPoint = endPoint;
                                break;
                            }

                            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
                            {
                                yield return n1;

                                if (rectSource.Contains(s1))
                                {
                                    yield return n2;
                                    yield return s2;
                                }
                                else
                                {
                                    yield return s1;
                                }

                                yield return endPoint;

                                currentPoint = endPoint;
                                break;
                            }
                            else
                            {
                                yield return n2;
                                if (rectSource.Contains(s2))
                                {
                                    yield return n1;
                                    yield return s1;
                                }
                                else
                                {
                                    yield return s2;
                                }

                                yield return endPoint;
                                currentPoint = endPoint;
                                break;
                            }
                        }
                        else if (n1Visible)
                        {
                            yield return n1;
                            if (rectSource.Contains(s1))
                            {
                                yield return n2;
                                yield return s2;
                            }
                            else
                            {
                                yield return s1;
                            }

                            yield return endPoint;
                            currentPoint = endPoint;
                            break;
                        }
                        else
                        {
                            yield return n2;
                            if (rectSource.Contains(s2))
                            {
                                yield return n1;
                                yield return s1;
                            }
                            else
                            {
                                yield return s2;
                            }

                            yield return endPoint;
                            currentPoint = endPoint;
                            break;
                        }
                    }

                    #endregion sink algorithm
                }
            }
            else
            {
                yield return endPoint;
                //linePoints.Add(endPoint);
            }
        }

        private static void OptimizeLinePoints(IList<Point> linePoints, ConnectorOrientation sourceOrientation, ConnectorOrientation sinkOrientation, out List<Point> points, params Rect[] rects)
        {
            #region Filter

            points = FilterPoint(linePoints, rects).ToList();

            #endregion Filter

            #region Line

            for (int j = 0; j < points.Count - 1; j++)
            {
                if (!points[j].X.Equals(points[j + 1].X) && !points[j].Y.Equals(points[j + 1].Y))
                {
                    ConnectorOrientation orientationFrom;
                    ConnectorOrientation orientationTo;

                    // orientation from point
                    if (j == 0)
                        orientationFrom = sourceOrientation;
                    else
                        orientationFrom = GetOrientation(points[j], points[j - 1]);

                    // orientation to pint
                    if (j == points.Count - 2)
                        orientationTo = sinkOrientation;
                    else
                        orientationTo = GetOrientation(points[j + 1], points[j + 2]);

                    if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                        (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                    {
                        double centerX = Math.Min(points[j].X, points[j + 1].X) + Math.Abs(points[j].X - points[j + 1].X) / 2;
                        points.Insert(j + 1, new Point(centerX, points[j].Y));
                        points.Insert(j + 2, new Point(centerX, points[j + 2].Y));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return;
                    }

                    if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                        (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                    {
                        double centerY = Math.Min(points[j].Y, points[j + 1].Y) + Math.Abs(points[j].Y - points[j + 1].Y) / 2;
                        points.Insert(j + 1, new Point(points[j].X, centerY));
                        points.Insert(j + 2, new Point(points[j + 2].X, centerY));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return;
                    }

                    if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                        (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                    {
                        points.Insert(j + 1, new Point(points[j + 1].X, points[j].Y));
                        return;
                    }

                    if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                        (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                    {
                        points.Insert(j + 1, new Point(points[j].X, points[j + 1].Y));
                        return;
                    }
                }
            }

            #endregion Line
        }

        private static ConnectorOrientation GetOrientation(Point p1, Point p2)
        {
            if (p1.X.Equals(p2.X))
            {
                if (p1.Y >= p2.Y)
                {
                    return ConnectorOrientation.Bottom;
                }
                return ConnectorOrientation.Top;
            }

            if (p1.Y.Equals(p2.Y))
            {
                if (p1.X >= p2.X)
                {
                    return ConnectorOrientation.Right;
                }

                return ConnectorOrientation.Left;
            }

            throw new Exception("Failed to retrieve orientation");
        }

        private static System.Windows.Controls.Orientation GetOrientation(ConnectorOrientation sourceOrientation)
        {
            switch (sourceOrientation)
            {
                case ConnectorOrientation.Left:
                    return System.Windows.Controls.Orientation.Horizontal;

                case ConnectorOrientation.Top:
                    return System.Windows.Controls.Orientation.Vertical;

                case ConnectorOrientation.Right:
                    return System.Windows.Controls.Orientation.Horizontal;

                case ConnectorOrientation.Bottom:
                    return System.Windows.Controls.Orientation.Vertical;

                default:
                    throw new Exception("Unknown ConnectorOrientation");
            }
        }

        private static Point GetNearestNeighborSource(ConnectorInfo source, Point endPoint, Rect rectSource, Rect rectSink, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(source.Orientation, rectSource, out n1, out n2);

            if (rectSink.Contains(n1))
            {
                flag = false;
                return n2;
            }

            if (rectSink.Contains(n2))
            {
                flag = true;
                return n1;
            }

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestNeighborSource(ConnectorInfo source, Point endPoint, Rect rectSource, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(source.Orientation, rectSource, out n1, out n2);

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestVisibleNeighborSink(Point currentPoint, Point endPoint, ConnectorInfo sink, Rect rectSource, Rect rectSink)
        {
            Point s1, s2; // neighbors on sink side
            GetNeighborCorners(sink.Orientation, rectSink, out s1, out s2);

            bool flag1 = IsPointVisible(currentPoint, s1, new Rect[] { rectSource, rectSink });
            bool flag2 = IsPointVisible(currentPoint, s2, new Rect[] { rectSource, rectSink });

            if (flag1) // s1 visible
            {
                if (flag2) // s1 and s2 visible
                {
                    if (rectSink.Contains(s1))
                        return s2;

                    if (rectSink.Contains(s2))
                        return s1;

                    if ((Distance(s1, endPoint) <= Distance(s2, endPoint)))
                        return s1;
                    else
                        return s2;
                }
                else
                {
                    return s1;
                }
            }
            else // s1 not visible
            {
                if (flag2) // only s2 visible
                {
                    return s2;
                }
                else // s1 and s2 not visible
                {
                    return new Point(double.NaN, double.NaN);
                }
            }
        }

        private static bool IsPointVisible(Point fromPoint, Point targetPoint, params Rect[] rectangles)
        {
            foreach (Rect rect in rectangles)
            {
                if (RectangleIntersectsLine(rect, fromPoint, targetPoint))
                    return false;
            }
            return true;
        }

        private static bool IsRectVisible(Point fromPoint, Rect targetRect, params Rect[] rectangles)
        {
            if (IsPointVisible(fromPoint, targetRect.TopLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.TopRight, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomRight, rectangles))
                return true;

            return false;
        }

        private static bool RectangleIntersectsLine(Rect rect, Point startPoint, Point endPoint)
        {
            rect.Inflate(-1, -1);
            return rect.IntersectsWith(new Rect(startPoint, endPoint));
        }

        private static void GetOppositeCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    n1 = rect.TopRight;
                    n2 = rect.BottomRight;
                    break;

                case ConnectorOrientation.Top:
                    n1 = rect.BottomLeft;
                    n2 = rect.BottomRight;
                    break;

                case ConnectorOrientation.Right:
                    n1 = rect.TopLeft;
                    n2 = rect.BottomLeft;
                    break;

                case ConnectorOrientation.Bottom:
                    n1 = rect.TopLeft;
                    n2 = rect.TopRight;
                    break;

                default:
                    throw new Exception("No opposite corners found!");
            }
        }

        private static void GetNeighborCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    n1 = rect.TopLeft;
                    n2 = rect.BottomLeft;
                    break;

                case ConnectorOrientation.Top:
                    n1 = rect.TopLeft;
                    n2 = rect.TopRight;
                    break;

                case ConnectorOrientation.Right:
                    n1 = rect.TopRight;
                    n2 = rect.BottomRight;
                    break;

                case ConnectorOrientation.Bottom:
                    n1 = rect.BottomLeft;
                    n2 = rect.BottomRight;
                    break;

                default:
                    throw new Exception("No neighour corners found!");
            }
        }

        private static double Distance(Point p1, Point p2)
        {
            return Point.Subtract(p1, p2).Length;
        }

        private static Rect GetRectWithMargin(ConnectorInfo connectorThumb, double margin)
        {
            Rect rect = new Rect(connectorThumb.DesignerItemLeft,
                                 connectorThumb.DesignerItemTop,
                                 0,
                                 0);

            rect.Inflate(margin, margin);

            return rect;
        }

        private static Point GetOffsetPoint(ConnectorInfo connector, Rect rect)
        {
            Point offsetPoint = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Left:
                    offsetPoint = new Point(rect.Left, connector.Position.Y);

                    offsetPoint.Y += 1;

                    break;

                case ConnectorOrientation.Top:
                    offsetPoint = new Point(connector.Position.X, rect.Top);

                    offsetPoint.X += 1;

                    break;

                case ConnectorOrientation.Right:
                    offsetPoint = new Point(rect.Right, connector.Position.Y);

                    offsetPoint.Y += 1;

                    break;

                case ConnectorOrientation.Bottom:
                    offsetPoint = new Point(connector.Position.X, rect.Bottom);

                    offsetPoint.X += 1;

                    break;

                default:
                    break;
            }

            return offsetPoint;
        }

        private static void CheckPathEnd(ConnectorInfo source, ConnectorInfo sink, bool showLastLine, IList<Point> linePoints)
        {
            if (showLastLine)
            {
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);
                double marginPath = 8;
                switch (source.Orientation)
                {
                    case ConnectorOrientation.Left:
                        startPoint = new Point(source.Position.X - marginPath, source.Position.Y + 1);
                        break;

                    case ConnectorOrientation.Top:
                        startPoint = new Point(source.Position.X + 1, source.Position.Y - marginPath);
                        break;

                    case ConnectorOrientation.Right:
                        startPoint = new Point(source.Position.X + marginPath, source.Position.Y + 1);
                        break;

                    case ConnectorOrientation.Bottom:
                        startPoint = new Point(source.Position.X + 1, source.Position.Y + marginPath);
                        break;

                    default:
                        break;
                }

                bool isBottom = default;

                switch (sink.Orientation)
                {
                    case ConnectorOrientation.Left:
                        endPoint = new Point(sink.Position.X - marginPath, sink.Position.Y + 1);
                        break;

                    case ConnectorOrientation.Top:
                        endPoint = new Point(sink.Position.X + 1, sink.Position.Y - marginPath);
                        break;

                    case ConnectorOrientation.Right:
                        endPoint = new Point(sink.Position.X + marginPath, sink.Position.Y + 1);
                        break;

                    case ConnectorOrientation.Bottom:
                        endPoint = new Point(sink.Position.X + 1, sink.Position.Y + marginPath);
                        isBottom = true;
                        break;

                    default:
                        break;
                }

                

                if (isBottom)
                {
                    var rePoints = linePoints.Reverse().ToList();

                    var tempList = new List<Point>();

                    for (var index = 0; index < rePoints.Count; index++)
                    {
                        var linePoint = rePoints[index];

                        linePoint.Y += 20;

                        tempList.Add(linePoint);
                    }

                    linePoints.Clear();

                    endPoint.Y += 10;

                    linePoints.Add(endPoint);

                    foreach (var point in tempList)
                    {
                        linePoints.Add(point);
                    }

                    linePoints.Add(startPoint);
                }
                else
                {
                    linePoints.Insert(0, startPoint);

                    linePoints.Add(endPoint);
                }
            }
            else
            {
                linePoints.Insert(0, source.Position);

                linePoints.Add(sink.Position);
            }
        }

        private static ConnectorOrientation GetOpositeOrientation(ConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Left:
                    return ConnectorOrientation.Right;

                case ConnectorOrientation.Top:
                    return ConnectorOrientation.Bottom;

                case ConnectorOrientation.Right:
                    return ConnectorOrientation.Left;

                case ConnectorOrientation.Bottom:
                    return ConnectorOrientation.Top;

                default:
                    return ConnectorOrientation.Top;
            }
        }
    }
}