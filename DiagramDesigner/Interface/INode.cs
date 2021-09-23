using System;
using System.Collections.Generic;

namespace DiagramDesigner.Interface
{
    /// <summary>
    /// 节点
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 多信宿
        /// </summary>
        public List<Guid> DestinationId { get; set; }


        /// <summary>
        /// 多信源
        /// </summary>

        public List<Guid> SourceId { get; set; }
    }
}