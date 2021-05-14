using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.DesignerItemViewModel
{
    /// <summary>
    /// 连接线
    /// </summary>
    public class ConnectorViewModel : SelectableDesignerItemViewModelBase
    {
        #region Filed

        public static IPathFinder PathFinder { get; set; }

        /// <summary>
        /// 判断线的两端是否已经连接成线了
        /// </summary>
        public bool IsFullConnection => _sinkConnectorInfo is FullyCreatedConnectorInfo;

        private Point _sourceA;

        /// <summary>
        /// 开始连接点的位置
        /// </summary>
        public Point SourceA
        {
            get => _sourceA;
            set
            {
                if (SetProperty(ref _sourceA, value))
                {
                    UpdateArea();
                }
            }
        }

        private Point _sourceB;

        /// <summary>
        /// 结束连接点的位置
        /// </summary>
        public Point SourceB
        {
            get => _sourceB;
            set
            {
                if (SetProperty(ref _sourceB, value))
                {
                    UpdateArea();
                }
            }
        }

        private List<Point> _connectionPoints;

        /// <summary>
        /// 连接线上的点
        /// </summary>
        public List<Point> ConnectionPoints
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

        private Rect _area;

        public Rect Area
        {
            get => _area;
            private set
            {
                if (SetProperty(ref _area, value))
                {
                    UpdateConnectionPoints();
                }
            }
        }

        public ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position) =>
            new ConnectorInfo()
            {
                Orientation = orientation,
                DesignerItemSize = new Size(_sourceConnectorInfo.DesignerItem.ItemWidth, _sourceConnectorInfo.DesignerItem.ItemHeight),
                DesignerItemLeft = left,
                DesignerItemTop = top,
                Position = position
            };

        private FullyCreatedConnectorInfo _sourceConnectorInfo;

        /// <summary>
        /// 开头连接上的点
        /// </summary>
        public FullyCreatedConnectorInfo SourceConnectorInfo
        {
            get => _sourceConnectorInfo;
            set
            {
                if (SetProperty(ref _sourceConnectorInfo, value))
                {
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    _sourceConnectorInfo.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                }
            }
        }

        private ConnectorInfoBase _sinkConnectorInfo;

        public ConnectorInfoBase SinkConnectorInfo
        {
            get => _sinkConnectorInfo;
            set
            {
                if (SetProperty(ref _sinkConnectorInfo, value))
                {
                    if (_sinkConnectorInfo is FullyCreatedConnectorInfo connectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector(connectorInfo);

                        connectorInfo.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                    }
                    else
                    {
                        SourceB = ((PartCreatedConnectionInfo)_sinkConnectorInfo).CurrentLocation;
                    }
                }
            }
        }

        #endregion Filed

        #region Construstor

        public ConnectorViewModel(Guid id, IDiagramViewModel parent,
            FullyCreatedConnectorInfo sourceConnectorInfo, FullyCreatedConnectorInfo sinkConnectorInfo)
            : base(id, parent)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
        }

        public ConnectorViewModel(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo) :
            base(Guid.NewGuid())
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
        }

        #endregion Construstor

        #region Function

        private void UpdateArea()
        {
            Area = new Rect(SourceA, SourceB);
        }

        private void UpdateConnectionPoints()
        {
            ConnectionPoints = new List<Point>()
                                   {
                                       new Point( SourceA.X  <  SourceB.X ? 0d : Area.Width, SourceA.Y  <  SourceB.Y ? 0d : Area.Height ),
                                       new Point(SourceA.X  >  SourceB.X ? 0d : Area.Width, SourceA.Y  >  SourceB.Y ? 0d : Area.Height)
                                   };

            ConnectorInfo sourceInfo = ConnectorInfo(SourceConnectorInfo.Orientation,
                                            ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0]);

            if (IsFullConnection)
            {
                EndPoint = ConnectionPoints.Last();
                ConnectorInfo sinkInfo = ConnectorInfo(SinkConnectorInfo.Orientation,
                                  ConnectionPoints[1].X,
                                  ConnectionPoints[1].Y,
                                  ConnectionPoints[1]);

                ConnectionPoints = PathFinder.GetConnectionLine(sourceInfo, sinkInfo, true);
            }
            else
            {
                ConnectionPoints = PathFinder.GetConnectionLine(sourceInfo, ConnectionPoints[1], ConnectorOrientation.Left);
                EndPoint = new Point();
            }
        }

        private void ConnectorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ItemHeight":
                case "ItemWidth":
                case "Left":
                case "Top":
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    if (this.SinkConnectorInfo is FullyCreatedConnectorInfo connectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector(connectorInfo);
                    }
                    break;
            }
        }

        private void Init(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            this.Parent = sourceConnectorInfo.DesignerItem.Parent;
            this.SourceConnectorInfo = sourceConnectorInfo;
            this.SinkConnectorInfo = sinkConnectorInfo;
            PathFinder = new OrthogonalPathFinder();
        }

        public override PersistenceAbleItemBase SaveInfo()
        {
            if (SinkConnectorInfo is FullyCreatedConnectorInfo sinkConnector)
            {
                Connection connection = new Connection(
                Id,
                    SourceConnectorInfo.DesignerItem.Id,
                    GetOrientationFromConnector(SourceConnectorInfo.Orientation),
                    sinkConnector.DesignerItem.Id,
                    GetOrientationFromConnector(sinkConnector.Orientation));

                //return new DiagramItemInfo(Id,connection);a

                return connection;
            }

            return null;
        }


        private Orientation GetOrientationFromConnector(ConnectorOrientation connectorOrientation)
        {
            Orientation result = Orientation.None;
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Bottom:
                    result = Orientation.Bottom;
                    break;

                case ConnectorOrientation.Left:
                    result = Orientation.Left;
                    break;

                case ConnectorOrientation.Top:
                    result = Orientation.Top;
                    break;

                case ConnectorOrientation.Right:
                    result = Orientation.Right;
                    break;
            }
            return result;
        }

        #endregion Function
    }
}