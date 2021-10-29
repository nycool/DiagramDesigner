using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using Prism.Ioc;
using System;
using System.Linq;

namespace DiagramDesigner.Persistence
{
    public abstract class DesignerItemBase : PersistenceAbleItemBase
    {
        #region Filed

        /// <summary>
        /// 模块高
        /// </summary>
        public double ItemHeight { get; set; }

        /// <summary>
        /// 模块宽
        /// </summary>
        public double ItemWidth { get; set; }

        /// <summary>
        /// 左偏移量
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 顶偏移量
        /// </summary>
        public double Top { get; set; }

        private DesignerItemData _designerItemData;

        /// <summary>
        /// 获取模块的基本数据以及用户数据
        /// </summary>
        public DesignerItemData DesignerItemData
        {
            get => _designerItemData;
            set
            {
                if (SetProperty(ref _designerItemData, value))
                {
                    InitPosition(_designerItemData.Position);
                }
            }
        }

        #endregion Filed

        private void InitPosition(DesignerItemPosition position)
        {
            this.Left = position.Left;
            this.Top = position.Top;
            this.ItemWidth = position.Width;
            this.ItemHeight = position.Height;
        }

        /// <summary>
        /// 获取控件的ViewModel类型
        /// </summary>
        /// <returns></returns>
        protected abstract Type GetDesignerItemViewModelType();

        public sealed override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var type = GetDesignerItemViewModelType();

            if (type == null)
            {
                throw new ArgumentNullException("DesignerItemViewModel Param is null");
            }

            var designerData = DesignerItemData;

            if (designerData == null)
            {
                throw new ArgumentNullException("DesignerData is null");
            }

            designerData.Parent = parent;

            var viewModel = ContainerLocator.Current.Resolve(type);

            if (viewModel is DesignerItemViewModelBase designerItem)
            {
                designerItem.LoadDesignerItemData(designerData);
            }

            if (viewModel is GroupDesignerItemViewModelBase group)
            {
                if (this is IDiagram diagram && diagram.DesignerAndConnectItems?.Any() == true)
                {
                    OnLoadGroup(diagram, group);

                    group.RemoveDesignerItemAction = parent.GroupRemoveDesignerItem;
                }
            }

            return viewModel as SelectableDesignerItemViewModelBase;
        }

        /// <summary>
        /// 加载分组数据
        /// </summary>
        /// <param name="diagram"></param>
        /// <param name="group"></param>
        private void OnLoadGroup(IDiagram diagram, IDiagramViewModel group)
        {
            foreach (var item in diagram.DesignerAndConnectItems)
            {
                var info = item.LoadSaveInfo(group);

                if (info != null)
                {
                    group.AddItemCommand.Execute(info);
                }
            }

            var connectInfos = group.ItemsSource.OfType<ConnectorViewModel>();

            var designerItems = group.ItemsSource.OfType<IConnect>().ToList();

            foreach (var connectInfo in connectInfos)
            {
                var srcVm = designerItems.Find(s => s == connectInfo.SourceConnector.DesignerItem);

                var dstVm = designerItems.Find(s => s == (connectInfo.SinkConnector as BaseClass.Connectors.Connector)?.DesignerItem);

                if (srcVm is { } srcConnect && dstVm is { } sinkConnect)
                {
                    srcConnect.ConnectDestination(sinkConnect);

                    sinkConnect.ConnectSource(srcConnect);
                }
            }
        }
    }
}