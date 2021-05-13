using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using Prism.Commands;

namespace DiagramDesigner.BaseClass.Interface
{
    public interface IDiagramViewModel
    {
        /// <summary>
        /// 增加designerItem or connection 
        /// </summary>
        DelegateCommand<SelectableDesignerItemViewModelBase> AddItemCommand { get; }

        /// <summary>
        /// 删除designerItem or connection 
        /// </summary>
        DelegateCommand<SelectableDesignerItemViewModelBase> RemoveItemCommand { get; }

        /// <summary>
        /// 清楚界面
        /// </summary>
        DelegateCommand ClearSelectedItemsCommand { get; }

        /// <summary>
        /// 分组
        /// </summary>
        DelegateCommand GroupCommand { get; }

        /// <summary>
        /// 清楚所有Item
        /// </summary>
        /// <returns></returns>

        DelegateCommand ClearCommand { get; }

        /// <summary>
        /// 选择的Item
        /// </summary>
        List<SelectableDesignerItemViewModelBase> SelectedItems { get; }

        /// <summary>
        /// 所有的iItem
        /// </summary>
        ObservableCollection<SelectableDesignerItemViewModelBase> ItemsSource { get; }
    }
}
