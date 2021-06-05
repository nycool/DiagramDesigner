using DiagramDesigner.DesignerItemViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiagramDesigner.Interface
{
    public interface ILoadXmlFile
    {
        Task<bool> LoadXml(IDiagramViewModel vm, string xmlFileName);

        bool SaveXml(IDiagramViewModel vm, string fileName);
    }
}