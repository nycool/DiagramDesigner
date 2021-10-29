using DiagramDesigner.Models;

namespace DiagramDesigner.DesignerItemViewModel
{
    public class DiagramViewModel : DiagramViewModelBase
    {
        protected override void RemoveOrAdd(SelectableDesignerItemViewModelBase removeItem, Operation operation)
        {
        }

        protected override GroupDesignerItemViewModelBase GetGroup(GroupType groupType = GroupType.分组)
        {
            throw new System.NotImplementedException();
        }
    }
}