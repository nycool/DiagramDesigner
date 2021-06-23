using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System;
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