using System;
using System.Windows;
using Frosty.Core;
using Frosty.Core.Controls;
using FilterType.Common;
using FilterType.Windows;

namespace FilterType
{
    public class ViewFilterTypeWindow : MenuExtension
    {
        public ViewFilterTypeWindow()
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    DataExplorerFilterHook.Install(App.Logger);
                }
                catch (Exception ex)
                {
                    App.Logger.Log("Hook install failed: " + ex.Message);
                }
            }));
        }

        public override string TopLevelMenuName => "View";
        public override string SubLevelMenuName => "";
        public override string MenuItemName => "Filter Assets";

        // Kept as a fallback UI in case the toolbar injection ever fails,
        // same filtering logic, just a standalone window instead.
        public override RelayCommand MenuItemClicked => new RelayCommand(o =>
        {
            FilterTypeWindow window = new FilterTypeWindow();
            window.Show();
        });
    }
}