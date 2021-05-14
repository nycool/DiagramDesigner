using Prism.Commands;

namespace DiagramDesigner.Interface
{
    public interface ISelectItems
    {
        DelegateCommand<bool?> SelectItemCommand { get; }
    }
}
