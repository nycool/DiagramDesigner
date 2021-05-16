using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagramDesigner.Models
{
    public class ToolBoxItemInfo
    {
        /// <summary>
        /// 控件显示图标的位置
        /// </summary>
        public string ImageUrl { get;  }

        /// <summary>
        /// 控件类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 控件分类
        /// </summary>
        public string Category { get; set; } = "工具箱";

        public ToolBoxItemInfo(string imageUrl, Type type)
        {
            this.ImageUrl = imageUrl;
            this.Type = type;
        }
    }
}
