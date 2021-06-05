using DiagramDesigner.Controls;
using DiagramDesigner.Custom;
using DiagramDesigner.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace DiagramDesigner.Behaviors
{
    public class DiagramViewBehavior : Behavior<DiagramView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var canvas = ElementHelper.FindVisualChildrenFirst<DesignerCanvas>(sender as FrameworkElement);

            if (canvas is { })
            {
                AssociatedObject.DesignerCanvas = canvas;
                e.Handled = true;
            }
        }
    }
}
