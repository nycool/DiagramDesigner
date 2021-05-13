using System;
using System.Collections.Generic;
using System.Text;
using DiagramDesigner.Persistence;

namespace DiagramDesigner.BaseClass.Interface
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
