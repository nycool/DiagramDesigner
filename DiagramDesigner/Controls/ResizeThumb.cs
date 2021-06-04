using DiagramDesigner.DesignerItemViewModel;
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
            DragDelta += ResizeThumb_DragDelta;
        }

        private const double _width = 80;

        private const double _height = 40;

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (DataContext is DesignerItemViewModelBase vm)
            {
                if (vm.IsSelected)
                {
                    var selectedDesignerItems = vm.SelectedItems.OfType<DesignerItemViewModelBase>();

                    CalculateDragLimits(selectedDesignerItems, out var minLeft, out var minTop,
                                        out var minDeltaHorizontal, out var minDeltaVertical);

                    foreach (var item in selectedDesignerItems)
                    {
                        double dragDeltaVertical;
                        double scale;

                        switch (VerticalAlignment)
                        {
                            case VerticalAlignment.Bottom:
                                {
                                    dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                                    scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                                    DragBottom(scale, item);
                                }

                                break;

                            case VerticalAlignment.Top:
                                {
                                    dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                    scale = (item.ActualHeight - dragDeltaVertical) / item.ActualHeight;
                                    DragTop(scale, item);
                                }

                                break;
                        }

                        double dragDeltaHorizontal;

                        switch (HorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:

                                {
                                    dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                                    scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                                    DragLeft(scale, item);
                                }
                                break;

                            case HorizontalAlignment.Right:
                                {
                                    dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                                    scale = (item.ActualWidth - dragDeltaHorizontal) / item.ActualWidth;
                                    DragRight(scale, item);
                                }
                                break;
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        #region Helper methods

        private void DragLeft(double scale, DesignerItemViewModelBase item)
        {
            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetLeft(scale, groupItem);
                }
            }
            else
            {
                SetLeft(scale, item);
            }
        }

        private void SetLeft(double scale, DesignerItemViewModelBase designerItem)
        {
            double delta = designerItem.ItemWidth * (scale - 1);

            double newGroupItemWidth;
            double newGroupItemLeft;

            {
                newGroupItemLeft = designerItem.Left - delta;

                newGroupItemWidth = designerItem.ActualWidth * scale;
            }

            if (IsWidthResize(newGroupItemWidth))
            {
                designerItem.Left = newGroupItemLeft;

                designerItem.ItemWidth = newGroupItemWidth;
            }
        }

        private void DragTop(double scale, DesignerItemViewModelBase item)
        {
            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetTop(scale, groupItem);
                }
            }
            else
            {
                SetTop(scale, item);
            }
        }

        private void SetTop(double scale, DesignerItemViewModelBase designerItem)
        {
            double delta = designerItem.ItemHeight * (scale - 1);

            double newGroupItemTop;

            double newGroupItemHeight;

            {
                newGroupItemTop = designerItem.Top - delta;

                newGroupItemHeight = designerItem.ActualHeight * scale;
            }

            if (IsHeightResize(newGroupItemHeight))
            {
                designerItem.Top = newGroupItemTop;

                designerItem.ItemHeight = newGroupItemHeight;
            }
        }

        private void DragRight(double scale, DesignerItemViewModelBase item)
        {
            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetRight(scale, groupItem);
                }
            }
            else
            {
                SetRight(scale, item);
            }
        }

        private void SetRight(double scale, DesignerItemViewModelBase designerItem)
        {
            var newGroupItemWidth = designerItem.ActualWidth * scale;

            if (IsWidthResize(newGroupItemWidth))
            {
                designerItem.ItemWidth = newGroupItemWidth;
            }
        }

        private void DragBottom(double scale, DesignerItemViewModelBase item)
        {
            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetBottom(scale, groupItem);
                }
            }
            else
            {
                SetBottom(scale, item);
            }
        }

        private void SetBottom(double scale, DesignerItemViewModelBase designerItem)
        {
            var groupItemHeight = designerItem.ActualHeight * scale;

            if (IsHeightResize(groupItemHeight))
            {
                designerItem.ItemHeight = groupItemHeight;
            }
        }

        private bool IsWidthResize(double width)
        {
            double value = 1E-12;

            if (Math.Abs(width - _width) > value)
            {
                return width >= _width;
            }

            return default;
        }

        private bool IsHeightResize(double height)
        {
            double value = 1E-12;

            if (Math.Abs(height - _height) > value)
            {
                return height >= _height;
            }

            return default;
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

                minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
            }
        }

        #endregion Helper methods
    }
}