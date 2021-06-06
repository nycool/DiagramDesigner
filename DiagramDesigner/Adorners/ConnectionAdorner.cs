using DiagramDesigner.BaseClass;
using DiagramDesigner.Controls;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner.Adorners
{
    public class ConnectionAdorner : Adorner
    {
        #region 属性字段
        /// <summary>
        /// 流程图画布
        /// </summary>
        private DesignerCanvas MyDesignerCanvas;
        private Canvas MyAdornerCanvas;
        /// <summary>
        /// 连接线
        /// </summary>
        private ConnectorViewModel MyConnection;
        /// <summary>
        /// 连接线路径
        /// </summary>
        private PathGeometry MyPathGeometry;

        private Connector FixConnector;

        private Connector DragConnector;
        /// <summary>
        /// 起始锚点？？
        /// </summary>
        private Thumb SourceDragThumb;
        /// <summary>
        /// 目标锚点？？
        /// </summary>
        private Thumb SinkDragThumb;

        private Pen DrawingPen;

        private DesignerItemViewModelBase _hitDesignerItem;
        /// <summary>
        /// 鼠标点中的流程图节点
        /// </summary>
        private DesignerItemViewModelBase HitDesignerItem
        {
            get { return _hitDesignerItem; }
            set
            {
                if (_hitDesignerItem != value)
                {
                    if (_hitDesignerItem != null)
                        _hitDesignerItem.IsDragConnectionOver = false;

                    _hitDesignerItem = value;

                    if (_hitDesignerItem != null)
                        _hitDesignerItem.IsDragConnectionOver = true;
                }
            }
        }

        private Connector _hitConnector;
        /// <summary>
        /// 鼠标点中的连接点
        /// </summary>
        private Connector HitConnector
        {
            get { return _hitConnector; }
            set
            {
                if (_hitConnector != value)
                {
                    _hitConnector = value;
                }
            }
        }

        private VisualCollection VisualChildren;
        protected override int VisualChildrenCount => this.VisualChildren.Count;

        protected override Visual GetVisualChild(int index) => this.VisualChildren[index];

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="designer"></param>
        /// <param name="connection"></param>
        public ConnectionAdorner(DesignerCanvas designer, ConnectorViewModel connection)
            : base(designer)
        {
            this.MyDesignerCanvas = designer;
            MyAdornerCanvas = new Canvas();
            this.VisualChildren = new VisualCollection(this);
            this.VisualChildren.Add(MyAdornerCanvas);

            this.MyConnection = connection;
            this.MyConnection.PropertyChanged += AnchorPositionChanged;

            InitializeDragThumbs();

            DrawingPen = new Pen(Brushes.LightSlateGray, 1);

            DrawingPen.LineJoin = PenLineJoin.Round;

            base.Unloaded += ConnectionAdorner_Unloaded;
        }

        /// <summary>
        /// 属性更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnchorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("AnchorPositionSource"))
            {
                Canvas.SetLeft(SourceDragThumb, MyConnection.SourceA.X);
                Canvas.SetTop(SourceDragThumb, MyConnection.SourceA.Y);
            }

            if (e.PropertyName.Equals("AnchorPositionSink"))
            {
                Canvas.SetLeft(SinkDragThumb, MyConnection.SourceB.X);
                Canvas.SetTop(SinkDragThumb, MyConnection.SourceB.Y);
            }
        }

        /// <summary>
        /// 拖放开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbDragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.HitDesignerItem = null;
            this.HitConnector = null;
            this.MyPathGeometry = null;
            this.Cursor = Cursors.Cross;
            this.MyConnection.StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });

            if (sender == SourceDragThumb)
            {
                FixConnector = MyConnection.Sink;
                DragConnector = MyConnection.Source;
            }
            else if (sender == SinkDragThumb)
            {
                DragConnector = MyConnection.Sink;
                FixConnector = MyConnection.Source;
            }
        }

        /// <summary>
        /// 鼠标拖拉中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbDragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point currentPosition = Mouse.GetPosition(this);
            this.HitTesting(currentPosition);
            this.MyPathGeometry = UpdatePathGeometry(currentPosition);
            this.InvalidateVisual();
        }

        /// <summary>
        /// 拖放完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbDragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (HitConnector == null)
            {
                var rst = ConnectionHelper.GetHitConnector(MyDesignerCanvas.DataContext as IDiagramViewModel, FixConnector, Mouse.GetPosition(this));
                if (rst != null)
                {
                    HitDesignerItem = rst.Item1;
                    HitConnector = rst.Item2;
                }
            }

            if (HitConnector != null && MyConnection != null)
            {
                ConfigConnection();
            }

            this.HitDesignerItem = null;
            this.HitConnector = null;
            this.MyPathGeometry = null;
            this.MyConnection.StrokeDashArray = null;
            this.InvalidateVisual();
        }

        /// <summary>
        /// 设置连接线的起始点
        /// </summary>
        private void ConfigConnection()
        {
            if (MyConnection.Source == FixConnector)
            {
                MyConnection.Sink = this.HitConnector;
            }
            else
            {
                MyConnection.Source = this.HitConnector;
            }
        }

        /// <summary>
        /// 界面渲染
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, DrawingPen, this.MyPathGeometry);
        }

        /// <summary>
        /// 重新排布
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            MyAdornerCanvas.Arrange(new Rect(0, 0, this.MyDesignerCanvas.ActualWidth, this.MyDesignerCanvas.ActualHeight));
            return finalSize;
        }

        /// <summary>
        /// 界面卸载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionAdorner_Unloaded(object sender, RoutedEventArgs e)
        {
            SourceDragThumb.DragDelta -= new DragDeltaEventHandler(ThumbDragThumb_DragDelta);
            SourceDragThumb.DragStarted -= new DragStartedEventHandler(ThumbDragThumb_DragStarted);
            SourceDragThumb.DragCompleted -= new DragCompletedEventHandler(ThumbDragThumb_DragCompleted);

            SinkDragThumb.DragDelta -= new DragDeltaEventHandler(ThumbDragThumb_DragDelta);
            SinkDragThumb.DragStarted -= new DragStartedEventHandler(ThumbDragThumb_DragStarted);
            SinkDragThumb.DragCompleted -= new DragCompletedEventHandler(ThumbDragThumb_DragCompleted);
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitializeDragThumbs()
        {
            Style dragThumbStyle = MyConnection.FindResource("ConnectionAdornerThumbStyle") as Style;

            //source drag thumb
            SourceDragThumb = new Thumb();
            Canvas.SetLeft(SourceDragThumb, MyConnection.AnchorPositionSource.X);
            Canvas.SetTop(SourceDragThumb, MyConnection.AnchorPositionSource.Y);
            this.MyAdornerCanvas.Children.Add(SourceDragThumb);
            if (dragThumbStyle != null)
                SourceDragThumb.Style = dragThumbStyle;

            SourceDragThumb.DragDelta += new DragDeltaEventHandler(ThumbDragThumb_DragDelta);
            SourceDragThumb.DragStarted += new DragStartedEventHandler(ThumbDragThumb_DragStarted);
            SourceDragThumb.DragCompleted += new DragCompletedEventHandler(ThumbDragThumb_DragCompleted);

            SinkDragThumb = new Thumb();
            Canvas.SetLeft(SinkDragThumb, MyConnection.AnchorPositionSink.X);
            Canvas.SetTop(SinkDragThumb, MyConnection.AnchorPositionSink.Y);
            this.MyAdornerCanvas.Children.Add(SinkDragThumb);
            if (dragThumbStyle != null)
                SinkDragThumb.Style = dragThumbStyle;

            SinkDragThumb.DragDelta += new DragDeltaEventHandler(ThumbDragThumb_DragDelta);
            SinkDragThumb.DragStarted += new DragStartedEventHandler(ThumbDragThumb_DragStarted);
            SinkDragThumb.DragCompleted += new DragCompletedEventHandler(ThumbDragThumb_DragCompleted);
        }

        /// <summary>
        /// 更新连接线路径
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private PathGeometry UpdatePathGeometry(Point position)
        {
            PathGeometry geometry = new PathGeometry();

            ConnectorOrientation targetOrientation;
            if (HitConnector != null)
            {
                targetOrientation = HitConnector.Orientation;
            }
            else
            {
                targetOrientation = DragConnector.Orientation;
            }

            List<Point> linePoints = PathFinder.GetConnectionLine(FixConnector.GetInfo(), position, targetOrientation);

            if (linePoints.Count > 0)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = linePoints[0];
                linePoints.Remove(linePoints[0]);
                figure.Segments.Add(new PolyLineSegment(linePoints, true));
                geometry.Figures.Add(figure);
            }

            return geometry;
        }

        /// <summary>
        /// 返回指定坐标上的当前元素中的输入元素（相对于当前元素的源）
        /// </summary>
        /// <param name="hitPoint"></param>
        private void HitTesting(Point hitPoint)
        {
            bool hitConnectorFlag = false;

            DependencyObject hitObject = MyDesignerCanvas.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != FixConnector.ParentDesignerItem &&
                   hitObject.GetType() != typeof(DesignerCanvas))
            {
                if (hitObject is Connector)
                {
                    HitConnector = hitObject as Connector;
                    hitConnectorFlag = true;
                }

                if (hitObject is DesignerItem)
                {
                    HitDesignerItem = hitObject as DesignerItem;
                    if (!hitConnectorFlag)
                        HitConnector = null;

                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
            HitDesignerItem = null;
        }
    }
}
