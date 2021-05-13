using System;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.Persistence
{
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
            this.SaveInfo = saveInfo;
        }

        public override SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent) =>
            SaveInfo.LoadSaveInfo(parent);

    }
}