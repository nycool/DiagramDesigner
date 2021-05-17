using DiagramDesigner.Persistence;
using System;

namespace WpfApp3.DesignerItem.Db
{
    public class SettingsDesignerItem : DesignerItemBase
    {
        protected override Type GetDesignerItemViewModelType() => typeof(SettingsDesignerItemViewModel);
    }
}