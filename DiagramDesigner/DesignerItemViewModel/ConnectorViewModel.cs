using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Connectors;
using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using DiagramDesigner.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace DiagramDesigner.DesignerItemViewModel
{
    /// <summary>
    /// 连接线
    /// </summary>
    public class ConnectorViewModel : SelectableDesignerItemViewModelBase
    {
        #region Filed

        private static IPathFinder PathFinder { get; } = new OrthogonalPathFinder();

        /// <summary>
        /// 更新分组以后旧的Src模块id
        /// </summary>
        public List<Guid> SourceOldId { get; private set; }

        /// <summary>
        /// 更新分组以后旧的Sink模块id
        /// </summary>
        public List<Guid> SinkOldId { get; private set; }

        /// <summary>
        /// 判断线的两端是否已经连接成线了
        /// </summary>
        public bool IsFullConnection => _sinkConnector is Connector;

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

        private ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position)
        {
            var info = new ConnectorInfo();
            info.Orientation = orientation;
            info.DesignerItemSize = new Size(_sourceConnector.DesignerItem.ItemWidth, _sourceConnector.DesignerItem.ItemHeight);
            info.DesignerItemLeft = left;
            info.DesignerItemTop = top;
            info.Position = position;
            return info;
        }

        private Connector _sourceConnector;

        /// <summary>
        /// 开头连接上的点
        /// </summary>
        public Connector SourceConnector
        {
            get => _sourceConnector;
            set
            {
                if (SetProperty(ref _sourceConnector, value))
                {
                    SourcePropertyChanged(_sourceConnector);
                }
            }
        }

        private ConnectorBase _sinkConnector;

        public ConnectorBase SinkConnector
        {
            get => _sinkConnector;
            set
            {
                if (SetProperty(ref _sinkConnector, value))
                {
                    SinkPropertyChanged(_sinkConnector);
                }
            }
        }

        #endregion Filed

        #region Construstor

        public ConnectorViewModel(DesignerItemData data, Guid[] oldSrc = default, Guid[] oldSink = default)
        {
            LoadDesignerItemData(data);

            RecConnectSourceModel(oldSrc);

            ReConnectSinkModel(oldSink);
        }

        #endregion Construstor

        #region Function

        /// <summary>
        /// reconnect source ship
        /// </summary>
        /// <param name="srcIds"></param>
        private void RecConnectSourceModel(Guid[] srcIds)
        {
            if (srcIds == default)
            {
                return;
            }

            SourceOldId = srcIds.ToList();

            if (SourceConnector.DesignerItem is IGroup group)
            {
                if (SinkConnector is Connector connector)
                {
                    foreach (var guid in SourceOldId)
                    {
                        if (group.FindDesignerItem(guid) is { } designerItem)
                        {
                            connector.DesignerItem.ConSrcDesignerItemAction(designerItem);

                            group.TryAddDesignerItem(designerItem, connector.DesignerItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// reconnect sink ship
        /// </summary>
        private void ReConnectSinkModel(Guid[] sinkIds)
        {
            if (sinkIds == default)
            {
                return;
            }

            SinkOldId = sinkIds.ToList();

            if (SinkConnector is Connector { DesignerItem: IGroup group } connector)
            {
                foreach (var guid in SinkOldId)
                {
                    if (group.FindDesignerItem(guid) is { } designerItem)
                    {
                        designerItem.ConSrcDesignerItemAction(SourceConnector.DesignerItem);

                        group.TryAddDesignerItem(SourceConnector.DesignerItem, designerItem);
                    }
                }
            }
        }

        /// <summary>
        /// 连接线的起点或者终点是否连接模块上
        /// </summary>
        /// <param name="designerItem"></param>
        /// <returns></returns>
        public bool IsContain(DesignerItemViewModelBase designerItem)
        {
            if (SourceConnector.DesignerItem == designerItem)
            {
                return true;
            }

            if (SinkConnector is Connector full && full.DesignerItem == designerItem)
            {
                return true;
            }

            return default;
        }

        /// <summary>
        /// 更新连接线Start
        /// </summary>
        /// <param name="designerItem"></param>
        public void UpdateSource(DesignerItemViewModelBase designerItem)
        {
            var id = SourceConnector.DesignerItem.GetCurrentId();

            SourceOldId ??= new List<Guid>();

            if (!SourceOldId.Contains(id))
            {
                SourceOldId.Add(id);
            }

            if (SinkConnector is Connector connector)
            {
                connector.DesignerItem.ConSrcDesignerItemAction(designerItem);

                designerItem.ConSinkDesignerItemAction(connector.DesignerItem);

                SourceConnector.DesignerItem.RemoveCon(connector.DesignerItem, RemoveTypes.Destination);

                if (designerItem is IGroup group)
                {
                    group.TryAddDesignerItem(SourceConnector.DesignerItem, connector.DesignerItem);
                }

                connector.DesignerItem.RemoveCon(SourceConnector.DesignerItem, RemoveTypes.Source);
            }

            SourceConnector.UpdateDesignerItem(designerItem, SourceConnector);

            SourcePropertyChanged(SourceConnector);
        }

        /// <summary>
        /// 更新连接线End
        /// </summary>
        /// <param name="designerItem"></param>
        public void UpdateSink(DesignerItemViewModelBase designerItem)
        {
            if (SinkConnector is Connector full)
            {
                var id = full.DesignerItem.GetCurrentId();

                SinkOldId ??= new List<Guid>();

                if (!SinkOldId.Contains(id))
                {
                    SinkOldId.Add(id);
                }

                SourceConnector.DesignerItem.ConSinkDesignerItemAction(designerItem);

                designerItem.ConSrcDesignerItemAction(SourceConnector.DesignerItem);

                if (designerItem is IGroup group)
                {
                    group.TryAddDesignerItem(SourceConnector.DesignerItem, full.DesignerItem);
                }

                SourceConnector.DesignerItem.RemoveCon(full.DesignerItem, RemoveTypes.Destination);

                full.UpdateDesignerItem(designerItem, SinkConnector as Connector);

                SinkPropertyChanged(SinkConnector);
            }
        }

        private void SourcePropertyChanged(Connector source)
        {
            SourceA = PointHelper.GetPointForConnector(source);
            source.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
        }

        private void SinkPropertyChanged(ConnectorBase sink)
        {
            if (sink is Connector connectorInfo)
            {
                SourceB = PointHelper.GetPointForConnector(connectorInfo);

                connectorInfo.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
            }
            else
            {
                SourceB = ((PartConnector)sink).CurrentLocation;
            }
        }

        public bool Connected()
        {
            bool result = default;

            if (SourceConnector.DesignerItem is IConnect srcConnect && (SinkConnector as Connector)?.DesignerItem is IConnect sinkConnect)
            {
                result = srcConnect.ConnectDestination(sinkConnect);

                result &= sinkConnect.ConnectSource(srcConnect);
            }

            return result;
        }

        public bool DisConnected()
        {
            bool result = default;

            if (SourceConnector.DesignerItem is IConnect srcConnect &&
                (SinkConnector as Connector)?.DesignerItem is IConnect sinkConnect)
            {
                result = srcConnect.Remove(sinkConnect, RemoveTypes.Destination);

                result = sinkConnect.Remove(srcConnect, RemoveTypes.Source);
            }

            return result;
        }

        public sealed override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);

            Init(data.SourceConnector, data.SinkConnector);
        }

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

            ConnectorInfo sourceInfo = ConnectorInfo(SourceConnector.Orientation,
                                            ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0]);

            if (IsFullConnection)
            {
                EndPoint = ConnectionPoints.Last();
                ConnectorInfo sinkInfo = ConnectorInfo(SinkConnector.Orientation,
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
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnector);
                    if (this.SinkConnector is Connector connectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector(connectorInfo);
                    }
                    break;
            }
        }

        private void Init(Connector sourceConnector, ConnectorBase sinkConnector)
        {
            this.Parent = sourceConnector.DesignerItem.Parent;
            this.SourceConnector = sourceConnector;
            this.SinkConnector = sinkConnector;
        }

        public override PersistenceAbleItemBase SaveInfo()
        {
            if (SinkConnector is Connector sinkConnector)
            {
                Guid srcId = SourceConnector.DesignerItem.Id;

                var srcOrientation = GetOrientationFromConnector(SourceConnector.Orientation);

                Guid sinkId = sinkConnector.DesignerItem.Id;

                var sinkOrientation = GetOrientationFromConnector(sinkConnector.Orientation);

                Connection connection =
                    new Connection(srcId, srcOrientation, sinkId, sinkOrientation, SourceOldId, SinkOldId);

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