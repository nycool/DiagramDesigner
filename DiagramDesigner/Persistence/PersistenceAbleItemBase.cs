using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using Prism.Mvvm;

namespace DiagramDesigner.Persistence
{
    public abstract class PersistenceAbleItemBase : BindableBase, ILoad
    {
        #region Abstrustor

        /// <summary>
        /// 加载保存的信息
        /// </summary>
        public abstract SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent);

        #endregion Abstrustor
    }
}