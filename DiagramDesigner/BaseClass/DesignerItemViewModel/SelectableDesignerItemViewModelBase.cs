using System;
using System.Collections.Generic;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.Persistence;
using Prism.Commands;
using Prism.Mvvm;

namespace DiagramDesigner.BaseClass.DesignerItemViewModel
{
    public abstract class SelectableDesignerItemViewModelBase : BindableBase, ISelectItems,ISave
    {
        #region Filed

        private bool _isSelected;

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; private set; }

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

        protected SelectableDesignerItemViewModelBase(Guid id)
        {
            Id = id;
            Init();
        }

        protected SelectableDesignerItemViewModelBase(Guid id, IDiagramViewModel parent)
        : this(id)
        {
            this.Parent = parent;
        }

        #endregion Construstor

        #region Function

        public abstract PersistenceAbleItemBase SaveInfo();

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