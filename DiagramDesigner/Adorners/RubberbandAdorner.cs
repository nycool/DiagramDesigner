using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Helpers;
using DiagramDesigner.Interface;
using DesignerCanvas = DiagramDesigner.Controls.DesignerCanvas;

namespace DiagramDesigner.Adorners
{
    public class RubberbandAdorner : Adorner
    {
        private Point? startPoint;
        private Point? endPoint;
        private Pen rubberbandPen;

        private DesignerCanvas designerCanvas;

        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this.designerCanvas = designerCanvas;
            this.startPoint = dragStartPoint;
            rubberbandPen = new Pen(Brushes.LightSlateGray, 1);
            rubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                endPoint = e.GetPosition(this);
                UpdateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            // release mouse capture
            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            // remove this adorner from adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired !
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (this.startPoint.HasValue && this.endPoint.HasValue)
                dc.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(this.startPoint.Value, this.endPoint.Value));
        }

        private void UpdateSelection()
        {
            if (designerCanvas.DataContext is IDiagramViewModel vm)
            {
                if (startPoint == null || endPoint == null)
                {
                    throw new ArgumentException(nameof(startPoint));
                }

                if (vm.ItemsSource == null)
                {
                    throw new ArgumentNullException(nameof(vm.ItemsSource));
                }

                if (vm.ItemsSource.Any())
                {
                    Rect rubberBand = new Rect(startPoint.Value, endPoint.Value);

                    ItemsControl itemsControl = ElementHelper.FindVisualParent<ItemsControl>(designerCanvas);

                    foreach (SelectableDesignerItemViewModelBase item in vm.ItemsSource)
                    {
                        if (item != null)
                        {
                            DependencyObject container = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

                            if (container != null)
                            {
                                Rect itemRect = VisualTreeHelper.GetDescendantBounds((Visual)container);

                                Rect itemBounds = ((Visual)container).TransformToAncestor(designerCanvas).TransformBounds(itemRect);

                                if (rubberBand.Contains(itemBounds))
                                {
                                    item.IsSelected = true;
                                }
                                else
                                {
                                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                                    {
                                        item.IsSelected = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(nameof(designerCanvas.DataContext));
            }
        }
    }
}