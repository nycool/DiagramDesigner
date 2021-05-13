using Prism.Mvvm;

namespace DiagramDesigner.BaseClass.ConnectorClass
{
    public abstract class ConnectorInfoBase : BindableBase
    {
        #region Filed

        private ConnectorOrientation _orientation;

        /// <summary>
        /// 线的方向
        /// </summary>
        public ConnectorOrientation Orientation
        {
            get => _orientation;
            set => SetProperty(ref _orientation, value);
        }

        public static double ConnectorWidth => 8;

        public static double ConnectorHeight => 8;

        #endregion Filed

        protected ConnectorInfoBase(ConnectorOrientation orientation)
        {
            this.Orientation = orientation;
        }
    }
}