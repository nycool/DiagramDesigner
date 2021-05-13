using Prism.Commands;

namespace DiagramDesigner.BaseClass.Interface
{
    public interface ISelectItems
    {
        DelegateCommand<bool?> SelectItemCommand { get; }
    }
}
