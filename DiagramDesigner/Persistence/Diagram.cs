using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;
using System;
using System.Collections.Generic;

namespace DiagramDesigner.Persistence
{
    public class Diagram : PersistenceAbleItemBase, IDiagram
    {
        #region Filed

        public List<PersistenceAbleItemBase> DesignerAndConnectItems { get; private set; }

        #endregion Filed

        public Diagram()
            : base(Guid.NewGuid())
        {
            DesignerAndConnectItems = new List<PersistenceAbleItemBase>();
        }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            throw new NotImplementedException();
        }
    }
}