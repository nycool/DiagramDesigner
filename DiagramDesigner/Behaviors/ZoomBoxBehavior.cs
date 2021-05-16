using System.Linq;
using DiagramDesigner.Controls;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using DiagramDesigner.Helpers;

namespace DiagramDesigner.Behaviors
{
    public class ZoomBoxBehavior : Behavior<ZoomBox>
    {
        public static readonly DependencyProperty PresenterProperty = DependencyProperty.Register(
            "Presenter", typeof(ItemsPresenter), typeof(ZoomBoxBehavior), new PropertyMetadata(default(ItemsPresenter)));

        public ItemsPresenter Presenter
        {
            get { return (ItemsPresenter)GetValue(PresenterProperty); }
            set { SetValue(PresenterProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (Presenter != null)
            {
                var canvas = ElementHelper.FindVisualChildren<DesignerCanvas>(Presenter).First();

                if (canvas != null)
                {
                    AssociatedObject.SetValue(ZoomBox.DesignerCanvasProperty, canvas);
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}