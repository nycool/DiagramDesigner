using DiagramDesigner.BaseClass;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiagramDesigner.BaseClass.Connectors;

namespace DiagramDesigner.Controls
{
    public partial class DesignerCanvas : Canvas
    {
        #region Filed

        private IDiagram CopyData { get; set; }

        /// <summary>
        /// 移动栈
        /// </summary>
        public Stack<MoveInfo> MoveStack { get; private set; }

        /// <summary>
        /// 删除栈
        /// </summary>
        private Stack<SelectableDesignerItemViewModelBase> _deleteStack;

        /// <summary>
        /// 恢复栈
        /// </summary>
        private Stack<object> _reStack;

        private ConnectorViewModel _partialConnection;

        /// <summary>
        /// 缓存点击模块四个点
        /// </summary>
        private List<Connector> _connectorsHit;

        /// <summary>
        /// 鼠标在canvas界面点击的位置
        /// </summary>
        private Point? _rubberbandSelectionStartPoint = null;

        private Connector _sourceConnector;

        /// <summary>
        /// 第一个点击的模块四角点
        /// </summary>
        public Connector SourceConnector
        {
            get => _sourceConnector;
            set
            {
                if (_sourceConnector != value)
                {
                    _sourceConnector = value;

                    AddLine(_sourceConnector);
                }
            }
        }

        #region Dependency

        public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register(
            "ShowGridLines", typeof(bool), typeof(DesignerCanvas), new FrameworkPropertyMetadata(default(bool), OnGridLinesPropertyChange));

        private static void OnGridLinesPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DesignerCanvas canvas)
            {
                if (e.NewValue is bool isShowGridLine)
                {
                    GridLinesToBack(canvas, isShowGridLine);
                }
            }
        }

        /// <summary>
        /// 显示表格
        /// </summary>
        public bool ShowGridLines
        {
            get => (bool)GetValue(ShowGridLinesProperty);
            set => SetValue(ShowGridLinesProperty, value);
        }

        #endregion Dependency

        #endregion Filed

        #region Event

        public Action<string> OpenAction { get; set; }

        public Action<string> SaveAction { get; set; }

        public Action Saving { get; set; }

        #endregion Event

        #region Construstor

        public DesignerCanvas()
        {
            Init();
        }

        #endregion Construstor

        #region Function

        private void Init()
        {
            InitCommand();
            InitCollection();
        }

        private void InitCollection()
        {
            _connectorsHit = new List<Connector>();
            MoveStack = new Stack<MoveInfo>();
            _deleteStack = new Stack<SelectableDesignerItemViewModelBase>();
            _reStack = new Stack<object>();
        }

        private void AddLine(Connector sourceConnector)
        {
            if (sourceConnector != null && sourceConnector.DataContext is BaseClass.Connectors.Connector sourceDataItem)
            {
                Rect rectangleBounds = sourceConnector.TransformToVisual(this).TransformBounds(new Rect(sourceConnector.RenderSize));

                Point point = new Point(rectangleBounds.Left + (rectangleBounds.Width / 2),
                    rectangleBounds.Bottom + (rectangleBounds.Height / 2));

                var partConnector = new PartConnector(point);

                _partialConnection = new ConnectorViewModel(new DesignerItemData(sourceDataItem, partConnector));

                sourceDataItem.DesignerItem.Parent.AddItemCommand.Execute(_partialConnection);
            }
        }

        private void HitTesting(Point hitPoint)
        {
            if (InputHitTest(hitPoint) is DependencyObject hitObject)
            {
                while (hitObject != null && hitObject.GetType() != typeof(DesignerCanvas))
                {
                    if (hitObject is Connector connector)
                    {
                        if (!_connectorsHit.Contains(connector))
                            _connectorsHit.Add(connector);
                    }
                    hitObject = VisualTreeHelper.GetParent(hitObject);
                }
            }
        }

        #endregion Function
    }
}