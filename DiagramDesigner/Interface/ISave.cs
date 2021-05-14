using DiagramDesigner.Persistence;

namespace DiagramDesigner.Interface
{
    public interface ISave
    {
        /// <summary>
        /// 保存模块的信息
        /// </summary>
        /// <returns></returns>
        PersistenceAbleItemBase SaveInfo();
    }
}
