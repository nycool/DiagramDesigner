using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Persistence;
using WpfApp3.DesignerItem.Db;

namespace WpfApp3
{
    public class SettingsDesignerItemViewModel : DesignerItemViewModelBase
    {
        #region Filed


        #endregion Filed

        #region Construstor

        public SettingsDesignerItemViewModel()
        {
            Init();
        }

        #endregion Construstor

        private void Init()
        {
            this.ShowConnectors = false;
        }

        public override PersistenceAbleItemBase SaveInfo()
        {
            var data = new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight);

            var itemData = new DesignerItemData(Id, data,ExternUserData);

            var item = new SettingsDesignerItem(itemData);
            return item;
        }
    }
}