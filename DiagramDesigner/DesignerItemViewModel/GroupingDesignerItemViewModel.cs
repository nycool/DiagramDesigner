using DiagramDesigner.BaseClass;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using System.Linq;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {
        #region Construstor

        public GroupingDesignerItemViewModel(DesignerItemData data)
        {
            LoadDesignerItemData(data);
        }

        public sealed override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);
        }

        #endregion Construstor

        public override PersistenceAbleItemBase SaveInfo()
        {
            var data = new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight);

            var groupItem = new GroupDesignerItem(new DesignerItemData(Id, data, ExternUserData));

            if (ItemsSource?.Any() == true)
            {
                SaveGroup(this);
            }

            return groupItem;
        }

        private void SaveGroup(IDiagramViewModel diagramVm)
        {
            foreach (var items in diagramVm.ItemsSource)
            {
                var item = items.SaveInfo();

                if (item != null)
                {
                    diagramVm.ItemsSource.Add(items);
                }
            }
        }
    }
}