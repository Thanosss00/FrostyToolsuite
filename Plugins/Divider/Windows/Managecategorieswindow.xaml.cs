using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Frosty.Controls;
using ModCategories.Common;

namespace ModCategories.Windows
{
    public partial class ManageCategoriesWindow : FrostyWindow
    {
        private readonly ObservableCollection<string> m_categories;

        public ManageCategoriesWindow()
        {
            InitializeComponent();
            m_categories = new ObservableCollection<string>(CategoryStore.Load());
            CategoryList.ItemsSource = m_categories;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            string name = NewCategoryBox.Text?.Trim();
            if (string.IsNullOrEmpty(name) || m_categories.Contains(name))
                return;

            m_categories.Add(name);
            NewCategoryBox.Text = "";
            CategoryStore.Save(new List<string>(m_categories));
        }

        private void RenameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(CategoryList.SelectedItem is string oldName))
            {
                Frosty.Controls.FrostyMessageBox.Show("Select a category to rename first.");
                return;
            }

            string newName = NewCategoryBox.Text?.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                Frosty.Controls.FrostyMessageBox.Show("Type the new name in the box.");
                return;
            }
            if (m_categories.Contains(newName))
            {
                Frosty.Controls.FrostyMessageBox.Show("That name is already in the list.");
                return;
            }
            int index = m_categories.IndexOf(oldName);
            m_categories[index] = newName;
            NewCategoryBox.Text = "";
            CategoryStore.Save(new List<string>(m_categories));
        }

        private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CategoryList.SelectedItem is string selected)
            {
                m_categories.Remove(selected);
                CategoryStore.Save(new List<string>(m_categories));
            }
        }
        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}