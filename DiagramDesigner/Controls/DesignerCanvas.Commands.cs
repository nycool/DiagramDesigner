﻿using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using Microsoft.Win32;
using NodeLib.NodeInfo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Core;

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

        /// <summary>
        /// 显示网格
        /// </summary>
        public static readonly RoutedCommand GridLines = new RoutedCommand();

        /// <summary>
        /// 判断是否显示网格
        /// </summary>
        private bool _isShowGridLines;

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
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, OnReDo, CanReDo));

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

            this.CommandBindings.Add(new CommandBinding(GridLines, OnGridLines));

            InitInputGesture();
        }

        #region CommandFunction

        #region ReDo

        private void CanReDo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _reStack.Any();
        }

        private void OnReDo(object sender, ExecutedRoutedEventArgs e)
        {
            if (_reStack.TryPop(out var result))
            {
                if (result is MoveInfo moveInfo)
                {
                    switch (moveInfo.Orientation)
                    {
                        case Orientation.Left:
                            moveInfo.DesignerItem.Left += moveInfo.Offset;
                            break;

                        case Orientation.Top:
                            moveInfo.DesignerItem.Top += moveInfo.Offset;
                            break;
                    }
                }
                else if (result is SelectableDesignerItemViewModelBase designerItem)
                {
                    if (GetDiagramVm(sender) is { } vm)
                    {
                        vm.ItemsSource.Remove(designerItem);
                    }
                }
            }
        }

        #endregion ReDo

        #region GirdLines

        private void OnGridLines(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DesignerCanvas canvas)
            {
                _isShowGridLines = !_isShowGridLines;

                GridLinesToBack(canvas, _isShowGridLines);
            }
        }

        private static void GridLinesToBack(DesignerCanvas canvas, bool isShowGrid)
        {
            if (isShowGrid)
            {
                var backGround = canvas.FindResource("ViewerGridLines") as DrawingBrush;

                canvas.Background = backGround;
            }
            else
            {
                canvas.Background = new SolidColorBrush(Colors.White);
            }
        }

        #endregion GirdLines

        #region DistributeVertical

        private void OnDistributeVertical(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var selectedItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                if (selectedItems.Any())
                {
                    double top = double.MaxValue;
                    double bottom = double.MinValue;
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

        #endregion DistributeVertical

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
                        right = Math.Max(right, item.Left + item.ItemWidth);
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

                    var data = designerItem.DesignerItemData;

                    oldMapNew.Add(data.Id, newGuid);

                    data.Id = newGuid;
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
                        vm.AddItemCommand.Execute(info);
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
                _reStack.Push(moveInfo);

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
                _reStack.Push(deleteItem);

                if (GetDiagramVm(sender) is { } vm)
                {
                    vm.ItemsSource.Add(deleteItem);
                }
            }
        }

        #endregion Undo

        #region Save

        private async void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();

            saveDialog.Filter = "(xml)|*.xml";

            saveDialog.Title = "保存文件";

            if (saveDialog.ShowDialog() == true)
            {
                string saveFileName = saveDialog.FileName;

                if (await Loading(saveFileName))
                {
                    MessageBox.Show("解决方案保存成功");
                }
            }
        }

        private Task<bool> Loading(string xmlFileName)
        {
            if (GetViewModel<IDiagramViewModel>(this) is { } vm)
            {
                var diagram = GetDiagram(vm.ItemsSource);

                if (diagram != null)
                {
                    XmlSerializerExtern.SerializerToPath(diagram, xmlFileName);

                    return Task.FromResult(true);
                }
            }

            return default;
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

        private async void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                var openDialog = new OpenFileDialog();

                openDialog.Filter = "(xml)|*.xml";

                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == true)
                {
                    vm.ClearCommand.Execute();

                    string fileName = openDialog.FileName;

                    if (await Opening(vm, fileName))
                    {
                        MessageBox.Show("解决方案加载成功");
                    }
                }
            }
        }

        private Task<bool> Opening(IDiagramViewModel vm, string fileName)
        {
            var diagram = XmlSerializerExtern.DeserializeFromPath<IDiagram>(fileName);

            if (diagram != null)
            {
                foreach (ILoad load in diagram.DesignerAndConnectItems)
                {
                    var info = load.LoadSaveInfo(vm);

                    vm.AddItemCommand.Execute(info);
                }

                var connectInfos = vm.ItemsSource.OfType<ConnectorViewModel>();

                var designerItems = vm.ItemsSource.OfType<IConnect>().ToList();

                foreach (var connectInfo in connectInfos)
                {
                    var srcVm = designerItems.Find(s => s == connectInfo.SourceConnectorInfo.DesignerItem);

                    var dstVm = designerItems.Find(s => s == (connectInfo.SinkConnectorInfo as FullyCreatedConnectorInfo)?.DesignerItem);

                    if (srcVm is { } srcConnect && dstVm is { } sinkConnect)
                    {
                        srcConnect.ConnectDestination(sinkConnect);

                        sinkConnect.ConnectSource(srcConnect);
                    }
                }

                return Task.FromResult(true);
            }

            return default;
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