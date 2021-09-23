using System;

namespace DiagramDesigner.Interface
{
    /// <summary>
    ///工具控件显示的图片以及名称
    /// </summary>
    public interface IToolInfo
    {
        /// <summary>
        /// 工具类别
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// 工具名称
        /// </summary>
        string ToolName { get; set; }

        /// <summary>
        /// 控件类型
        /// </summary>
        Type ViewModelType { get; set; }

        /// <summary>
        /// 控件图标
        /// </summary>
        string IconUrl { get; set; }

        /// <summary>
        /// 是否已经重命名
        /// </summary>
        bool IsReName { get; set; }

        /// <summary>
        /// 重置工具的名称
        /// </summary>
        /// <param name="name"></param>
        void ReNameDesignerItem(string name);
    }
}
