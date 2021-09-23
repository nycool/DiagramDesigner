using DiagramDesigner.Persistence;
using System;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {
        #region Override

        protected override Type GetPersistenceItemType() => typeof(GroupDesignerItem);

        #endregion Override
    }
}