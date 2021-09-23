using Prism.Mvvm;

namespace DiagramDesigner.BaseClass.Connectors
{
    public abstract class ConnectorBase : BindableBase
    {
        #region Filed

        private ConnectorOrientation _orientation;

        /// <summary>
        /// 点线的方向
        /// </summary>
        public ConnectorOrientation Orientation
        {
            get => _orientation;
            set => SetProperty(ref _orientation, value);
        }

        public static double ConnectorWidth => 8;

        public static double ConnectorHeight => 8;

        #endregion Filed

        protected ConnectorBase(ConnectorOrientation orientation)
        {
            this.Orientation = orientation;
        }
    }
}