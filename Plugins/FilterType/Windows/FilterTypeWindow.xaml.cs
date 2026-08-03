using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Frosty.Controls;
using Frosty.Core;
using FilterType.Common;

namespace FilterType.Windows
{
    public partial class FilterTypeWindow : FrostyDockableWindow
    {
        public FilterTypeWindow()
        {
            InitializeComponent();
            TypeBox.ItemsSource = new[] { FilterRegistry.AnyType }
            .Concat(FilterRegistry.Types
            .Where(t => t != FilterRegistry.AnyType)
            .OrderBy(t => t.DisplayName));
            TypeBox.SelectedItem = FilterRegistry.AnyType;
        }

        private void FilterChanged(object sender, System.EventArgs e)
        {
            RunFilter();
        }

        private void RunFilter()
        {
            AssetTypeFilter selected = TypeBox.SelectedItem as AssetTypeFilter ?? FilterRegistry.AnyType;
            bool modifiedOnly = ShowOnlyModifiedBox.IsChecked == true;
            var results = App.AssetManager
                .EnumerateEbx(type: selected.EbxType, modifiedOnly: modifiedOnly)
                .OrderBy(entry => entry.Name)
                .ToList();
            ResultsList.ItemsSource = results;
            ResultCountText.Text = $"{results.Count} asset{(results.Count == 1 ? "" : "s")} found";
            OpenButton.IsEnabled = false;
        }

        private void ResultsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenButton.IsEnabled = ResultsList.SelectedItem != null;
        }

        private void ResultsList_OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenSelected();
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenSelected();
        }

        private void OpenSelected()
        {
            if (ResultsList.SelectedItem is FrostySdk.Managers.EbxAssetEntry entry)
            {
                App.EditorWindow.OpenAsset(entry);
            }
        }
        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}