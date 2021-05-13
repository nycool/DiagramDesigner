using System;
using System.Collections.Generic;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.Persistence
{
    public class DiagramItem : PersistenceAbleItemBase, IDiagramItem
    {
        #region Filed

        public List<PersistenceAbleItemBase> DesignerItems { get; private set; }
        public List<ConnectionInfo> Connections { get; private set; }

        #endregion Filed

        public DiagramItem()
            : base(Guid.NewGuid())
        {
            Init();
        }

        public void Init()
        {
            this.DesignerItems = new List<PersistenceAbleItemBase>();
            this.Connections = new List<ConnectionInfo>();
        }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            throw new NotImplementedException();
        }
    }
}