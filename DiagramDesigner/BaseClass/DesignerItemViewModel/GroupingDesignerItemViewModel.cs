using System;
using System.Linq;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.BaseClass.DesignerItemViewModel
{

    /// <summary>
    /// 类结构多继承设计的有点问题
    /// </summary>
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {

        #region Construstor

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top)
            : base(id, parent, left, top)
        {

        }

        public GroupingDesignerItemViewModel()
        {

        }

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight)
            : base(id, parent, left, top, itemWidth, itemHeight)
        {

        }


        #endregion


        public override PersistenceAbleItemBase SaveInfo()
        {
            var groupItem = new GroupDesignerItem(Id, Left, Top, ItemWidth, ItemHeight);

            if (ItemsSource?.Any() == true)
            {
                SaveGroup(groupItem, this);
            }

            return new DiagramItemInfo(Id,groupItem);
        }


        private void SaveGroup(IDiagramItem diagram, IDiagramViewModel diagramVm)
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
