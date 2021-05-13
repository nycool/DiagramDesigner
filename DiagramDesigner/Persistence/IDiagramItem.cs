using System.Collections.Generic;

namespace DiagramDesigner.Persistence
{
    public interface IDiagramItem 
    {
        /// <summary>
        /// 模块包含的数据
        /// </summary>
        List<PersistenceAbleItemBase> DesignerItems { get; }

        /// <summary>
        /// 模块连接的ID
        /// </summary>
        List<ConnectionInfo> Connections { get;  }

        /// <summary>
        /// 初始化Item状态
        /// </summary>
        void Init();
    }
}
