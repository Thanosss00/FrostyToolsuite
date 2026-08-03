using System.Windows;
using System.Windows.Media;

namespace FilterType.Common
{
    public static class VisualTreeHelperEx
    {
        public static T FindChild<T>(DependencyObject parent, string name = null) where T : DependencyObject
        {
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typed && (name == null || (child as FrameworkElement)?.Name == name))
                    return typed;

                T result = FindChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}