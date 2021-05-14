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

        public String HostUrl { get; set; }

        #endregion Filed

        #region Construsor
        public PersistDesignerItemViewModel(Guid id, IDiagramViewModel parent, DesignerItemPosition position, string hostUrl)
            : this(id, parent, position)
        {
            this.HostUrl = hostUrl;
        }


        public PersistDesignerItemViewModel(Guid id, IDiagramViewModel parent,DesignerItemPosition position)
        :this(id,parent)
        {
            InitPosition(position);
        }
      

        public PersistDesignerItemViewModel(Guid id, IDiagramViewModel parent)
        : this(id)
        {
            Parent = parent;
        }

        public PersistDesignerItemViewModel(Guid id)
        : this()
        {
            Id = id;
        }

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
            var item = new PersistDesignerItem(Id, Left, Top, ItemWidth, ItemHeight, HostUrl);

            //return new DiagramItemInfo(Id,item);

            return item;
        }
    }
}