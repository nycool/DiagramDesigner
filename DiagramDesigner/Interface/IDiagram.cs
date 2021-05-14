using System.Collections.Generic;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.BaseClass.Interface
{
    /// <summary>
    /// 保存的完整流程
    /// </summary>
    public interface IDiagram 
    {
        /// <summary>
        /// 保存的模块与线的信息
        /// </summary>
        List<PersistenceAbleItemBase> DesignerAndConnectItems { get; }
    }
}
