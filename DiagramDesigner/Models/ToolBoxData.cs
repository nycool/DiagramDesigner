using System;

namespace DiagramDesigner.Models
{
    public class ToolBoxData
    {
        public string ImageUrl { get; private set; }
        public Type Type { get; private set; }

        public ToolBoxData(string imageUrl, Type type)
        {
            this.ImageUrl = imageUrl;
            this.Type = type;
        }
    }
}
