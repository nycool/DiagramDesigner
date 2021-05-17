using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DiagramDesigner.Controls
{
    /// <summary>
    /// 调整控件大小
    /// </summary>
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            base.DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is FrameworkElement framework && DataContext is DesignerItemViewModelBase vm)
            {
                var designer = ElementHelper.FindVisualParent<DesignerCanvas>(framework);

                if (designer != null && vm.IsSelected)
                {
                    double minLeft;
                    double minTop;
                    double minDeltaHorizontal;
                    double minDeltaVertical;
                    double dragDeltaVertical;
                    double dragDeltaHorizontal;
                    double scale;

                    var selectedDesignerItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                    CalculateDragLimits(selectedDesignerItems, out minLeft, out minTop,
                                        out minDeltaHorizontal, out minDeltaVertical);

                    foreach (var item in selectedDesignerItems)
                    {
                        {
                            switch (base.VerticalAlignment)
                            {
                                case VerticalAlignment.Bottom:
                                    dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                    scale = (item.ItemHeight - dragDeltaVertical) / item.ItemHeight;
                                    DragBottom(scale, item,designer);
                                    break;

                                case VerticalAlignment.Top:
                                    double top = item.Top;
                                    dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                    scale = (item.ItemHeight - dragDeltaVertical) / item.ItemHeight;
                                    DragTop(scale, item,designer);
                                    break;

                                default:
                                    break;
                            }

                            switch (base.HorizontalAlignment)
                            {
                                case HorizontalAlignment.Left:
                                    double left = item.Left;
                                    dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                    scale = (item.ItemWidth - dragDeltaHorizontal) / item.ItemWidth;
                                    DragLeft(scale, item,designer);
                                    break;

                                case HorizontalAlignment.Right:
                                    dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                                    scale = (item.ItemWidth - dragDeltaHorizontal) / item.ItemWidth;
                                    DragRight(scale, item,designer);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    e.Handled = true;
                }
            }
        }

#if true

        #region Helper methods

        private void DragLeft(double scale, DesignerItemViewModelBase item,DesignerCanvas canvas)
        {
            double groupLeft = item.Left + item.ItemWidth;

            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetLeft(groupLeft, scale, groupItem,canvas);
                }
            }
            else
            {
                SetLeft(groupLeft, scale, item,canvas);
            }
        }

        private void SetLeft(double left, double scale, DesignerItemViewModelBase designerItem,DesignerCanvas canvas)
        {
            double groupItemLeft = designerItem.Left;
            double delta = (left - groupItemLeft) * (scale - 1);
            double newGroupItemWidth;
            double newGroupItemLeft;

            if (canvas.AlignToGrid)
            {
                newGroupItemLeft = canvas.GridSize * Math.Round((groupItemLeft - delta) / canvas.GridSize);
                newGroupItemWidth = canvas.GridSize * Math.Round((designerItem.ItemWidth - (newGroupItemLeft - groupItemLeft)) / canvas.GridSize);
                if (newGroupItemWidth < canvas.GridSize)
                {
                    newGroupItemWidth = canvas.GridSize;
                }
            }
            else
            {
                newGroupItemLeft = groupItemLeft - delta;
                newGroupItemWidth = designerItem.ItemWidth * scale;
            }

            designerItem.ItemWidth = newGroupItemLeft;

            designerItem.ItemWidth = newGroupItemWidth;
        }

        private void DragTop(double scale, DesignerItemViewModelBase item,DesignerCanvas canvas)
        {
            double top = item.Top + item.ItemHeight;

            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetTop(top, scale, groupItem,canvas);
                }
            }
            else
            {
                SetTop(top, scale, item,canvas);
            }
        }

        private void SetTop(double top, double scale, DesignerItemViewModelBase designerItem,DesignerCanvas canvas)
        {
            double groupItemTop = designerItem.Top;
            double delta = (top - groupItemTop) * (scale - 1);
            double newGroupItemTop;
            double newGroupItemHeight;

            if (canvas.AlignToGrid)
            {
                var size = canvas.GridSize;

                newGroupItemTop = size * Math.Round((groupItemTop - delta) / size);
                newGroupItemHeight = size * Math.Round((designerItem.ItemHeight - (newGroupItemTop - groupItemTop)) / size);
                if (newGroupItemHeight < size)
                {
                    newGroupItemHeight = size;
                }
            }
            else
            {
                newGroupItemTop = groupItemTop - delta;
                newGroupItemHeight = designerItem.ItemHeight * scale;
            }

            designerItem.Top = newGroupItemTop;

            designerItem.ItemHeight = newGroupItemHeight;
        }

        private void DragRight(double scale, DesignerItemViewModelBase item,DesignerCanvas canvas)
        {
            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetRight(scale, groupItem,canvas);
                }
            }
            else
            {
                SetRight(scale, item,canvas);
            }
        }

        private void SetRight(double scale, DesignerItemViewModelBase designerItem,DesignerCanvas canvas)
        {
            double groupItemLeft = designerItem.Left;
            double delta = (groupItemLeft - designerItem.Left) * (scale - 1);
            double newGroupItemLeft;
            double newGroupItemWidth;

            if (canvas.AlignToGrid)
            {
                var size = canvas.GridSize;
                newGroupItemLeft = size * Math.Round((groupItemLeft + delta) / size);
                newGroupItemWidth = size * Math.Round((designerItem.ItemWidth * scale) / size);
                if (newGroupItemWidth < size)
                {
                    newGroupItemWidth = size;
                }
            }
            else
            {
                newGroupItemLeft = groupItemLeft + delta;
                newGroupItemWidth = designerItem.ItemWidth * scale;
            }

            designerItem.Left = newGroupItemLeft;
            designerItem.ItemWidth = newGroupItemWidth;
        }

        private void DragBottom(double scale, DesignerItemViewModelBase item,DesignerCanvas canvas)
        {
            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetBottom(scale, groupItem,canvas);
                }
            }
            else
            {
                SetBottom(scale, item,canvas);
            }
        }

        private void SetBottom(double scale, DesignerItemViewModelBase designerItem,DesignerCanvas canvas)
        {
            double groupItemTop = designerItem.Top;
            double delta = (groupItemTop - designerItem.Top) * (scale - 1);
            double groupItemHeight;

            if (canvas.AlignToGrid)
            {
                var size = canvas.GridSize;

                groupItemTop = size * Math.Round((groupItemTop + delta) / size);
                groupItemHeight = size * Math.Round((designerItem.ItemHeight * scale) / size);
                if (groupItemHeight < size)
                {
                    groupItemHeight = size;
                }
            }
            else
            {
                groupItemTop = groupItemTop + delta;
                groupItemHeight = designerItem.ItemHeight * scale;
            }

            designerItem.Top = groupItemTop;

            designerItem.ItemHeight = groupItemHeight;
        }

        /// <summary>
        /// 获取拖动的边界值
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <param name="minLeft"></param>
        /// <param name="minTop"></param>
        /// <param name="minDeltaHorizontal"></param>
        /// <param name="minDeltaVertical"></param>
        private void CalculateDragLimits(IEnumerable<DesignerItemViewModelBase> selectedItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            // drag limits are set by these parameters: canvas top, canvas left, minHeight, minWidth
            // calculate min value for each parameter for each item
            foreach (var item in selectedItems)
            {
                double left = item.Left;
                double top = item.Top;

                minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                //minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                //minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);

                minDeltaVertical = Math.Min(minDeltaVertical, item.ItemHeight - item.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ItemWidth - item.MinWidth);
            }
        }

        #endregion Helper methods

#endif
    }
}