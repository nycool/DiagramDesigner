using DiagramDesigner.DesignerItemViewModel;

namespace DiagramDesigner.BaseClass.Connectors
{
    /// <summary>
    /// 连接上的点
    /// </summary>
    public class Connector : ConnectorBase
    {
        #region Filed

        private bool _showConnectors;

        /// <summary>
        /// 是否显示连接点
        /// </summary>
        public bool ShowConnectors
        {
            get => _showConnectors;
            set => SetProperty(ref _showConnectors, value);
        }

        /// <summary>
        /// 点所在的模块
        /// </summary>
        public DesignerItemViewModelBase DesignerItem { get; private set; }

        #endregion Filed

        public Connector(DesignerItemViewModelBase designerItem, ConnectorOrientation orientation)
            : base(orientation)
        {
            this.DesignerItem = designerItem;
        }

        public void UpdateDesignerItem(DesignerItemViewModelBase designerItem, Connector oldSource)
        {
            DesignerItem = designerItem;

            Orientation = oldSource.Orientation;
        }
    }
}