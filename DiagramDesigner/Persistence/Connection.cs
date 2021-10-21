using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Connectors;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner.Persistence
{
    public class Connection : PersistenceAbleItemBase
    {
        #region Filed

        public Guid SourceId { get; set; }
        public Orientation SourceOrientation { get; set; }
        public Guid SinkId { get; set; }
        public Orientation SinkOrientation { get; set; }

        /// <summary>
        /// 更新连接线之前旧StartID
        /// </summary>
        public Guid[] SourceOldId { get; set; }

        /// <summary>
        ///  更新连接线之前旧EndID
        /// </summary>
        public Guid[] SinkOldId { get; set; }

        #endregion Filed

        #region Construstor

        public Connection()
        {
        }

        public Connection(Guid sourceId, Orientation sourceOrientation, Guid sinkId, Orientation sinkOrientation, IList<Guid> oldSrc, IList<Guid> oldSink)
        {
            this.SourceId = sourceId;
            this.SourceOrientation = sourceOrientation;

            this.SinkId = sinkId;
            this.SinkOrientation = sinkOrientation;

            if (oldSrc?.Any() == true)
            {
                SourceOldId = oldSrc.ToArray();
            }

            if (oldSink?.Any() == true)
            {
                SinkOldId = oldSink.ToArray();
            }
        }

        #endregion Construstor

        #region Function

        #region Override

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var sourceItem = GetConnectorDataItem(parent, SourceId);

            if (sourceItem == default)
            {
                return default;
            }

            var sourceConnectorOrientation = GetOrientationForConnector(SourceOrientation);
            var sourceConnectorInfo = GetFullConnectorInfo(sourceItem, sourceConnectorOrientation);

            var sinkItem = GetConnectorDataItem(parent, SinkId);

            if (sinkItem == default)
            {
                return default;
            }

            var sinkConnectorOrientation = GetOrientationForConnector(SinkOrientation);
            var sinkConnectorInfo = GetFullConnectorInfo(sinkItem, sinkConnectorOrientation);

            var data = new DesignerItemData(parent, sourceConnectorInfo, sinkConnectorInfo);

            var vm = new ConnectorViewModel(data, SourceOldId, SinkOldId);

            return vm;
        }

        #endregion Override

        private DesignerItemViewModelBase GetConnectorDataItem(IDiagramViewModel diagramVm, Guid id)
        {
            return diagramVm.ItemsSource.FirstOrDefault(x => x is DesignerItemViewModelBase designerItem && designerItem.Id == id) as DesignerItemViewModelBase;
        }

        private ConnectorOrientation GetOrientationForConnector(Orientation persistedOrientation)
        {
            ConnectorOrientation result = ConnectorOrientation.None;
            switch (persistedOrientation)
            {
                case Orientation.Bottom:
                    result = ConnectorOrientation.Bottom;
                    break;

                case Orientation.Left:
                    result = ConnectorOrientation.Left;
                    break;

                case Orientation.Top:
                    result = ConnectorOrientation.Top;
                    break;

                case Orientation.Right:
                    result = ConnectorOrientation.Right;
                    break;
            }
            return result;
        }

        private Connector GetFullConnectorInfo(DesignerItemViewModelBase designerItem, ConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Top:
                    return designerItem.TopConnector;

                case ConnectorOrientation.Left:
                    return designerItem.LeftConnector;

                case ConnectorOrientation.Right:
                    return designerItem.RightConnector;

                case ConnectorOrientation.Bottom:
                    return designerItem.BottomConnector;

                default:
                    throw new InvalidOperationException(
                        $"Found invalid persisted Connector Orientation for Connector Id: {designerItem.Id}");
            }
        }

        #endregion Function
    }
}