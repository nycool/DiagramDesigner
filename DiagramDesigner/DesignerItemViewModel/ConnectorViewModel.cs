using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Connectors;
using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using DiagramDesigner.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace DiagramDesigner.DesignerItemViewModel
{
    /// <summary>
    /// 连接线
    /// </summary>
    public class ConnectorViewModel : StartConnectorViewModel, IFull
    {
        #region Filed

        /// <summary>
        /// 更新分组以后旧的Src模块id
        /// </summary>
        public List<Guid> SourceOldId { get; private set; }

        /// <summary>
        /// 更新分组以后旧的Sink模块id
        /// </summary>
        public List<Guid> SinkOldId { get; private set; }


        private PathGeometry _pathGeometry;

        /// <summary>
        /// 连接线路径形状
        /// </summary>
        public PathGeometry PathGeometry
        {
            get => _pathGeometry;
            set
            {
                if (SetProperty(ref _pathGeometry, value))
                {
                    UpdateAnchorPosition();
                }
            }
        }

        #endregion Filed

        #region Construstor

        public ConnectorViewModel(DesignerItemData data, Guid[] oldSrc = default, Guid[] oldSink = default)
            : base(data)
        {
            RecConnectSourceModel(oldSrc);

            ReConnectSinkModel(oldSink);
        }

        #endregion Construstor

        #region Function

        /// <summary>
        /// 更新Anchor位置
        /// </summary>
        private void UpdateAnchorPosition()
        {
            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector
            // on PathGeometry at the specified fraction of its length

            this.PathGeometry.GetPointAtFractionLength(0, out var pathStartPoint, out var pathTangentAtStartPoint);

            this.PathGeometry.GetPointAtFractionLength(1, out var pathEndPoint, out var pathTangentAtEndPoint);

            // get angle from tangent vector
            //this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            //this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            // add some margin on source and sink side for visual reasons only
            pathStartPoint.Offset(-pathTangentAtStartPoint.X * 5, -pathTangentAtStartPoint.Y * 5);
            pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
        }

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
                if (SinkConnector is ConnectInfo connector)
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

            if (SinkConnector is ConnectInfo { DesignerItem: IGroup group } connector)
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

            if (SinkConnector is ConnectInfo full && full.DesignerItem == designerItem)
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

            if (SinkConnector is ConnectInfo connector)
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
            if (SinkConnector is ConnectInfo full)
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

                full.UpdateDesignerItem(designerItem, SinkConnector as ConnectInfo);

                SinkPropertyChanged(SinkConnector);
            }
        }

        public bool Connected()
        {
            bool result = default;

            if (SourceConnector.DesignerItem is IConnect srcConnect && (SinkConnector as ConnectInfo)?.DesignerItem is IConnect sinkConnect)
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
                (SinkConnector as ConnectInfo)?.DesignerItem is IConnect sinkConnect)
            {
                result = srcConnect.Remove(sinkConnect, RemoveTypes.Destination);

                result = sinkConnect.Remove(srcConnect, RemoveTypes.Source);
            }

            return result;
        }

        public override PersistenceAbleItemBase SaveInfo()
        {
            if (SinkConnector is ConnectInfo sinkConnector)
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