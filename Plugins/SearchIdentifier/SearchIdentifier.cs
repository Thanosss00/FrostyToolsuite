using Frosty.Core;
using Frosty.Core.Controls;
using SearchIdentifier.Windows;

namespace SearchIdentifier
{
    public class ViewSearchIdentifierWindow : MenuExtension
    {
        public override string TopLevelMenuName => "View";
        public override string MenuItemName => "SearchIdentifier";

        public override RelayCommand MenuItemClicked => new RelayCommand(o =>
        {
            SearchIdentifierWindow window = new SearchIdentifierWindow();
            window.Show();
        });
    }
}