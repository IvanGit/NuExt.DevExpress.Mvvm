using System.Windows;
using System.Windows.Controls;

namespace DevExpress.Mvvm.UI
{
    public sealed class DialogButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ButtonTemplate { get; set; }

        public DataTemplate? DefaultButtonTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is UICommand { IsDefault: true })
            {
                return DefaultButtonTemplate;
            }

            return ButtonTemplate;
        }
    }
}
