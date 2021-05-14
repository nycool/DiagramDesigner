using System;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Persistence;

namespace WpfApp3.DesignerItem.Db
{
    public class PersistDesignerItem : DesignerItemBase
    {
        public PersistDesignerItem(Guid id, double left, double top, double itemWidth, double itemHeight, string hostUrl)
            : base(id, left, top, itemWidth, itemHeight)
        {
            this.HostUrl = hostUrl;
        }

        public string HostUrl { get; set; }


        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent)
        {
            var vm = new PersistDesignerItemViewModel(Id,parent, new DesignerItemPosition(Left, Top, ItemWidth, ItemHeight), HostUrl);

            return vm;
        }
    }
}
