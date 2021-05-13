using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas
    {
        #region Command




        #endregion

        private void InitCommand()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpen));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, OnSave));
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
            if (GetDiagramVm(sender) is {} vm)
            {
                vm.SelectedItemsCommand.Execute(true);
            }
        }

        #endregion


        #region Redo

        

        #endregion


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
            throw new NotImplementedException();
        }

        #endregion Save

        #region Open

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            if (GetDiagramVm(sender) is { } vm)
            {
                vm.ClearCommand.Execute();
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
            if (sender is DesignerCanvas { DataContext: IDiagramViewModel vm })
            {
                return vm;
            }

            return null;
        }

        private bool ItemsToDeleteHasConnector(List<SelectableDesignerItemViewModelBase> itemsToRemove, FullyCreatedConnectorInfo connector)
        {
            return itemsToRemove.Contains(connector.DesignerItem);
        }

        #endregion CommandFunction
    }
}