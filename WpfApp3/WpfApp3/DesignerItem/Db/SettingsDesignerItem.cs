using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;

namespace WpfApp3.DesignerItem.Db
{
    public class SettingsDesignerItem : DesignerItemBase
    {
        public SettingsDesignerItem(Guid id,double left, double top, double itemWidth, double itemHeight, string setting1)
            : base(id,left, top, itemWidth, itemHeight)
        {
            this.Setting1 = setting1;
        }

        public string Setting1 { get; set; }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var vm = new SettingsDesignerItemViewModel(Id,parent, new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight), Setting1);

            return vm;
        }
    }
}