using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
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
        protected override void LoadUseData(IExternUserData userData)
        {
            
        }

        protected override IExternUserData GetExternUserData()
        {
            return default;
        }


        private void Init()
        {
            this.ShowConnectors = false;
        }

        

       
    }
}