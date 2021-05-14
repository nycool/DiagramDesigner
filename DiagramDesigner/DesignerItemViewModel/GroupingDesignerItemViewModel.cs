using System;
using System.Linq;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.DesignerItemViewModel
{

 
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {

        #region Construstor

        public GroupingDesignerItemViewModel()
        {

        }

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, DesignerItemPosition position)
            : this()
        {
            Id = id;
            Parent = parent;
            InitPosition(position);
        }


        #endregion


        public override PersistenceAbleItemBase SaveInfo()
        {
            var groupItem = new GroupDesignerItem(Id, Left, Top, ItemWidth, ItemHeight);

            if (ItemsSource?.Any() == true)
            {
                SaveGroup(groupItem, this);
            }

            //return new DiagramItemInfo(Id,groupItem);

            return groupItem;
        }


        private void SaveGroup(IDiagram diagram, IDiagramViewModel diagramVm)
        {
            foreach (var items in diagramVm.ItemsSource)
            {
                var item = items.SaveInfo();

                if (item!=null)
                {
                    diagramVm.ItemsSource.Add(items);
                }
            }
        }
    }
}
