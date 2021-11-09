using DiagramDesigner.DesignerItemViewModel;

namespace DiagramDesigner.BaseClass.Connectors
{
    /// <summary>
    /// 连接上的点
    /// </summary>
    public class ConnectInfo : ConnectBaseInfo
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

        public ConnectInfo(DesignerItemViewModelBase designerItem, ConnectorOrientation orientation)
            : base(orientation)
        {
            this.DesignerItem = designerItem;
        }

        public void UpdateDesignerItem(DesignerItemViewModelBase designerItem, ConnectInfo oldSource)
        {
            DesignerItem = designerItem;

            Orientation = oldSource.Orientation;
        }
    }
}