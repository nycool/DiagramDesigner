using System;

namespace DiagramDesigner.Models
{
    public class ToolBoxItemInfo
    {
        #region Filed

        /// <summary>
        /// 控件显示图标的位置
        /// </summary>
        public string ImageUrl { get; }

        /// <summary>
        /// 控件类型
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// 控件分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 工具名
        /// </summary>
        public string ToolName { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string ToolTip { get; }

        #endregion Filed

        public ToolBoxItemInfo(string category, string toolName, string imageUrl, Type type)
        {
            Category = category;
            ToolName = toolName;
            ImageUrl = imageUrl;
            ViewModelType = type;

            ToolTip = toolName;
        }
    }
}