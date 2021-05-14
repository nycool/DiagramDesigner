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

       

      

        #endregion Construstor

        #region Function

        #region Override

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent) => ((ILoad)SaveInfo).LoadSaveInfo(parent);

        #endregion Override

        #endregion Function
    }
}