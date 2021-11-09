using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Connectors;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class StartConnectorViewModel : SelectableDesignerItemViewModelBase,IPart
    {
        #region Filed

        private static IPathFinder PathFinder { get; } = new OrthogonalPathFinder();

        private Point _anchorPositionSource;

        /// <summary>
        /// 开始连接点的位置
        ///   /// <summary>
        /// between source connector position and the beginning
        /// of the path geometry we leave some space for visual reasons;
        /// so the anchor position source really marks the beginning
        /// of the path geometry on the source side
        /// </summary>
        /// </summary>
        public Point AnchorPositionSource
        {
            get => _anchorPositionSource;
            set
            {
                if (SetProperty(ref _anchorPositionSource, value))
                {
                    UpdateArea();
                }
            }
        }

        private Point _anchorPositionSink;

        /// <summary>
        /// 结束连接点的位置 analogue to source side
        /// </summary>
        public Point AnchorPositionSink
        {
            get => _anchorPositionSink;
            set
            {
                if (SetProperty(ref _anchorPositionSink, value))
                {
                    UpdateArea();
                }
            }
        }

        private ConnectInfo _sourceConnector;

        /// <summary>
        /// 开头连接上的点
        /// </summary>
        public ConnectInfo SourceConnector
        {
            get => _sourceConnector;
            private set
            {
                if (SetProperty(ref _sourceConnector, value))
                {
                    SourcePropertyChanged(_sourceConnector);
                }
            }
        }

        private ConnectBaseInfo _sinkConnector;

        public ConnectBaseInfo SinkConnector
        {
            get => _sinkConnector;
            private set
            {
                if (SetProperty(ref _sinkConnector, value))
                {
                    SinkPropertyChanged(_sinkConnector);
                }
            }
        }

        private Rect _area;

        public Rect Area
        {
            get => _area;
            protected set
            {
                if (SetProperty(ref _area, value))
                {
                    UpdateConnectionPoints();
                }
            }
        }

        // pattern of dashes and gaps that is used to outline the connection path
        private DoubleCollection _strokeDashArray = new DoubleCollection(new double[] { 1, 2 });

        public DoubleCollection StrokeDashArray
        {
            get => _strokeDashArray;
            set => SetProperty(ref _strokeDashArray, value);
        }

        private IList<Point> _connectionPoints;

        /// <summary>
        /// 连接线上的点
        /// </summary>
        public IList<Point> ConnectionPoints
        {
            get => _connectionPoints;
            private set => SetProperty(ref _connectionPoints, value);
        }

        private Point _endPoint;

        /// <summary>
        /// 最后一个点
        /// </summary>
        public Point EndPoint
        {
            get => _endPoint;
            private set => SetProperty(ref _endPoint, value);
        }

        /// <summary>
        /// 判断线的两端是否已经连接成线了
        /// </summary>
        public bool IsFullConnection => SinkConnector is ConnectInfo;

        #endregion Filed

        #region Construstor

        public StartConnectorViewModel(DesignerItemData data)
        {
            LoadDesignerItemData(data);
        }

        #endregion Construstor

        #region Function

        public void SetSinkConnector(ConnectBaseInfo info)
        {
            SinkConnector = info;
        }

        public sealed override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);

            Init(data.SourceConnector, data.SinkConnector);
        }

        private void Init(ConnectInfo sourceConnector, ConnectBaseInfo sinkConnector)
        {
            //Parent = sourceConnector.DesignerItem.Parent;
            SourceConnector = sourceConnector;
            SinkConnector = sinkConnector;
        }

        private void UpdateArea()
        {
            Area = new Rect(AnchorPositionSource, AnchorPositionSink);
        }

        private void UpdateConnectionPoints()
        {
            ConnectionPoints = new List<Point>()
                                   {
                                       new Point( AnchorPositionSource.X  <  AnchorPositionSink.X ? 0d : Area.Width, AnchorPositionSource.Y  <  AnchorPositionSink.Y ? 0d : Area.Height ),
                                       new Point(AnchorPositionSource.X  >  AnchorPositionSink.X ? 0d : Area.Width, AnchorPositionSource.Y  >  AnchorPositionSink.Y ? 0d : Area.Height)
                                   };

            var sourceInfo = GetConnectorInfo(SourceConnector.Orientation,
                                            ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0]);
            if (IsFullConnection)
            {
                EndPoint = ConnectionPoints.Last();
                var sinkInfo = GetConnectorInfo(SinkConnector.Orientation,
                                  ConnectionPoints[1].X,
                                  ConnectionPoints[1].Y,
                                  ConnectionPoints[1]);

                StrokeDashArray = default;

                ConnectionPoints = PathFinder.GetConnectionLine(sourceInfo, sinkInfo, true);
            }
            else
            {
                ConnectionPoints = PathFinder.GetConnectionLine(sourceInfo, ConnectionPoints[1], ConnectorOrientation.Left);
                EndPoint = new Point();
            }

            //ConnectionPoints.Remove(ConnectionPoints.Last());

            //if (PathGeometry.Figures.Any())
            //{
            //    if (PathGeometry.Figures[0] is { } figure1)
            //    {
            //        if (figure1.Segments[0] is { } polyLine)
            //        {
            //            figure1.Segments.Remove(polyLine);
            //            figure1.Segments.Add(new PolyLineSegment(ConnectionPoints, true));
            //            PathGeometry.Figures.Add(figure1);
            //        }
            //    }
            //}
            //else
            //{
            //    PathFigure figure = new PathFigure();
            //    figure.StartPoint = ConnectionPoints[0];
            //    //ConnectionPoints.Remove(ConnectionPoints[0]);
            //    figure.Segments.Add(new PolyLineSegment(ConnectionPoints, true));
            //    PathGeometry.Figures.Add(figure);
            //}
        }

        public override PersistenceAbleItemBase SaveInfo() => default;

        protected void SourcePropertyChanged(ConnectInfo source)
        {
            AnchorPositionSource = PointHelper.GetPointForConnector(source);
            source.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
        }

        private void ConnectorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ItemHeight":
                case "ItemWidth":
                case "Left":

                case "Top":
                    AnchorPositionSource = PointHelper.GetPointForConnector(this.SourceConnector);
                    if (this.SinkConnector is ConnectInfo connectorInfo)
                    {
                        AnchorPositionSink = PointHelper.GetPointForConnector(connectorInfo);
                    }
                    break;
            }
        }

        protected void SinkPropertyChanged(ConnectBaseInfo sink)
        {
            if (sink is ConnectInfo connectorInfo)
            {
                AnchorPositionSink = PointHelper.GetPointForConnector(connectorInfo);

                connectorInfo.DesignerItem.PropertyChanged += new WeakEventHandler(this.ConnectorViewModel_PropertyChanged).Handler;
            }
            else
            {
                AnchorPositionSink = ((PartConnectInfo)sink).CurrentLocation;
            }
        }

        private ConnectorInfo GetConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position)
        {
            var info = new ConnectorInfo();
            info.Orientation = orientation;
            info.DesignerItemSize = new Size(SourceConnector.DesignerItem.ActualWidth, SourceConnector.DesignerItem.ActualHeight);
            info.DesignerItemLeft = left;
            info.DesignerItemTop = top;
            info.Position = position;
            return info;
        }

        #endregion Function
    }
}