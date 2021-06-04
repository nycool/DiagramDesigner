using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using NodeLib.NodeInfo.Interfaces;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class DesignerItemViewModelBase : SelectableDesignerItemViewModelBase, IConnect, INode
    {
        #region Filed

        private double _left;

        public double Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }

        private double _top;

        public double Top
        {
            get => _top;
            set => SetProperty(ref _top, value);
        }

        private int _zIndex;

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }

        private double _itemWidth = 80;

        /// <summary>
        /// DesignerItem的宽
        /// </summary>
        public double ItemWidth
        {
            get => _itemWidth;
            set => SetProperty(ref _itemWidth, value);
        }

        private double _itemHeight = 40;

        /// <summary>
        /// DesignerItem的高
        /// </summary>
        public double ItemHeight
        {
            get => _itemHeight;
            set => SetProperty(ref _itemHeight, value);
        }

        private double _minWidth;

        public double MinWidth
        {
            get => _minWidth;
            set => SetProperty(ref _minWidth, value);
        }

        private double _minHeight;

        public double MinHeight
        {
            get => _minHeight;
            set => SetProperty(ref _minHeight, value);
        }


        private double _actualHeight;

        public double ActualHeight
        {
            get => _actualHeight;
            set => SetProperty(ref _actualHeight, value);
        }


        private double _actualWidth;

        public double ActualWidth
        {
            get => _actualWidth;
            set => SetProperty(ref _actualWidth, value);
        }


        private bool _isDragConnectionOver;

        public bool IsDragConnectionOver
        {
            get => _isDragConnectionOver;
            set => SetProperty(ref _isDragConnectionOver, value);
        }



        private bool _showConnectors = false;

        /// <summary>
        /// 是否显示四点
        /// </summary>
        public bool ShowConnectors
        {
            get => _showConnectors;
            set
            {
                if (SetProperty(ref _showConnectors, value))
                {
                    TopConnector.ShowConnectors = value;
                    BottomConnector.ShowConnectors = value;
                    RightConnector.ShowConnectors = value;
                    LeftConnector.ShowConnectors = value;
                }
            }
        }

        private List<FullyCreatedConnectorInfo> _connectors;

        public FullyCreatedConnectorInfo TopConnector => _connectors[0];

        public FullyCreatedConnectorInfo BottomConnector => _connectors[1];

        public FullyCreatedConnectorInfo LeftConnector => _connectors[2];

        public FullyCreatedConnectorInfo RightConnector => _connectors[3];

        private ExternUserDataBase _externUserData;

        /// <summary>
        /// ViewModel上绑定的数据
        /// </summary>
        public ExternUserDataBase ExternUserData
        {
            get => _externUserData;
            set => SetProperty(ref _externUserData, value);
        }


        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DestinationId { get; set; }

        public Guid SourceId { get; set; }

        #endregion Filed

        #region Construstor

        public DesignerItemViewModelBase()
        {
            Init();
        }

        #endregion Construstor

        #region Override

        /// <summary>
        /// 获取保存数据的类型
        /// </summary>
        /// <returns></returns>
        protected abstract Type GetPersistenceItemType();

        public override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);
            InitPosition(data.Position);
            ExternUserData = data.UserData;
            Id=data.Id;
        }

        /// <summary>
        /// 获取用户存储数据
        /// </summary>
        /// <returns></returns>

        protected abstract ExternUserDataBase GetExternUserData();


        public sealed override PersistenceAbleItemBase SaveInfo()
        {
            var position = new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight);

            var itemData = new DesignerItemData(Id, position, GetExternUserData());

            var type = GetPersistenceItemType();

            if (type == null)
            {
                throw new ArgumentNullException("neet save Persistence type is null");
            }

            var info = ContainerLocator.Current.Resolve(type);

            if (info is PersistenceAbleItemBase persistenceAbleItem)
            {
                var item = persistenceAbleItem;

                if (item is DesignerItemBase designerItem)
                {
                    designerItem.DesignerItemData = itemData;

                    if (this is GroupingDesignerItemViewModel groupVm)
                    {
                        if (groupVm.ItemsSource.Any())
                        {
                            if (designerItem is GroupDesignerItem groupDesignerItem)
                            {
                                SaveGroup(groupDesignerItem, groupVm);
                            }
                        }
                    }

                    return item;
                }
            }

            throw new AggregateException($"your type:{type} is not PersistenceAbleItemBase child");
        }

        private void SaveGroup(IDiagram diagram, IDiagramViewModel diagramVm)
        {
            foreach (var items in diagramVm.ItemsSource)
            {
                var item = items.SaveInfo();

                if (item != null)
                {
                    diagram.DesignerAndConnectItems.Add(item);
                }
            }
        }

        #endregion Override

        #region Function

        private void Init()
        {
            _connectors = new List<FullyCreatedConnectorInfo>
            {
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top),
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom),
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left),
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right)
            };
        }

        private void InitPosition(DesignerItemPosition position)
        {
            Left = position.Left;
            Top = position.Top;
            ItemWidth = position.Width;
            ItemHeight = position.Height;
        }

        public void ConnectSource(IConnect parent)
        {
            SourceId = parent.GetCurrentId();
        }

        public void ConnectDestination(IConnect child)
        {
            DestinationId = child.GetCurrentId();
        }

        public Guid GetCurrentId() => Id;

        #endregion Function

    }
}