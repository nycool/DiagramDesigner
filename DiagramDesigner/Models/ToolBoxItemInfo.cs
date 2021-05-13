using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagramDesigner.Models
{
    public class ToolBoxItemInfo
    {
        public ImageSource ImageUrl { get; private set; }
        public Type Type { get; private set; }

        public ToolBoxItemInfo(ImageSource imageSource, Type type)
        {
            this.ImageUrl = imageSource;
            this.Type = type;
        }
    }
}
