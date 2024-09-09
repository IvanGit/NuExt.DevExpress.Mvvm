using System.Windows;

namespace DevExpress.Mvvm.UI
{
    public class ScrollViewerHelper
    {
        #region ComputedHorizontalScrollBarVisibility

        private static readonly DependencyPropertyKey ComputedHorizontalScrollBarVisibilityPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewerHelper), new UIPropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = ComputedHorizontalScrollBarVisibilityPropertyKey.DependencyProperty;

        public static Visibility GetComputedHorizontalScrollBarVisibility(DependencyObject element)
        {
            return (Visibility)element.GetValue(ComputedHorizontalScrollBarVisibilityProperty);
        }

        internal static void SetComputedHorizontalScrollBarVisibility(DependencyObject element, Visibility value)
        {
            element.SetValue(ComputedHorizontalScrollBarVisibilityPropertyKey, value);
        }

        #endregion

        #region ComputedVerticalScrollBarVisibility

        private static readonly DependencyPropertyKey ComputedVerticalScrollBarVisibilityPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewerHelper), new UIPropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = ComputedVerticalScrollBarVisibilityPropertyKey.DependencyProperty;

        public static Visibility GetComputedVerticalScrollBarVisibility(DependencyObject element)
        {
            return (Visibility)element.GetValue(ComputedVerticalScrollBarVisibilityProperty);
        }

        internal static void SetComputedVerticalScrollBarVisibility(DependencyObject element, Visibility value)
        {
            element.SetValue(ComputedVerticalScrollBarVisibilityPropertyKey, value);
        }

        #endregion

        #region HorizontalScrollBarActualHeight

        private static readonly DependencyPropertyKey HorizontalScrollBarActualHeightPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("HorizontalScrollBarActualHeight", typeof(double), typeof(ScrollViewerHelper), new UIPropertyMetadata(0d));

        public static readonly DependencyProperty HorizontalScrollBarActualHeightProperty = HorizontalScrollBarActualHeightPropertyKey.DependencyProperty;

        public static double GetHorizontalScrollBarActualHeight(DependencyObject element)
        {
            return (double)element.GetValue(HorizontalScrollBarActualHeightProperty);
        }

        internal static void SetHorizontalScrollBarActualHeight(DependencyObject element, double value)
        {
            element.SetValue(HorizontalScrollBarActualHeightPropertyKey, value);
        }

        #endregion

        #region HorizontalScrollBarActualWidth

        private static readonly DependencyPropertyKey HorizontalScrollBarActualWidthPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("HorizontalScrollBarActualWidth", typeof(double), typeof(ScrollViewerHelper), new UIPropertyMetadata(0d));

        public static readonly DependencyProperty HorizontalScrollBarActualWidthProperty = HorizontalScrollBarActualWidthPropertyKey.DependencyProperty;

        public static double GetHorizontalScrollBarActualWidth(DependencyObject element)
        {
            return (double)element.GetValue(HorizontalScrollBarActualWidthProperty);
        }

        internal static void SetHorizontalScrollBarActualWidth(DependencyObject element, double value)
        {
            element.SetValue(HorizontalScrollBarActualWidthPropertyKey, value);
        }

        #endregion

        #region VerticalScrollBarActualHeight

        private static readonly DependencyPropertyKey VerticalScrollBarActualHeightPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("VerticalScrollBarActualHeight", typeof(double), typeof(ScrollViewerHelper), new UIPropertyMetadata(0d));

        public static readonly DependencyProperty VerticalScrollBarActualHeightProperty = VerticalScrollBarActualHeightPropertyKey.DependencyProperty;

        public static double GetVerticalScrollBarActualHeight(DependencyObject element)
        {
            return (double)element.GetValue(VerticalScrollBarActualHeightProperty);
        }

        internal static void SetVerticalScrollBarActualHeight(DependencyObject element, double value)
        {
            element.SetValue(VerticalScrollBarActualHeightPropertyKey, value);
        }

        #endregion

        #region VerticalScrollBarActualWidth

        private static readonly DependencyPropertyKey VerticalScrollBarActualWidthPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("VerticalScrollBarActualWidth", typeof(double), typeof(ScrollViewerHelper), new UIPropertyMetadata(0d));

        public static readonly DependencyProperty VerticalScrollBarActualWidthProperty = VerticalScrollBarActualWidthPropertyKey.DependencyProperty;

        public static double GetVerticalScrollBarActualWidth(DependencyObject element)
        {
            return (double)element.GetValue(VerticalScrollBarActualWidthProperty);
        }

        internal static void SetVerticalScrollBarActualWidth(DependencyObject element, double value)
        {
            element.SetValue(VerticalScrollBarActualWidthPropertyKey, value);
        }

        #endregion
    }
}
