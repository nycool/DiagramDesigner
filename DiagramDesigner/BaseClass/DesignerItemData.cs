using DiagramDesigner.Interface;
using System;
using DiagramDesigner.BaseClass.Connectors;

namespace DiagramDesigner.BaseClass
{
    /// <summary>
    /// 模块的数据信息
    /// </summary>
    public class DesignerItemData
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 上级canvas
        /// </summary>
        public IDiagramViewModel Parent { get; set; }

        /// <summary>
        /// 模块的位置信息
        /// </summary>
        public DesignerItemPosition Position { get; set; }

        /// <summary>
        /// 用户需要存储的信息
        /// </summary>
        public IUserData UserData { get; set; }

        /// <summary>
        /// 线头
        /// </summary>
        public Connector SourceConnector { get; set; }

        /// <summary>
        /// 线尾
        /// </summary>
        public ConnectorBase SinkConnector { get; set; }

        public DesignerItemData(IDiagramViewModel parent, Connector sourceConnector, Connector sinkConnector)
        : this(sourceConnector, sinkConnector)
        {
            Parent = parent;
        }

        public DesignerItemData(Guid id, Connector sourceConnector, ConnectorBase sinkConnector)
        : this(sourceConnector, sinkConnector)
        {
            Id = id;
        }

        public DesignerItemData(Connector sourceConnector, ConnectorBase sinkConnector)
        {
            SourceConnector = sourceConnector;
            SinkConnector = sinkConnector;
        }

        public DesignerItemData(Guid id, DesignerItemPosition position)
        {
            Id = id;

            Position = position;
        }

        public DesignerItemData(Guid id, DesignerItemPosition position, IUserData userData)
        : this(id, position)
        {
            UserData = userData;
        }

        public DesignerItemData(Guid id, DesignerItemPosition position, IDiagramViewModel parent)
        : this(id, position)
        {
            Parent = parent;
        }

        public DesignerItemData(Guid id, IDiagramViewModel parent, DesignerItemPosition position)
        : this(id, parent)
        {
            Position = position;
        }

        public DesignerItemData(Guid id, IDiagramViewModel parent)
        : this(id)
        {
            Parent = parent;
        }

        public DesignerItemData(Guid id)
        : this()
        {
            Id = id;
        }

        public DesignerItemData()
        {
        }
    }
}