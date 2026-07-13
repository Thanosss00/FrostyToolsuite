using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;

namespace SearchIdentifier.Common
{
    public static class SearchIdCache
    {
        public static int Version = 0x00000001;
        private static string CacheDirectory => System.AppDomain.CurrentDomain.BaseDirectory + @"Caches\";

        private static readonly string[] IdentifierFieldNames =
        {
            "Identifier",
            "PlayerAbilityIdentifier",
            "ConceptIdentifier",
            "ActionIdentifier"
        };

        public static string GetCacheFilePath(AssetTypeDefinition type)
        {
            return CacheDirectory + Enum.GetName(typeof(ProfileVersion), ProfilesLibrary.DataVersion) + "_SearchIdentifier_" + type.CacheSuffix + ".cache";
        }

        public static void Generate(AssetTypeDefinition type, FrostyTaskWindow task)
        {
            AssetManager AM = App.AssetManager;
            List<EbxAssetEntry> ebxFiles = AM.EnumerateEbx(type: type.EbxType).ToList();
            int index = 0;
            int count = ebxFiles.Count;
            List<UInt32> ids = new List<UInt32>();
            List<string> paths = new List<string>();

            ebxFiles.ForEach((entry) =>
            {
                if (entry.IsAdded)
                {
                    App.Logger.Log("Skipping " + entry.Name + " as it is an added asset.");
                    return;
                }

                EbxAsset asset = AM.GetEbx(entry);
                task.Update($"Scanning {type.DisplayName} ({index + 1}/{count}): {entry.Filename}", index / (double)count * 100);

                object root = asset.RootObject;
                uint? idValue = null;

                foreach (string fieldName in IdentifierFieldNames)
                {
                    object rawValue = null;

                    System.Reflection.PropertyInfo prop = root.GetType().GetProperty(fieldName);
                    if (prop != null)
                    {
                        rawValue = prop.GetValue(root);
                    }
                    else
                    {
                        System.Reflection.FieldInfo field = root.GetType().GetField(fieldName);
                        if (field != null)
                            rawValue = field.GetValue(root);
                    }

                    if (rawValue == null) continue;

                    try
                    {
                        idValue = Convert.ToUInt32(rawValue);
                        break;
                    }
                    catch
                    {
                    }
                }

                if (idValue == null)
                {
                    App.Logger.Log("Skipping " + entry.Name + " - no known Identifier field found.");
                    return;
                }

                ids.Add(idValue.Value);
                paths.Add(entry.Name);
                index++;
            });

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);

            using (NativeWriter writer = new NativeWriter(new FileStream(GetCacheFilePath(type), FileMode.Create)))
            {
                writer.Write(Version);
                writer.Write(ids.Count);
                for (int i = 0; i < ids.Count; i++)
                {
                    writer.Write(ids[i]);
                    writer.WriteNullTerminatedString(paths[i]);
                }
            }
        }

        public static bool Load(AssetTypeDefinition type, out Dictionary<uint, string> idToNameMap, out string errorMessage)
        {
            idToNameMap = new Dictionary<uint, string>();
            errorMessage = null;
            using (NativeReader reader = new NativeReader(new FileStream(GetCacheFilePath(type), FileMode.Open)))
            {
                int version = reader.ReadInt();
                if (version != Version)
                {
                    errorMessage = $"Cache is out of date (found v{version}, expected v{Version}). Delete the cache file and generate again.";
                    return false;
                }
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    uint id = reader.ReadUInt();
                    string name = reader.ReadNullTerminatedString();
                    idToNameMap[id] = name;
                }
            }
            return true;
        }
    }
}