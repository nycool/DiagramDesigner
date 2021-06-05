using DiagramDesigner.DesignerItemViewModel;
using DiagramDesigner.Interface;
using DiagramDesigner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Win.Lib;

namespace WpfApp3
{
    public class Window1ViewModel : DiagramViewModelBase, ILoadCmdLineArgs
    {
        #region Filed

        private List<ToolBoxItemInfo> _toolBoxDatas;

        public List<ToolBoxItemInfo> ToolBoxItems
        {
            get => _toolBoxDatas;
            set => SetProperty(ref _toolBoxDatas, value);
        }

        #endregion Filed

        public Window1ViewModel()
        {
            Init();
        }

        private new void Init()
        {
            InitCollection();
        }

        private void InitCollection()
        {
            ToolBoxItems = new List<ToolBoxItemInfo>();

            string str = @"pack://application:,,,/WpfApp3;component/Images/Setting.png";

            var into = new ToolBoxItemInfo("tool", "tol", str, typeof(SettingsDesignerItemViewModel));
            ToolBoxItems.Add(into);

            str = @"pack://application:,,,/WpfApp3;component/Images/Persist.png";

            into = new ToolBoxItemInfo("tool", "tol", str, typeof(PersistDesignerItemViewModel));
            ToolBoxItems.Add(into);
        }

        private ILoadXmlFile _canvas;

        public ILoadXmlFile Canvas
        {
            get => _canvas;
            set => SetProperty(ref _canvas, value);
        }

        public void LoadCmdLineArgs(string[] args)
        {
            if (args.Any())
            {
                Canvas?.LoadXml(this, args[0]);
                //MessageBox.Show(args[0]);
            }
        }
    }
}