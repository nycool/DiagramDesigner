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
                                    DragBottom(scale, item);
                                    break;

                                case VerticalAlignment.Top:
                                    double top = item.Top;
                                    dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                                    scale = (item.ItemHeight - dragDeltaVertical) / item.ItemHeight;
                                    DragTop(scale, item);
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
                                    DragLeft(scale, item);
                                    break;

                                case HorizontalAlignment.Right:
                                    dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                                    scale = (item.ItemWidth - dragDeltaHorizontal) / item.ItemWidth;
                                    DragRight(scale, item);
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

        private void DragLeft(double scale, DesignerItemViewModelBase item)
        {
            double groupLeft = item.Left + item.ItemWidth;

            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetLeft(groupLeft, scale, groupItem);
                }
            }
            else
            {
                SetLeft(groupLeft, scale, item);
            }
        }

        private void SetLeft(double left, double scale, DesignerItemViewModelBase designerItem)
        {
            double groupItemLeft = designerItem.Left;
            double delta = (left - groupItemLeft) * (scale - 1);
            double newGroupItemWidth;
            double newGroupItemLeft;

            //if (designer.AlignToGrid)
            //{
            //    newGroupItemLeft = designer.GridSize * Math.Round((groupItemLeft - delta) / designer.GridSize);
            //    newGroupItemWidth = designer.GridSize * Math.Round((groupItem.ActualWidth - (newGroupItemLeft - groupItemLeft)) / designer.GridSize);
            //    if (newGroupItemWidth < designer.GridSize)
            //    {
            //        newGroupItemWidth = designer.GridSize;
            //    }
            //}
            //else
            {
                newGroupItemLeft = groupItemLeft - delta;
                newGroupItemWidth = designerItem.ItemWidth * scale;
            }

            designerItem.ItemWidth = newGroupItemLeft;

            designerItem.ItemWidth = newGroupItemWidth;
        }

        private void DragTop(double scale, DesignerItemViewModelBase item)
        {
            double top = item.Top + item.ItemHeight;

            if (item is GroupingDesignerItemViewModel group)
            {
                foreach (var groupItem in group.ItemsSource.OfType<DesignerItemViewModelBase>())
                {
                    SetTop(top, scale, groupItem);
                }
            }
            else
            {
                SetTop(top, scale, item);
            }
        }

        private void SetTop(double top, double scale, DesignerItemViewModelBase designerItem)
        {
            double groupItemTop = designerItem.Top;
            double delta = (top - groupItemTop) * (scale - 1);
            double newGroupItemTop;
            double newGroupItemHeight;

            //if (designer.AlignToGrid)
            //{
            //    newGroupItemTop = designer.GridSize * Math.Round((groupItemTop - delta) / designer.GridSize);
            //    newGroupItemHeight = designer.GridSize * Math.Round((groupItem.ActualHeight - (newGroupItemTop - groupItemTop)) / designer.GridSize);
            //    if (newGroupItemHeight < designer.GridSize)
            //    {
            //        newGroupItemHeight = designer.GridSize;
            //    }
            //}
            //else
            {
                newGroupItemTop = groupItemTop - delta;
                newGroupItemHeight = designerItem.ItemHeight * scale;
            }

            designerItem.Top = newGroupItemTop;

            designerItem.ItemHeight = newGroupItemHeight;
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
            double groupItemLeft = designerItem.Left;
            double delta = (groupItemLeft - designerItem.Left) * (scale - 1);
            double newGroupItemLeft;
            double newGroupItemWidth;

            //if (designer.AlignToGrid)
            //{
            //    newGroupItemLeft = designer.GridSize * Math.Round((groupItemLeft + delta) / designer.GridSize);
            //    newGroupItemWidth = designer.GridSize * Math.Round((groupItem.ActualWidth * scale) / designer.GridSize);
            //    if (newGroupItemWidth < designer.GridSize)
            //    {
            //        newGroupItemWidth = designer.GridSize;
            //    }
            //}
            //else
            {
                newGroupItemLeft = groupItemLeft + delta;
                newGroupItemWidth = designerItem.ItemWidth * scale;
            }

            designerItem.Left = newGroupItemLeft;
            designerItem.ItemWidth = newGroupItemWidth;
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
            double groupItemTop = designerItem.Top;
            double delta = (groupItemTop - designerItem.Top) * (scale - 1);
            double groupItemHeight;

            //if (designer.AlignToGrid)
            //{
            //    groupItemTop = designer.GridSize * Math.Round((groupItemTop + delta) / designer.GridSize);
            //    groupItemHeight = designer.GridSize * Math.Round((groupItem.ActualHeight * scale) / designer.GridSize);
            //    if (groupItemHeight < designer.GridSize)
            //    {
            //        groupItemHeight = designer.GridSize;
            //    }
            //}
            //else
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