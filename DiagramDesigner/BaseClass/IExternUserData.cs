using System;
using System.Collections.Generic;
using System.Text;
using NodeLib.NodeInfo.Interfaces;

namespace DiagramDesigner.BaseClass
{
    /// <summary>
    /// 保存模块额外的用户数据
    /// </summary>
    public interface IExternUserData
    {
        public IConfigParam Config { get; set; }
    }
}
