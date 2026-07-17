using FrostyModManager;

namespace ModCategories.Common
{
    /// <summary>
    /// A fake mod that appears like a real one but can't be enabled(IsFound=false).
    /// </summary>
    public class DividerAppliedMod : FrostyAppliedMod
    {
        public const string Marker = "##DIVIDER##";
        public string CategoryName { get; }

        public DividerAppliedMod(string categoryName)
            : base(Marker + categoryName, false)
        {
            CategoryName = categoryName;
        }
        public new string ModName => CategoryName;
    }
}
