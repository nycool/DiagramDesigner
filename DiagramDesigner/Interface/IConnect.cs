using System;
using DiagramDesigner.Models;

namespace DiagramDesigner.Interface
{
    /// <summary>
    /// 关联上下级
    /// </summary>
    public interface IConnect
    {
        /// <summary>
        /// 连接上一级
        /// </summary>
        /// <param name="parent"></param>
        bool ConnectSource(IConnect parent);

        /// <summary>
        /// 连接下一级
        /// </summary>
        /// <param name="child"></param>
        bool ConnectDestination(IConnect child);

        bool Remove(IConnect connect, RemoveTypes removeType);

        /// <summary>
        /// 获取当前节点ID
        /// </summary>
        /// <returns></returns>
        Guid GetCurrentId();
    }
}