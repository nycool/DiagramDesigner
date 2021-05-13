using DiagramDesigner.Adorners;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas
    {
        #region Ststic

        /// <summary>
        /// KeyBoard Move Offset
        /// </summary>
        private static double MoveOffset = 5;

        #endregion Ststic

        #region override

        /// <summary>
        /// 清除界面模块的已选择状态
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if we are source of event, we are rubberband selecting
                if (Equals(e.Source, this))
                {
                    // in case that this click is the start for a
                    // drag operation we cache the start point
                    _rubberbandSelectionStartPoint = e.GetPosition(this);

                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        if (DataContext is IDiagramViewModel vm)
                        {
                            vm.SelectedItemsCommand.Execute(false);
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            //Mediator.Instance.NotifyColleagues<bool>("DoneDrawingMessage", true);

            if (_sourceConnector != null)
            {
                FullyCreatedConnectorInfo sourceDataItem = _sourceConnector.DataContext as FullyCreatedConnectorInfo;
                if (ConnectorsHit.Count == 2)
                {
                    Connector sinkConnector = ConnectorsHit.Last();
                    FullyCreatedConnectorInfo sinkDataItem = sinkConnector.DataContext as FullyCreatedConnectorInfo;

                    int indexOfLastTempConnection = sinkDataItem.DesignerItem.Parent.ItemsSource.Count - 1;
                    sinkDataItem.DesignerItem.Parent.RemoveItemCommand.Execute(
                        sinkDataItem.DesignerItem.Parent.ItemsSource[indexOfLastTempConnection]);
                    sinkDataItem.DesignerItem.Parent.AddItemCommand.Execute(new ConnectorViewModel(sourceDataItem, sinkDataItem));
                }
                else
                {
                    //Need to remove last item as we did not finish drawing the path
                    int indexOfLastTempConnection = sourceDataItem.DesignerItem.Parent.ItemsSource.Count - 1;
                    sourceDataItem.DesignerItem.Parent.RemoveItemCommand.Execute(
                        sourceDataItem.DesignerItem.Parent.ItemsSource[indexOfLastTempConnection]);
                }
            }
            ConnectorsHit.Clear();
            SourceConnector = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (SourceConnector != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPoint = e.GetPosition(this);
                    _partialConnection.SinkConnectorInfo = new PartCreatedConnectionInfo(currentPoint);
                    HitTesting(currentPoint);
                }
            }
            else
            {
                // if mouse button is not pressed we have no drag operation, ...
                if (e.LeftButton != MouseButtonState.Pressed)
                    _rubberbandSelectionStartPoint = null;

                // ... but if mouse button is pressed and start
                // point value is set we do have one
                if (this._rubberbandSelectionStartPoint.HasValue)
                {
                    // create rubberband adorner
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null)
                    {
                        RubberbandAdorner adorner = new RubberbandAdorner(this, _rubberbandSelectionStartPoint);
                        if (adorner != null)
                        {
                            adornerLayer.Add(adorner);
                        }
                    }
                }
            }
            //e.Handled = true;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetData(typeof(DragObject)) is DragObject dragObject)
            {
                if (DataContext is IDiagramViewModel vm)
                {
                    vm.SelectedItemsCommand?.Execute(false);
                    Point position = e.GetPosition(this);

                    if (Activator.CreateInstance(dragObject.ContentType) is DesignerItemViewModelBase itemInfo)
                    {
                        itemInfo.Left = Math.Max(0, position.X - itemInfo.ItemWidth / 2);
                        itemInfo.Top = Math.Max(0, position.Y - itemInfo.ItemHeight / 2);
                        itemInfo.IsSelected = true;
                        vm.AddItemCommand?.Execute(itemInfo);
                        e.Handled = true;
                    }
                }
            }
        }

        #region KeyBoard Move

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            //var canvas = ElementHelper.FindVisualParent<DesignerCanvas>(e.OriginalSource);

            if (e.Source is DependencyObject obj)
            {
                var canvas = ElementHelper.FindVisualParent<DesignerCanvas>(obj);

                if (canvas == null)
                {
                    return;
                }

                if (GetDiagramVm(canvas) is { } vm)
                {
                    if (Keyboard.IsKeyDown(Key.Up))
                    {
                        AddOffset(Key.Up, MoveOffset, vm);
                    }
                    else if (Keyboard.IsKeyDown(Key.Down))
                    {
                        AddOffset(Key.Down, MoveOffset, vm);
                    }
                    else if (Keyboard.IsKeyDown(Key.Left))
                    {
                        AddOffset(Key.Left, MoveOffset, vm);
                    }
                    else if (Keyboard.IsKeyDown(Key.Right))
                    {
                        AddOffset(Key.Right, MoveOffset, vm);
                    }
                }
            }

            e.Handled = false;
        }

        private void AddOffset(Key key, double value, IDiagramViewModel vm)
        {
            var selectedItems = vm.SelectedItems;

            if (selectedItems.Any())
            {
                var items = selectedItems.OfType<DesignerItemViewModelBase>();

                if (Key.Up == key)
                {
                    foreach (var item in items)
                    {
                        double top = item.Top;

                        top -= value;

                        item.Top = top;
                    }
                }
                else if (Key.Down == key)
                {
                    foreach (var item in items)
                    {
                        double top = item.Top;

                        top += value;

                        item.Top = top;
                    }
                }
                else if (Key.Left == key)
                {
                    foreach (var item in items)
                    {
                        double left = item.Left;

                        left -= value;

                        item.Left = left;
                    }
                }
                else if (Key.Right == key)
                {
                    foreach (var item in items)
                    {
                        double left = item.Left;

                        left += value;

                        item.Left = left;
                    }
                }
            }
        }

        #endregion KeyBoard Move

        #endregion override
    }
}