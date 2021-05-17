using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Persistence;
using WpfApp3.DesignerItem.Db;

namespace WpfApp3
{
    public class PersistDesignerItemViewModel : DesignerItemViewModelBase
    {
        #region Construsor

        public PersistDesignerItemViewModel()
        {
            Init();
        }

        #endregion Construsor

        protected override Type GetPersistenceItemType() => typeof(PersistDesignerItem);

        private void Init()
        {
            this.ShowConnectors = false;
        }

      

        
    }
}