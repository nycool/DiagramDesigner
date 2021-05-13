using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;
using System;

namespace DiagramDesigner.Persistence
{
    /// <summary>
    /// 保存的单个DesignerItem or Connection
    /// </summary>
    public class DiagramItemInfo : PersistenceAbleItemBase
    {
        #region Filed

        /// <summary>
        /// 保存的控件信息
        /// </summary>
        public PersistenceAbleItemBase SaveInfo { get; }

        #endregion Filed

        public DiagramItemInfo(Guid id, PersistenceAbleItemBase saveInfo)
        : base(id)
        {
            SaveInfo = saveInfo;
        }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent) => ((ILoad)SaveInfo).LoadSaveInfo(parent);
    }
}