using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner.Controls
{
    public class ZoomBox : Control
    {
        private Thumb zoomThumb;
        private Canvas zoomCanvas;
        private Slider zoomSlider;
        private ScaleTransform scaleTransform;
        private double mouseOffsetX = 0.0;
        private double mouseOffsety = 0.0;

        #region DPs

        #region ScrollViewer

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomBox));

        #endregion ScrollViewer

        #region DesignerCanvas

        public static readonly DependencyProperty DesignerCanvasProperty =
            DependencyProperty.Register("DesignerCanvas", typeof(DiagramDesigner.Controls.DesignerCanvas), typeof(ZoomBox),
                new FrameworkPropertyMetadata(null,
                    OnDesignerCanvasChanged));

        public DesignerCanvas DesignerCanvas
        {
            get => (DesignerCanvas)GetValue(DesignerCanvasProperty);
            set => SetValue(DesignerCanvasProperty, value);
        }

        private static void OnDesignerCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomBox target = (ZoomBox)d;
            DesignerCanvas oldDesignerCanvas = (DesignerCanvas)e.OldValue;
            var newDesignerCanvas = target.DesignerCanvas;
            target.OnDesignerCanvasChanged(oldDesignerCanvas, newDesignerCanvas);
        }

        protected virtual void OnDesignerCanvasChanged(DesignerCanvas oldDesignerCanvas, DesignerCanvas newDesignerCanvas)
        {
            if (oldDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated -= this.DesignerCanvas_LayoutUpdated;
                newDesignerCanvas.MouseWheel -= this.DesignerCanvas_MouseWheel;
            }

            if (newDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated += this.DesignerCanvas_LayoutUpdated;
                newDesignerCanvas.MouseWheel += this.DesignerCanvas_MouseWheel;
                newDesignerCanvas.LayoutTransform = this.scaleTransform;
            }
        }

        #endregion DesignerCanvas

        #region Slider

        public static readonly DependencyProperty SliderProperty = DependencyProperty.Register(
            "Slider", typeof(Slider), typeof(ZoomBox), new PropertyMetadata(default(Slider)));

        public Slider Slider
        {
            get { return (Slider)GetValue(SliderProperty); }
            set { SetValue(SliderProperty, value); }
        }

        #endregion Slider

        #endregion DPs

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.ScrollViewer == null)
                return;

            if (Slider == null)
            {
                return;
            }

            this.zoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
            if (this.zoomThumb == null)
                throw new Exception("PART_ZoomThumb template is missing!");

            this.zoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;
            if (this.zoomCanvas == null)
                throw new Exception("PART_ZoomCanvas template is missing!");

            //this.zoomSlider = Template.FindName("PART_ZoomSlider", this) as Slider;
            //if (this.zoomSlider == null)
            //    throw new Exception("PART_ZoomSlider template is missing!");

            zoomSlider = Slider;

            this.zoomThumb.DragDelta += this.Thumb_DragDelta;

            this.zoomSlider.ValueChanged += this.ZoomSlider_ValueChanged;
            this.scaleTransform = new ScaleTransform();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scale = e.NewValue / e.OldValue;
            double halfViewportHeight = this.ScrollViewer.ViewportHeight / 2;
            double newVerticalOffset = ((this.ScrollViewer.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight);
            double halfViewportWidth = this.ScrollViewer.ViewportWidth / 2;
            double newHorizontalOffset = ((this.ScrollViewer.HorizontalOffset + halfViewportWidth) * scale - halfViewportWidth);
            this.scaleTransform.ScaleX *= scale;
            this.scaleTransform.ScaleY *= scale;
            this.ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            this.ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.InvalidateScale(out var scale, out var xOffset, out var yOffset);

            this.ScrollViewer.ScrollToHorizontalOffset(this.ScrollViewer.HorizontalOffset + e.HorizontalChange / scale);

            this.ScrollViewer.ScrollToVerticalOffset(this.ScrollViewer.VerticalOffset + e.VerticalChange / scale);
        }

        private void DesignerCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            this.InvalidateScale(out var scale, out var xOffset, out var yOffset);

            this.zoomThumb.Width = this.ScrollViewer.ViewportWidth * scale;

            this.zoomThumb.Height = this.ScrollViewer.ViewportHeight * scale;

            double left = xOffset + this.ScrollViewer.HorizontalOffset * scale;

            double top = yOffset + this.ScrollViewer.VerticalOffset * scale;

            Canvas.SetLeft(this.zoomThumb, left);

            Canvas.SetTop(this.zoomThumb, top);
        }

        private void DesignerCanvas_MouseWheel(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                MouseWheelEventArgs wheel = (MouseWheelEventArgs)e;

                //divide the value by 10 so that it is more smooth
                double value = Math.Max(0, wheel.Delta / 10);
                value = Math.Min(wheel.Delta / 12, 10);
                this.zoomSlider.Value += value;
            }
        }

        private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
        {
            double w = DesignerCanvas.ActualWidth * this.scaleTransform.ScaleX;
            double h = DesignerCanvas.ActualHeight * this.scaleTransform.ScaleY;

            // zoom canvas size
            double x = this.zoomCanvas.ActualWidth;
            double y = this.zoomCanvas.ActualHeight;
            double scaleX = x / w;
            double scaleY = y / h;

            scale = (scaleX < scaleY) ? scaleX : scaleY;

            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;
        }
    }
}