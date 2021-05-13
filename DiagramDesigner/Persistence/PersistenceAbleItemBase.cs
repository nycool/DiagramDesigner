using System;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.Persistence
{
    public abstract class PersistenceAbleItemBase:ILoad
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid Id { get; }

        protected PersistenceAbleItemBase(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// 加载保存的信息
        /// </summary>
        public abstract SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent);
    }
}
