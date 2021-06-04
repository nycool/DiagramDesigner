using DiagramDesigner.BaseClass;
using DiagramDesigner.Helpers;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiagramDesigner.Controls
{
    public class Connector : Control
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            var canvas = ElementHelper.FindVisualParent<DesignerCanvas>(this);
            if (canvas != null)
            {
                canvas.SourceConnector = this;
                e.Handled = true;
            }
        }

        public ConnectorOrientation Orientation { get; set; }
    }
}