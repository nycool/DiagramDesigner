using DiagramDesigner.BaseClass;
using DiagramDesigner.Helpers;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiagramDesigner.Controls
{
    public class Connector : Control
    {
        private DesignerCanvas _canvas;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _canvas ??= ElementHelper.FindVisualParent<DesignerCanvas>(this);

            if (_canvas != null)
            {
                _canvas.SourceConnector = this;

                e.Handled = true;
            }
        }

        public ConnectorOrientation Orientation { get; set; }
    }
}