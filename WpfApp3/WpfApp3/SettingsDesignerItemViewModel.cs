using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Persistence;
using WpfApp3.DesignerItem.Db;

namespace WpfApp3
{
    public class SettingsDesignerItemViewModel : DesignerItemViewModelBase
    {
        #region Construstor

        public SettingsDesignerItemViewModel()
        {
            Init();
        }

        #endregion Construstor

        protected override Type GetPersistenceItemType() => typeof(SettingsDesignerItem);
        protected override ExternUserDataBase GetExternUserData()
        {
            return null;
        }

        private void Init()
        {
            this.ShowConnectors = false;
        }

        

       
    }
}