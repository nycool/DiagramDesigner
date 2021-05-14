using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;
using System;
using DiagramDesigner.BaseClass;
using WpfApp3.DesignerItem.Db;

namespace WpfApp3
{
    public class SettingsDesignerItemViewModel : DesignerItemViewModelBase
    {
        #region Filed

        public String Setting1 { get; set; }

        #endregion Filed

        #region Construstor

        public SettingsDesignerItemViewModel(Guid id, IDiagramViewModel parent, DesignerItemPosition position, string setting1)
             : this()
        {
            Id = id;
            Parent = parent;
            InitPosition(position);
            this.Setting1 = setting1;
        }

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
            var item = new SettingsDesignerItem(Id, Left, Top, ItemWidth, ItemHeight, Setting1);

            //return new DiagramItemInfo(Id, item);
            return item;
        }
    }
}