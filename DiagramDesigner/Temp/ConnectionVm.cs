using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DiagramDesigner.Temp
{
    public class ConnectionVm : ConnectorViewModel
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

        #endregion Filed

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
            if (SourceConnector != null && SinkConnector != null)
            {
                PathGeometry geometry = new PathGeometry();
                //List<Point> linePoints = PathFinder.GetConnectionLine(SourceConnector, SinkConnector, true);
                var linePoints = ConnectionPoints;
                if (linePoints.Count > 0)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = linePoints[0];
                    linePoints.Remove(linePoints[0]);
                    figure.Segments.Add(new PolyLineSegment(linePoints, true));
                    geometry.Figures.Add(figure);

                    this.PathGeometry = geometry;
                }
            }
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

        #region Construstor

        public ConnectionVm(DesignerItemData data, Guid[] oldSrc = default, Guid[] oldSink = default)
            : base(data, oldSrc, oldSink)
        {
           
        }

        #endregion Construstor
    }
}