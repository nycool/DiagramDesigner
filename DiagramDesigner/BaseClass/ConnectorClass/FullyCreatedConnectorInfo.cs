﻿using DiagramDesigner.BaseClass.DesignerItemViewModel;

namespace DiagramDesigner.BaseClass.ConnectorClass
{
    /// <summary>
    /// 连接上的点
    /// </summary>
    public class FullyCreatedConnectorInfo : ConnectorInfoBase
    {
        #region Filed

        private bool showConnectors;

        /// <summary>
        /// 是否显示连接点
        /// </summary>
        public bool ShowConnectors
        {
            get => showConnectors;
            set => SetProperty(ref showConnectors, value);
        }


        /// <summary>
        /// 点所在的模块
        /// </summary>
        public DesignerItemViewModelBase DesignerItem { get; }
        #endregion


        public FullyCreatedConnectorInfo(DesignerItemViewModelBase designerItem, ConnectorOrientation orientation)
            : base(orientation)
        {
            this.DesignerItem = designerItem;
        }
    }
}