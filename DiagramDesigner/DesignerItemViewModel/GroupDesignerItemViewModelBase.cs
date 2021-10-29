using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using DiagramDesigner.Persistence.ExternUserData;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class GroupDesignerItemViewModelBase : DesignerItemViewModelBase, IDiagramViewModel, IGroup
    {
        #region Filed

        public new List<SelectableDesignerItemViewModelBase> SelectedItems =>
            ItemsSource.Where(x => x.IsSelected).ToList();

        private ObservableCollection<SelectableDesignerItemViewModelBase> _itemSource;

        public ObservableCollection<SelectableDesignerItemViewModelBase> ItemsSource
        {
            get => _itemSource;
            set => SetProperty(ref _itemSource, value);
        }

        private bool _isExpended = true;

        public bool IsExpended
        {
            get => _isExpended;
            set => SetProperty(ref _isExpended, value);
        }

        /// <summary>
        /// src->dst
        /// </summary>
        private Dictionary<DesignerItemViewModelBase, DesignerItemViewModelBase> _srcMapDst;

        /// <summary>
        /// dst->src
        /// </summary>
        private Dictionary<DesignerItemViewModelBase, DesignerItemViewModelBase> _dstMapSrc;

        /// <summary>
        /// DesignerItem Buffer
        /// </summary>
        private Dictionary<Guid, DesignerItemViewModelBase> _designerItemDic;

        private double ExpendWidth { get; set; }
        private double ExpendHeight { get; set; }

        #endregion Filed

        #region Command

        public DelegateCommand<SelectableDesignerItemViewModelBase> AddItemCommand { get; private set; }
        public DelegateCommand<SelectableDesignerItemViewModelBase> RemoveItemCommand { get; private set; }
        public DelegateCommand<bool?> SelectedItemsCommand { get; private set; }
        public DelegateCommand<GroupType?> GroupCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        public DelegateCommand ExpandedCommand { get; private set; }
        public DelegateCommand CollapsedCommand { get; private set; }
        public DelegateCommand LoadedCommand { get; private set; }

        #endregion Command

        #region Event

        /// <summary>
        /// 移除分组内的模块事件
        /// </summary>
        public Action<DesignerItemViewModelBase> RemoveDesignerItemAction { get; set; }

        #endregion Event

        #region Construstor

        protected GroupDesignerItemViewModelBase()
        {
            Init();
        }

        #endregion Construstor

        #region Function

        private void Init()
        {
            InitCollection();
            InitCommand();
            this.ShowConnectors = false;
        }

        private void InitCommand()
        {
            AddItemCommand = new DelegateCommand<SelectableDesignerItemViewModelBase>(OnAdd);
            RemoveItemCommand = new DelegateCommand<SelectableDesignerItemViewModelBase>(OnRemove);
            SelectedItemsCommand = new DelegateCommand<bool?>(OnSelectedItems);
            ClearCommand = new DelegateCommand(OnClear);
            GroupCommand = new DelegateCommand<GroupType?>(OnGroup);
            ExpandedCommand = new DelegateCommand(OnExpanded);
            CollapsedCommand = new DelegateCommand(OnCollapsed);
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        public void GroupRemoveDesignerItem(DesignerItemViewModelBase designerItem)
        {
            var connectVms = ItemsSource.OfType<ConnectorViewModel>();

            var id = designerItem.Id;

            foreach (var connector in connectVms.Where(s => s.IsContain(designerItem)))
            {
                if (connector.SinkOldId?.Any() == true)
                {
                    connector.SinkOldId.Remove(id);
                }

                if (connector.SourceOldId?.Any() == true)
                {
                    connector.SourceOldId.Remove(id);
                }
            }
        }

        protected override void LoadUseData(IUserData userData)
        {
            if (userData is GroupData data)
            {
                ExpendHeight = data.ExpendHeight;

                ExpendWidth = data.ExpendWidth;

                IsExpended = data.IsExpended;
            }
        }

        protected override IUserData GetExternUserData()
        {
            ExternUserData ??= new GroupData();

            if (ExternUserData is GroupData data)
            {
                data.ExpendHeight = ExpendHeight;
                data.ExpendWidth = ExpendWidth;
                data.IsExpended = IsExpended;
            }

            return ExternUserData;
        }

        private void OnLoaded()
        {
            if (IsExpended)
            {
                ExpendHeight = ItemHeight;

                ExpendWidth = ItemWidth;
            }
        }

        private void OnCollapsed()
        {
            ItemHeight = 120;

            ItemHeight = 45;
        }

        private void OnExpanded()
        {
            ItemHeight = ExpendHeight;

            ItemWidth = ExpendWidth;
        }

        private void OnGroup(GroupType? groupType)
        {
        }

        private void InitCollection()
        {
            ItemsSource = new ObservableCollection<SelectableDesignerItemViewModelBase>();
        }

        private void OnAdd(SelectableDesignerItemViewModelBase addItem)
        {
            if (addItem == null)
            {
                return;
            }

            addItem.Parent = this;

            if (addItem is DesignerItemViewModelBase designerItem)
            {
                _designerItemDic ??= new Dictionary<Guid, DesignerItemViewModelBase>();

                _designerItemDic.Add(designerItem.Id, designerItem);
            }

            ItemsSource.Add(addItem);
        }

        private void OnRemove(SelectableDesignerItemViewModelBase removeItem)
        {
            if (removeItem == null)
            {
                return;
            }

            if (removeItem is DesignerItemViewModelBase designerItem)
            {
                RemoveDesignerItemAction?.Invoke(designerItem);

                _designerItemDic.Remove(designerItem.Id);

                if (_dstMapSrc?.ContainsKey(designerItem) == true)
                {
                    var src = _dstMapSrc[designerItem];

                    _dstMapSrc.Remove(designerItem);

                    if (_srcMapDst?.ContainsKey(src) == true)
                    {
                        _srcMapDst.Remove(src);
                    }
                }
            }

            ItemsSource.Remove(removeItem);
        }

        private void OnSelectedItems(bool? isSelected)
        {
            foreach (var item in ItemsSource)
            {
                item.IsSelected = isSelected ?? false;
            }
        }

        private void OnClear()
        {
            _designerItemDic?.Clear();

            ItemsSource.Clear();

            _srcMapDst?.Clear();

            _dstMapSrc?.Clear();
        }

        public bool TryAddDesignerItem(DesignerItemViewModelBase srcDesignerItem, DesignerItemViewModelBase designerItem)
        {
            _srcMapDst ??= new Dictionary<DesignerItemViewModelBase, DesignerItemViewModelBase>();

            _dstMapSrc ??= new Dictionary<DesignerItemViewModelBase, DesignerItemViewModelBase>();

            if (!_srcMapDst.ContainsKey(srcDesignerItem))
            {
                _dstMapSrc.TryAdd(designerItem, srcDesignerItem);

                return _srcMapDst.TryAdd(srcDesignerItem, designerItem);
            }

            return default;
        }

        public bool TryGetDesignerItem(DesignerItemViewModelBase src, out DesignerItemViewModelBase designerItem)
        {
            designerItem = default;

            if (_srcMapDst == default)
            {
                return default;
            }

            return _srcMapDst.TryGetValue(src, out designerItem);
        }

        public bool TryGetDstToSrc(DesignerItemViewModelBase des, out DesignerItemViewModelBase src)
        {
            src = default;

            if (_dstMapSrc == default)
            {
                return default;
            }

            return _dstMapSrc.TryGetValue(des, out src);
        }

        public bool Remove(DesignerItemViewModelBase key)
        {
            if (_srcMapDst == default)
            {
                return default;
            }

            return _srcMapDst.Remove(key);
        }

        public DesignerItemViewModelBase FindDesignerItem(Guid id)
        {
            if (_designerItemDic?.ContainsKey(id) == true)
            {
                return _designerItemDic[id];
            }

            return default;
        }

        #endregion Function
    }
}