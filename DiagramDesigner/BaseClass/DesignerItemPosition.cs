using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DiagramDesigner.BaseClass
{
    public struct DesignerItemPosition
    {
        public double Top;

        public double Left;

        public double Width;

        public double Height;


        public DesignerItemPosition(double left, double top)
        {
            Left = left;
            Top = top;

            Width = 0;
            Height = 0;
        }

        public DesignerItemPosition(double left, double top, double width, double height)
        : this(left, top)
        {
            Width = width;
            Height = height;
        }
    }
}
