using System;
using System.Windows;
using System.Windows.Controls;
using DiagramDesigner.BaseClass.DesignerItemViewModel;

namespace DiagramDesigner.StyleSelectors
{
    public class DesignerItemsStyleSelector : StyleSelector
    {
       
        private static DesignerItemsStyleSelector _instance;

        public static DesignerItemsStyleSelector Instance =>
            _instance ??= new DesignerItemsStyleSelector();


        public override Style SelectStyle(object item, DependencyObject container)
        {
            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            if (itemsControl == null)
                throw new InvalidOperationException("DesignerItemsControlItemStyleSelector : Could not find ItemsControl");

            if (item is DesignerItemViewModelBase)
            {

                return (Style)itemsControl.FindResource("DesignerItemStyle");
            }

            if (item is ConnectorViewModel)
            {
                return (Style)itemsControl.FindResource("ConnectorItemStyle");
            }

            return null;
        }
    }
}
