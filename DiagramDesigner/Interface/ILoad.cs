using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;

namespace DiagramDesigner.Interface
{
    public interface ILoad
    {
        /// <summary>
        /// 加载保存的流程信息
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent);
    }
}
