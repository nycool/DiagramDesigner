using DiagramDesigner.BaseClass;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.Interface;
using System;
using System.Collections.Generic;

namespace DiagramDesigner.DesignerItemViewModel
{
    public abstract class DesignerItemViewModelBase : SelectableDesignerItemViewModelBase
    {
        #region Filed

        private double _left;

        public double Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }

        private double _top;

        public double Top
        {
            get => _top;
            set => SetProperty(ref _top, value);
        }


        private int _zIndex;

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }


        private double _itemWidth = 65;

        /// <summary>
        /// DesignerItem的宽
        /// </summary>
        public double ItemWidth
        {
            get => _itemWidth;
            set => SetProperty(ref _itemWidth, value);
        }

        private double _itemHeight = 65;

        /// <summary>
        /// DesignerItem的高
        /// </summary>
        public double ItemHeight
        {
            get => _itemHeight;
            set => SetProperty(ref _itemHeight, value);
        }

        private bool _showConnectors = false;

        /// <summary>
        /// 是否显示四点
        /// </summary>
        public bool ShowConnectors
        {
            get => _showConnectors;
            set
            {
                if (SetProperty(ref _showConnectors, value))
                {
                    TopConnector.ShowConnectors = value;
                    BottomConnector.ShowConnectors = value;
                    RightConnector.ShowConnectors = value;
                    LeftConnector.ShowConnectors = value;
                }
            }
        }

        private List<FullyCreatedConnectorInfo> _connectors;

        public FullyCreatedConnectorInfo TopConnector => _connectors[0];

        public FullyCreatedConnectorInfo BottomConnector => _connectors[1];

        public FullyCreatedConnectorInfo LeftConnector => _connectors[2];

        public FullyCreatedConnectorInfo RightConnector => _connectors[3];

        #endregion Filed

        #region Construstor

        protected DesignerItemViewModelBase(Guid id, IDiagramViewModel parent, DesignerItemPosition position)
            : this(id, parent)
        {
            InitPosition(position);
        }

        public DesignerItemViewModelBase(Guid id, IDiagramViewModel parent)
        : this(id)
        {
            Parent = parent;
        }

        public DesignerItemViewModelBase(Guid id)
        : this()
        {
            Id = id;
        }

        public DesignerItemViewModelBase()
        {
            Init();
        }

        #endregion Construstor

        #region Function

        private void Init()
        {
            _connectors = new List<FullyCreatedConnectorInfo>
            {
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top),
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom),
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left),
                new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right)
            };
        }


        protected  void InitPosition(DesignerItemPosition position)
        {
            Left = position.Left;
            Top = position.Top;
            ItemWidth = position.Width;
            ItemHeight = position.Height;
        }

        #endregion Function
    }
}