using System.Windows.Controls;

namespace DevExpress.Mvvm.UI
{
    public static class TabControlHelper
    {
        public static void ClearStyle(this TabItem? tabItem)
        {
            if (tabItem is null)
            {
                return;
            }

            tabItem.Template = null;
            tabItem.Style = null;
        }
    }
}
