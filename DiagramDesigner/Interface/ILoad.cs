using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;

namespace DiagramDesigner.Interface
{
    public interface ILoad
    {
        SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent);
    }
}
