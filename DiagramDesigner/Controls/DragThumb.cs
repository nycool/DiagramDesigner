using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Helpers;
using DiagramDesigner.Interface;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using DiagramDesigner.BaseClass;

namespace DiagramDesigner.Controls
{
    public class DragThumb : Thumb
    {
        public DragThumb()
        {
            DragDelta += DragThumb_DragDelta;
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is FrameworkElement framework)
            {
                if (ElementHelper.FindVisualParent<DesignerCanvas>(framework) is { } canvas)
                {
                    if (DataContext is DesignerItemViewModelBase designerItem)
                    {
                        if (designerItem.IsSelected)
                        {
                            double minLeft = double.MaxValue;
                            double minTop = double.MaxValue;

                            // we only move DesignerAndConnectItems
                            var designerItems = designerItem.SelectedItems;

                            if (designerItems?.Any() == true)
                            {
                                foreach (DesignerItemViewModelBase item in designerItems.OfType<DesignerItemViewModelBase>())
                                {
                                    double left = item.Left;
                                    double top = item.Top;
                                    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                                    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                                    double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                                    double deltaVertical = Math.Max(-minTop, e.VerticalChange);
                                    item.Left += deltaHorizontal;
                                    item.Top += deltaVertical;

                                    canvas.MoveStack.Push(new MoveInfo(item,deltaHorizontal,Orientation.Left));
                                    canvas.MoveStack.Push(new MoveInfo(item,deltaVertical,Orientation.Top));

                                    // prevent dragging items out of groupitem
                                    if (item.Parent is DesignerItemViewModelBase groupItem)
                                    {
                                        if (item.Left + item.ItemWidth >= groupItem.ItemWidth) item.Left = groupItem.ItemWidth - item.ItemWidth;
                                        if (item.Top + item.ItemHeight >= groupItem.ItemHeight) item.Top = groupItem.ItemHeight - item.ItemHeight;
                                    }
                                }
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}