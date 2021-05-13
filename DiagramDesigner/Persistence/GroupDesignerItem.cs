using System;
using System.Collections.Generic;
using System.Linq;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.Persistence
{
    public class GroupDesignerItem : DesignerItemBase, IDiagramItem
    {
        public List<PersistenceAbleItemBase> DesignerItems { get; private set; }
        public List<ConnectionInfo> Connections { get; private set; }

        public GroupDesignerItem(Guid id, double left, double top, double itemWidth, double itemHeight)
            : base(id, left, top, itemWidth, itemHeight)
        {
            Init();
        }

        public void Init()
        {
            this.DesignerItems = new List<PersistenceAbleItemBase>();
            this.Connections = new List<ConnectionInfo>();
        }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var vm = new GroupingDesignerItemViewModel(Id, parent, Left, Top, ItemWidth, ItemHeight);
            if (DesignerItems?.Any() == true)
            {
                OnLoadGroup(this, vm);
            }

            return vm;
        }


        private void OnLoadGroup(IDiagramItem diagram, IDiagramViewModel diagramVm)
        {
            foreach (var item in diagram.DesignerItems)
            {
                var info = item.LoadSaveInfo(diagramVm);

                if (info != null)
                {
                    diagramVm.ItemsSource.Add(info);
                }
            }
        }
    }
}