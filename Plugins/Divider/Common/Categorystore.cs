using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
 
namespace ModCategories.Common
{
    public static class CategoryStore
    {
        private static string FilePath => AppDomain.CurrentDomain.BaseDirectory + @"Caches\ModCategories.txt";
 
        public static List<string> Load()
        {
            List<string> list = new List<string>();
            if (File.Exists(FilePath))
            {
                list.AddRange(File.ReadAllLines(FilePath).Where(l => !string.IsNullOrWhiteSpace(l)));
            }
 
            if (list.Count == 0)
            {
                // Default categories
                // Editable in Manage Default Categories
                list.AddRange(new[] { "Gameplay", "Cosmetic", "Audio", "UserInterface"});
                Save(list);
            }
            return list;
        }
 
        public static void Save(List<string> categories)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"Caches\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllLines(FilePath, categories);
        }
    }
}