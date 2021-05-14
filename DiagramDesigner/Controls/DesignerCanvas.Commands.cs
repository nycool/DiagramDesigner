using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas
    {
        #region Command

        /// <summary>
        /// 向上移动一层
        /// </summary>
        public static readonly RoutedCommand BringForward = new RoutedCommand();

        /// <summary>
        /// 向下移动一层
        /// </summary>
        public static readonly RoutedCommand SendBackward = new RoutedCommand();

        /// <summary>
        /// 置顶
        /// </summary>
        public static readonly RoutedCommand BringToFront = new RoutedCommand();

        /// <summary>
        /// 置底
        /// </summary>
        public static readonly RoutedCommand SendToBack = new RoutedCommand();

        /// <summary>
        /// 顶部对齐
        /// </summary>
        public static readonly RoutedCommand AlignTop = new RoutedCommand();

        /// <summary>
        /// 分组
        /// </summary>

        public static readonly RoutedCommand Group = new RoutedCommand();

        /// <summary>
        /// 垂直居中
        /// </summary>
        public static readonly RoutedCommand AlignVerticalCenters = new RoutedCommand();

        /// <summary>
        /// 底部对齐
        /// </summary>

        public static readonly RoutedCommand AlignBottom = new RoutedCommand();

        /// <summary>
        /// 左对齐
        /// </summary>
        public static readonly RoutedCommand AlignLeft = new RoutedCommand();

        /// <summary>
        /// 水平居中
        /// </summary>
        public static readonly RoutedCommand AlignHorizontalCenters = new RoutedCommand();

        /// <summary>
        /// 右对齐
        /// </summary>
        public static readonly RoutedCommand AlignRight = new RoutedCommand();

        /// <summary>
        /// 水平分布
        /// </summary>
        public static readonly RoutedCommand DistributeHorizontal = new RoutedCommand();

        /// <summary>
        /// 垂直分布
        /// </summary>
        public static readonly RoutedCommand DistributeVertical = new RoutedCommand();
        #endregion Command

        private void InitCommand()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpen));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSave, CanSave));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanHandle));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanHandle));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete, CanHandle));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, OnSelectedAll));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUnDo, CanUnDo));

            this.CommandBindings.Add(new CommandBinding(BringForward, OnBringForward, CanHandle));
            this.CommandBindings.Add(new CommandBinding(SendBackward, OnSendBackward, CanHandle));

            this.CommandBindings.Add(new CommandBinding(BringToFront, OnBringToFront, CanHandle));
            this.CommandBindings.Add(new CommandBinding(SendToBack, OnSendToBack, CanHandle));

            this.CommandBindings.Add(new CommandBinding(AlignTop, OnAlignTop, CanHandle));
            this.CommandBindings.Add(new CommandBinding(AlignVerticalCenters, OnAlignVerticalCenters, CanHandle));
            this.CommandBindings.Add(new CommandBinding(AlignBottom, OnAlignBottom, CanHandle));
            this.CommandBindings.Add(new CommandBinding(AlignLeft, OnAlignLeft, CanHandle));
            this.CommandBindings.Add(new CommandBinding(AlignHorizontalCenters, OnAlignHorizontalCenters, CanHandle));
            this.CommandBindings.Add(new CommandBinding(AlignRight, OnAlignRight, CanHandle));
            this.CommandBindings.Add(new CommandBinding(DistributeHorizontal, OnDistributeHorizontal, CanHandle));
            this.CommandBindings.Add(new CommandBinding(DistributeVertical, OnDistributeVertical, CanHandle));

            this.CommandBindings.Add(new CommandBinding(Group, OnGroup, CanHandle));

            InitInputGesture();
        }


        #region CommandFunction

        #region DistributeVertical

        private void OnDistributeVertical(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is {} vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    double top = Double.MaxValue;
                    double bottom = Double.MinValue;
                    double sumHeight = 0;

                    foreach (var item in selectedItems)
                    {
                        top = Math.Min(top, item.Top);
                        bottom = Math.Max(bottom, item.Top + item.ItemHeight);
                        sumHeight += item.ItemHeight;
                    }

                    var first = selectedItems.First();
                    double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
                    double offset = first.Top;

                    foreach (var item in selectedItems)
                    {
                        double delta = offset - item.Top;
                        item.Top += delta;
                        offset = offset + item.ItemHeight + distance;
                    }
                }
            }
        }


        #endregion


        #region DistributeHorizontal

        private void OnDistributeHorizontal(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    double left = Double.MaxValue;
                    double right = Double.MinValue;
                    double sumWidth = 0;

                    foreach (var item in selectedItems)
                    {
                        left = Math.Min(left, item.Left);
                        right = Math.Max(right,item.Left + item.ItemWidth);
                        sumWidth += item.ItemWidth;
                    }

                    var first = selectedItems.First();

                    double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
                    double offset = first.Left;

                    foreach (var item in selectedItems)
                    {
                        double delta = offset - item.Left;

                        item.Left += delta;

                        offset = offset + item.ItemWidth + distance;
                    }
                }
            }
        }

        #endregion DistributeHorizontal

        #region AlignRight

        private void OnAlignRight(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    var first = selectedItems.First();

                    double right = first.Left + first.ItemWidth;

                    foreach (var item in selectedItems)
                    {
                        double delta = right - (item.Left + item.ItemWidth);

                        item.Left += delta;
                    }
                }
            }
        }

        #endregion AlignRight

        #region AlignHorizontalCenters

        private void OnAlignHorizontalCenters(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    var first = selectedItems.First();

                    double center = first.Left + (first.ItemWidth / 2);

                    foreach (var item in selectedItems)
                    {
                        double delta = center - (item.Left + (item.ItemWidth / 2));

                        item.Left += delta;
                    }
                }
            }
        }

        #endregion AlignHorizontalCenters

        #region AlignLeft

        private void OnAlignLeft(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    var first = selectedItems.First();

                    double left = first.Left;

                    foreach (var item in selectedItems)
                    {
                        double delta = left - item.Left;

                        item.Left += delta;
                    }
                }
            }
        }

        #endregion AlignLeft

        #region AlignBottom

        private void OnAlignBottom(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    var first = selectedItems.First();

                    double bottom = first.Top + first.ItemHeight;

                    foreach (var item in selectedItems)
                    {
                        double delta = bottom - (item.Top + item.ItemHeight);

                        item.Top += delta;
                    }
                }
            }
        }

        #endregion AlignBottom

        #region AlignVerticalCenters

        private void OnAlignVerticalCenters(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    var item = selectedItems.First();

                    double bottom = item.Top + (item.ItemHeight / 2);

                    foreach (var selectedItem in selectedItems)
                    {
                        double delta = bottom - (selectedItem.Top + (selectedItem.ItemHeight / 2));

                        selectedItem.Top += delta;
                    }
                }
            }
        }

        #endregion AlignVerticalCenters

        #region Group

        private void OnGroup(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                vm.GroupCommand.Execute();
            }
        }

        #endregion Group

        #region AlignTop

        private void OnAlignTop(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var designerItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (designerItems.Any())
                {
                    double top = designerItems.First().Top;

                    foreach (var designerItem in designerItems)
                    {
                        double delta = top - designerItem.Top;

                        designerItem.Top += delta;
                    }
                }
            }
        }

        #endregion AlignTop

        #region SendToBack

        private void OnSendToBack(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedSorted = vm.SelectedItems.OfType<DesignerItemViewModelBase>()
                    .OrderByDescending(s => s.ZIndex).ToList();

                var items = vm.ItemsSource.OfType<DesignerItemViewModelBase>().OrderByDescending(s => s.ZIndex)
                    .ToList();

                int i = 0;
                int j = 0;
                foreach (var item in items)
                {
                    if (selectedSorted.Contains(item))
                    {
                        int value = j++;

                        item.ZIndex = value;
                    }
                    else
                    {
                        int value = selectedSorted.Count + i;
                        item.ZIndex = value;
                        i++;
                    }
                }
            }
        }

        #endregion SendToBack

        #region SendBackward

        private void OnSendBackward(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedOrders = vm.SelectedItems.OfType<DesignerItemViewModelBase>()
                    .OrderByDescending(s => s.ZIndex).ToList();

                var items = vm.ItemsSource.OfType<DesignerItemViewModelBase>();

                for (int i = 0; i < selectedOrders.Count; i++)
                {
                    var item = selectedOrders[i];

                    int currentIndex = item.ZIndex;

                    int newIndex = Math.Min(i, currentIndex - 1);

                    if (currentIndex != newIndex)
                    {
                        item.ZIndex = newIndex;

                        foreach (var info in items.Where(s => s.ZIndex == newIndex))
                        {
                            if (info != item)
                            {
                                info.ZIndex = currentIndex;
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion SendBackward

        #region BringToFront

        private void OnBringToFront(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedSort = vm.SelectedItems.OfType<DesignerItemViewModelBase>()
                    .OrderByDescending(s => s.ZIndex).ToList();

                var items = vm.ItemsSource.OfType<DesignerItemViewModelBase>().OrderByDescending(s => s.ZIndex).ToList();

                int i = 0;
                int j = 0;
                foreach (var item in items)
                {
                    if (selectedSort.Contains(item))
                    {
                        var abs = items.Count - selectedSort.Count;

                        int value = abs + j;

                        item.ZIndex = value;

                        j++;
                    }
                    else
                    {
                        int value = i++;

                        item.ZIndex = value;
                    }
                }
            }
        }

        #endregion BringToFront

        #region BringForward

        private void CanHandle(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelectItems(sender);
        }

        private void OnBringForward(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                if (vm.SelectedItems.Any())
                {
                    var designerItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>().OrderByDescending(s => s.ZIndex).ToList();

                    int count = vm.ItemsSource.Count;

                    var allItems = vm.ItemsSource.OfType<DesignerItemViewModelBase>();

                    if (designerItems.Any())
                    {
                        for (int i = 0; i < designerItems.Count; i++)
                        {
                            var item = designerItems[i];

                            int currentIndex = item.ZIndex;

                            int newIndex = Math.Min(count - 1 - i, currentIndex + 1);

                            if (currentIndex != newIndex)
                            {
                                item.ZIndex = newIndex;

                                foreach (var info in allItems.Where(s => s.ZIndex == newIndex))
                                {
                                    if (info != item)
                                    {
                                        info.ZIndex = currentIndex;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion BringForward

        #region SelctedAll

        private void OnSelectedAll(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                vm.SelectedItemsCommand.Execute(true);
            }
        }

        #endregion SelctedAll

        #region Cut

        private void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
        }

        #endregion Cut

        #region Copy

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                CopyData = GetDiagram(vm.SelectedItems);
            }
        }

        #endregion Copy

        #region Paste

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CopyData != null;
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                vm.SelectedItemsCommand.Execute(false);

                var connections = CopyData.DesignerAndConnectItems.OfType<Connection>();

                var designerItems = CopyData.DesignerAndConnectItems.OfType<DesignerItemBase>();

                var oldMapNew = new Dictionary<Guid, Guid>();

                foreach (var designerItem in designerItems)
                {
                    var newGuid = Guid.NewGuid();

                    oldMapNew.Add(designerItem.Id, newGuid);

                    designerItem.Id = newGuid;
                }

                foreach (var connection in connections)
                {
                    if (oldMapNew.ContainsKey(connection.SourceId))
                    {
                        connection.SourceId = oldMapNew[connection.SourceId];
                    }

                    if (oldMapNew.ContainsKey(connection.SinkId))
                    {
                        connection.SinkId = oldMapNew[connection.SinkId];
                    }
                }

                foreach (ILoad load in CopyData.DesignerAndConnectItems)
                {
                    var info = load.LoadSaveInfo(vm);

                    if (info is DesignerItemViewModelBase designerItem)
                    {
                        designerItem.Top += 5;
                        designerItem.Left += 5;
                        designerItem.IsSelected = true;
                    }

                    if (info != null)
                    {
                        vm.ItemsSource.Add(info);
                    }
                }
            }
        }

        #endregion Paste

        #region Delete

        private void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                if (vm.SelectedItems.Any())
                {
                    var selectedItems = vm.SelectedItems;

                    var items = vm.ItemsSource;

                    var connectors = items.OfType<ConnectorViewModel>().ToList();

                    for (int i = connectors.Count - 1; i >= 0; i--)
                    {
                        var connector = connectors[i];

                        if (ItemsToDeleteHasConnector(selectedItems, connector.SourceConnectorInfo))
                        {
                            vm.RemoveItemCommand.Execute(connector);

                            _deleteStack.Push(connector);
                        }

                        if (ItemsToDeleteHasConnector(selectedItems, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                        {
                            vm.RemoveItemCommand.Execute(connector);
                            _deleteStack.Push(connector);
                        }
                    }

                    foreach (var selectedItem in selectedItems)
                    {
                        vm.RemoveItemCommand.Execute(selectedItem);
                        _deleteStack.Push(selectedItem);
                    }
                }
            }
        }

        #endregion Delete

        #region Undo

        private void CanUnDo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MoveStack.Any() || _deleteStack.Any();
        }

        private void OnUnDo(object sender, ExecutedRoutedEventArgs e)
        {
            if (MoveStack.TryPop(out var moveInfo))
            {
                switch (moveInfo.Orientation)
                {
                    case Orientation.Left:
                        moveInfo.DesignerItem.Left -= moveInfo.Offset;
                        break;

                    case Orientation.Top:
                        moveInfo.DesignerItem.Top -= moveInfo.Offset;
                        break;
                }
            }

            if (_deleteStack.TryPop(out var deleteItem))
            {
                if (GetDiagramVm(sender) is { } vm)
                {
                    vm.ItemsSource.Add(deleteItem);
                }
            }
        }

        #endregion Undo

        #region Save

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetViewModel<IDiagramViewModel>(sender) is { } vm)
            {
                var diagram = GetDiagram(vm.ItemsSource);

                if (diagram != null)
                {
#warning  节点信息如何存储
                }
            }
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                e.CanExecute = vm.ItemsSource.Any();
            }
        }

        #endregion Save

        #region Open

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                vm.ClearCommand.Execute();

#warning 文件如何加载上来

                //foreach (ILoad load in wholeDiagramToLoad.DesignerAndConnectItems)
                //{
                //    var info = load.LoadSaveInfo(vm);

                //    if (info != null)
                //    {
                //        vm.ItemsSource.Add(info);
                //    }
                //}
            }
        }

        #endregion Open

        /// <summary>
        /// 初始化快捷键
        /// </summary>
        private void InitInputGesture()
        {
        }

        /// <summary>
        /// 获取保存的信息
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        private IDiagram GetDiagram(IEnumerable<SelectableDesignerItemViewModelBase> selectedItems)
        {
            IDiagram diagram = null;

            if (selectedItems.Any())
            {
                diagram = new Diagram();

                foreach (var designerItem in selectedItems)
                {
                    var item = designerItem.SaveInfo();

                    if (item != null)
                    {
                        diagram.DesignerAndConnectItems.Add(item);
                    }
                }
            }

            return diagram;
        }

        private T GetViewModel<T>(object obj) where T : IDiagramViewModel
        {
            if (obj is FrameworkElement framework)
            {
                if (framework.DataContext is T t)
                {
                    return t;
                }
            }

            return default;
        }

        /// <summary>
        /// 判断是否选择了控件
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool IsSelectItems(object sender)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                return vm.SelectedItems.Any();
            }

            return false;
        }

        /// <summary>
        /// 获取Canvas的ViewModel
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private IDiagramViewModel GetDiagramVm(object sender)
        {
            return GetViewModel<IDiagramViewModel>(sender);
        }

        private bool ItemsToDeleteHasConnector(List<SelectableDesignerItemViewModelBase> itemsToRemove, FullyCreatedConnectorInfo connector)
        {
            return itemsToRemove.Contains(connector.DesignerItem);
        }

        #endregion CommandFunction
    }
}