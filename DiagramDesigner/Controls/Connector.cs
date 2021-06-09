using DiagramDesigner.Annotations;
using DiagramDesigner.BaseClass;
using DiagramDesigner.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiagramDesigner.Controls
{
    public class Connector : Control, INotifyPropertyChanged
    {
        private DesignerCanvas _canvas;

        public Connector()
        {
            LayoutUpdated += Connector_LayoutUpdated;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _canvas = ElementHelper.FindVisualParent<DesignerCanvas>(this);
            if (_canvas != null)
            {
                _canvas.SourceConnector = this;
                e.Handled = true;
            }
        }

        private void Connector_LayoutUpdated(object sender, System.EventArgs e)
        {
            //get centre position of this Connector relative to the DesignerCanvas
            //this.Position = this.TransformToAncestor(_canvas).Transform(new Point(this.Width / 2, this.Height / 2));
        }

        public ConnectorOrientation Orientation { get; set; }


        private Point _position;

        /// <summary>
        /// 连接线中心点相对于流程图画布的坐标
        /// center position of this Connector relative to the DesignerCanvas
        /// </summary>
        public Point Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}