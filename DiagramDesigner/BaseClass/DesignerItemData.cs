using DiagramDesigner.Interface;
using System;
using DiagramDesigner.BaseClass.ConnectorClass;

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
        public ExternUserDataBase UserData { get; set; }

        /// <summary>
        /// 线头
        /// </summary>
        public FullyCreatedConnectorInfo SourceConnectorInfo { get; set; }

        /// <summary>
        /// 线尾
        /// </summary>
        public ConnectorInfoBase SinkConnectorInfo { get; set; }


        public DesignerItemData(IDiagramViewModel parent,FullyCreatedConnectorInfo sourceConnector,FullyCreatedConnectorInfo sinkConnectorInfo)
        :this(sourceConnector,sinkConnectorInfo)
        {
            Parent = parent;
        }

        public DesignerItemData(Guid id, FullyCreatedConnectorInfo sourceConnector, ConnectorInfoBase sinkConnector)
        : this(sourceConnector,sinkConnector)
        {
            Id = id;
        }

        public DesignerItemData(FullyCreatedConnectorInfo sourceConnector, ConnectorInfoBase sinkConnector)
        {
            SourceConnectorInfo = sourceConnector;
            SinkConnectorInfo = sinkConnector;
        }

        public DesignerItemData(Guid id, DesignerItemPosition position)
        {
            Id = id;

            Position = position;
        }

        public DesignerItemData(Guid id, DesignerItemPosition position, ExternUserDataBase userData)
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