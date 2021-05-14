using Prism.Mvvm;

namespace WpfApp3.Db.Models
{
    /// <summary>
    /// This is passed to the PopupWindow.xaml window, where a DataTemplate is used to provide the
    /// ContentControl with the look for this data. This class is also used to allow
    /// the popup to be cancelled without applying any changes to the calling ViewModel
    /// whos data will be updated if the PopupWindow.xaml window is closed successfully
    /// </summary>
    public class PersistDesignerItemData: BindableBase
    {
        private string hostUrl = "";

        public PersistDesignerItemData(string currentHostUrl)
        {
            hostUrl = currentHostUrl;
        }

        public string HostUrl
        {
            get => hostUrl;
            set => SetProperty(ref hostUrl, value);
        }
    }
}
