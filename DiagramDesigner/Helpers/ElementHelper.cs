using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagramDesigner.Helpers
{
    public class ElementHelper
    {
        /// <summary>
        /// 获取列表控件鼠标点所在的子控件
        /// </summary>
        /// <param name="itemsControl">列表控件</param>
        /// <param name="point">鼠标点坐标</param>
        /// <returns>鼠标点所在子控件，null为没有子控件</returns>
        public static object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                {
                    return null;
                }

                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                {
                    return item;
                }

                element = (UIElement)VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        /// <summary>
        /// 查找父控件
        /// </summary>
        /// <typeparam name="T">父控件的类型</typeparam>
        /// <param name="obj">要找的是obj的父控件</param>
        /// <param name="name">想找的父控件的Name属性</param>
        /// <returns>目标父控件</returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            if (string.IsNullOrEmpty(name))
            {
                while (parent != null)
                {
                    if (parent is T t)
                    {
                        return t;
                    }

                    // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
            else
            {
                while (parent != null)
                {
                    if (parent is T element && element.Name == name)
                    {
                        return element;
                    }

                    // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }

        /// <summary>
        /// 查找子控件
        /// </summary>
        /// <typeparam name="T">子控件的类型</typeparam>
        /// <param name="obj">要找的是obj的子控件</param>
        /// <param name="name">想找的子控件的Name属性</param>
        /// <returns>目标子控件</returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            if (string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    child = VisualTreeHelper.GetChild(obj, i);

                    if (child is T t)
                    {
                        return t;
                    }

                    // 在下一级中没有找到指定名字的子控件，就再往下一级找
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            else
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    child = VisualTreeHelper.GetChild(obj, i);

                    if (child is T element && element.Name == name)
                    {
                        return element;
                    }

                    // 在下一级中没有找到指定名字的子控件，就再往下一级找
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有同一类型的子控件
        /// </summary>
        /// <typeparam name="T">子控件的类型</typeparam>
        /// <param name="obj">要找的是obj的子控件集合</param>
        /// <param name="name">想找的子控件的Name属性</param>
        /// <returns>子控件集合</returns>
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            if (string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    child = VisualTreeHelper.GetChild(obj, i);

                    if (child is T t)
                    {
                        childList.Add(t);
                    }

                    childList.AddRange(GetChildObjects<T>(child, name));
                }
            }
            else
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    child = VisualTreeHelper.GetChild(obj, i);

                    if (child is T t && t.Name == name)
                    {
                        childList.Add(t);
                    }

                    childList.AddRange(GetChildObjects<T>(child, name));
                }
            }

            return childList;
        }

        /// <summary>
        /// 查找指定类型的子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : FrameworkElement
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child is T t)
                        yield return t;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        /// <summary>
        /// 查找指定类型的父控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static T FindVisualParent<T>(DependencyObject depObj) where T : FrameworkElement
        {
            if (depObj is Visual)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);
                while (parent != null)
                {
                    if (parent is T t)
                    {
                        return t;
                    }

                    // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }

        /// <summary>
        /// 根据类的熟悉名称获取对应的熟悉的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mClass">类的实例对象</param>
        /// <param name="propertyName">类中属性的名称</param>
        /// <returns></returns>
        public static object GetValue<T>(T mClass, string propertyName) where T : class
        {
            return mClass.GetType().GetProperty(propertyName)?.GetValue(mClass, null);
        }

        /// <summary>
        /// 根据类的熟悉名称获取对应的熟悉的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mTClass">类的实例对象</param>
        /// <param name="propertyName">类中属性的名称</param>
        /// <returns></returns>
        public static IEnumerable<object> GetValues<T>(IEnumerable<T> mTClass, string propertyName) where T : class
        {
            var t = typeof(T);
            foreach (var item in mTClass)
            {
                yield return t.GetProperty(propertyName)?.GetValue(item, null);
            }
        }
    }
}