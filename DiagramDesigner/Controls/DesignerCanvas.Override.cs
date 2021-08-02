using DiagramDesigner.Adorners;
using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using Prism.Ioc;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

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
                Focus();

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
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (_sourceConnector != default)
            {
                if (_sourceConnector.DataContext is FullyCreatedConnectorInfo sourceDataItem)
                {
                    if (_connectorsHit.Count == 2)
                    {
                        Connector sinkConnector = _connectorsHit.Last();

                        if (sinkConnector.DataContext is FullyCreatedConnectorInfo sinkDataItem)
                        {
                            int indexOfLastTempConnection = sinkDataItem.DesignerItem.Parent.ItemsSource.Count - 1;

                            sinkDataItem.DesignerItem.Parent.RemoveItemCommand.Execute(
                                sinkDataItem.DesignerItem.Parent.ItemsSource[indexOfLastTempConnection]);

                            var connector = new ConnectorViewModel(new DesignerItemData(sourceDataItem, sinkDataItem));

                            connector.Connected();

                            sinkDataItem.DesignerItem.Parent.AddItemCommand.Execute(connector);
                        }
                    }
                    else
                    {
                        //Need to remove last item as we did not finish drawing the path
                        int indexOfLastTempConnection = sourceDataItem.DesignerItem.Parent.ItemsSource.Count - 1;
                        sourceDataItem.DesignerItem.Parent.RemoveItemCommand.Execute(
                            sourceDataItem.DesignerItem.Parent.ItemsSource[indexOfLastTempConnection]);
                    }
                }
            }
            _connectorsHit.Clear();
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
                        var adorner = new RubberbandAdorner(this, _rubberbandSelectionStartPoint);
                        adornerLayer.Add(adorner);
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

        protected override async void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
                {
                    if (files.Length == 1)
                    {
                        string fileName = files.First();

                        if (File.Exists(fileName))
                        {
                            if (GetDiagramVm(this) is { } vm)
                            {
                                vm.ClearCommand.Execute();
                                if (await Opening(vm, fileName))
                                {
                                    MessageBox.Show("解决方案加载成功");
                                }
                            }
                        }
                    }
                }

                return;
            }

            if (e.Data.GetData(typeof(DragObject)) is DragObject dragObject)
            {
                if (DataContext is IDiagramViewModel vm)
                {
                    vm.SelectedItemsCommand?.Execute(false);

                    Point position = e.GetPosition(this);

                    if (ContainerLocator.Current.Resolve(dragObject.ContentType) is DesignerItemViewModelBase itemInfo)
                    {
                        itemInfo.Left = Math.Max(0, position.X - itemInfo.ItemWidth / 2);
                        itemInfo.Top = Math.Max(0, position.Y - itemInfo.ItemHeight / 2);

                        itemInfo.IsSelected = true;

                        var itemPosition = new DesignerItemPosition(itemInfo.Left, itemInfo.Top, itemInfo.ItemWidth,
                            itemInfo.ItemHeight);
                        var data = new DesignerItemData(Guid.NewGuid(),
                            itemPosition);

                        itemInfo.LoadDesignerItemData(data);

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

            if (GetDiagramVm(this) is { } vm)
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

                e.Handled = true;
            }
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