using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Attributes;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace DuplicateGameModePlugin.Windows
{
    /// <summary>
    /// Interaction logic for DuplicateAssetWindow.xaml
    /// </summary>
    public partial class DuplicateGameModeWindow : FrostyDockableWindow
    {
        public string GameModeName { get; private set; } = "";
        public string DuplicateToPath { get; private set; } = "";
        public string DuplicateFromPath { get; private set; } = "";

        public Type SelectedType { get; private set; } = null;
        private EbxAssetEntry entry;

        public DuplicateGameModeWindow(EbxAssetEntry currentEntry)
        {
            InitializeComponent();

            pathSelector.ItemsSource = App.AssetManager.EnumerateEbx();
            entry = currentEntry;

            gameModeName.Text = "DupedMode";
            duplicateFrom.Text = App.SelectedAsset.Name;
        }

        private void AssetNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string tmp = duplicateFrom.Text.Replace('\\', '/').Trim('/');
            string fullName = pathSelector.SelectedPath + "/" + tmp;

            if (!string.IsNullOrEmpty(duplicateFrom.Text) && !entry.Name.Equals(fullName, StringComparison.OrdinalIgnoreCase))
            {
                if (!tmp.Contains("//"))
                {
                    DuplicateToPath = pathSelector.SelectedPath;
                    GameModeName = gameModeName.Text;
                    DuplicateFromPath = duplicateFrom.Text;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    FrostyMessageBox.Show("Name of asset is invalid", "Frosty Editor");
                }
            }
            else
            {
                FrostyMessageBox.Show("Name of asset must be unique", "Frosty Editor");
            }
        }

        private void FrostyDockableWindow_FrostyLoaded(object sender, EventArgs e)
        {
            pathSelector.SelectAsset(entry);
        }

        private void ClassSelector_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        
    }
}
