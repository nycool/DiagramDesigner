using DiagramDesigner.BaseClass;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class SelectableDesignerItemViewModelBase : BindableBase, ISelectItems, ISave
    {
        #region Filed

        private bool _isSelected;

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    OnSelectedChanged();
                }
            }
        }

        /// <summary>
        /// 上一级模块
        /// </summary>
        public IDiagramViewModel Parent { get; set; }

        /// <summary>
        /// 选择的模块
        /// </summary>
        public List<SelectableDesignerItemViewModelBase> SelectedItems => Parent.SelectedItems;

        #endregion Filed

        #region Command

        public DelegateCommand<bool?> SelectItemCommand { get; private set; }

        #endregion Command

        #region Construstor

        public SelectableDesignerItemViewModelBase()
        {
            Init();
        }

        #endregion Construstor

        #region Function

        public abstract PersistenceAbleItemBase SaveInfo();

        protected virtual void OnSelectedChanged()
        {
        }

        /// <summary>
        /// 加载模块的数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void LoadDesignerItemData(DesignerItemData data)
        {
            Parent = data.Parent;
        }

        private void Init()
        {
            SelectItemCommand = new DelegateCommand<bool?>(ExecuteSelectItemCommand);
        }

        private void ExecuteSelectItemCommand(bool? isNewSelect)
        {
            SelectItem((bool)isNewSelect, !IsSelected);
        }

        private void SelectItem(bool isNewSelect, bool select)
        {
            if (isNewSelect)
            {
                foreach (var designerItemViewModelBase in Parent.SelectedItems)
                {
                    designerItemViewModelBase._isSelected = false;
                }
            }

            IsSelected = select;
        }

        #endregion Function
    }
}