using DiagramDesigner.BaseClass;
using DiagramDesigner.Persistence;
using System;

namespace WpfApp3.DesignerItem.Db
{
    public class SettingsDesignerItem : DesignerItemBase
    {
        private DesignerItemData _designerItemData;

        public SettingsDesignerItem(DesignerItemData data)
        {
            _designerItemData = data;
        }

        public override Type GetDesignerItemType() => typeof(SettingsDesignerItemViewModel);

        public override DesignerItemData GetDesignerItemData() => _designerItemData;
    }
}