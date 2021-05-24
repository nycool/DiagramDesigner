﻿using DiagramDesigner.Controls;
using DiagramDesigner.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner.Custom
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DemoApp.Custom"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DemoApp.Custom;assembly=DemoApp.Custom"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:DiagramView/>
    ///
    /// </summary>

    [TemplatePart(Name = "DesignerScrollViewer", Type = typeof(ScrollViewer))]
    public class DiagramView : ItemsControl
    {
        static DiagramView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramView), new FrameworkPropertyMetadata(typeof(DiagramView)));
        }

        #region Construstor

        public DiagramView()
        {
            PreviewMouseLeftButtonDown += DiagramView_PreviewMouseLeftButtonDown;
        }

        #endregion Construstor

        #region Function

        private void DiagramView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DiagramView diagramView)
            {
                var canvas = DesignerCanvas;

                if (canvas == null)
                {
                    canvas = ElementHelper.FindVisualChildren<DesignerCanvas>(diagramView).First();

                    SetValue(DesignerCanvasProperty, canvas);
                }

                canvas.Focus();
            }

            e.Handled = false;
        }

        #endregion Function

        #region Dependiency

        public static readonly DependencyProperty DesignerCanvasProperty = DependencyProperty.Register(
            "DesignerCanvas", typeof(DesignerCanvas), typeof(DiagramView), new FrameworkPropertyMetadata(default(DesignerCanvas), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DesignerCanvas DesignerCanvas
        {
            get { return (DesignerCanvas)GetValue(DesignerCanvasProperty); }
            set { SetValue(DesignerCanvasProperty, value); }
        }

        #endregion Dependiency
    }
}