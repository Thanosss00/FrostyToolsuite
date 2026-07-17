using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Frosty.Controls;
using FrostyModManager;
using ModCategories.Common;

namespace ModCategories.Windows
{
    public partial class InsertDividerWindow : FrostyWindow
    {
        public InsertDividerWindow()
        {
            InitializeComponent();
            CategoryBox.ItemsSource = CategoryStore.Load();
            if (CategoryBox.Items.Count > 0)
                CategoryBox.SelectedIndex = 0;
        }

        private void CategoryBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Picking from the dropdown clears any typed override.
            if (NewCategoryTextBox.Text.Length > 0)
                NewCategoryTextBox.Text = "";
        }

        private void NewCategoryTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Typing a new name overrides whatever is selected in the dropdown.
            if (NewCategoryTextBox.Text.Length > 0)
                CategoryBox.SelectedIndex = -1;
        }

        private string GetChosenCategoryName()
        {
            if (!string.IsNullOrWhiteSpace(NewCategoryTextBox.Text))
                return NewCategoryTextBox.Text.Trim();

            return CategoryBox.SelectedItem as string;
        }

        private void InsertButton_OnClick(object sender, RoutedEventArgs e)
        {
            string categoryName = GetChosenCategoryName();
            if (string.IsNullOrEmpty(categoryName))
            {
                FrostyMessageBox.Show("Pick a category from the dropdown, or type a new one.");
                return;
            }

            Window mainWindow = Application.Current.MainWindow;
            ComboBox packsComboBox = mainWindow.FindName("packsComboBox") as ComboBox;
            ListBox appliedModsList = mainWindow.FindName("appliedModsList") as ListBox;

            if (packsComboBox == null || appliedModsList == null)
            {
                FrostyMessageBox.Show("Could not find the mod list - this FrostyModManager version may not be supported by this plugin.");
                return;
            }

            FrostyPack selectedPack = packsComboBox.SelectedItem as FrostyPack;
            if (selectedPack == null)
            {
                FrostyMessageBox.Show("Select a mod pack first.");
                return;
            }

            ModListHook.Install(mainWindow, null);

            int insertIndex = appliedModsList.SelectedIndex >= 0
                ? appliedModsList.SelectedIndex
                : selectedPack.AppliedMods.Count;

            selectedPack.AppliedMods.Insert(insertIndex, new DividerAppliedMod(categoryName));
            selectedPack.Refresh();
            appliedModsList.Items.Refresh();

            // Remember new category names for next time.
            List<string> categories = CategoryStore.Load();
            if (!categories.Contains(categoryName))
            {
                categories.Add(categoryName);
                CategoryStore.Save(categories);
            }
            Close();
        }
        private void CancelButton_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}