using DiagramDesigner.Interface;
using DiagramDesigner.Persistence.ExternUserData;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class GroupDesignerItemViewModelBase : DesignerItemViewModelBase, IDiagramViewModel
    {
        #region Filed

        public new List<SelectableDesignerItemViewModelBase> SelectedItems =>
            ItemsSource.Where(x => x.IsSelected).ToList();

        private ObservableCollection<SelectableDesignerItemViewModelBase> _itemSource;

        public ObservableCollection<SelectableDesignerItemViewModelBase> ItemsSource
        {
            get => _itemSource;
            set => SetProperty(ref _itemSource, value);
        }

        private bool _isExpended = true;

        public bool IsExpended
        {
            get => _isExpended;
            set => SetProperty(ref _isExpended, value);
        }

        private double ExpendWidth { get; set; }
        private double ExpendHeight { get; set; }

        #endregion Filed

        #region Command

        public DelegateCommand<SelectableDesignerItemViewModelBase> AddItemCommand { get; private set; }
        public DelegateCommand<SelectableDesignerItemViewModelBase> RemoveItemCommand { get; private set; }
        public DelegateCommand<bool?> SelectedItemsCommand { get; private set; }
        public DelegateCommand GroupCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }

        public DelegateCommand ExpandedCommand { get; private set; }
        public DelegateCommand CollapsedCommand { get; private set; }
        public DelegateCommand LoadedCommand { get; private set; }

        #endregion Command

        #region Construstor

        protected GroupDesignerItemViewModelBase()
        {
            Init();
        }

        #endregion Construstor

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
            ExpandedCommand = new DelegateCommand(OnExpanded);
            CollapsedCommand = new DelegateCommand(OnCollapsed);
            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        protected override void LoadUseData(IUserData userData)
        {
            if (userData is GroupData data)
            {
                ExpendHeight = data.ExpendHeight;

                ExpendWidth = data.ExpendWidth;

                IsExpended = data.IsExpended;
            }
        }

        protected override IUserData GetExternUserData()
        {
            ExternUserData ??= new GroupData();

            if (ExternUserData is GroupData data)
            {
                data.ExpendHeight = ExpendHeight;
                data.ExpendWidth = ExpendWidth;
                data.IsExpended = IsExpended;
            }

            return ExternUserData;
        }

        private void OnLoaded()
        {
            if (IsExpended)
            {
                ExpendHeight = ItemHeight;

                ExpendWidth = ItemWidth;
            }
        }

        private void OnCollapsed()
        {
            ItemHeight = 120;

            ItemHeight = 45;
        }

        private void OnExpanded()
        {
            ItemHeight = ExpendHeight;

            ItemWidth = ExpendWidth;
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

        #endregion Function
    }
}