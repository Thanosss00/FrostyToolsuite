using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using SearchIdentifier.Common;
namespace SearchIdentifier.Windows
{
    public partial class SearchIdentifierWindow : FrostyDockableWindow
    {
        private Dictionary<uint, string> idToNameMap = new Dictionary<uint, string>();
        private bool lookupFailed = true;
        private AssetTypeDefinition currentType;
        private string matchedAssetPath;

        public SearchIdentifierWindow()
        {
            InitializeComponent();
            TypeSelector.ItemsSource = AssetTypeRegistry.Types;
            TypeSelector.SelectedIndex = 0; // triggers TypeSelector_OnSelectionChanged which loads the first category
        }
        private void TypeSelector_OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            currentType = TypeSelector.SelectedItem as AssetTypeDefinition;
            if (currentType == null) return;
            idToNameMap.Clear();
            IdInput.Text = "";
            lookupFailed = true;
            SetOpenAssetEnabled(false);
            ResultText.Text = $"Loading {currentType.DisplayName} data...";
            FrostyTaskWindow.Show($"{currentType.DisplayName} Search", $"Checking for cached {currentType.DisplayName} data...", (task) =>
            {
                if (!File.Exists(SearchIdCache.GetCacheFilePath(currentType)))
                {
                    task.Update("Generating Cache", 0);
                    SearchIdCache.Generate(currentType, task);
                }
                task.Update("Loading...", 90);
                if (SearchIdCache.Load(currentType, out idToNameMap, out string error))
                {
                    task.Update("Ready", 100);
                }
                else
                {
                    task.Update("Failed", 100);
                    FrostyMessageBox.Show(error);
                }
            });
            ResultText.Text = ModeByName.IsChecked == true
                ? $"Type a {currentType.DisplayName} name above."
                : $"Type a {currentType.DisplayName} Identifier above.";
        }

        private void Mode_OnChanged(object sender, RoutedEventArgs e)
        {
            if (IdInput == null) return;
            IdInput.Text = "";
            IdInput.WatermarkText = ModeById.IsChecked == true ? "Enter Identifier..." : "Enter Name...";
            SetOpenAssetEnabled(false);
            matchedAssetPath = null;
            if (currentType != null)
            {
                ResultText.Text = ModeByName.IsChecked == true
                    ? $"Type a {currentType.DisplayName} name above."
                    : $"Type a {currentType.DisplayName} Identifier above.";
            }
        }

        private void IdInput_OnKeyUp(object sender, KeyEventArgs e)
        {
            string query = IdInput.Text;

            if (ModeById.IsChecked == true)
            {
                uint value;
                try
                {
                    value = Convert.ToUInt32(query, CultureInfo.InvariantCulture);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    ResultText.Text = ex is FormatException ? "That's not a valid number." : "That number is too large to be an Identifier.";
                    lookupFailed = true;
                    SetOpenAssetEnabled(false);
                    return;
                }
                if (idToNameMap.TryGetValue(value, out string name))
                {
                    ResultText.Text = name;
                    matchedAssetPath = name;
                    lookupFailed = false;
                }
                else
                {
                    ResultText.Text = $"No {currentType.DisplayName} found with that Identifier.";
                    lookupFailed = true;
                }
                SetOpenAssetEnabled(!lookupFailed);
            }
            else // Searchbyname
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    ResultText.Text = $"Type a {currentType.DisplayName} name above.";
                    SetOpenAssetEnabled(false);
                    return;
                }

                var matches = idToNameMap
                    .Where(kv => kv.Value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Take(50)
                    .ToList();

                if (matches.Count == 0)
                {
                    ResultText.Text = "No matches found.";
                    matchedAssetPath = null;
                    SetOpenAssetEnabled(false);
                }
                else if (matches.Count == 1)
                {
                    ResultText.Text = $"{matches[0].Key} → {matches[0].Value}";
                    matchedAssetPath = matches[0].Value;
                    SetOpenAssetEnabled(true);
                }
                else
                {
                    ResultText.Text = string.Join("\n", matches.Select(m => $"{m.Key} → {m.Value}")) +
                        (matches.Count == 50 ? "\n(showing first 50 - narrow your search)" : "");
                    matchedAssetPath = null;
                    SetOpenAssetEnabled(false);
                }
            }
        }

        private void SetOpenAssetEnabled(bool enabled)
        {
            OpenAssetButton.IsEnabled = enabled;
        }
        private void OpenAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(matchedAssetPath)) return;
            Close();
            App.EditorWindow.OpenAsset(App.AssetManager.GetEbxEntry(matchedAssetPath));
        }
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}