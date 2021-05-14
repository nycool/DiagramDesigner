using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System;
using DiagramDesigner.BaseClass;

namespace DiagramDesigner.Persistence
{
    public abstract class PersistenceAbleItemBase : ILoad
    {
        #region Filed

        ///// <summary>
        ///// 模块ID
        ///// </summary>
        //public Guid Id { get; set; }

        #endregion Filed

        #region Construstor

        //public PersistenceAbleItemBase()
        //{
        //}

        //protected PersistenceAbleItemBase(Guid id)
        //{
        //    this.Id = id;
        //}

        #endregion Construstor

        #region Abstrustor

        /// <summary>
        /// 加载保存的信息
        /// </summary>
        public abstract SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent);



        #endregion Abstrustor
    }
}