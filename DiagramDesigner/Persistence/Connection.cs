using System;
using System.Linq;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.Persistence
{
    public class Connection : PersistenceAbleItemBase
    {

        #region Filed
        public Guid SourceId { get; private set; }
        public Orientation SourceOrientation { get; private set; }
        public Guid SinkId { get; private set; }
        public Orientation SinkOrientation { get; private set; }

        #endregion


        #region Construstor

        public Connection(Guid id, Guid sourceId, Orientation sourceOrientation, Guid sinkId, Orientation sinkOrientation)
            : base(id)
        {
            this.SourceId = sourceId;
            this.SourceOrientation = sourceOrientation;

            this.SinkId = sinkId;
            this.SinkOrientation = sinkOrientation;
        }


        #endregion
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

        private FullyCreatedConnectorInfo GetFullConnectorInfo(Guid connectorId, DesignerItemViewModelBase dataItem, ConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Top:
                    return dataItem.TopConnector;

                case ConnectorOrientation.Left:
                    return dataItem.LeftConnector;

                case ConnectorOrientation.Right:
                    return dataItem.RightConnector;

                case ConnectorOrientation.Bottom:
                    return dataItem.BottomConnector;

                default:
                    throw new InvalidOperationException(
                        $"Found invalid persisted Connector Orientation for Connector Id: {connectorId}");
            }
        }
    }

}
