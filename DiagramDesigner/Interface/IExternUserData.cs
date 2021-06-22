using NodeLib.NodeInfo.Interfaces;

namespace DiagramDesigner.Interface
{
    /// <summary>
    /// 保存模块额外的用户数据
    /// </summary>
    public interface IExternUserData
    {
        public IConfigParam Config { get; set; }
    }
}
