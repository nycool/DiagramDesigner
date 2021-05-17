using DiagramDesigner.BaseClass;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {
        #region Override

        protected override PersistenceAbleItemBase GetPersistenceItem() => new GroupDesignerItem();

        public sealed override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);
        }

        #endregion Override
    }
}