using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using System;
using DiagramDesigner.BaseClass;
using WpfApp3.DesignerItem.Db;

namespace WpfApp3
{
    public class PersistDesignerItemViewModel : DesignerItemViewModelBase
    {
        #region Filed


        #endregion Filed

        #region Construsor
        
        public PersistDesignerItemViewModel()
        {
            Init();
        }

        #endregion Construsor

        private void Init()
        {
            this.ShowConnectors = false;
        }

        public override PersistenceAbleItemBase SaveInfo()
        {
            var data = new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight);

            var itemData = new DesignerItemData(Id, data, ExternUserData);

            var item = new PersistDesignerItem(itemData);

            return item;
        }
    }
}