using System.Windows;
using System.Windows.Input;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;

namespace DiagramDesigner.AttachedProperties
{
    public static class SelectionProps
    {
        #region EnabledForSelection

        public static readonly DependencyProperty EnabledForSelectionProperty =
            DependencyProperty.RegisterAttached("EnabledForSelection", typeof(bool), typeof(SelectionProps),
                new FrameworkPropertyMetadata((bool)false,
                    OnEnabledForSelectionChanged));

        public static bool GetEnabledForSelection(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledForSelectionProperty);
        }

        public static void SetEnabledForSelection(DependencyObject d, bool value)
        {
            d.SetValue(EnabledForSelectionProperty, value);
        }

        private static void OnEnabledForSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if ((bool)e.NewValue)
            {
                fe.PreviewMouseDown += Fe_PreviewMouseDown;

                fe.SizeChanged += Fe_SizeChanged;
            }
            else
            {
                fe.PreviewMouseDown -= Fe_PreviewMouseDown;
                fe.SizeChanged -= Fe_SizeChanged;
            }
        }

        private static void Fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DesignerItemViewModelBase designerItem)
            {
                designerItem.ActualWidth = element.ActualWidth;
                designerItem.ActualHeight = element.ActualHeight;
            }
        }

        #endregion EnabledForSelection

        private static void Fe_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement framework && framework.DataContext is SelectableDesignerItemViewModelBase selectableDesignerItemViewModelBase)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
                    {
                        selectableDesignerItemViewModelBase.IsSelected = !selectableDesignerItemViewModelBase.IsSelected;
                    }

                    if ((Keyboard.Modifiers & (ModifierKeys.Control)) != ModifierKeys.None)
                    {
                        selectableDesignerItemViewModelBase.IsSelected = !selectableDesignerItemViewModelBase.IsSelected;
                    }
                }
                else if (!selectableDesignerItemViewModelBase.IsSelected)
                {
                    foreach (SelectableDesignerItemViewModelBase item in selectableDesignerItemViewModelBase.Parent.SelectedItems)
                    {
                        if (item is IDiagramViewModel diagramVim)
                        {
                            foreach (SelectableDesignerItemViewModelBase gItem in diagramVim.ItemsSource)
                            {
                                gItem.IsSelected = false;
                            }
                        }
                        if (selectableDesignerItemViewModelBase.Parent is SelectableDesignerItemViewModelBase designerItem)
                        {
                            designerItem.IsSelected = false;
                        }
                        item.IsSelected = false;
                    }
                    if (selectableDesignerItemViewModelBase is IDiagramViewModel diagramViewModel)
                    {
                        foreach (SelectableDesignerItemViewModelBase gItem in diagramViewModel.ItemsSource)
                        {
                            gItem.IsSelected = false;
                        }
                    }
                    selectableDesignerItemViewModelBase.Parent.SelectedItems.Clear();
                    selectableDesignerItemViewModelBase.IsSelected = true;
                }
            }
        }
    }
}