using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Connectors;
using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using DiagramDesigner.Persistence;
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

        private double _itemWidth = 120;

        /// <summary>
        /// DesignerItem的宽
        /// </summary>
        public double ItemWidth
        {
            get => _itemWidth;
            set => SetProperty(ref _itemWidth, value);
        }

        private double _itemHeight = 45;

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

        private bool _canResize;

        public bool CanResize
        {
            get => _canResize;
            set => SetProperty(ref _canResize, value);
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

        private List<BaseClass.Connectors.ConnectInfo> _connectors;

        private BaseClass.Connectors.ConnectInfo _topConnector;

        public BaseClass.Connectors.ConnectInfo TopConnector
        {
            get => _topConnector;
            set => SetProperty(ref _topConnector, value);
        }

        private BaseClass.Connectors.ConnectInfo _bottomConnector;

        public BaseClass.Connectors.ConnectInfo BottomConnector
        {
            get => _bottomConnector;
            set => SetProperty(ref _bottomConnector, value);
        }

        private BaseClass.Connectors.ConnectInfo _leftConnector;

        public BaseClass.Connectors.ConnectInfo LeftConnector
        {
            get => _leftConnector;
            set => SetProperty(ref _leftConnector, value);
        }

        private BaseClass.Connectors.ConnectInfo _rightConnector;

        public BaseClass.Connectors.ConnectInfo RightConnector
        {
            get => _rightConnector;
            set => SetProperty(ref _rightConnector, value);
        }

        private IUserData _externUserData;

        /// <summary>
        /// ViewModel上绑定的数据
        /// </summary>
        public IUserData ExternUserData
        {
            get => _externUserData;
            set => SetProperty(ref _externUserData, value);
        }

        public List<Guid> DestinationId { get; set; }

        public List<Guid> SourceId { get; set; }

        #endregion Filed

        #region Event

        /// <summary>
        /// 连接源
        /// </summary>
        protected Action<DesignerItemViewModelBase> ConnectSourceAction { get; set; }

        /// <summary>
        /// 连接目标
        /// </summary>
        protected Action<DesignerItemViewModelBase> ConnectDstAction { get; set; }

        /// <summary>
        /// 移除连接点
        /// </summary>
        protected Action<DesignerItemViewModelBase, RemoveTypes> RemoveAction { get; set; }

        #endregion Event

        #region Construstor

        protected DesignerItemViewModelBase()
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

        /// <summary>
        /// 加载用户数据
        /// </summary>
        /// <param name="userData"></param>
        protected abstract void LoadUseData(IUserData userData);

        public override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);
            InitPosition(data.Position);
            ExternUserData = data.UserData;
            Id = data.Id;
            LoadUseData(ExternUserData);
        }

        /// <summary>
        /// 获取用户存储数据
        /// </summary>
        /// <returns></returns>

        protected abstract IUserData GetExternUserData();

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

        /// <summary>
        /// 连接源线事件
        /// </summary>
        /// <param name="designerItem"></param>
        public void ConSrcDesignerItemAction(DesignerItemViewModelBase designerItem)
        {
            ConnectSourceAction?.Invoke(designerItem);
        }

        /// <summary>
        /// 连接子线事件
        /// </summary>
        /// <param name="designerItem"></param>
        public void ConSinkDesignerItemAction(DesignerItemViewModelBase designerItem)
        {
            ConnectDstAction?.Invoke(designerItem);
        }

        /// <summary>
        /// 移除连线
        /// </summary>
        /// <param name="designerItem"></param>
        /// <param name="removeTypes"></param>
        public void RemoveCon(DesignerItemViewModelBase designerItem, RemoveTypes removeTypes)
        {
            RemoveAction?.Invoke(designerItem, removeTypes);
        }

        private void Init()
        {
            _connectors = new List<BaseClass.Connectors.ConnectInfo>
            {
                new BaseClass.Connectors.ConnectInfo(this, ConnectorOrientation.Top),
                new BaseClass.Connectors.ConnectInfo(this, ConnectorOrientation.Bottom),
                new BaseClass.Connectors.ConnectInfo(this, ConnectorOrientation.Left),
                new BaseClass.Connectors.ConnectInfo(this, ConnectorOrientation.Right)
            };

            TopConnector = _connectors[0];

            BottomConnector = _connectors[1];

            LeftConnector = _connectors[2];

            RightConnector = _connectors[3];
        }

        private void InitPosition(DesignerItemPosition position)
        {
            Left = position.Left;
            Top = position.Top;
            ItemWidth = position.Width;
            ItemHeight = position.Height;
        }

        public bool ConnectSource(IConnect parent)
        {
            SourceId ??= new List<Guid>();

            var parentId = parent.GetCurrentId();

            if (!SourceId.Contains(parentId))
            {
                SourceId.Add(parentId);

                ConnectSourceAction?.Invoke(parent as DesignerItemViewModelBase);

                return true;
            }

            return default;
        }

        public bool ConnectDestination(IConnect child)
        {
            DestinationId ??= new List<Guid>();

            var childId = child.GetCurrentId();

            if (!DestinationId.Contains(childId))
            {
                DestinationId.Add(childId);

                ConnectDstAction?.Invoke(child as DesignerItemViewModelBase);

                return true;
            }

            return default;
        }

        public bool Remove(IConnect connect, RemoveTypes removeType)
        {
            if (connect is DesignerItemViewModelBase designerItem)
            {
                RemoveAction?.Invoke(designerItem, removeType);
            }

            switch (removeType)
            {
                case RemoveTypes.Source:

                    if (SourceId != default)
                    {
                        return SourceId.Remove(connect.GetCurrentId());
                    }
                    break;

                case RemoveTypes.Destination:

                    if (DestinationId != default)
                    {
                        return DestinationId.Remove(connect.GetCurrentId());
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(removeType), removeType, null);
            }

            return default;
        }

        public Guid GetCurrentId() => Id;

        #endregion Function
    }
}