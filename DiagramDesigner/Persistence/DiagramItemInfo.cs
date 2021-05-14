using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using System;
using DiagramDesigner.Interface;

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
        public PersistenceAbleItemBase SaveInfo { get; set; }

        #endregion Filed

        #region Construstor

        public DiagramItemInfo()
        {
            
        }

        public DiagramItemInfo(Guid id)
            : base(id)
        {
        }

        public DiagramItemInfo(Guid id, PersistenceAbleItemBase saveInfo)
        : this(id)
        {
            SaveInfo = saveInfo;
        }

        #endregion Construstor

        #region Function

        #region Override

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent) => ((ILoad)SaveInfo).LoadSaveInfo(parent);

        #endregion Override

        #endregion Function
    }
}