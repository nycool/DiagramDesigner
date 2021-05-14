using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using System;
using System.Collections.Generic;
using DiagramDesigner.Interface;

namespace DiagramDesigner.Persistence
{
    public class GroupDesignerItem : DesignerItemBase, IDiagram
    {
        #region Filed

        public List<PersistenceAbleItemBase> DesignerAndConnectItems { get; set; }


        private readonly DesignerItemData _designerItemData;

        #endregion Filed

        #region Construstor

        public GroupDesignerItem()
        {
            DesignerAndConnectItems = new List<PersistenceAbleItemBase>();
        }

        public GroupDesignerItem(DesignerItemData data)
            : this()
        {
            InitPosition(data.Position);
            _designerItemData = data;
        }

        #endregion Construstor

        #region Function

        #region Override


        public override Type GetDesignerItemType() => typeof(GroupingDesignerItemViewModel);

        public override DesignerItemData GetDesignerItemData() => _designerItemData;

        #endregion Override

        #endregion Function
    }
}