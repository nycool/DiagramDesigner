using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DiagramDesigner.Controls;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;

namespace DiagramDesigner
{
    public static class ConnectionHelper
    {
        /// <summary>
        /// 按照控件中心坐标计算，离鼠标点最近的坐标；
        /// </summary>
        /// <param name="point">松开鼠标的位置点</param>
        public static Tuple<DesignerItemViewModelBase, Connector> GetHitConnector(IDiagramViewModel designerCanvas, Connector fixConnector, Point point)
        {
            double maxLimitDistance = 80;
            double minDistance = double.MaxValue;
            DesignerItemViewModelBase targetDesignItem = null;

            // 查找最近的流程图节点
            foreach (var item in designerCanvas.ItemsSource.OfType<DesignerItemViewModelBase>())
            {
                if (item == fixConnector.ParentDesignerItem)
                {
                    continue;
                }
                var top = item.Top;
                var left = item.Left;
                //中心点
                var x = left + item.ActualWidth / 2;
                var y = top + item.ActualHeight / 2;
                var value = Math.Sqrt(Math.Pow(point.X - x, 2) + Math.Pow(point.Y - y, 2));
                if (minDistance > value)
                {
                    minDistance = value;
                    targetDesignItem = item;
                }
            }

            // 超出最大限制则直接返回
            if (minDistance > maxLimitDistance)
            {
                return null;
            }

            //中心点
            var centerX = targetDesignItem.Left + targetDesignItem.ActualWidth / 2;
            var centerY = targetDesignItem.Top + targetDesignItem.ActualHeight / 2;

            Dictionary<string, Point> dictPoint = new Dictionary<string, Point>
            {
                { "Left", new Point(centerX - targetDesignItem.ActualWidth / 2, centerY) },
                { "Right", new Point(centerX + targetDesignItem.ActualWidth / 2, centerY) },
                { "Buttom", new Point(centerX, centerY + targetDesignItem.ActualHeight / 2) },
                { "Top", new Point(centerX, centerY - targetDesignItem.ActualHeight / 2) }
            };

            string lastTarget = "";
            double targetMin = double.MaxValue;
            foreach (var item in dictPoint)
            {
                var distance = Math.Sqrt(Math.Pow(point.X - item.Value.X, 2) + Math.Pow(point.Y - item.Value.Y, 2));
                if (targetMin > distance)
                {
                    targetMin = distance;
                    lastTarget = item.Key;
                }
            }

            Control connectorDecorator = targetDesignItem?.Template?.FindName("PART_ConnectorDecorator", targetDesignItem) as Control;
            connectorDecorator?.ApplyTemplate();
            return new Tuple<DesignerItemViewModelBase, Connector>(targetDesignItem, connectorDecorator?.Template?.FindName(lastTarget, connectorDecorator) as Connector);
        }
    }
}
