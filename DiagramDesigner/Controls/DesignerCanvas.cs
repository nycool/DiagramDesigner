using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.DesignerItemViewModel;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas : Canvas
    {
        #region Filed

        private ConnectorViewModel _partialConnection;

        /// <summary>
        /// 缓存点击模块四个点
        /// </summary>
        private List<Connector> ConnectorsHit { get; }

        /// <summary>
        /// 鼠标在canvas界面点击的位置
        /// </summary>
        private Point? _rubberbandSelectionStartPoint = null;


        private Connector _sourceConnector;

        /// <summary>
        /// 第一个点击的模块四角点
        /// </summary>
        public Connector SourceConnector
        {
            get => _sourceConnector;
            set
            {
                if (_sourceConnector != value)
                {
                    _sourceConnector = value;

                    AddLine(_sourceConnector);
                }
            }
        }

        #endregion Filed

        #region Construstor

        public DesignerCanvas()
        {
            this.AllowDrop = true;
            //Mediator.Instance.Register(this);

            ConnectorsHit = new List<Connector>();
        }

        #endregion Construstor

        #region Function

        private void AddLine(Connector sourceConnector)
        {
            if (sourceConnector != null && sourceConnector.DataContext is FullyCreatedConnectorInfo sourceDataItem)
            {
                Rect rectangleBounds = sourceConnector.TransformToVisual(this).TransformBounds(new Rect(sourceConnector.RenderSize));

                Point point = new Point(rectangleBounds.Left + (rectangleBounds.Width / 2),
                    rectangleBounds.Bottom + (rectangleBounds.Height / 2));

                var partConnector = new PartCreatedConnectionInfo(point);

                _partialConnection = new ConnectorViewModel(sourceDataItem, partConnector);

                sourceDataItem.DesignerItem.Parent.AddItemCommand.Execute(_partialConnection);
            }
        }


        private void HitTesting(Point hitPoint)
        {
            if (InputHitTest(hitPoint) is DependencyObject hitObject)
            {
                while (hitObject != null && hitObject.GetType() != typeof(DesignerCanvas))
                {
                    if (hitObject is Connector connector)
                    {
                        if (!ConnectorsHit.Contains(connector))
                            ConnectorsHit.Add(connector);
                    }
                    hitObject = VisualTreeHelper.GetParent(hitObject);
                }
            }
        }

        #endregion Function
    }
}