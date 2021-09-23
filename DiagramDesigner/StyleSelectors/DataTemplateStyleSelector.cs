using System;
using DiagramDesigner.DesignerItemViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner.StyleSelectors
{
    public class DataTemplateStyleSelector : DataTemplateSelector
    {
        private static DataTemplateStyleSelector _instance;

        public static DataTemplateStyleSelector Instance => _instance ??= new DataTemplateStyleSelector();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //if (container is FrameworkElement element)
            //{
            //    if (item is GroupDesignerItemViewModelBase)
            //    {
            //        return (DataTemplate)element.FindResource("GroupDesignerContent");
            //    }

            //    if (item is DesignerItemViewModelBase)
            //    {
            //        return (DataTemplate)element.FindResource("DesignerItemContentTemplate");
            //    }
            //}

            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            if (itemsControl == null)
                throw new InvalidOperationException("DesignerItemsControlItemStyleSelector : Could not find ItemsControl");

            if (item is GroupDesignerItemViewModelBase)
            {
                return (DataTemplate)itemsControl.FindResource("GroupDesignerContent");
            }

            if (item is DesignerItemViewModelBase)
            {
                return (DataTemplate)itemsControl.FindResource("DesignerItemContent");
            }

            return null;
        }
    }
}