using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using System;
using System.Linq;
using DiagramDesigner.Interface;

namespace DiagramDesigner.Persistence
{
    public class Connection : PersistenceAbleItemBase
    {
        #region Filed

        public Guid SourceId { get; set; }
        public Orientation SourceOrientation { get; set; }
        public Guid SinkId { get; set; }
        public Orientation SinkOrientation { get; set; }

        #endregion Filed

        #region Construstor

        public Connection()
        {
        }

        public Connection(Guid id)
        : base(id)
        {
        }

        public Connection(Guid id, Guid sourceId, Orientation sourceOrientation, Guid sinkId, Orientation sinkOrientation)
            : this(id)
        {
            this.SourceId = sourceId;
            this.SourceOrientation = sourceOrientation;

            this.SinkId = sinkId;
            this.SinkOrientation = sinkOrientation;
        }

        #endregion Construstor

        #region Function

        #region Override

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var sourceItem = GetConnectorDataItem(parent, SourceId);
            var sourceConnectorOrientation = GetOrientationForConnector(SourceOrientation);
            var sourceConnectorInfo = GetFullConnectorInfo(Id, sourceItem, sourceConnectorOrientation);

            var sinkItem = GetConnectorDataItem(parent, SinkId);
            var sinkConnectorOrientation = GetOrientationForConnector(SinkOrientation);
            var sinkConnectorInfo = GetFullConnectorInfo(Id, sinkItem, sinkConnectorOrientation);

            var vm = new ConnectorViewModel(Id, parent, sourceConnectorInfo, sinkConnectorInfo);

            return vm;
        }

        #endregion Override

        private DesignerItemViewModelBase GetConnectorDataItem(IDiagramViewModel diagramVm, Guid id)
        {
            return diagramVm.ItemsSource.FirstOrDefault(x => x.Id == id) as DesignerItemViewModelBase;
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

        private FullyCreatedConnectorInfo GetFullConnectorInfo(Guid connectorId, DesignerItemViewModelBase designerItem, ConnectorOrientation connectorOrientation)
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
                        $"Found invalid persisted Connector Orientation for Connector Id: {connectorId}");
            }
        }

        #endregion Function
    }
}