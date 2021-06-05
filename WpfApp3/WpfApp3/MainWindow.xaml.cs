using System.Windows;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Window1ViewModel window1ViewModel;

        public MainWindow()
        {
            InitializeComponent();

            window1ViewModel = new Window1ViewModel();
            window1ViewModel.Canvas = DiagramView.DesignerCanvas;
            this.DataContext = window1ViewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            window1ViewModel.Canvas = DiagramView.DesignerCanvas;
            //var parent = DataContext as IDiagramViewModel;

            //SettingsDesignerItemViewModel item1 = new SettingsDesignerItemViewModel();
            //item1.Parent = parent;
            //item1.Left = 100;
            //item1.Top = 100;
            //item1.Id = Guid.NewGuid();
            //window1ViewModel.ItemsSource.Add(item1);

            //PersistDesignerItemViewModel item2 = new PersistDesignerItemViewModel();
            //item2.Parent = parent;
            //item2.Left = 300;
            //item2.Id = Guid.NewGuid();
            //item2.Top = 300;
            //window1ViewModel.ItemsSource.Add(item2);

            //ConnectorViewModel con1 = new ConnectorViewModel(new DesignerItemData(item1.RightConnector, item2.TopConnector));
            //con1.Parent = parent;
            //window1ViewModel.ItemsSource.Add(con1);
        }
    }
}