using DiagramDesigner.BaseClass;
using DiagramDesigner.Models;
using System.Windows;
using System.Windows.Input;

namespace DiagramDesigner.AttachedProperties
{
    public static class DragAndDropProps
    {
        #region EnabledForDrag

        public static readonly DependencyProperty EnabledForDragProperty =
            DependencyProperty.RegisterAttached("EnabledForDrag", typeof(bool), typeof(DragAndDropProps),
                new FrameworkPropertyMetadata((bool)false,
                    OnEnabledForDragChanged));

        public static bool GetEnabledForDrag(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledForDragProperty);
        }

        public static void SetEnabledForDrag(DependencyObject d, bool value)
        {
            d.SetValue(EnabledForDragProperty, value);
        }

        private static void OnEnabledForDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;

            if ((bool)e.NewValue)
            {
                fe.PreviewMouseDown += Fe_PreviewMouseDown;
                fe.PreviewGiveFeedback += Fe_PreviewGiveFeedback;
                fe.MouseMove += Fe_MouseMove;
            }
            else
            {
                fe.PreviewMouseDown -= Fe_PreviewMouseDown;
                fe.MouseMove -= Fe_MouseMove;
                fe.PreviewGiveFeedback -= Fe_PreviewGiveFeedback;
            }
        }

        private static void Fe_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;

            Mouse.SetCursor(Cursors.Hand);

            e.Handled = true;
        }

        #endregion EnabledForDrag

        #region DragStartPoint

        public static readonly DependencyProperty DragStartPointProperty =
            DependencyProperty.RegisterAttached("DragStartPoint", typeof(Point?), typeof(DragAndDropProps));

        public static Point? GetDragStartPoint(DependencyObject d)
        {
            return (Point?)d.GetValue(DragStartPointProperty);
        }

        public static void SetDragStartPoint(DependencyObject d, Point? value)
        {
            d.SetValue(DragStartPointProperty, value);
        }

        #endregion DragStartPoint

        private static void Fe_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point? dragStartPoint = GetDragStartPoint((DependencyObject)sender);

            if (e.LeftButton != MouseButtonState.Pressed)
                dragStartPoint = null;

            if (dragStartPoint.HasValue)
            {
                if (sender is FrameworkElement framework)
                {
                    if (framework.DataContext is ToolBoxItemInfo toolBoxData)
                    {
                        DragObject dataObject = new DragObject();
                        dataObject.ContentType = toolBoxData.ViewModelType;
                        dataObject.DesiredSize = new Size(65, 65);
                        DragDrop.DoDragDrop(framework, dataObject, DragDropEffects.Copy);
                        e.Handled = true;
                    }
                }
            }
        }

        private static void Fe_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetDragStartPoint((DependencyObject)sender, e.GetPosition((IInputElement)sender));
        }
    }
}