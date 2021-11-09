using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Connectors;
using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class DiagramViewModelBase : BindableBase, IDiagramViewModel, IDiagramTitle
    {
        #region Filed

        private string _title = "编辑";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public List<SelectableDesignerItemViewModelBase> SelectedItems => ItemsSource.Where(s => s.IsSelected).ToList();

        private ObservableCollection<SelectableDesignerItemViewModelBase> _itemsSource;

        public ObservableCollection<SelectableDesignerItemViewModelBase> ItemsSource
        {
            get => _itemsSource;
            set => SetProperty(ref _itemsSource, value);
        }

        private Dictionary<Type, SortedDictionary<int, IToolInfo>> _sortDictionary;

        private Dictionary<Type, Dictionary<IToolInfo, int>> _toolDic;

        #endregion Filed

        #region Command

        public DelegateCommand<SelectableDesignerItemViewModelBase> AddItemCommand { get; private set; }
        public DelegateCommand<SelectableDesignerItemViewModelBase> RemoveItemCommand { get; private set; }
        public DelegateCommand<bool?> SelectedItemsCommand { get; private set; }
        public DelegateCommand<GroupType?> GroupCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        #endregion Command

        #region Construstor

        public DiagramViewModelBase()
        {
            InitData();
        }

        #endregion Construstor

        #region Fucntion

        private void InitData()
        {
            InitCommand();
            InitCollection();
            Init();
        }

        protected virtual void Init()
        {
        }

        protected virtual void InitCommand()
        {
            AddItemCommand = new DelegateCommand<SelectableDesignerItemViewModelBase>(OnAdd);
            RemoveItemCommand = new DelegateCommand<SelectableDesignerItemViewModelBase>(OnRemove);
            SelectedItemsCommand = new DelegateCommand<bool?>(OnSelectedItem);
            ClearCommand = new DelegateCommand(OnClear);
            GroupCommand = new DelegateCommand<GroupType?>(OnGroup);
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

        private void UnGroup(IList<ConnectorViewModel> lines, GroupDesignerItemViewModelBase group)
        {
            if (group is { } groupViewModel)
            {
                //GroupingDesignerItemViewModel groupObject = SelectedItems[0] as GroupingDesignerItemViewModel;

                var dic = new Dictionary<Guid, DesignerItemViewModelBase>();

                foreach (var item in groupViewModel.ItemsSource)
                {
                    if (item is DesignerItemViewModelBase designerItem)
                    {
                        designerItem.Top += groupViewModel.Top;
                        designerItem.Left += groupViewModel.Left;
                        dic.Add(designerItem.Id, designerItem);

                        designerItem.RemoveCon(group, RemoveTypes.Destination);
                        designerItem.RemoveCon(group, RemoveTypes.Source);
                    }

                    item.Parent = this;

                    AddItemCommand.Execute(item);
                }

                for (int j = 0; j < lines.Count; j++)
                {
                    var line = lines[j];

                    if (default != line.SourceOldId)
                    {
                        int count = line.SourceOldId.Count;

                        for (int i = 0; i < count; i++)
                        {
                            var guid = line.SourceOldId[0];

                            if (dic.ContainsKey(guid))
                            {
                                line.UpdateSource(dic[guid]);
                            }
                            else
                            {
                                RemoveItemCommand.Execute(line);
                            }

                            //line.SourceOldId.Remove(guid);
                        }

                        line.SourceOldId.Clear();
                    }

                    if (default != line.SinkOldId)
                    {
                        int count = line.SinkOldId.Count;

                        for (int i = 0; i < count; i++)
                        {
                            var guid = line.SinkOldId[0];

                            if (dic.ContainsKey(guid))
                            {
                                line.UpdateSink(dic[guid]);
                            }
                            else
                            {
                                RemoveItemCommand.Execute(line);
                            }

                            //line.SinkOldId.Remove(guid);
                        }

                        line.SinkOldId.Clear();
                    }
                }

                RemoveItemCommand.Execute(group);
            }
        }

        private void CreateGroup(GroupType groupType)
        {
            double margin = 25;

            Rect rect = GetBoundingRectangle(SelectedItems, margin);

            var data = new DesignerItemData(Guid.NewGuid(), this,
                new DesignerItemPosition(rect.Left, rect.Top));

            var groupItem = GetGroup(groupType);

            groupItem.LoadDesignerItemData(data);

            groupItem.ItemWidth = rect.Width;

            groupItem.ItemHeight = rect.Height;

            //选择分组的模块
            List<DesignerItemViewModelBase> selectedItems = new List<DesignerItemViewModelBase>();

            foreach (var designerItem in SelectedItems.OfType<DesignerItemViewModelBase>())
            {
                designerItem.Parent = groupItem;

                designerItem.Top -= rect.Top;

                designerItem.Left -= rect.Left;

                groupItem.AddItemCommand.Execute(designerItem);

                selectedItems.Add(designerItem);
            }

            List<ConnectorViewModel> removeItems = new List<ConnectorViewModel>();

            foreach (var connect in ItemsSource.OfType<ConnectorViewModel>())
            {
                if (connect.IsFullConnection)
                {
                    if (connect.SourceConnector is { } source)
                    {
                        bool isSource = default;

                        bool isSink = default;

                        if (selectedItems.Contains(source.DesignerItem))
                        {
                            isSource = true;
                        }

                        if (connect.SinkConnector is BaseClass.Connectors.ConnectInfo fullyConnect)
                        {
                            if (selectedItems.Contains(fullyConnect.DesignerItem))
                            {
                                isSink = true;
                            }
                        }

                        if (isSink && isSource)
                        {
                            groupItem.AddItemCommand.Execute(connect);

                            removeItems.Add(connect);
                        }

                        if (isSource && !isSink)
                        {
                            connect.UpdateSource(groupItem);
                        }

                        if (isSink && !isSource)
                        {
                            connect.UpdateSink(groupItem);
                        }
                    }
                }
            }

            foreach (var removeItem in removeItems)
            {
                RemoveItemCommand.Execute(removeItem);
            }

            foreach (var selectedItem in selectedItems)
            {
                RemoveItemCommand.Execute(selectedItem);
            }

            SelectedItems.Clear();

            ConnectGroupShip(groupItem);

            AddItemCommand.Execute(groupItem);
        }

        /// <summary>
        /// reconnect group ship
        /// </summary>
        /// <param name="group"></param>
        private void ConnectGroupShip(IDiagramViewModel group)
        {
            var connectInfos = group.ItemsSource.OfType<ConnectorViewModel>();

            var designerItems = group.ItemsSource.OfType<IConnect>().ToList();

            foreach (var connectInfo in connectInfos)
            {
                var srcVm = designerItems.Find(s => s == connectInfo.SourceConnector.DesignerItem);

                var dstVm = designerItems.Find(s => s == (connectInfo.SinkConnector as BaseClass.Connectors.ConnectInfo)?.DesignerItem);

                if (srcVm is { } srcConnect && dstVm is { } sinkConnect)
                {
                    srcConnect.ConnectDestination(sinkConnect);

                    sinkConnect.ConnectSource(srcConnect);
                }
            }
        }

        private void OnGroup(GroupType? groupType)
        {
            if (SelectedItems.Any())
            {
                // if only one selected item is a Grouping item -> ungroup
                if (SelectedItems[0] is GroupingDesignerItemViewModel group && SelectedItems.Count == 1)
                {
                    var lines = ItemsSource.OfType<ConnectorViewModel>().Where(s => s.IsContain(group)).ToList();

                    UnGroup(lines, group);
                }
                else if (SelectedItems.Count > 1)
                {
                    if (SelectedItems.OfType<DesignerItemViewModelBase>().Count() > 1)
                    {
                        if (groupType is { } type)
                        {
                            CreateGroup(type);
                        }
                    }
                }
            }
        }

        private Rect GetBoundingRectangle(IEnumerable<SelectableDesignerItemViewModelBase> items, double margin)
        {
            double x1 = double.MaxValue;
            double y1 = double.MaxValue;
            double x2 = double.MinValue;
            double y2 = double.MinValue;

            foreach (DesignerItemViewModelBase item in items.OfType<DesignerItemViewModelBase>())
            {
                x1 = Math.Min(item.Left - margin, x1);
                y1 = Math.Min(item.Top - margin, y1);

                x2 = Math.Max(item.Left + item.ItemWidth + margin, x2);
                y2 = Math.Max(item.Top + item.ItemHeight + margin, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private void OnClear()
        {
            if (ItemsSource.Any())
            {
                while (ItemsSource.Any())
                {
                    RemoveItemCommand.Execute(ItemsSource[0]);
                }

                _sortDictionary.Clear();

                _toolDic.Clear();
            }
        }

        private void OnSelectedItem(bool? isSelected)
        {
            foreach (var item in ItemsSource)
            {
                item.IsSelected = isSelected ?? false;
            }
        }

        protected abstract void RemoveOrAdd(SelectableDesignerItemViewModelBase removeItem, Operation operation);

        private void OnRemove(SelectableDesignerItemViewModelBase removeItem)
        {
            if (removeItem == null)
            {
                return;
            }

            if (removeItem is ConnectorViewModel connector)
            {
                bool res = connector.DisConnected();

                //if (!res)
                //{
                //    throw new ArgumentException("移除关系失败!!!");
                //}
            }

            if (removeItem is GroupDesignerItemViewModelBase group)
            {
                group.RemoveDesignerItemAction = default;
            }

            ItemsSource.Remove(removeItem);

            if (removeItem is IToolInfo tool)
            {
                RemoveDesignerNo(tool);
            }

            RemoveOrAdd(removeItem, Operation.Remove);
        }

        private void OnAdd(SelectableDesignerItemViewModelBase addItem)
        {
            if (addItem == null)
            {
                return;
            }

            addItem.Parent = this;

            ItemsSource.Add(addItem);

            if (addItem is IToolInfo { IsReName: false } tool)
            {
                CreateDesignerItemNo(tool);
            }

            RemoveOrAdd(addItem, Operation.Add);
        }

        private void RemoveDesignerNo(IToolInfo tool)
        {
            if (_sortDictionary.ContainsKey(tool.ViewModelType))
            {
                var sort = _sortDictionary[tool.ViewModelType];

                var dic = _toolDic[tool.ViewModelType];

                if (sort.ContainsValue(tool))
                {
                    if (dic.ContainsKey(tool))
                    {
                        int key = dic[tool];

                        sort.Remove(key);

                        dic.Remove(tool);
                    }
                }

                if (!sort.Any())
                {
                    _sortDictionary.Remove(tool.ViewModelType);
                    _toolDic.Remove(tool.ViewModelType);
                }
            }
        }

        private void CreateDesignerItemNo(IToolInfo tool)
        {
            if (_sortDictionary.ContainsKey(tool.ViewModelType))
            {
                var sort = _sortDictionary[tool.ViewModelType];

                var dic = _toolDic[tool.ViewModelType];

                var count = sort.Keys.Last();

                int key = default;

                bool isAdd = true;

                for (int i = 0; i < count; i++)
                {
                    if (!sort.ContainsKey(i))
                    {
                        key = i;

                        isAdd = default;

                        break;
                    }
                }

                if (isAdd && sort.ContainsKey(0))
                {
                    key = count + 1;
                }

                sort.Add(key, tool);

                dic.Add(tool, key);
            }
            else
            {
                var sort = new SortedDictionary<int, IToolInfo>();

                sort.Add(default, tool);

                _sortDictionary.Add(tool.ViewModelType, sort);

                _toolDic.Add(tool.ViewModelType, new Dictionary<IToolInfo, int>() { { tool, default } });
            }
        }

        private void InitCollection()
        {
            ItemsSource = new ObservableCollection<SelectableDesignerItemViewModelBase>();

            _sortDictionary = new Dictionary<Type, SortedDictionary<int, IToolInfo>>();

            _toolDic = new Dictionary<Type, Dictionary<IToolInfo, int>>();
        }

        /// <summary>
        ///获取不同工具类型的编号
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        protected bool GetNo(IToolInfo tool, out int no)
        {
            no = default;

            var type = tool.ViewModelType;

            if (_toolDic.ContainsKey(type))
            {
                if (_toolDic[type].ContainsKey(tool))
                {
                    no = _toolDic[type][tool];

                    return true;
                }
            }

            return default;
        }

        #endregion Fucntion

        #region Abstrust

        protected virtual GroupDesignerItemViewModelBase GetGroup(GroupType groupType = GroupType.分组) => new GroupingDesignerItemViewModel();

        #endregion Abstrust
    }
}