using System;
using System.Collections.Generic;
using DiagramDesigner.BaseClass.ConnectorClass;
using DiagramDesigner.BaseClass.Interface;

namespace DiagramDesigner.BaseClass.DesignerItemViewModel
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

        #endregion


        #region Construstor

        protected DesignerItemViewModelBase(Guid id, IDiagramViewModel parent, double left, double top)
            : base(id, parent)
        {
            this._left = left;
            this._top = top;
            Init();
        }

        protected DesignerItemViewModelBase(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight)
            : base(id, parent)
        {
            this._left = left;
            this._top = top;
            this._itemWidth = itemWidth;
            this._itemHeight = itemHeight;

            Init();
        }

        protected DesignerItemViewModelBase()
        : base(Guid.NewGuid())
        {
            Init();
        }
        #endregion



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

        #endregion



    }
}
