using DiagramDesigner.DesignerItemViewModel;
using System.Windows;
using System.Windows.Input;

namespace DiagramDesigner.AttachedProperties
{
    public static class ItemConnectProps
    {
        #region EnabledForConnection

        public static readonly DependencyProperty EnabledForConnectionProperty =
            DependencyProperty.RegisterAttached("EnabledForConnection", typeof(bool), typeof(ItemConnectProps),
                new FrameworkPropertyMetadata((bool)false,
                    OnEnabledForConnectionChanged));

        public static bool GetEnabledForConnection(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledForConnectionProperty);
        }

        public static void SetEnabledForConnection(DependencyObject d, bool value)
        {
            d.SetValue(EnabledForConnectionProperty, value);
        }

        private static void OnEnabledForConnectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;

            if ((bool)e.NewValue)
            {
                fe.MouseEnter += Fe_MouseEnter;

                //fe.MouseLeave += Fe_MouseLeave;
            }
            else
            {
                fe.MouseEnter -= Fe_MouseEnter;
                //fe.MouseLeave -= Fe_MouseLeave;
            }
        }

        private static void Fe_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement framework)
            {
                if (framework.DataContext is DesignerItemViewModelBase designerItem)
                {
                    designerItem.ShowConnectors = false;
                }
            }
        }

        #endregion EnabledForConnection

        private static void Fe_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement framework)
            {
                if (framework.DataContext is DesignerItemViewModelBase designerItem)
                {
                    designerItem.ShowConnectors = true;
                }
            }
        }
    }
}