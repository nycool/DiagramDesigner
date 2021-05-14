using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using System;
using System.Collections.Generic;
using DiagramDesigner.Interface;

namespace DiagramDesigner.Persistence
{
    public class Diagram : PersistenceAbleItemBase, IDiagram
    {
        #region Filed

        public List<PersistenceAbleItemBase> DesignerAndConnectItems { get; set; }

        #endregion Filed

        #region Construstor

        public Diagram()
        : base(Guid.NewGuid())
        {
            Init();
        }

        #endregion Construstor

        #region Function

        private void Init()
        {
            DesignerAndConnectItems = new List<PersistenceAbleItemBase>();
        }

        #region Override

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            throw new NotImplementedException();
        }

        #endregion Override

        #endregion Function
    }
}