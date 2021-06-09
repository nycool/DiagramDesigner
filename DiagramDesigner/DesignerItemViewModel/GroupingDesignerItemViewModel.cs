using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class GroupingDesignerItemViewModel : GroupDesignerItemViewModelBase
    {
        #region Override

        protected override Type GetPersistenceItemType() => typeof(GroupDesignerItem);
        protected override void LoadUseData(IExternUserData userData)
        {
            throw new NotImplementedException();
        }

        public sealed override void LoadDesignerItemData(DesignerItemData data)
        {
            base.LoadDesignerItemData(data);
        }

        protected override IExternUserData GetExternUserData()
        {
            throw new NotImplementedException();
        }

        #endregion Override
    }
}