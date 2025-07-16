using System.Windows;
using System.Windows.Controls;

namespace ValoCord.Handlers;

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

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(SpacingSetter), new UIPropertyMetadata(0.0, SpacingChangedCallback));

    private static void SpacingChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel) return;
        
        panel.Loaded += (s, args) =>
        {
            ApplySpacing(panel);
        };
    }

    private static void ApplySpacing(Panel panel)
    {
        if (panel is not StackPanel stackPanel) return;

        double spacing = GetSpacing(panel);

        for (int i = 0; i < stackPanel.Children.Count; i++)
        {
            if (stackPanel.Children[i] is not FrameworkElement child) continue;
            if (i >= stackPanel.Children.Count - 1)
            {
                continue;
            }
            child.Margin = stackPanel.Orientation == Orientation.Horizontal
                ? new Thickness(0, 0, spacing, 0)
                : new Thickness(0, 0, 0, spacing);
        }
    }
}

public static class GridSpacer
{
    #region RowSpacing Property

    public static readonly DependencyProperty RowSpacingProperty =
        DependencyProperty.RegisterAttached("RowSpacing", typeof(double), typeof(GridSpacer),
            new PropertyMetadata(0.0, OnSpacingChanged));

    public static double GetRowSpacing(DependencyObject obj) => (double)obj.GetValue(RowSpacingProperty);
    public static void SetRowSpacing(DependencyObject obj, double value) => obj.SetValue(RowSpacingProperty, value);

    #endregion

    #region ColumnSpacing Property

    public static readonly DependencyProperty ColumnSpacingProperty =
        DependencyProperty.RegisterAttached("ColumnSpacing", typeof(double), typeof(GridSpacer),
            new PropertyMetadata(0.0, OnSpacingChanged));

    public static double GetColumnSpacing(DependencyObject obj) => (double)obj.GetValue(ColumnSpacingProperty);
    public static void SetColumnSpacing(DependencyObject obj, double value) => obj.SetValue(ColumnSpacingProperty, value);

    #endregion

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Grid grid) return;

        grid.Loaded -= OnGridLoaded;
        grid.Loaded += OnGridLoaded;
    }

    private static void OnGridLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Grid grid) return;
        
        double rowSpacing = GetRowSpacing(grid);
        double colSpacing = GetColumnSpacing(grid);
        
        var children = grid.Children.Cast<UIElement>().ToList();

        if (rowSpacing > 0 && grid.RowDefinitions.Count > 1)
        {
            for (int i = grid.RowDefinitions.Count - 1; i > 0; i--)
            {
                grid.RowDefinitions.Insert(i, new RowDefinition { Height = new GridLength(rowSpacing) });
                foreach (var child in children.Where(ch => Grid.GetRow(ch) >= i))
                {
                    Grid.SetRow(child, Grid.GetRow(child) + 1);
                }
            }
        }
        
        if (colSpacing > 0 && grid.ColumnDefinitions.Count > 1)
        {
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