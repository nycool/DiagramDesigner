using DiagramDesigner.Persistence;
using System;

namespace WpfApp3.DesignerItem.Db
{
    public class PersistDesignerItem : DesignerItemBase
    {
        protected override Type GetDesignerItemViewModelType() => typeof(PersistDesignerItemViewModel);
    }
}