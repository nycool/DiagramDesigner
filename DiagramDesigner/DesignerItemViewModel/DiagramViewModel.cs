using DiagramDesigner.Models;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class DiagramViewModel : DiagramViewModelBase
    {
        protected override void RemoveOrAdd(SelectableDesignerItemViewModelBase removeItem, Operation operation)
        {
        }

        protected override GroupDesignerItemViewModelBase GetGroup()
        {
            throw new System.NotImplementedException();
        }
    }
}