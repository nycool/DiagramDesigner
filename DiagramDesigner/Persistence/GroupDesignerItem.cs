using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System;
using System.Collections.Generic;

namespace DiagramDesigner.Persistence
{
    public class GroupDesignerItem : DesignerItemBase, IDiagram
    {
        #region Filed

        public List<PersistenceAbleItemBase> DesignerAndConnectItems { get; set; }

        #endregion Filed

        #region Construstor

        public GroupDesignerItem()
        {
            DesignerAndConnectItems = new List<PersistenceAbleItemBase>();
        }

        #endregion Construstor

        #region Override

        protected override Type GetDesignerItemType() => typeof(GroupingDesignerItemViewModel);

        #endregion Override
    }
}