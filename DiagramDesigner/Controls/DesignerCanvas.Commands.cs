using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas
    {
        private void InitCommand()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpen));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSave, CanSave));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCut));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCopy));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete, CanDelete));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, OnSelectedAll));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUnDo, CanUnDo));


            InitInputGesture();
        }

        #region CommandFunction

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

        private void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelectItems(sender);
        }

        #endregion Cut

        #region Copy

        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion Copy

        #region Paste

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelectItems(sender);
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
        }

        #endregion Paste

        #region Delete

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsSelectItems(sender);
        }

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
                        }

                        if (ItemsToDeleteHasConnector(selectedItems, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                        {
                            vm.RemoveItemCommand.Execute(connector);
                        }
                    }

                    foreach (var selectedItem in selectedItems)
                    {
                        vm.RemoveItemCommand.Execute(selectedItem);
                    }
                }
            }
        }

        #endregion Delete

        #region Undo

        private void CanUnDo(object sender, CanExecuteRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnUnDo(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion Undo

        #region Save

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetViewModel<IDiagramViewModel>(sender) is { } vm)
            {
                foreach (ISave save in vm.ItemsSource)
                {
                    var info = save.SaveInfo();

                    if (info != null)
                    {
#warning  节点信息如何存储
                        //wholeDiagramToSave.DesignerAndConnectItems.Add(info);
                    }
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