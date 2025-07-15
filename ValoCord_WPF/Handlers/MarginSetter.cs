using System.Windows;
using System.Windows.Controls;

namespace ValoCord_WPF.Handlers;

public class MarginSetter {

    public static Thickness GetMargin(DependencyObject obj)

    {
        return (Thickness) obj.GetValue(MarginProperty);
    }

    public static void SetMargin(DependencyObject obj, Thickness value)

    {
        obj.SetValue(MarginProperty, value);
    }

    // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...

    public static readonly DependencyProperty MarginProperty =

        DependencyProperty.RegisterAttached("Margin", typeof (Thickness), typeof (MarginSetter), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

    public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)

    {

        // Make sure this is put on a panel

        var panel = sender as Panel;
        if (panel == null) return;
        panel.Loaded += new RoutedEventHandler(panel_Loaded);

    }

    static void panel_Loaded(object sender, RoutedEventArgs e)

    {

        var panel = sender as Panel;

        // Go over the children and set margin for them:
        foreach(var child in panel.Children)

        {

            var fe = child as FrameworkElement;
            if (fe == null) continue;
            fe.Margin = MarginSetter.GetMargin(panel);

        }

    }
}

    public class SpacingSetter
    {
        public static double GetSpacing(DependencyObject obj)
        {
            return (double)obj.GetValue(SpacingProperty);
        }

        public static void SetSpacing(DependencyObject obj, double value)
        {
            obj.SetValue(SpacingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Spacing.
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(SpacingSetter), new UIPropertyMetadata(0.0, SpacingChangedCallback));

        public static void SpacingChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make sure this is put on a Panel
            if (sender is not Panel panel) return;

            // Defer the logic until the panel and its children are loaded
            panel.Loaded += (s, args) =>
            {
                ApplySpacing(panel);
            };
        }

        private static void ApplySpacing(Panel panel)
        {
            // This logic is specifically for StackPanel
            if (panel is not StackPanel stackPanel) return;

            double spacing = GetSpacing(panel);

            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (stackPanel.Children[i] is not FrameworkElement child) continue;

                // Do not apply a margin to the last child
                if (i >= stackPanel.Children.Count - 1)
                {
                    // You might want to clear any existing margin on the last item
                    // child.Margin = new Thickness(0); 
                    continue;
                }

                // Apply margin based on the StackPanel's orientation
                child.Margin = stackPanel.Orientation == Orientation.Horizontal
                    ? new Thickness(0, 0, spacing, 0)  // Apply margin to the right
                    : new Thickness(0, 0, 0, spacing); // Apply margin to the bottom
            }
        }
    }
    
    public static class GridSpacingHelper
    {
        #region RowSpacing Property

        public static readonly DependencyProperty RowSpacingProperty =
            DependencyProperty.RegisterAttached("RowSpacing", typeof(double), typeof(GridSpacingHelper),
                new PropertyMetadata(0.0, OnSpacingChanged));

        public static double GetRowSpacing(DependencyObject obj) => (double)obj.GetValue(RowSpacingProperty);
        public static void SetRowSpacing(DependencyObject obj, double value) => obj.SetValue(RowSpacingProperty, value);

        #endregion

        #region ColumnSpacing Property

        public static readonly DependencyProperty ColumnSpacingProperty =
            DependencyProperty.RegisterAttached("ColumnSpacing", typeof(double), typeof(GridSpacingHelper),
                new PropertyMetadata(0.0, OnSpacingChanged));

        public static double GetColumnSpacing(DependencyObject obj) => (double)obj.GetValue(ColumnSpacingProperty);
        public static void SetColumnSpacing(DependencyObject obj, double value) => obj.SetValue(ColumnSpacingProperty, value);

        #endregion

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid grid) return;

            // Use the Loaded event to ensure all children and definitions are ready
            grid.Loaded -= OnGridLoaded; // Unsubscribe to prevent multiple subscriptions
            grid.Loaded += OnGridLoaded;
        }

        private static void OnGridLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Grid grid) return;

            // Get the spacing values
            double rowSpacing = GetRowSpacing(grid);
            double colSpacing = GetColumnSpacing(grid);

            // Get all children to update their row/column indices later
            var children = grid.Children.Cast<UIElement>().ToList();

            // --- Apply Row Spacing ---
            if (rowSpacing > 0 && grid.RowDefinitions.Count > 1)
            {
                // Insert new spacer rows and update child row indices
                for (int i = grid.RowDefinitions.Count - 1; i > 0; i--)
                {
                    grid.RowDefinitions.Insert(i, new RowDefinition { Height = new GridLength(rowSpacing) });
                    foreach (var child in children.Where(ch => Grid.GetRow(ch) >= i))
                    {
                        Grid.SetRow(child, Grid.GetRow(child) + 1);
                    }
                }
            }

            // --- Apply Column Spacing ---
            if (colSpacing > 0 && grid.ColumnDefinitions.Count > 1)
            {
                // Insert new spacer columns and update child column indices
                for (int i = grid.ColumnDefinitions.Count - 1; i > 0; i--)
                {
                    grid.ColumnDefinitions.Insert(i, new ColumnDefinition { Width = new GridLength(colSpacing) });
                    foreach (var child in children.Where(ch => Grid.GetColumn(ch) >= i))
                    {
                        Grid.SetColumn(child, Grid.GetColumn(child) + 1);
                    }
                }
            }
        }
    }