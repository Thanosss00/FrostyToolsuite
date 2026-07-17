using System;
using System.Windows;
using Frosty.Core;
using Frosty.Core.Controls;
using ModCategories.Common;
using ModCategories.Windows;

namespace ModCategories
{
    public class InsertDividerMenuExtension : MenuExtension
    {
        public InsertDividerMenuExtension()
        {
            try
            {
                Window mainWindow = Application.Current?.MainWindow;
                if (mainWindow != null)
                    ModListHook.Install(mainWindow, App.Logger);
            }
            catch (Exception ex)
            {
                App.Logger.Log("Hook install failed: " + ex.Message);
            }
        }

        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => "Mod Categories";
        public override string MenuItemName => "Insert Category Divider";

        public override RelayCommand MenuItemClicked => new RelayCommand(o =>
        {
            // just incase the constructor install above ever fails.
            ModListHook.Install(Application.Current.MainWindow, App.Logger);

            InsertDividerWindow window = new InsertDividerWindow
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
        });
    }

    public class ManageCategoriesMenuExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Tools";
        public override string SubLevelMenuName => "Mod Categories";
        public override string MenuItemName => "Manage Default Categories";

        public override RelayCommand MenuItemClicked => new RelayCommand(o =>
        {
            ManageCategoriesWindow window = new ManageCategoriesWindow
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();
        });
    }
}