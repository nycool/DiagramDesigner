using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using DiagramDesigner.Persistence;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp3
{
    public class Window1ViewModel : DiagramViewModelBase
    {
        private List<Guid> savedDiagrams = new List<Guid>();
        private Guid? savedDiagramId;
        private List<SelectableDesignerItemViewModelBase> itemsToRemove;

        //private IDatabaseAccessService databaseAccessService;
        private bool isBusy = false;

        public Window1ViewModel()
        {
            //databaseAccessService = ApplicationServicesProvider.Instance.Provider.DatabaseAccessService;

            //foreach (var savedDiagram in databaseAccessService.FetchAllDiagram())
            //{
            //    savedDiagrams.Add(savedDiagram.Id);
            //}

            DeleteSelectedItemsCommand = new DelegateCommand(ExecuteDeleteSelectedItemsCommand);
            SaveDiagramCommand = new DelegateCommand(ExecuteSaveDiagramCommand);
            LoadDiagramCommand = new DelegateCommand(ExecuteLoadDiagramCommand);

            SaveCommand = new DelegateCommand(OnSave);

            OpenCommand = new DelegateCommand(OnOpen);
            //OrthogonalPathFinder is a pretty bad attempt at finding path points, it just shows you, you can swap this out with relative
            //ease if you wish just create a new IPathFinder class and pass it in right here
            ConnectorViewModel.PathFinder = new OrthogonalPathFinder();

            DeleteCommand = new DelegateCommand(() =>
            {
                foreach (var selectedItem in SelectedItems)
                {
                    RemoveItemCommand.Execute(selectedItem);
                }


            });

            Init();
        }

        private IDiagram _vm;

        private void OnOpen()
        {
            this.ItemsSource.Clear();

            if (_vm == null)
            {
                return;
            }

            LoadPerstistDesignerItems(_vm, this);
        }

        private void LoadPerstistDesignerItems(IDiagram wholeDiagramToLoad, IDiagramViewModel diagramViewModel)
        {
            foreach (ILoad load in wholeDiagramToLoad.DesignerAndConnectItems)
            {
                var info = load.LoadSaveInfo(diagramViewModel);

                if (info != null)
                {
                    diagramViewModel.ItemsSource.Add(info);
                }
            }

#if false

            //load diagram items
            foreach (var diagramItemData in wholeDiagramToLoad.DesignerAndConnectItems)
            {
                if (diagramItemData.SaveInfo.GetType() == typeof(PersistDesignerItem))
                {
                    PersistDesignerItem persistedDesignerItem = diagramItemData.SaveInfo as PersistDesignerItem;
                    PersistDesignerItemViewModel persistDesignerItemViewModel =
                        new PersistDesignerItemViewModel(persistedDesignerItem.Id, diagramViewModel, persistedDesignerItem.Left, persistedDesignerItem.Top, persistedDesignerItem.ItemWidth, persistedDesignerItem.ItemHeight, persistedDesignerItem.HostUrl);
                    diagramViewModel.ItemsSource.Add(persistDesignerItemViewModel);
                }
                if (diagramItemData.SaveInfo.GetType() == typeof(SettingsDesignerItem))
                {
                    SettingsDesignerItem settingsDesignerItem = diagramItemData.SaveInfo as SettingsDesignerItem; ;
                    SettingsDesignerItemViewModel settingsDesignerItemViewModel =
                        new SettingsDesignerItemViewModel(settingsDesignerItem.Id, diagramViewModel, settingsDesignerItem.Left, settingsDesignerItem.Top, settingsDesignerItem.ItemWidth, settingsDesignerItem.ItemHeight, settingsDesignerItem.Setting1);
                    diagramViewModel.ItemsSource.Add(settingsDesignerItemViewModel);
                }
                if (diagramItemData.SaveInfo.GetType() == typeof(GroupDesignerItem))
                {
                    GroupDesignerItem groupDesignerItem = diagramItemData.SaveInfo as GroupDesignerItem;
                    GroupingDesignerItemViewModel groupingDesignerItemViewModel =
                        new GroupingDesignerItemViewModel(groupDesignerItem.Id, diagramViewModel, groupDesignerItem.Left, groupDesignerItem.Top, groupDesignerItem.ItemWidth, groupDesignerItem.ItemHeight);
                    if (groupDesignerItem.DesignerAndConnectItems != null && groupDesignerItem.DesignerAndConnectItems.Count > 0)
                    {
                        LoadPerstistDesignerItems(groupDesignerItem, groupingDesignerItemViewModel);
                    }
                    diagramViewModel.ItemsSource.Add(groupingDesignerItemViewModel);
                }
            }
            //load connection items
            foreach (var connection in wholeDiagramToLoad.Connections)
            {
                DesignerItemViewModelBase sourceItem = GetConnectorDataItem(diagramViewModel, connection.SourceId);
                ConnectorOrientation sourceConnectorOrientation = GetOrientationForConnector(connection.SourceOrientation);
                FullyCreatedConnectorInfo sourceConnectorInfo = GetFullConnectorInfo(connection.Id, sourceItem, sourceConnectorOrientation);

                DesignerItemViewModelBase sinkItem = GetConnectorDataItem(diagramViewModel, connection.SinkId);
                ConnectorOrientation sinkConnectorOrientation = GetOrientationForConnector(connection.SinkOrientation);
                FullyCreatedConnectorInfo sinkConnectorInfo = GetFullConnectorInfo(connection.Id, sinkItem, sinkConnectorOrientation);

                ConnectorViewModel connectionVM = new ConnectorViewModel(connection.Id, diagramViewModel, sourceConnectorInfo, sinkConnectorInfo);
                diagramViewModel.ItemsSource.Add(connectionVM);
            }

#endif
        }

        private DesignerItemViewModelBase GetConnectorDataItem(IDiagramViewModel diagramVm, Guid id)
        {
            return diagramVm.ItemsSource.FirstOrDefault(x => x is DesignerItemViewModelBase designerItem && designerItem.Id == id) as DesignerItemViewModelBase;
        }

        private void OnSave()
        {
            ExecuteSaveDiagramCommand1();
        }

        private void ExecuteSaveDiagramCommand1()
        {
            if (!ItemsSource.Any())
            {
                return;
            }

            var wholeDiagramToSave = new Diagram();

            //ensure that itemsToRemove is cleared ready for any new changes within a session
            itemsToRemove = new List<SelectableDesignerItemViewModelBase>();

            SavePersistDesignerItem(wholeDiagramToSave, this);
        }

        private void SavePersistDesignerItem(IDiagram wholeDiagramToSave, IDiagramViewModel diagramViewModel)
        {
            foreach (ISave save in diagramViewModel.ItemsSource)
            {
                var info = save.SaveInfo();

                if (info != null)
                {
                    wholeDiagramToSave.DesignerAndConnectItems.Add(info);
                }
            }

            ////Save all PersistDesignerItemViewModel
            //foreach (var persistItemVm in diagramViewModel.ItemsSource.OfType<PersistDesignerItemViewModel>())
            //{
            //    PersistDesignerItem persistDesignerItem = new PersistDesignerItem(persistItemVm.Id, persistItemVm.Left, persistItemVm.Top, persistItemVm.ItemWidth, persistItemVm.ItemHeight, persistItemVm.HostUrl);
            //    wholeDiagramToSave.DesignerAndConnectItems.Add(new DiagramItemInfo(persistDesignerItem));
            //}
            ////Save all SettingsDesignerItemViewModel
            //foreach (var settingsItemVm in diagramViewModel.ItemsSource.OfType<SettingsDesignerItemViewModel>())
            //{
            //    SettingsDesignerItem settingsDesignerItem = new SettingsDesignerItem(settingsItemVm.Id, settingsItemVm.Left, settingsItemVm.Top, settingsItemVm.ItemWidth, settingsItemVm.ItemHeight, settingsItemVm.Setting1);
            //    wholeDiagramToSave.DesignerAndConnectItems.Add(new DiagramItemInfo(settingsDesignerItem));
            //}
            ////Save all GroupingDesignerItemViewModel
            //foreach (var groupingItemVm in diagramViewModel.ItemsSource.OfType<GroupingDesignerItemViewModel>())
            //{
            //    GroupDesignerItem groupDesignerItem = new GroupDesignerItem(groupingItemVm.Id, groupingItemVm.Left, groupingItemVm.Top, groupingItemVm.ItemWidth, groupingItemVm.ItemHeight);
            //    if (groupingItemVm.ItemsSource != null && groupingItemVm.ItemsSource.Count > 0)
            //    {
            //        SavePersistDesignerItem(groupDesignerItem, groupingItemVm);
            //    }
            //    wholeDiagramToSave.DesignerAndConnectItems.Add(new DiagramItemInfo(groupDesignerItem));
            //}

            //Save all connections which should now have their Connection.DataItems filled in with correct Ids
            //foreach (var connectionVm in diagramViewModel.ItemsSource.OfType<ConnectorViewModel>())
            //{
            //    FullyCreatedConnectorInfo sinkConnector = connectionVm.SinkConnectorInfo as FullyCreatedConnectorInfo;

            //    Connection connectionInfo = new Connection(
            //        connectionVm.Id,
            //        connectionVm.SourceConnectorInfo.DesignerItem.Id,
            //        GetOrientationFromConnector(connectionVm.SourceConnectorInfo.Orientation),
            //        sinkConnector.DesignerItem.Id,
            //        GetOrientationFromConnector(sinkConnector.Orientation));

            //    wholeDiagramToSave.Connections.Add(connectionInfo);
            //}

            _vm = wholeDiagramToSave;
        }

        #region Filed

        private List<ToolBoxItemInfo> _toolBoxDatas;

        public List<ToolBoxItemInfo> ToolBoxItems
        {
            get => _toolBoxDatas;
            set => SetProperty(ref _toolBoxDatas, value);
        }

        #endregion Filed

        private void Init()
        {
            InitCollection();
        }

        private void InitCollection()
        {
            ToolBoxItems = new List<ToolBoxItemInfo>();

            string str = @"pack://application:,,,/WpfApp3;component/Images/Setting.png";

            var into = new ToolBoxItemInfo("tool", "tol", str, typeof(SettingsDesignerItemViewModel));
            ToolBoxItems.Add(into);

            str = @"pack://application:,,,/WpfApp3;component/Images/Persist.png";

            into = new ToolBoxItemInfo("tool", "tol", str, typeof(PersistDesignerItemViewModel));
            ToolBoxItems.Add(into);
        }

        #region Commamd

        public DelegateCommand DeleteSelectedItemsCommand { get; private set; }
        public DelegateCommand SaveDiagramCommand { get; private set; }
        public DelegateCommand LoadDiagramCommand { get; private set; }

        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        public DelegateCommand DeleteCommand { get; set; }

        #endregion Commamd

        #region Function

        private void ExecuteAddItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase)
            {
                SelectableDesignerItemViewModelBase item = (SelectableDesignerItemViewModelBase)parameter;
                item.Parent = this;
                ItemsSource.Add(item);
            }
        }

        private void ExecuteRemoveItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase)
            {
                SelectableDesignerItemViewModelBase item = (SelectableDesignerItemViewModelBase)parameter;
                ItemsSource.Remove(item);
            }
        }

        private void ExecuteClearSelectedItemsCommand()
        {
            foreach (SelectableDesignerItemViewModelBase item in ItemsSource)
            {
                item.IsSelected = false;
            }
        }

        #endregion Function

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public List<Guid> SavedDiagrams
        {
            get => savedDiagrams;
            set => SetProperty(ref savedDiagrams, value);
        }

        public Guid? SavedDiagramId
        {
            get => savedDiagramId;
            set => SetProperty(ref savedDiagramId, value);
        }

        private void ExecuteDeleteSelectedItemsCommand()
        {
            itemsToRemove = SelectedItems;
            List<SelectableDesignerItemViewModelBase> connectionsToAlsoRemove = new List<SelectableDesignerItemViewModelBase>();

            foreach (var connector in ItemsSource.OfType<ConnectorViewModel>())
            {
                if (ItemsToDeleteHasConnector(itemsToRemove, connector.SourceConnectorInfo))
                {
                    connectionsToAlsoRemove.Add(connector);
                }

                if (ItemsToDeleteHasConnector(itemsToRemove, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                {
                    connectionsToAlsoRemove.Add(connector);
                }
            }
            itemsToRemove.AddRange(connectionsToAlsoRemove);
            foreach (var selectedItem in itemsToRemove)
            {
                RemoveItemCommand.Execute(selectedItem);
            }
        }

        private void ExecuteCreateNewDiagramCommand()
        {
            //ensure that itemsToRemove is cleared ready for any new changes within a session
            itemsToRemove = new List<SelectableDesignerItemViewModelBase>();
            SavedDiagramId = null;
            ClearCommand.Execute();
        }

        private void ExecuteSaveDiagramCommand()
        {
            //if (!ItemsSource.Any())
            //{
            //    return;
            //}

            //IsBusy = true;
            //Diagram wholeDiagramToSave = null;

            //Task<Guid> task = Task.Factory.StartNew<Guid>(() =>
            //{
            //    if (SavedDiagramId != null)
            //    {
            //        var currentSavedDiagramId = SavedDiagramId.Value;
            //        wholeDiagramToSave = databaseAccessService.FetchDiagram(currentSavedDiagramId);

            //        //If we have a saved diagram, we need to make sure we clear out all the removed items that
            //        //the user deleted as part of this work sesssion
            //        foreach (var itemToRemove in itemsToRemove)
            //        {
            //            DeleteFromDatabase(wholeDiagramToSave, itemToRemove);
            //        }
            //        //start with empty collections of connections and items, which will be populated based on current diagram
            //        wholeDiagramToSave.Init();
            //    }
            //    else
            //    {
            //        wholeDiagramToSave = new Diagram();
            //    }

            //    //ensure that itemsToRemove is cleared ready for any new changes within a session
            //    itemsToRemove = new List<SelectableDesignerItemViewModelBase>();

            //    SavePersistDesignerItem(wholeDiagramToSave, this);

            //    wholeDiagramToSave.Id = databaseAccessService.SaveDiagram(wholeDiagramToSave);
            //    return wholeDiagramToSave.Id;
            //});
            //task.ContinueWith((ant) =>
            //{
            //    var wholeDiagramToSaveId = ant.Result;
            //    if (!savedDiagrams.Contains(wholeDiagramToSaveId))
            //    {
            //        List<Guid> newDiagrams = new List<Guid>(savedDiagrams);
            //        newDiagrams.Add(wholeDiagramToSaveId);
            //        SavedDiagrams = newDiagrams;
            //    }
            //    IsBusy = false;
            //}, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        //private void SavePersistDesignerItem(IDiagram wholeDiagramToSave, IDiagramViewModel diagramViewModel)
        //{
        //    ////Save all PersistDesignerItemViewModel
        //    //foreach (var persistItemVM in diagramViewModel.ItemsSource.OfType<PersistDesignerItemViewModel>())
        //    //{
        //    //    PersistDesignerItem persistDesignerItem = new PersistDesignerItem(persistItemVM.Id, persistItemVM.Left, persistItemVM.Top, persistItemVM.ItemWidth, persistItemVM.ItemHeight, persistItemVM.HostUrl);
        //    //    persistItemVM.Id = databaseAccessService.SavePersistDesignerItem(persistDesignerItem);
        //    //    wholeDiagramToSave.DesignerAndConnectItems.Add(new DiagramItemInfo(persistDesignerItem.Id, typeof(PersistDesignerItem)));
        //    //}
        //    ////Save all SettingsDesignerItemViewModel
        //    //foreach (var settingsItemVM in diagramViewModel.ItemsSource.OfType<SettingsDesignerItemViewModel>())
        //    //{
        //    //    SettingsDesignerItem settingsDesignerItem = new SettingsDesignerItem(settingsItemVM.Id, settingsItemVM.Left, settingsItemVM.Top, settingsItemVM.ItemWidth, settingsItemVM.ItemHeight, settingsItemVM.Setting1);
        //    //    settingsItemVM.Id = databaseAccessService.SaveSettingDesignerItem(settingsDesignerItem);
        //    //    wholeDiagramToSave.DesignerAndConnectItems.Add(new DiagramItemInfo(settingsDesignerItem.Id, typeof(SettingsDesignerItem)));
        //    //}
        //    ////Save all GroupingDesignerItemViewModel
        //    //foreach (var groupingItemVM in diagramViewModel.ItemsSource.OfType<GroupingDesignerItemViewModel>())
        //    //{
        //    //    GroupDesignerItem groupDesignerItem = new GroupDesignerItem(groupingItemVM.Id, groupingItemVM.Left, groupingItemVM.Top, groupingItemVM.ItemWidth, groupingItemVM.ItemHeight);
        //    //    if (groupingItemVM.ItemsSource != null && groupingItemVM.ItemsSource.Count > 0)
        //    //    {
        //    //        SavePersistDesignerItem(groupDesignerItem, groupingItemVM);
        //    //    }
        //    //    groupingItemVM.Id = databaseAccessService.SaveGroupingDesignerItem(groupDesignerItem);
        //    //    wholeDiagramToSave.DesignerAndConnectItems.Add(new DiagramItemInfo(groupDesignerItem.Id, typeof(GroupDesignerItem)));
        //    //}
        //    //Save all connections which should now have their Connection.DataItems filled in with correct Ids
        //    foreach (var connectionVM in diagramViewModel.ItemsSource.OfType<ConnectorViewModel>())
        //    {
        //        FullyCreatedConnectorInfo sinkConnector = connectionVM.SinkConnectorInfo as FullyCreatedConnectorInfo;

        //        //Connection connection = new Connection(
        //        //    connectionVM.Id,
        //        //    connectionVM.SourceConnectorInfo.DesignerItem.Id,
        //        //    GetOrientationFromConnector(connectionVM.SourceConnectorInfo.Orientation),
        //        //    GetTypeOfDiagramItem(connectionVM.SourceConnectorInfo.DesignerItem),
        //        //    sinkConnector.DesignerItem.Id,
        //        //    GetOrientationFromConnector(sinkConnector.Orientation),
        //        //    GetTypeOfDiagramItem(sinkConnector.DesignerItem));

        //        //connectionVM.Id = databaseAccessService.SaveConnection(connection);
        //        wholeDiagramToSave.Connections.Add(connectionVM.Id);
        //    }
        //}

        private void ExecuteLoadDiagramCommand()
        {
            //IsBusy = true;
            //Diagram wholeDiagramToLoad = null;
            //if (SavedDiagramId == null)
            //{
            //    return;
            //}

            //Task<DiagramViewModel> task = Task.Factory.StartNew<DiagramViewModel>(() =>
            //{
            //    //ensure that itemsToRemove is cleared ready for any new changes within a session
            //    itemsToRemove = new List<SelectableDesignerItemViewModelBase>();
            //    DiagramViewModel diagramViewModel = new DiagramViewModel();

            //    wholeDiagramToLoad = databaseAccessService.FetchDiagram(SavedDiagramId.Value);

            //    LoadPerstistDesignerItems(wholeDiagramToLoad, diagramViewModel);

            //    return diagramViewModel;
            //});
            //task.ContinueWith((ant) =>
            //{
            //    this.ItemsSource = ant.Result.ItemsSource;
            //    IsBusy = false;

            //}, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        //private void LoadPerstistDesignerItems(IDiagram wholeDiagramToLoad, IDiagramViewModel diagramViewModel)
        //{
        //    //load diagram items
        //    //foreach (DiagramItemInfo diagramItemData in wholeDiagramToLoad.DesignerAndConnectItems)
        //    //{
        //    //    if (diagramItemData.ItemType == typeof(PersistDesignerItem))
        //    //    {
        //    //        PersistDesignerItem persistedDesignerItem = databaseAccessService.FetchPersistDesignerItem(diagramItemData.ItemId);
        //    //        PersistDesignerItemViewModel persistDesignerItemViewModel =
        //    //            new PersistDesignerItemViewModel(persistedDesignerItem.Id, diagramViewModel, persistedDesignerItem.Left, persistedDesignerItem.Top, persistedDesignerItem.ItemWidth, persistedDesignerItem.ItemHeight, persistedDesignerItem.HostUrl);
        //    //        diagramViewModel.ItemsSource.Add(persistDesignerItemViewModel);
        //    //    }
        //    //    if (diagramItemData.ItemType == typeof(SettingsDesignerItem))
        //    //    {
        //    //        SettingsDesignerItem settingsDesignerItem = databaseAccessService.FetchSettingsDesignerItem(diagramItemData.ItemId);
        //    //        SettingsDesignerItemViewModel settingsDesignerItemViewModel =
        //    //            new SettingsDesignerItemViewModel(settingsDesignerItem.Id, diagramViewModel, settingsDesignerItem.Left, settingsDesignerItem.Top, settingsDesignerItem.ItemWidth, settingsDesignerItem.ItemHeight, settingsDesignerItem.Setting1);
        //    //        diagramViewModel.ItemsSource.Add(settingsDesignerItemViewModel);
        //    //    }
        //    //    if (diagramItemData.ItemType == typeof(GroupDesignerItem))
        //    //    {
        //    //        GroupDesignerItem groupDesignerItem = databaseAccessService.FetchGroupingDesignerItem(diagramItemData.ItemId);
        //    //        GroupingDesignerItemViewModel groupingDesignerItemViewModel =
        //    //            new GroupingDesignerItemViewModel(groupDesignerItem.Id, diagramViewModel, groupDesignerItem.Left, groupDesignerItem.Top, groupDesignerItem.ItemWidth, groupDesignerItem.ItemHeight);
        //    //        if (groupDesignerItem.DesignerAndConnectItems != null && groupDesignerItem.DesignerAndConnectItems.Count > 0)
        //    //        {
        //    //            LoadPerstistDesignerItems(groupDesignerItem, groupingDesignerItemViewModel);
        //    //        }
        //    //        diagramViewModel.ItemsSource.Add(groupingDesignerItemViewModel);
        //    //    }
        //    //}
        //    //load connection items
        //    foreach (Guid connectionId in wholeDiagramToLoad.Connections)
        //    {
        //        //Connection connection = databaseAccessService.FetchConnection(connectionId);

        //        //DesignerItemViewModelBase sourceItem = GetConnectorDataItem(diagramViewModel, connection.SourceId, connection.SourceType);
        //        //ConnectorOrientation sourceConnectorOrientation = GetOrientationForConnector(connection.SourceOrientation);
        //        //FullyCreatedConnectorInfo sourceConnectorInfo = GetFullConnectorInfo(connection.Id, sourceItem, sourceConnectorOrientation);

        //        //DesignerItemViewModelBase sinkItem = GetConnectorDataItem(diagramViewModel, connection.SinkId, connection.SinkType);
        //        //ConnectorOrientation sinkConnectorOrientation = GetOrientationForConnector(connection.SinkOrientation);
        //        //FullyCreatedConnectorInfo sinkConnectorInfo = GetFullConnectorInfo(connection.Id, sinkItem, sinkConnectorOrientation);

        //        //ConnectorViewModel connectionVM = new ConnectorViewModel(connection.Id, diagramViewModel, sourceConnectorInfo, sinkConnectorInfo);
        //        //diagramViewModel.ItemsSource.Add(connectionVM);
        //    }
        //}

#if false

        private void ExecuteGroupCommand()
        {
            if (SelectedItems.Count > 0)
            {
                // if only one selected item is a Grouping item -> ungroup
                if (SelectedItems[0] is GroupingDesignerItemViewModel && SelectedItems.Count == 1)
                {
                    GroupingDesignerItemViewModel groupObject = SelectedItems[0] as GroupingDesignerItemViewModel;
                    foreach (var item in groupObject.ItemsSource)
                    {
                        if (item is DesignerItemViewModelBase)
                        {
                            DesignerItemViewModelBase tmp = (DesignerItemViewModelBase)item;
                            tmp.Top += groupObject.Top;
                            tmp.Left += groupObject.Left;
                        }
                        AddItemCommand.Execute(item);
                        item.Parent = this;
                    }

                    // "cut" connections between DiagramItems and the Group
                    List<SelectableDesignerItemViewModelBase> GroupedItemsToRemove = new List<SelectableDesignerItemViewModelBase>();
                    foreach (var connector in ItemsSource.OfType<ConnectorViewModel>())
                    {
                        if (groupObject == connector.SourceConnectorInfo.DesignerItem)
                        {
                            GroupedItemsToRemove.Add(connector);
                        }

                        if (groupObject == ((FullyCreatedConnectorInfo)connector.SinkConnectorInfo).DesignerItem)
                        {
                            GroupedItemsToRemove.Add(connector);
                        }
                    }
                    GroupedItemsToRemove.Add(groupObject);
                    foreach (var selectedItem in GroupedItemsToRemove)
                    {
                        RemoveItemCommand.Execute(selectedItem);
                    }
                }
                else if (SelectedItems.Count > 1)
                {
                    double margin = 15;
                    Rect rekt = GetBoundingRectangle(SelectedItems, margin);

                    GroupingDesignerItemViewModel groupItem = new GroupingDesignerItemViewModel(Guid.NewGuid(), this, rekt.Left, rekt.Top);
                    groupItem.ItemWidth = rekt.Width;
                    groupItem.ItemHeight = rekt.Height;
                    foreach (var item in SelectedItems)
                    {
                        if (item is DesignerItemViewModelBase)
                        {
                            DesignerItemViewModelBase tmp = (DesignerItemViewModelBase)item;
                            tmp.Top -= rekt.Top;
                            tmp.Left -= rekt.Left;
                        }
                        groupItem.ItemsSource.Add(item);
                        item.Parent = groupItem;
                    }

                    // "cut" connections between DiagramItems which are going to be grouped and
                    // Diagramitems which are not going to be grouped
                    List<SelectableDesignerItemViewModelBase> GroupedItemsToRemove = SelectedItems;
                    List<SelectableDesignerItemViewModelBase> connectionsToAlsoRemove = new List<SelectableDesignerItemViewModelBase>();

                    foreach (var connector in ItemsSource.OfType<ConnectorViewModel>())
                    {
                        if (ItemsToDeleteHasConnector(GroupedItemsToRemove, connector.SourceConnectorInfo))
                        {
                            connectionsToAlsoRemove.Add(connector);
                        }

                        if (ItemsToDeleteHasConnector(GroupedItemsToRemove, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                        {
                            connectionsToAlsoRemove.Add(connector);
                        }
                    }
                    GroupedItemsToRemove.AddRange(connectionsToAlsoRemove);
                    foreach (var selectedItem in GroupedItemsToRemove)
                    {
                        RemoveItemCommand.Execute(selectedItem);
                    }

                    SelectedItems.Clear();
                    ItemsSource.Add(groupItem);
                }
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<SelectableDesignerItemViewModelBase> items, double margin)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (DesignerItemViewModelBase item in items.OfType<DesignerItemViewModelBase>())
            {
                x1 = Math.Min(item.Left - margin, x1);
                y1 = Math.Min(item.Top - margin, y1);

                x2 = Math.Max(item.Left + item.ItemWidth + margin, x2);
                y2 = Math.Max(item.Top + item.ItemHeight + margin, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }
#endif

        private FullyCreatedConnectorInfo GetFullConnectorInfo(Guid connectorId, DesignerItemViewModelBase dataItem, ConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Top:
                    return dataItem.TopConnector;

                case ConnectorOrientation.Left:
                    return dataItem.LeftConnector;

                case ConnectorOrientation.Right:
                    return dataItem.RightConnector;

                case ConnectorOrientation.Bottom:
                    return dataItem.BottomConnector;

                default:
                    throw new InvalidOperationException(
                        string.Format("Found invalid persisted Connector Orientation for Connector Id: {0}", connectorId));
            }
        }

        //private Type GetTypeOfDiagramItem(DesignerItemViewModelBase vmType)
        //{
        //    if (vmType is PersistDesignerItemViewModel)
        //        return typeof(PersistDesignerItem);
        //    if (vmType is SettingsDesignerItemViewModel)
        //        return typeof(SettingsDesignerItem);
        //    if (vmType is GroupingDesignerItemViewModel)
        //        return typeof(GroupDesignerItem);

        //    throw new InvalidOperationException(string.Format("Unknown diagram type. Currently only {0} and {1} are supported",
        //        typeof(PersistDesignerItem).AssemblyQualifiedName,
        //        typeof(SettingsDesignerItemViewModel).AssemblyQualifiedName
        //        ));
        //}

        //private DesignerItemViewModelBase GetConnectorDataItem(IDiagramViewModel diagramViewModel, Guid conectorDataItemId, Type connectorDataItemType)
        //{
        //    DesignerItemViewModelBase dataItem = null;

        //    if (connectorDataItemType == typeof(PersistDesignerItem))
        //    {
        //        dataItem = diagramViewModel.ItemsSource.OfType<PersistDesignerItemViewModel>().Single(x => x.Id == conectorDataItemId);
        //    }

        //    if (connectorDataItemType == typeof(SettingsDesignerItem))
        //    {
        //        dataItem = diagramViewModel.ItemsSource.OfType<SettingsDesignerItemViewModel>().Single(x => x.Id == conectorDataItemId);
        //    }
        //    //if (connectorDataItemType == typeof(GroupDesignerItem))
        //    //{
        //    //    dataItem = diagramViewModel.ItemsSource.OfType<GroupingDesignerItemViewModel>().Single(x => x.Id == conectorDataItemId);
        //    //}
        //    return dataItem;
        //}

        private Orientation GetOrientationFromConnector(ConnectorOrientation connectorOrientation)
        {
            Orientation result = Orientation.None;
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Bottom:
                    result = Orientation.Bottom;
                    break;

                case ConnectorOrientation.Left:
                    result = Orientation.Left;
                    break;

                case ConnectorOrientation.Top:
                    result = Orientation.Top;
                    break;

                case ConnectorOrientation.Right:
                    result = Orientation.Right;
                    break;
            }
            return result;
        }

        private ConnectorOrientation GetOrientationForConnector(Orientation persistedOrientation)
        {
            ConnectorOrientation result = ConnectorOrientation.None;
            switch (persistedOrientation)
            {
                case Orientation.Bottom:
                    result = ConnectorOrientation.Bottom;
                    break;

                case Orientation.Left:
                    result = ConnectorOrientation.Left;
                    break;

                case Orientation.Top:
                    result = ConnectorOrientation.Top;
                    break;

                case Orientation.Right:
                    result = ConnectorOrientation.Right;
                    break;
            }
            return result;
        }

        private bool ItemsToDeleteHasConnector(IList<SelectableDesignerItemViewModelBase> itemsToRemove, FullyCreatedConnectorInfo connector)
        {
            return itemsToRemove.Contains(connector.DesignerItem);
        }

        //private void DeleteFromDatabase(Diagram wholeDiagramToAdjust, SelectableDesignerItemViewModelBase itemToDelete)
        //{
        //    //make sure the item is removes from Diagram as well as removing them as individual items from database
        //    if (itemToDelete is PersistDesignerItemViewModel)
        //    {
        //        DiagramItemInfo diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerAndConnectItems.Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(PersistDesignerItem)).Single();
        //        wholeDiagramToAdjust.DesignerAndConnectItems.Remove(diagramItemToRemoveFromParent);
        //        databaseAccessService.DeletePersistDesignerItem(itemToDelete.Id);
        //    }
        //    if (itemToDelete is SettingsDesignerItemViewModel)
        //    {
        //        DiagramItemInfo diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerAndConnectItems.Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(SettingsDesignerItem)).Single();
        //        wholeDiagramToAdjust.DesignerAndConnectItems.Remove(diagramItemToRemoveFromParent);
        //        databaseAccessService.DeleteSettingDesignerItem(itemToDelete.Id);
        //    }
        //    if (itemToDelete is ConnectorViewModel)
        //    {
        //        wholeDiagramToAdjust.Connections.Remove(itemToDelete.Id);
        //        databaseAccessService.DeleteConnection(itemToDelete.Id);
        //    }

        //    databaseAccessService.SaveDiagram(wholeDiagramToAdjust);
        //}
    }
}