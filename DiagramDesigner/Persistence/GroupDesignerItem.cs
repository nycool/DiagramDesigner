using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using DiagramDesigner.BaseClass;
using DiagramDesigner.Interface;

namespace DiagramDesigner.Persistence
{
    public class GroupDesignerItem : DesignerItemBase, IDiagram
    {
        #region Filed

        public List<PersistenceAbleItemBase> DesignerAndConnectItems { get; set; }

        #endregion Filed

        #region Construstor

        public GroupDesignerItem()
        {
            
        }

        public GroupDesignerItem(Guid id):
            base(id)
        {
            
        }


        public GroupDesignerItem(Guid id, double left, double top, double itemWidth, double itemHeight)
            : this(id)
        {
            Init(left, top, itemWidth, itemHeight);
        }

        #endregion Construstor

        #region Function

        #region Override

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var vm = new GroupingDesignerItemViewModel(Id, parent, new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight));
            if (DesignerAndConnectItems?.Any() == true)
            {
                OnLoadGroup(this, vm);
            }

            return vm;
        }


        protected sealed override void Init(double left, double top, double itemWidth, double itemHeight)
        {
            base.Init(left, top, itemWidth, itemHeight);
            DesignerAndConnectItems = new List<PersistenceAbleItemBase>();
        }

        #endregion Override

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

        #endregion Function
    }
}