using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DiagramDesigner.BaseClass.ConnectorClass;

namespace DiagramDesigner.Temp
{
    public class ConnectionVm : SelectableDesignerItemViewModelBase
    {
        #region Filed

        private PathGeometry _pathGeometry;

        /// <summary>
        /// 连接线路径形状
        /// </summary>
        public PathGeometry PathGeometry
        {
            get => _pathGeometry;
            set
            {
                if (SetProperty(ref _pathGeometry, value))
                {
                    UpdateAnchorPosition();
                }
            }
        }

        private Point _anchorPositionSource;

        /// <summary>
        /// between source connector position and the beginning
        /// of the path geometry we leave some space for visual reasons;
        /// so the anchor position source really marks the beginning
        /// of the path geometry on the source side
        /// </summary>
        public Point AnchorPositionSource
        {
            get => _anchorPositionSource;
            set => SetProperty(ref _anchorPositionSource, value);
        }

        private double _anchorAngleSource;

        /// <summary>
        /// slope of the path at the anchor position
        /// needed for the rotation angle of the arrow
        /// </summary>
        public double AnchorAngleSource
        {
            get => _anchorAngleSource;
            set => SetProperty(ref _anchorAngleSource, value);
        }

        private Point _anchorPositionSink;

        /// <summary>
        /// analogue to source side
        /// </summary>
        public Point AnchorPositionSink
        {
            get => _anchorPositionSink;
            set => SetProperty(ref _anchorPositionSink, value);
        }

        private double _anchorAngleSink;

        /// <summary>
        /// analogue to source side
        /// </summary>
        public double AnchorAngleSink
        {
            get => _anchorAngleSink;
            set => SetProperty(ref _anchorAngleSink, value);
        }

        private DoubleCollection _strokeDashArray;

        /// <summary>
        ///  pattern of dashes and gaps that is used to outline the connection path
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get => _strokeDashArray;
            set => SetProperty(ref _strokeDashArray, value);
        }


        private FullyCreatedConnectorInfo _sourceConnectorInfo;

        /// <summary>
        /// 开头连接上的点
        /// </summary>
        public FullyCreatedConnectorInfo SourceConnectorInfo
        {
            get => _sourceConnectorInfo;
            set
            {
                if (SetProperty(ref _sourceConnectorInfo, value))
                {
                    //SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    //_sourceConnectorInfo.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                }
            }
        }

        private FullyCreatedConnectorInfo _sinkConnectorInfo;

        public FullyCreatedConnectorInfo SinkConnectorInfo
        {
            get => _sinkConnectorInfo;
            set
            {
                if (SetProperty(ref _sinkConnectorInfo, value))
                {
                    //if (_sinkConnectorInfo is FullyCreatedConnectorInfo connectorInfo)
                    {
                        //SourceB = PointHelper.GetPointForConnector(connectorInfo);

                        //connectorInfo.DesignerItem.PropertyChanged += new WeakEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                    }
                    //else
                    {
                        //SourceB = ((PartCreatedConnectionInfo)_sinkConnectorInfo).CurrentLocation;
                    }
                }
            }
        }

        #endregion Filed

        #region Construstor

        public ConnectionVm(DesignerItemData designerItemData)
        {
        }

        #endregion Construstor

        #region Function

        /// <summary>
        /// 连接线更新
        /// 当Source或Sink的Position属性更新了以后，连接线需要更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Position"))
            {
                UpdatePathGeometry();
            }
        }

        /// <summary>
        /// 更新连接线路径
        /// </summary>
        private void UpdatePathGeometry()
        {
            if (SourceConnectorInfo != null && SinkConnectorInfo != null)
            {
                PathGeometry geometry = new PathGeometry();
                //List<Point> linePoints = PathFinder.GetConnectionLine(ss.GetInfo(), Sink.GetInfo(), true);
                //if (linePoints.Count > 0)
                //{
                //    PathFigure figure = new PathFigure();
                //    figure.StartPoint = linePoints[0];
                //    linePoints.Remove(linePoints[0]);
                //    figure.Segments.Add(new PolyLineSegment(linePoints, true));
                //    geometry.Figures.Add(figure);

                //    this.PathGeometry = geometry;
                //}
            }
        }


        private ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position)
        {
            var info = new ConnectorInfo();
            info.Orientation = orientation;
            info.DesignerItemSize = new Size(_sourceConnectorInfo.DesignerItem.ItemWidth, _sourceConnectorInfo.DesignerItem.ItemHeight);
            info.DesignerItemLeft = left;
            info.DesignerItemTop = top;
            info.Position = position;
            return info;
        }

        /// <summary>
        /// 更新Anchor位置
        /// </summary>
        private void UpdateAnchorPosition()
        {
            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector
            // on PathGeometry at the specified fraction of its length

            this.PathGeometry.GetPointAtFractionLength(0, out var pathStartPoint, out var pathTangentAtStartPoint);

            this.PathGeometry.GetPointAtFractionLength(1, out var pathEndPoint, out var pathTangentAtEndPoint);

            // get angle from tangent vector
            this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            // add some margin on source and sink side for visual reasons only
            pathStartPoint.Offset(-pathTangentAtStartPoint.X * 5, -pathTangentAtStartPoint.Y * 5);
            pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
        }

        #endregion Function

        #region Override

        public override PersistenceAbleItemBase SaveInfo()
        {
            throw new NotImplementedException();
        }

        #endregion Override
    }
}