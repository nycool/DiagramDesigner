using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;

namespace WpfApp3.DesignerItem.Db
{
    public class PersistDesignerItem : DesignerItemBase
    {
        private DesignerItemData _designerItemData;

        public PersistDesignerItem(DesignerItemData data)
        {
            _designerItemData = data;
        }

        public override Type GetDesignerItemType() => typeof(PersistDesignerItemViewModel);

        public override DesignerItemData GetDesignerItemData() => _designerItemData;

    }
}
