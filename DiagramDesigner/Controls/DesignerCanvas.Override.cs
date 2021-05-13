using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DiagramDesigner.Adorners;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.DesignerItemViewModel;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas
    {
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
                            vm.ClearSelectedItemsCommand.Execute();
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
            e.Handled = true;
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
                    vm.ClearSelectedItemsCommand?.Execute();
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

        #endregion override
    }
}
