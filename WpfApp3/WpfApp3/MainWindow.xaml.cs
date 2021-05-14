using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiagramDesigner.BaseClass.Interface;
using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window1ViewModel window1ViewModel;
        public MainWindow()
        {
            InitializeComponent();

            window1ViewModel = new Window1ViewModel();
            this.DataContext = window1ViewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = DataContext as IDiagramViewModel;

            SettingsDesignerItemViewModel item1 = new SettingsDesignerItemViewModel();
            item1.Parent = parent;
            item1.Left = 100;
            item1.Top = 100;
            window1ViewModel.ItemsSource.Add(item1);

            PersistDesignerItemViewModel item2 = new PersistDesignerItemViewModel(Guid.NewGuid());
            item2.Parent = parent;
            item2.Left = 300;
            item2.Top = 300;
            window1ViewModel.ItemsSource.Add(item2);

            ConnectorViewModel con1 = new ConnectorViewModel(item1.RightConnector, item2.TopConnector);
            con1.Parent = parent;
            window1ViewModel.ItemsSource.Add(con1);
        }
    }
}
