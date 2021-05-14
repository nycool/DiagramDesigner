using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.Interface;
using Prism.Commands;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class GroupDesignerItemViewModelBase : DesignerItemViewModelBase, IDiagramViewModel
    {
        #region Filed

        public ObservableCollection<SelectableDesignerItemViewModelBase> ItemsSource { get; private set; }

        public new List<SelectableDesignerItemViewModelBase> SelectedItems => ItemsSource.Where(x => x.IsSelected).ToList();

        #endregion

        #region Command

        public DelegateCommand<SelectableDesignerItemViewModelBase> AddItemCommand { get; private set; }
        public DelegateCommand<SelectableDesignerItemViewModelBase> RemoveItemCommand { get; private set; }
        public DelegateCommand<bool?> SelectedItemsCommand { get; private set; }
        public DelegateCommand GroupCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        #endregion


        #region Construstor

        protected GroupDesignerItemViewModelBase()
        {
            Init();
        }

        protected GroupDesignerItemViewModelBase(Guid id, IDiagramViewModel parent, DesignerItemPosition position)
            : base(id, parent, position)
        {
            Init();
        }

        #endregion


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
            GroupCommand = new DelegateCommand(OnGroup);
        }

        private void OnGroup()
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

            ItemsSource.Add(addItem);
        }

        private void OnRemove(SelectableDesignerItemViewModelBase removeItem)
        {

            if (removeItem == null)
            {
                return;
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
            ItemsSource.Clear();
        }

        #endregion
    }
}
