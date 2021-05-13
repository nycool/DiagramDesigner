using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner.Persistence
{
    public class GroupDesignerItem : DesignerItemBase, IDiagram
    {
        public List<PersistenceAbleItemBase> DesignerAndConnectItems { get; private set; }

        public GroupDesignerItem(Guid id, double left, double top, double itemWidth, double itemHeight)
            : base(id, left, top, itemWidth, itemHeight)
        {
            DesignerAndConnectItems = new List<PersistenceAbleItemBase>();
        }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var vm = new GroupingDesignerItemViewModel(Id, parent, Left, Top, ItemWidth, ItemHeight);
            if (DesignerAndConnectItems?.Any() == true)
            {
                OnLoadGroup(this, vm);
            }

            return vm;
        }

        private void OnLoadGroup(IDiagram diagram, IDiagramViewModel diagramVm)
        {
            foreach (var item in diagram.DesignerAndConnectItems)
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