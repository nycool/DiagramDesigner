using Prism.Mvvm;

namespace WpfApp3.Db.Models
{
    /// <summary>
    /// This is passed to the PopupWindow.xaml window, where a DataTemplate is used to provide the
    /// ContentControl with the look for this data. This class is also used to allow
    /// the popup to be cancelled without applying any changes to the calling ViewModel
    /// whos data will be updated if the PopupWindow.xaml window is closed successfully
    /// </summary>
    public class SettingsDesignerItemData : BindableBase
    {
        private string setting1 = "";

        public SettingsDesignerItemData(string currentSetting1)
        {
            setting1 = currentSetting1;
        }


        public string Setting1
        {
            get
            {
                return setting1;
            }
            set => SetProperty(ref setting1, value);
        }
    }
}
