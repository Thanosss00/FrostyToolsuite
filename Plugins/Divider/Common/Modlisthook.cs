using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FrostyModManager;
using FrostySdk.Interfaces;

namespace ModCategories.Common
{
    /// <summary>
    /// Installs a DataTemplateSelector on the existing appliedModsList
    /// so divider rows render as a category header while
    /// every normal mod keeps rendering with Frosty's original template.
    /// </summary>
    public static class ModListHook
    {
        private static bool s_installed;
        public static bool IsDivider(object item)
        {
            if (item is FrostyAppliedMod mod)
            {
                string name = mod.ModName;
                return !string.IsNullOrEmpty(name) && name.StartsWith(DividerAppliedMod.Marker);
            }
            return false;
        }

        public static void Install(Window mainWindow, ILogger logger)
        {
            if (s_installed) return;

            ListBox appliedModsList = mainWindow.FindName("appliedModsList") as ListBox;
            if (appliedModsList == null)
            {
                logger?.Log("[ModCategories] Could not find 'appliedModsList' - divider rendering disabled");
                return;
            }

            // Capture the original template exactly as Frosty defined it,
            // so normal mod rows are rendered identically to before
            DataTemplate originalTemplate = appliedModsList.ItemTemplate;

            const string dividerXaml = @"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                              xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                              xmlns:common=""clr-namespace:ModCategories.Common;assembly=ModCategories"">
                    <DataTemplate.Resources>
                        <common:DividerNameConverter x:Key=""DividerNameConverter""/>
                    </DataTemplate.Resources>
                    <Border Background=""#FF1F6FB2""
                            BorderBrush=""#FF6CB4F0""
                            BorderThickness=""0,2,0,2""
                            Padding=""14,12,14,12""
                            Margin=""0,4,0,4"">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width=""6""/>
                                <ColumnDefinition Width=""*""/>
                            </Grid.ColumnDefinitions>
                            <Rectangle Grid.Column=""0"" Fill=""#FFFFC94A"" Margin=""-14,-12,10,-12""/>
                            <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" HorizontalAlignment=""Center"">
                                <TextBlock Text=""&#128193;""
                                           FontSize=""18""
                                           Margin=""0,0,10,0""
                                           VerticalAlignment=""Center""/>
                                <TextBlock Text=""{Binding Path=ModName, Converter={StaticResource DividerNameConverter}}""
                                           FontWeight=""Bold""
                                           FontSize=""16""
                                           Foreground=""White""
                                           VerticalAlignment=""Center"">
                                    <TextBlock.Effect>
                                        <DropShadowEffect ShadowDepth=""0"" BlurRadius=""4"" Color=""Black"" Opacity=""0.6""/>
                                    </TextBlock.Effect>
                                </TextBlock>
                            </StackPanel>
                        </Grid>
                    </Border>
                </DataTemplate>";

            DataTemplate dividerTemplate = (DataTemplate)XamlReader.Parse(dividerXaml);

            appliedModsList.ItemTemplateSelector = new DividerTemplateSelector(originalTemplate, dividerTemplate);

            s_installed = true;
            logger?.Log("[ModCategories] Divider rendering installed.");
        }
    }

    public class DividerNameConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string ?? "";
            return s.StartsWith(DividerAppliedMod.Marker) ? s.Substring(DividerAppliedMod.Marker.Length) : s;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotSupportedException();
        }
    }

    public class DividerTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate m_normalTemplate;
        private readonly DataTemplate m_dividerTemplate;

        public DividerTemplateSelector(DataTemplate normalTemplate, DataTemplate dividerTemplate)
        {
            m_normalTemplate = normalTemplate;
            m_dividerTemplate = dividerTemplate;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return ModListHook.IsDivider(item) ? m_dividerTemplate : m_normalTemplate;
        }
    }
}