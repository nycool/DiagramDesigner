using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
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

        #endregion Filed

        #region Command

        public DelegateCommand<SelectableDesignerItemViewModelBase> AddItemCommand { get; private set; }
        public DelegateCommand<SelectableDesignerItemViewModelBase> RemoveItemCommand { get; private set; }
        public DelegateCommand<bool?> SelectedItemsCommand { get; private set; }
        public DelegateCommand GroupCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        #endregion Command

        #region Construstor

        public DiagramViewModelBase()
        {
            Init();
        }

        #endregion Construstor

        #region Fucntion

        protected virtual void Init()
        {
            InitCommand();
            InitCollection();
        }

        protected virtual void InitCommand()
        {
            AddItemCommand = new DelegateCommand<SelectableDesignerItemViewModelBase>(OnAdd);
            RemoveItemCommand = new DelegateCommand<SelectableDesignerItemViewModelBase>(OnRemove);
            SelectedItemsCommand = new DelegateCommand<bool?>(OnSelectedItem);
            ClearCommand = new DelegateCommand(OnClear);
            GroupCommand = new DelegateCommand(OnGroup);
        }

        private void OnGroup()
        {
            if (SelectedItems.Count > 0)
            {
                // if only one selected item is a Grouping item -> ungroup
                if (SelectedItems[0] is GroupingDesignerItemViewModel && SelectedItems.Count == 1)
                {
                    if (SelectedItems[0] is GroupingDesignerItemViewModel groupViewModel)
                    {
                        //GroupingDesignerItemViewModel groupObject = SelectedItems[0] as GroupingDesignerItemViewModel;
                        foreach (var item in groupViewModel.ItemsSource)
                        {
                            if (item is DesignerItemViewModelBase tmp)
                            {
                                tmp.Top += groupViewModel.Top;
                                tmp.Left += groupViewModel.Left;
                            }

                            item.Parent = this;

                            AddItemCommand.Execute(item);
                        }

                        // "cut" connections between DiagramItems and the Group
                        var groupedItemsToRemove = new List<SelectableDesignerItemViewModelBase>();

                        foreach (var connector in ItemsSource.OfType<ConnectorViewModel>())
                        {
                            if (groupViewModel == connector.SourceConnectorInfo.DesignerItem)
                            {
                                groupedItemsToRemove.Add(connector);
                            }

                            if (groupViewModel == ((FullyCreatedConnectorInfo)connector.SinkConnectorInfo).DesignerItem)
                            {
                                groupedItemsToRemove.Add(connector);
                            }
                        }

                        groupedItemsToRemove.Add(groupViewModel);

                        foreach (var selectedItem in groupedItemsToRemove)
                        {
                            RemoveItemCommand.Execute(selectedItem);
                        }
                    }
                }
                else if (SelectedItems.Count > 1)
                {
                    double margin = 15;

                    Rect rect = GetBoundingRectangle(SelectedItems, margin);

                    var data = new DesignerItemData(Guid.NewGuid(), this,
                        new DesignerItemPosition(rect.Left, rect.Top));

                    var groupItem = new GroupingDesignerItemViewModel();

                    groupItem.LoadDesignerItemData(data);

                    groupItem.ItemWidth = rect.Width;
                    groupItem.ItemHeight = rect.Height;

                    foreach (var item in SelectedItems)
                    {
                        if (item is DesignerItemViewModelBase designerItem)
                        {
                            designerItem.Top -= rect.Top;
                            designerItem.Left -= rect.Left;
                        }

                        item.Parent = groupItem;

                        groupItem.ItemsSource.Add(item);
                    }

                    // "cut" connections between DiagramItems which are going to be grouped and
                    // Diagramitems which are not going to be grouped
                    List<SelectableDesignerItemViewModelBase> groupedItemsToRemove = SelectedItems;

                    var connectionsToAlsoRemove = new List<SelectableDesignerItemViewModelBase>();

                    foreach (var connector in ItemsSource.OfType<ConnectorViewModel>())
                    {
                        if (ItemsToDeleteHasConnector(groupedItemsToRemove, connector.SourceConnectorInfo))
                        {
                            connectionsToAlsoRemove.Add(connector);
                        }

                        if (ItemsToDeleteHasConnector(groupedItemsToRemove, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                        {
                            connectionsToAlsoRemove.Add(connector);
                        }
                    }
                    groupedItemsToRemove.AddRange(connectionsToAlsoRemove);

                    foreach (var selectedItem in groupedItemsToRemove)
                    {
                        RemoveItemCommand.Execute(selectedItem);
                    }

                    SelectedItems.Clear();
                    ItemsSource.Add(groupItem);
                }
            }
        }

        private bool ItemsToDeleteHasConnector(IList<SelectableDesignerItemViewModelBase> itemsToRemove, FullyCreatedConnectorInfo connector)
        {
            return itemsToRemove.Contains(connector.DesignerItem);
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
                ItemsSource.Clear();

                RemoveOrAdd(null, Operation.Clear);
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

            ItemsSource.Remove(removeItem);

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

            RemoveOrAdd(addItem, Operation.Add);
        }

        private void InitCollection()
        {
            ItemsSource = new ObservableCollection<SelectableDesignerItemViewModelBase>();
        }

        #endregion Fucntion
    }
}