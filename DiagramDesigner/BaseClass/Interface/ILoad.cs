using System;
using System.Collections.Generic;
using System.Text;
using DiagramDesigner.BaseClass.DesignerItemViewModel;

namespace DiagramDesigner.BaseClass.Interface
{
    public interface ILoad
    {
        SelectableDesignerItemViewModelBase LoadSaveInfo(IDiagramViewModel parent);
    }
}
