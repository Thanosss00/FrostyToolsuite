using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using FrostySdk.Interfaces;

namespace FilterType.Common
{
    /// <summary>
    /// Injects an icon type filter into FrostyDataExplorer's toolbar,
    /// next to Show Only Modified checkbox. 
    /// It has the same filter Frosty already has (type:AssetTypeName)
    /// </summary>
    public static class DataExplorerFilterHook
    {
        private static bool s_installed;

        public static void Install(ILogger logger)
        {
            if (s_installed) return;

            FrameworkElement dataExplorer = null;
            foreach (Window window in Application.Current.Windows)
            {
                FrameworkElement found = window.FindName("dataExplorer") as FrameworkElement;
                if (found != null)
                {
                    dataExplorer = found;
                    break;
                }
            }

            if (dataExplorer == null)
            {
                logger?.Log("Could not find 'dataExplorer' in any open window yet, retrying.");
                RetryInstall(logger, 0);
                return;
            }
            StartInjectLoop(dataExplorer, logger);
        }

        private static void RetryInstall(ILogger logger, int attempt)
        {
            if (attempt > 15) // ~4.5 seconds max
            {
                logger?.Log("Gave up looking for the main editor window.");
                return;
            }

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                if (s_installed) return;

                foreach (Window window in Application.Current.Windows)
                {
                    FrameworkElement found = window.FindName("dataExplorer") as FrameworkElement;
                    if (found != null)
                    {
                        StartInjectLoop(found, logger);
                        return;
                    }
                }
                RetryInstall(logger, attempt + 1);
            };
            timer.Start();
        }

        private static void StartInjectLoop(FrameworkElement dataExplorer, ILogger logger)
        {
            int attempts = 0;
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            timer.Tick += (s, e) =>
            {
                attempts++;
                if (TryInject(dataExplorer, logger) || attempts > 15) // ~4.5 seconds max
                {
                    timer.Stop();
                    if (attempts > 15 && !s_installed)
                        logger?.Log("Gave up waiting for Data Explorer's toolbar to load.");
                }
            };
            timer.Start();
        }

        private static bool TryInject(FrameworkElement dataExplorer, ILogger logger)
        {
            CheckBox modifiedCheckBox = VisualTreeHelperEx.FindChild<CheckBox>(dataExplorer, "PART_ShowOnlyModifiedCheckBox");
            TextBox filterTextBox = VisualTreeHelperEx.FindChild<TextBox>(dataExplorer, "PART_FilterTextBox");

            if (modifiedCheckBox == null || filterTextBox == null)
                return false;

            if (!(modifiedCheckBox.Parent is Panel parentPanel))
            {
                logger?.Log("Found the checkbox but its parent isn't a Panel - can't insert next to it.");
                return true;
            }

            Polygon funnelIcon = new Polygon
            {
                Points = new PointCollection(new[]
                {
                    new Point(0, 0), new Point(16, 0), new Point(9.5, 8),
                    new Point(9.5, 15), new Point(6.5, 13), new Point(6.5, 8)
                }),
                Fill = Brushes.White,
                Width = 16,
                Height = 15,
                Stretch = Stretch.Uniform
            };

            Button filterButton = new Button
            {
                Content = funnelIcon,
                Width = 26,
                Height = 20,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "Filter by type"
            };

            // The ListBox sizes itself to its widest item's natural width,
            // so every type name shows in full, no horizontal scrolling needed
            ListBox typeList = new ListBox
            {
                ItemsSource = new[] { FilterRegistry.AnyType }
                    .Concat(FilterRegistry.Types.Where(t => t != FilterRegistry.AnyType).OrderBy(t => t.DisplayName))
                    .ToList(),
                MaxHeight = 420,
                Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x2A)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0)
            };

            Popup popup = new Popup
            {
                PlacementTarget = filterButton,
                Placement = PlacementMode.Bottom,
                StaysOpen = false,
                Child = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x2A)),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Child = typeList
                }
            };

            filterButton.Click += (s, e) => { popup.IsOpen = !popup.IsOpen; };
            typeList.SelectionChanged += (s, e) =>
            {
                if (!(typeList.SelectedItem is AssetTypeFilter selected)) return;
                filterTextBox.Text = string.IsNullOrEmpty(selected.EbxType) ? "" : "type:" + selected.EbxType;
                filterTextBox.Focus();
                Keyboard.ClearFocus();
                popup.IsOpen = false;
                filterButton.ToolTip = string.IsNullOrEmpty(selected.EbxType)
                    ? "Filter by type"
                    : "Filter: " + selected.DisplayName;
            };
            int index = parentPanel.Children.IndexOf(modifiedCheckBox);
            parentPanel.Children.Insert(index + 1, filterButton);
            s_installed = true;
            logger?.Log("Type filter added to Data Explorer toolbar.");
            return true;
        }
    }
}