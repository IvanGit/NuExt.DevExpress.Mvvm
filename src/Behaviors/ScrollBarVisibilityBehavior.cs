using DevExpress.Mvvm.UI.Interactivity;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DevExpress.Mvvm.UI
{
    [TargetType(typeof(Control))]
    public class ScrollBarVisibilityBehavior : EventTriggerBase<Control>
    {
        public ScrollBarVisibilityBehavior()
        {
            Event = ScrollViewer.ScrollChangedEvent;
        }

        protected override void OnEvent(object sender, object eventArgs)
        {
            base.OnEvent(sender, eventArgs);
            if (eventArgs is ScrollChangedEventArgs { OriginalSource: ScrollViewer sv })
            {
                SetProperties(sv);
            }
        }

        private void SetProperties(ScrollViewer sv)
        {
            if (!IsAttached) return;
            ScrollViewerHelper.SetComputedHorizontalScrollBarVisibility(AssociatedObject, sv.ComputedHorizontalScrollBarVisibility);
            ScrollViewerHelper.SetComputedVerticalScrollBarVisibility(AssociatedObject, sv.ComputedVerticalScrollBarVisibility);

            var horizontalScrollBar = sv.Template.FindName("PART_HorizontalScrollBar", sv) as ScrollBar;
            var verticalScrollBar = sv.Template.FindName("PART_VerticalScrollBar", sv) as ScrollBar;
            Debug.Assert(horizontalScrollBar != null);
            Debug.Assert(verticalScrollBar != null);

            ScrollViewerHelper.SetHorizontalScrollBarActualHeight(AssociatedObject, horizontalScrollBar?.ActualHeight ?? 0);
            ScrollViewerHelper.SetHorizontalScrollBarActualWidth(AssociatedObject, horizontalScrollBar?.ActualWidth ?? 0);

            ScrollViewerHelper.SetVerticalScrollBarActualHeight(AssociatedObject, verticalScrollBar?.ActualHeight ?? 0);
            ScrollViewerHelper.SetVerticalScrollBarActualWidth(AssociatedObject, verticalScrollBar?.ActualWidth ?? 0);
        }
    }
}
