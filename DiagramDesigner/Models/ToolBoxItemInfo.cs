using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagramDesigner.Models
{
    public class ToolBoxItemInfo
    {
        public string ImageUrl { get;  }
        public Type Type { get; }

        public ToolBoxItemInfo(string imageUrl, Type type)
        {
            this.ImageUrl = imageUrl;
            this.Type = type;
        }
    }
}
