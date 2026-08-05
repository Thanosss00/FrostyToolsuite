using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Managers;
using FrostySdk.IO;
using FrostySdk;
using FrostySdk.Ebx;
using DuplicationPlugin;
using System.Numerics;
using System.IO;
using Guid = System.Guid;
using DuplicateGameModePlugin.Windows;
using System.Windows.Media.Imaging;
using Frosty.Core.Controls.Editors;
using System.Windows.Markup;

namespace DuplicateGameModePlugin
{
    // Credit to Magix for example code + advice
    public struct Transforms
    {

        public Vector3 Translation;
        public Quaternion Rotation;
        public Vector3 Scale;
        public static readonly Transforms One = new() { Scale = new Vector3(1, 1, 1) };

        public Transforms()
        {
            Translation = new Vector3();
            Rotation = new Quaternion();
            Scale = new Vector3(1);
        }
        public Transforms(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }
        public static Transforms FromMatrix4x4(Matrix4x4 matrix)
        {
            matrix = new Matrix4x4(
                matrix.M11, -matrix.M12, -matrix.M13, matrix.M14,
                -matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                -matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                -matrix.M41, matrix.M42, matrix.M43, matrix.M44
                );
            //var matrix2 = new Matrix4x4(
            //    matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            //    matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            //    matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            //    matrix.M31, matrix.M32, matrix.M33, matrix.M44
            //    );
            //matrix = Matrix4x4.Transpose( matrix );

            Transforms trns = new Transforms();
            trns.Translation = new Vector3();
            trns.Rotation = new Quaternion();
            trns.Scale = new Vector3();

            Matrix4x4.Decompose(matrix, out System.Numerics.Vector3 scale, out System.Numerics.Quaternion rotation, out System.Numerics.Vector3 translation);

            trns.Translation.X = translation.X;
            trns.Translation.Y = translation.Y;
            trns.Translation.Z = translation.Z;

            trns.Scale.X = scale.X;
            trns.Scale.Y = scale.Y;
            trns.Scale.Z = scale.Z;

            trns.Rotation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

            return trns;
        }
        public readonly LinearTransform ToLinearTransform()
        {
            float val = (float)(Math.PI / 180.0);
            LinearTransform transform = new LinearTransform();
            Matrix4x4 m = Matrix4x4.CreateFromQuaternion(Rotation);
            m *= Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);

            transform.right.x = m.M11;
            transform.right.y = -m.M12;
            transform.right.z = -m.M13;

            transform.up.x = -m.M21;
            transform.up.y = m.M22;
            transform.up.z = m.M23;

            transform.forward.x = -m.M31;
            transform.forward.y = m.M32;
            transform.forward.z = m.M33;

            transform.trans.x = -Translation.X;
            transform.trans.y = Translation.Y;
            transform.trans.z = Translation.Z;


            // for the negatations
            //matrix = new Matrix4x4(
            //    matrix.M11, -matrix.M12, -matrix.M13, matrix.M14,
            //    -matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            //    -matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            //    -matrix.M41, matrix.M42, matrix.M43, matrix.M44
            //    );

            return transform;
        }

    }
// This will eventually be used for complete Level Duplication. A little ways away though :P
#if false
    public class BundleRVM : DataExplorerContextMenuExtension
    {
        //DuplicationTool.ResAssetEntry resDuper;
        public override string ContextItemName => "Bundle RVM";
        public override RelayCommand ContextItemClicked => new RelayCommand(o => {

            App.Logger.Log("Adds RVMs to the Asset bundles of the asset that's selected");

            DuplicateRVM();
            DuplicateAntState();
            AddAntStateToBundle();

            if (App.SelectedAsset.Type.ToString() == "LevelData")
            {

                int selectedAssetBundle = App.AssetManager.GetEbxEntry(App.SelectedAsset.Name).Bundles[0];

                EbxAssetEntry c = App.AssetManager.GetEbxEntry("S2/Levels/CloudCity_01/CloudCity_01");

                string[] resAssets = { "levels/mp/hoth_01/hoth_01/rvmdatabase_dx11nvrvmdatabase", "levels/mp/hoth_01/hoth_01/rvmdatabase_dx11rvmdatabase",
                "levels/mp/hoth_01/hoth_01/rvmdatabase_dx12pcrvmdatabase", "levels/mp/hoth_01/hoth_01/rvmdatabase_dx12nvrvmdatabase", 
                "levels/mp/endor_01/endor_01/rvmdatabase_dx11nvrvmdatabase", "levels/mp/endor_01/endor_01/rvmdatabase_dx11rvmdatabase",
                "levels/mp/endor_01/endor_01/rvmdatabase_dx12pcrvmdatabase", "levels/mp/endor_01/endor_01/rvmdatabase_dx12nvrvmdatabase",
                "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx11nvrvmdatabase", "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx11rvmdatabase",
                "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx12pcrvmdatabase", "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx12nvrvmdatabase"
                };

            
                foreach (string asset in resAssets)
                {
                    ResAssetEntry a = App.AssetManager.GetResEntry(asset);
                    if (a == null)
                    {
                        App.Logger.Log(asset + "Is Invalid");
                    }
                    else
                    {
                        a.AddToBundle(selectedAssetBundle);
                        App.Logger.Log("Added " + asset + " to bundle: " + App.SelectedAsset.Name);
                    }
                }
            }
            else
            {
                string[] resAssets = { "levels/mp/hoth_01/hoth_01/rvmdatabase_dx11nvrvmdatabase", "levels/mp/hoth_01/hoth_01/rvmdatabase_dx11rvmdatabase",
                "levels/mp/hoth_01/hoth_01/rvmdatabase_dx12pcrvmdatabase", "levels/mp/hoth_01/hoth_01/rvmdatabase_dx12nvrvmdatabase",
                "levels/mp/endor_01/endor_01/rvmdatabase_dx11nvrvmdatabase", "levels/mp/endor_01/endor_01/rvmdatabase_dx11rvmdatabase",
                "levels/mp/endor_01/endor_01/rvmdatabase_dx12pcrvmdatabase", "levels/mp/endor_01/endor_01/rvmdatabase_dx12nvrvmdatabase",
                "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx11nvrvmdatabase", "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx11rvmdatabase",
                "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx12pcrvmdatabase", "a3/levels/sp/rootlevel/rootlevel_a3/rootlevel_a3/rvmdatabase_dx12nvrvmdatabase"
                };

                EbxAssetEntry c = App.AssetManager.GetEbxEntry("S2/Levels/CloudCity_01/CloudCity_01");

                foreach (string asset in resAssets)
                {
                    ResAssetEntry a = App.AssetManager.GetResEntry(asset);
                    if (a == null)
                    {
                        App.Logger.Log(asset + "Is Invalid");
                    }
                    else
                    {
                        a.AddToBundle(c.Bundles[0]);
                        App.Logger.Log("Added " + asset + " to bundle: " + App.SelectedAsset.Name);
                    }
                }
                //App.Logger.Log("Only Supports LevelData");
            }
        });

        
        public void DuplicateRVM()
        {

            EbxAssetEntry c = App.AssetManager.GetEbxEntry("S10/Levels/Clouds_01/Clouds_01");
            int newBundle = c.AddedBundles[0];

            #region dx11nvrvm
            ResAssetEntry resToDupe = App.AssetManager.GetResEntry("s2/levels/cloudcity_01/cloudcity_01/rvmdatabase_dx11nvrvmdatabase");
            ResAssetEntry newRvm = DuplicateRes(resToDupe, "s10/levels/clouds_01/clouds_01/rvmdatabase_dx11nvrvmdatabase", ResourceType.Dx11NvRvmDatabase);
            newRvm.AddToBundle(newBundle);
            #endregion

            #region dx11rvm
            ResAssetEntry resToDupe2 = App.AssetManager.GetResEntry("s2/levels/cloudcity_01/cloudcity_01/rvmdatabase_dx11rvmdatabase");
            ResAssetEntry newRvm2 = DuplicateRes(resToDupe, "s10/levels/clouds_01/clouds_01/rvmdatabase_dx11rvmdatabase", ResourceType.Dx11RvmDatabase);
            newRvm2.AddToBundle(newBundle);
            #endregion

            #region dx12nvrvm
            ResAssetEntry resToDupe3 = App.AssetManager.GetResEntry("s2/levels/cloudcity_01/cloudcity_01/rvmdatabase_dx12nvrvmdatabase");
            ResAssetEntry newRvm3 = DuplicateRes(resToDupe, "s10/levels/clouds_01/clouds_01/rvmdatabase_dx12nvrvmdatabase", ResourceType.Dx12NvRvmDatabase);
            newRvm3.AddToBundle(newBundle);
            #endregion

            #region dx12pcrvm
            ResAssetEntry resToDupe4 = App.AssetManager.GetResEntry("s2/levels/cloudcity_01/cloudcity_01/rvmdatabase_dx12pcrvmdatabase");
            ResAssetEntry newRvm4 = DuplicateRes(resToDupe, "s10/levels/clouds_01/clouds_01/rvmdatabase_dx12pcrvmdatabase", ResourceType.Dx12PcRvmDatabase);
            newRvm4.AddToBundle(newBundle);
            #endregion

        }
        public void DuplicateAntState()
        {
            ResAssetEntry resToDupe = App.AssetManager.GetResEntry("animations/antanimations/s2/levels/cloudcity_01/cloudcity_01_win32_antstate");
            ResAssetEntry newRvm = DuplicateRes(resToDupe, "animations/antanimations/s10/levels/clouds_01/clouds_01_win32_antstate", ResourceType.AssetBank);
            newRvm.AddToBundle(App.AssetManager.GetEbxEntry("S10/Levels/Clouds_01/Clouds_01").AddedBundles[0]);
        }
        public void AddAntStateToBundle()
        {
            EbxAssetEntry c = App.AssetManager.GetEbxEntry("S10/Levels/Clouds_01/Clouds_01");
            int newBundle = c.AddedBundles[0];

            ResAssetEntry a = App.AssetManager.GetResEntry("animations/antanimations/s10/levels/clouds_01/clouds_01_win32_antstate");
            a.AddToBundle(newBundle);
        }
        public static ResAssetEntry DuplicateRes(ResAssetEntry entry, string name, ResourceType resType)
        {
            if (App.AssetManager.GetResEntry(name) == null)
            {
                ResAssetEntry newEntry;
                using (NativeReader reader = new NativeReader(App.AssetManager.GetRes(entry)))
                {
                    newEntry = App.AssetManager.AddRes(name, resType, entry.ResMeta, reader.ReadToEnd(), entry.EnumerateBundles().ToArray());
                }

                App.Logger.Log(string.Format("Duped res {0} to {1}", entry.Name, newEntry.Name));
                return newEntry;
            }
            else
            {
                App.Logger.Log(name + " already has a res files");
                return null;
            }
        }
    }
#endif
    public class ImportSpawns : DataExplorerContextMenuExtension
    {
        DuplicationTool.DuplicateAssetExtension ebxDuper;

        const string Port_Link_SpawnPoints = "Spawns"; // ->0x00000000
        const string Port_Link_OOBs = "CombatArea"; // ->0x00000000 // 0xf8f067fa
        const string Port_Link_SpawnLocationFinders = "0xf8f067fa"; // ->0x00000000
        const string Port_Property_EnableSpawnPoints = "0x19988b12"; // ->Enabled

        const string Port_Event_LoadIntro = "StreamInIntro"; // ->Enabledable
        const string Port_Event_DeloadIntro = "IntroDone"; // ->Enabledable

        const string DefaultLinkPort = "0x00000000"; // ->Enabled
        public override string ContextItemName => "Import Spawns";

        public override RelayCommand ContextItemClicked => new RelayCommand(o =>
        {
            string selectedAssetName = App.SelectedAsset.Name;
            EbxAsset selectedSubWorld = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(selectedAssetName));
            try
            {
                FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import Spawns", "Text files (*.txt)|*.txt|All files (*.*)|*.*", "AlternateSpawnEntityData");
                if (ofd.ShowDialog())
                {
                    ebxDuper = new();

                    var parentAssetEntry = ebxDuper.DuplicateAsset(App.AssetManager.GetEbxEntry(selectedAssetName), selectedAssetName + "_New_Spawns", true, typeof(LayerData));
                    var parentAsset = App.AssetManager.GetEbx(parentAssetEntry);

                    string filePath = ofd.FileName;
                    var lines = File.ReadAllLines(filePath);

                    Dictionary<int, PointerRef> gameModeBlueprint = GetDefaultAlternateSpawnConnection(selectedSubWorld.RootObject as SubWorldData);
                    Dictionary<int, CString>  gameModeLinkNames = GetDefaultGameModeLinkName(selectedSubWorld.RootObject as SubWorldData, "SP");
                    Dictionary<int, CString>  defaultSLFNames = GetDefaultGameModeLinkName(selectedSubWorld.RootObject as SubWorldData, "SLF");

                    if (true) //@TODO Add Option Value
                    {
                        for(int i = 0; i < gameModeLinkNames.Count; i++)
                        {
                            DisableConnectionsWithName(gameModeLinkNames[i], selectedSubWorld.RootObject as SubWorldData);
                        }
                        for (int i = 0; i < defaultSLFNames.Count; i++)
                        {
                            DisableConnectionsWithName(defaultSLFNames[i], selectedSubWorld.RootObject as SubWorldData);
                        }

                    }

                    Dictionary<int, List<Vector3>> splineGroups = new Dictionary<int, List<Vector3>>();
                    Dictionary<int, int> splineTeams = new Dictionary<int, int>();
                    int index1 = 0;
                    int index = 0;
                    try
                    {
                        foreach (string line in lines)
                        {
                            var parts = line.Split(',');
                            if (parts.Length != 8) continue;

                            String type = parts[0];

                            if (type == "AlternateSpawnEntityData")
                            {
                                float num1 = float.Parse(parts[1]);
                                float num2 = float.Parse(parts[2]);
                                float num3 = float.Parse(parts[3]);
                                float num4 = float.Parse(parts[4]);
                                int team = int.Parse(parts[5]);
                                int priority = int.Parse(parts[6]);
                                bool defaultEnabled = bool.Parse(parts[7]);

                                Transforms newTransform = new Transforms();

                                float rotate = 0;
                                if(num4 < 0)
                                {
                                    rotate = num4;
                                }
                                else
                                {
                                    rotate = -num4;
                                }

                                newTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, ToRadians(rotate)); 
                                newTransform.Scale = new Vector3(1, 1, 1); // or whatever
                                newTransform.Translation = new Vector3(num1, num2, num3);

                                LinearTransform linear = newTransform.ToLinearTransform();

                                PointerRef spPtr = CreateExternalPointer(parentAsset, CreateNewSpawnPoint(parentAsset, linear, team.ToString(), priority, defaultEnabled));

                                // Create connections to this spawn point
                                for (int i = 0; i < gameModeBlueprint.Count; i++)
                                {
                                    CreateLinkConnection(selectedSubWorld.RootObject as SubWorldData, gameModeBlueprint[i], gameModeLinkNames[i], spPtr, DefaultLinkPort);
                                }
                                index1+=gameModeLinkNames.Count;
                                //App.Logger.Log(num1 + " " + num2 + " " + num3 + " " + num4 + " " + team + " " + " " + priority + " " + defaultEnabled);
                            }
                            else if (type == "SpawnLocationFinderEntityData")
                            {
                                float num1 = -float.Parse(parts[1]);
                                float num2 = float.Parse(parts[2]);
                                float num3 = float.Parse(parts[3]);
                                int splineIndex = (int)float.Parse(parts[4]);

                                if (!splineGroups.ContainsKey(splineIndex))
                                    splineGroups[splineIndex] = new List<Vector3>();

                                splineGroups[splineIndex].Add(new Vector3(num1, num2, num3));
                                splineTeams[splineIndex] = int.Parse(parts[5]);
                            }
                            else
                            {
                                App.Logger.Log("Incorrectly Formatted File, Type " + parts[0] + " is not valid");
                            }
                        }

                        foreach (var kvp in splineGroups)
                        {
                            int splineIndex = kvp.Key;
                            List<Vector3> points = kvp.Value;

                            int team = splineTeams[splineIndex];
                            bool defaultEnabled = true;

                            PointerRef spPtr = CreateExternalPointer(parentAsset, CreateNewSpawnLocationFinderShape(parentAsset, points, team.ToString(), defaultEnabled));
                            for (int i = 0; i < gameModeBlueprint.Count; i++)
                            {
                                CreateLinkConnection(selectedSubWorld.RootObject as SubWorldData, gameModeBlueprint[i], defaultSLFNames[i], spPtr, DefaultLinkPort);
                            }
                            index+= defaultSLFNames.Count;
                        }

                        App.Logger.Log("Added " + index1 + " AlternateSpawns");
                        App.Logger.Log("Added " + index + " SpawnLocationFinders");
                        App.AssetManager.ModifyEbx(parentAssetEntry.Name, parentAsset);

                        try
                        {
                            CreateNewLayerReference(selectedSubWorld.RootObject as SubWorldData, parentAsset);
                        }
                        catch
                        {
                            App.Logger.Log("Failed to create Layer Reference on " + selectedAssetName);
                        }

                        App.AssetManager.ModifyEbx(selectedAssetName, selectedSubWorld);
                    }
                    catch
                    {
                        App.Logger.Log("Failed to import correctly. Is your .txt formatted correctly?");
                    }
                }
            }
            catch
            {
                App.Logger.Log("Failed to Import Spawns. Make sure you haven't done so for the current gamemode already!");
            }

        });
        public static float ToRadians(float degrees) => (float)(Math.PI * degrees / 180f);

        public static SpawnLocationFinderShapeData CreateNewSpawnLocationFinderShape(EbxAsset parentAsset, string teamId)
        {
            SpawnLocationFinderShapeData data = new SpawnLocationFinderShapeData();
            data.Flags = 369098752u;
            data.Points =
            [
                new () { x = -10_000, y = 50, z = -10_000 },
            new () { x = -10_000, y = 50, z = 10_000 },
            new () { x = 10_000, y = 50, z = 10_000 },
            new () { x = 10_000, y = 50, z = -10_000 },
        ];
            data.Enabled = true;
            data.Team = (TeamId)Enum.Parse(typeof(TeamId), teamId, true);
            data.IsClosed = true;

            parentAsset.AddRootObject((object)data);
            return data;
        }

        public static SpawnLocationFinderShapeData CreateNewSpawnLocationFinderShape(EbxAsset parentAsset, List<Vector3> points, string teamId, bool isDefaultEnabled)
        {
            SpawnLocationFinderShapeData data = new SpawnLocationFinderShapeData();
            data.Flags = 369098752u;
            data.Points = points.ConvertAll(v => new Vec3() { x = v.X, y = v.Y, z = v.Z });
            data.Enabled = isDefaultEnabled;
            data.Team = (TeamId)Enum.Parse(typeof(TeamId), teamId, true);
            data.IsClosed = true;

            parentAsset.AddRootObject((object)data);
            return data;
        }

        public void DisableConnectionsWithName(CString name, Blueprint bp)
        {

            foreach (LinkConnection connection in bp.LinkConnections)
            {
                try
                {
                    if (connection.SourceField == name)
                    {
                        var disabledName = "DISABLED_" + name;

                        connection.SourceField = disabledName;
                    }
                }
                catch (Exception ex)
                {
                    App.Logger.Log($"Failed modifying connection for {connection.SourceField}: {ex.Message}");
                }
            }
        }



        public static dynamic CreateNewClass(string type, EbxAsset parentAsset)
        {
            dynamic obj = TypeLibrary.CreateObject(type);
            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(parentAsset.Objects, (Type)parentAsset.GetType(), parentAsset.FileGuid), -1);
            obj.SetInstanceGuid(guid);
            parentAsset.AddObject(obj, true);
            return obj;
        }

        public static DataContainer CreateRootClass(Type type, EbxAsset asset, SubWorldData bp)
        {
            DataContainer obj = CreateNewClass(type.Name, asset);
            if (obj == null)
                throw new Exception($"CreateNewClass returned null for type {type.Name}");

            if (bp?.Objects == null)
                throw new Exception("SubWorldData.Objects is null");

            bp.Objects.Add(new PointerRef(obj));

            if (obj is DataBusPeer obj2)
            {
                var instanceGuid = obj2.GetInstanceGuid();
                if (instanceGuid == default)
                    throw new Exception("Invalid instance GUID");

                var guid = instanceGuid.ExportedGuid;

                if (guid == null)
                    throw new Exception("ExportedGuid is null");

                byte[] b = guid.ToByteArray();
                uint value = (uint)((b[2] << 16) | (b[1] << 8) | b[0]);
                obj2.Flags = value;
            }

            return obj;
        }


        public void CreateNewLayerReference(SubWorldData newSubWorld, EbxAsset newLayer)
        {
            var layerRef = (LayerReferenceObjectData)ImportSpawns.CreateRootClass(typeof(LayerReferenceObjectData), newLayer, newSubWorld);
            {
                layerRef.Blueprint = DuplicateGameMode.RefToFile(newLayer);
                layerRef.LightmapResolutionScale = 1;
            }
        }
        
        public struct TypeToReferences
        {
            string type;
            int id;
            PointerRef sourceBlueprint;
            CString correspondingSourceField;
        }

        public void CreateNewWorldPartReferenc(SubWorldData newSubWorld)
        {

        }
        public Dictionary<int, PointerRef> GetDefaultAlternateSpawnConnection(SubWorldData subWorld)
        {
            Dictionary<int, PointerRef> blueprintRefs = new();

            int found = 0;
            foreach (LinkConnection connection in subWorld.LinkConnections)
            {
                if (connection.SourceField == "AlternativeSpawnPoints" || connection.SourceField == "0x88b5346e" || connection.SourceField == "Spawns" || connection.SourceField == "0xc6273ced" || connection.SourceField == "0xf4349b22")
                {
                    if (connection.TargetField == "0x00000000" && !blueprintRefs.ContainsValue(connection.Source))
                    {
                        blueprintRefs.Add(found, connection.Source);
                        found++;                   
                    }
                }
            }
            if(found == 0)
            {
                PointerRef emptyPointer = new PointerRef();
                blueprintRefs.Add(0, emptyPointer);
                App.Logger.Log("Didn't find Pointer Ref to existing prefab! Replacing with null reference");
            }
            return blueprintRefs;
        }
        public Dictionary<int, CString> GetDefaultGameModeLinkName(SubWorldData subWorld, string nametype)
        {
            Dictionary<int, CString> foundNames = new Dictionary<int, CString>();

            int found = 0;
            foreach (LinkConnection connection in subWorld.LinkConnections)
            {
                if(nametype == "SP")
                {
                    if (connection.SourceField == "AlternativeSpawnPoints" || connection.SourceField == "0x88b5346e" || connection.SourceField == "Spawns" || connection.SourceField == "0xc6273ced" || connection.SourceField == "0xf4349b22")
                    {
                        if (connection.TargetField == "0x00000000" && !foundNames.ContainsValue(connection.SourceField))
                        {
                            App.Logger.Log("Found Existing Link Connection " + connection.SourceField);
                            foundNames.Add(found, connection.SourceField);
                            found++;
                        }

                    }
                }
                else if (nametype == "SLF")
                {
                    if (connection.SourceField == "0xf8f067fa" || connection.SourceField == "0x588516b5")
                    {
                        if (connection.TargetField == "0x00000000" && !foundNames.ContainsValue(connection.SourceField))
                        {
                            App.Logger.Log("Found Existing Link Connection " + connection.SourceField);
                            foundNames.Add(found, connection.SourceField);
                            found++;
                        }

                    }
                }
            }
            if(found == 0)
            {
                foundNames.Add(0, "TEMP_CONNECTION");
            }
            //App.Logger.Log($"Found {found}");
            return foundNames;
        }
        
        public static AlternateSpawnEntityData CreateNewSpawnPoint(EbxAsset parentAsset, LinearTransform transform, string teamId, int priority, bool defaultEnabled)
        {
            AlternateSpawnEntityData data = new AlternateSpawnEntityData();
            data.Transform = transform;
            data.Team = (TeamId)Enum.Parse(typeof(TeamId), teamId, true);
            data.Flags = 369098752u;
            data.Priority = priority;
            data.Enabled = defaultEnabled;
            parentAsset.AddRootObject((object)data);
            return data;
        }
        public static PointerRef CreateExternalPointer(EbxAsset parentAsset, DataContainer obj) =>
            new PointerRef(
                new EbxImportReference()
                {
                    FileGuid = parentAsset.FileGuid,
                    ClassGuid = obj.GetInstanceGuid().ExportedGuid
                });
        public static void CreateLinkConnection(Blueprint bp,
            PointerRef a, string porta,
            PointerRef b, string portb) => CreateConnection2(bp, a, porta, b, portb, "link");
        public static void CreateConnection2(Blueprint bp,
            PointerRef a, string porta,
            PointerRef b, string portb,
            string targetType, string connectionType = "", uint flags = 3)
        {
            switch (targetType.ToLower())
            {
                case "event":
                    {
                        EventConnection connection = new EventConnection();
                        connection.Source = a;
                        connection.Target = b;
                        connection.SourceEvent = new() { Name = porta };
                        connection.TargetEvent = new() { Name = portb };
                        connection.TargetType = (EventConnectionTargetType)Enum.Parse(typeof(EventConnectionTargetType), $"EventConnectionTargetType_{connectionType}", true);
                        bp.EventConnections.Add(connection);
                    }
                    break;

                case "link":
                    {
                        LinkConnection connection = new LinkConnection();
                        connection.Source = a;
                        connection.Target = b;
                        connection.SourceField = porta;
                        connection.TargetField = portb;
                        bp.LinkConnections.Add(connection);
                    }
                    break;

                case "property":
                    {
                        PropertyConnection connection = new PropertyConnection();
                        connection.Source = a;
                        connection.Target = b;
                        connection.SourceField = porta;
                        connection.TargetField = portb;
                        connection.Flags = flags;
                        bp.PropertyConnections.Add(connection);
                    }
                    break;
            }
        }
    }
    public class DuplicateGameMode : DataExplorerContextMenuExtension
    {
        public string subWorldToDuplicate = "";
        public string subWorldToCopyLogicFrom = "";
        public string theNewSubWorld = "";
        public string duplicateToPath = "";
        public string newGameModeName;

        EbxAssetEntry originalGamemodesSubworld;
        EbxAsset originalGamemodesSubworldAsset;

        EbxAssetEntry logicLayerEntry;
        EbxAsset logicLayerAsset;
        EbxAssetEntry levelDataLogicLayerEntry;
        EbxAsset levelDataLogicLayerAsset;

        DuplicationTool.DuplicateAssetExtension ebxDuper;
        DuplicationTool.SubWorldDataExtension subWorldDuper;

        Dictionary<string, EbxAssetEntry> gamemodeSubworlds;
        Dictionary<string, List<EbxAssetEntry>> gamemodeLayers;
        Dictionary<string, Dictionary<string, EbxAssetEntry>> gamemodeLayersByName;

        public static PointerRef RefToFile(EbxAssetEntry entry) => RefToFile(App.AssetManager.GetEbx(entry));
        public static PointerRef RefToFile(string filePath) => RefToFile(App.AssetManager.GetEbxEntry(filePath));
        public static PointerRef RefToFile(EbxAsset asset) =>
            new PointerRef(new EbxImportReference { FileGuid = asset.FileGuid, ClassGuid = asset.RootInstanceGuid });

        public override string ContextItemName => "Duplicate Game Mode";

        public override RelayCommand ContextItemClicked => new RelayCommand(o =>
        {
            if (App.SelectedAsset.Type == "SubWorldData")
            {
                if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII))
                {
                    DuplicateGameModeWindow win = new DuplicateGameModeWindow(App.SelectedAsset as EbxAssetEntry);

                    if (win.ShowDialog() == false)
                        return;

                    subWorldToCopyLogicFrom = win.DuplicateFromPath;
                    theNewSubWorld = win.DuplicateToPath + "/" + win.GameModeName.Replace(" ", "");
                    duplicateToPath = win.DuplicateToPath;
                    newGameModeName = win.GameModeName;

                    App.Logger.Log("Duplicating " + subWorldToCopyLogicFrom.Split('/').Last() + " to " + theNewSubWorld);
                    App.Logger.Log("SubWorldDatas inhert from their superbundles. This can vary per game but to be safe we must duplicate a subworld that already exists on a level.");

                    ebxDuper = new();
                    subWorldDuper = new();
                    DupeSubworld();
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefront) || ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare2) || ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare))
                {
                    DuplicateGameModeWindow win = new DuplicateGameModeWindow(App.SelectedAsset as EbxAssetEntry);

                    if (win.ShowDialog() == false)
                        return;

                    subWorldToCopyLogicFrom = win.DuplicateFromPath;
                    theNewSubWorld = win.DuplicateToPath + "/" + win.GameModeName.Replace(" ", "");
                    duplicateToPath = win.DuplicateToPath;
                    newGameModeName = win.GameModeName;

                    App.Logger.Log("Duplicating " + subWorldToCopyLogicFrom.Split('/').Last() + " to " + theNewSubWorld);
                    App.Logger.Log("SubWorldDatas inhert from their superbundles. This can vary per game but to be safe we must duplicate a subworld that already exists on a level.");

                    ebxDuper = new();
                    subWorldDuper = new();
                    Dupe2015Subworld();
                }
            }
            else
            {
                App.Logger.Log("Only SubWorldDatas are supported. Files of type: " + App.SelectedAsset.Type + " are not supported.");
            }
        });
        public void Dupe2015Subworld()
        {
            gamemodeSubworlds = new();
            gamemodeLayers = new();
            gamemodeLayersByName = new();

            subWorldToDuplicate = App.SelectedAsset.Name;

            //Gets the subworld to copy logic from
            EbxAsset originalGameModeAsset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(subWorldToCopyLogicFrom));
            var originalSubWorld = originalGameModeAsset.RootObject as SubWorldData;

            //duplicates subworld
            EbxAssetEntry duplicatedSubWorld = App.AssetManager.GetEbxEntry(subWorldToDuplicate); //Copies the origina
            var newGameModeEntry = subWorldDuper.DuplicateAsset(duplicatedSubWorld, theNewSubWorld, false, TypeLibrary.GetType(nameof(SubWorldData)));
            var newGameModeAsset = App.AssetManager.GetEbx(newGameModeEntry);
            var newSubWorldData = newGameModeAsset.RootObject as SubWorldData;

            //copies the data from the original to the new
            ClearAll2015Connections(newSubWorldData, newGameModeAsset);
            newSubWorldData.Objects.Clear();
            if (originalSubWorld.Objects != null)
            {
                newSubWorldData.EventConnections = originalSubWorld.EventConnections;
                newSubWorldData.LinkConnections = originalSubWorld.LinkConnections;
                newSubWorldData.PropertyConnections = originalSubWorld.PropertyConnections;
                newSubWorldData.Interface = originalSubWorld.Interface;
                newSubWorldData.Objects = originalSubWorld.Objects;
            }

            Dupe2015WorldPartDatas(App.AssetManager.GetEbxEntry(subWorldToCopyLogicFrom), App.SelectedAsset.Path, newSubWorldData);
            App.AssetManager.ModifyEbx(newGameModeEntry.Name, newGameModeAsset);
        }
        public void DupeSubworld()
        {
            gamemodeSubworlds = new();
            gamemodeLayers = new();
            gamemodeLayersByName = new();

            subWorldToDuplicate = App.SelectedAsset.Name; //To Duplicate From

            //Gets the subworld to copy logic from
            EbxAsset originalGameModeAsset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(subWorldToCopyLogicFrom));
            var originalSubWorld = originalGameModeAsset.RootObject as SubWorldData;

            //duplicates subworld
            EbxAssetEntry duplicatedSubWorld = App.AssetManager.GetEbxEntry(subWorldToDuplicate); //Copies the origina
            var newGameModeEntry = subWorldDuper.DuplicateAsset(duplicatedSubWorld, theNewSubWorld, false, TypeLibrary.GetType(nameof(SubWorldData)));
            var newGameModeAsset = App.AssetManager.GetEbx(newGameModeEntry);
            var newSubWorldData = newGameModeAsset.RootObject as SubWorldData;

            ClearAllConnections(newSubWorldData, newGameModeAsset);
            if (originalSubWorld.Objects != null)
            {
                newSubWorldData.EventConnections = originalSubWorld.EventConnections;
                newSubWorldData.LinkConnections = originalSubWorld.LinkConnections;
                newSubWorldData.PropertyConnections = originalSubWorld.PropertyConnections;
                newSubWorldData.Interface = originalSubWorld.Interface;
                newSubWorldData.Objects = originalSubWorld.Objects;
            }
            DuplicateSubwWorldLayers(App.AssetManager.GetEbxEntry(subWorldToCopyLogicFrom), App.SelectedAsset.Path, newSubWorldData);
            App.AssetManager.ModifyEbx(newGameModeEntry.Name, newGameModeAsset);

        }
        public void DuplicateSubwWorldLayers(EbxAssetEntry existingGmSubworld, string name, SubWorldData subWorldData)
        {
           
            Dictionary<Guid, EbxAssetEntry> layers = new();
            Dictionary<Guid, Guid> oldPtrToNew = new();

            foreach (EbxAssetEntry layer in existingGmSubworld.DependentAssets.Select(App.AssetManager.GetEbxEntry).Where(entry => entry.Type == nameof(LayerData)))
            {
                EbxAssetEntry newLayer = ebxDuper.DuplicateAsset(layer, duplicateToPath + "/" + layer.Filename.Replace(subWorldToCopyLogicFrom.Split('/').Last(), newGameModeName.Replace(" ", "")) + "_Duped", false, typeof(LayerData));
                layers.Add(newLayer.Guid, newLayer);
                newLayer.AddedBundles.Clear();
                oldPtrToNew.Add(layer.Guid, newLayer.Guid);
            }

            gamemodeLayers.Add("DupedGameMode", layers.Values.ToList());
            gamemodeLayersByName.Add("DupedGameMode", layers.Values.ToDictionary(en => en.Filename, en => en));

            // correct LayerReference ptrs
            foreach (LayerReferenceObjectData layerRef in subWorldData.Objects.Where(ptr => ptr.Internal is LayerReferenceObjectData)
                                                                              .Select(ptr => ptr.Internal as LayerReferenceObjectData))
            {
                if (!oldPtrToNew.ContainsKey(layerRef.Blueprint.External.FileGuid))
                {
                    continue;
                }
                var newGuid = oldPtrToNew[layerRef.Blueprint.External.FileGuid];
                layerRef.Blueprint = DuplicateGameMode.RefToFile(App.AssetManager.GetEbxEntry(newGuid));
            }
            foreach (IEnumerable<dynamic> connectionGroup in new List<IEnumerable<dynamic>> { subWorldData.EventConnections.Select(v => (dynamic)v), subWorldData.PropertyConnections.Select(v => (dynamic)v), subWorldData.LinkConnections.Select(v => (dynamic)v) })
                foreach (var connection in connectionGroup)
                {
                    if (connection.Target.Type == PointerRefType.External && oldPtrToNew.ContainsKey(connection.Target.External.FileGuid))
                    {
                        connection.Target = new PointerRef(new EbxImportReference { FileGuid = oldPtrToNew[connection.Target.External.FileGuid], ClassGuid = connection.Target.External.ClassGuid });
                    }
                    if (connection.Source.Type == PointerRefType.External && oldPtrToNew.ContainsKey(connection.Source.External.FileGuid))
                    {
                        connection.Source = new PointerRef(new EbxImportReference { FileGuid = oldPtrToNew[connection.Source.External.FileGuid], ClassGuid = connection.Source.External.ClassGuid });
                    }
                }
            ApplyChanges();
        }
        public void Dupe2015WorldPartDatas(EbxAssetEntry existingGmSubworld, string name, SubWorldData subWorldData)
        {
            Dictionary<Guid, EbxAssetEntry> layers = new();
            Dictionary<Guid, Guid> oldPtrToNew = new();

            foreach (EbxAssetEntry layer in existingGmSubworld.DependentAssets.Select(App.AssetManager.GetEbxEntry).Where(entry => entry.Type == nameof(WorldPartData)))
            {
                EbxAssetEntry newLayer = ebxDuper.DuplicateAsset(layer, duplicateToPath + "/" + layer.Filename.Replace(subWorldToCopyLogicFrom.Split('/').Last(), newGameModeName.Replace(" ", "")) + "_Duped", false, typeof(WorldPartReferenceObjectData));
                layers.Add(newLayer.Guid, newLayer);
                newLayer.AddedBundles.Clear();
                oldPtrToNew.Add(layer.Guid, newLayer.Guid);

            }

            gamemodeLayers.Add("DupedGameMode", layers.Values.ToList());
            gamemodeLayersByName.Add("DupedGameMode", layers.Values.ToDictionary(en => en.Filename, en => en));

            foreach (WorldPartReferenceObjectData layerRef in subWorldData.Objects.Where(ptr => ptr.Internal is WorldPartReferenceObjectData)
                                                                              .Select(ptr => ptr.Internal as WorldPartReferenceObjectData))
            {
                if (!oldPtrToNew.ContainsKey(layerRef.Blueprint.External.FileGuid))
                {
                    continue;
                }
                var newGuid = oldPtrToNew[layerRef.Blueprint.External.FileGuid];
                layerRef.Blueprint = DuplicateGameMode.RefToFile(App.AssetManager.GetEbxEntry(newGuid));
            }
            foreach (IEnumerable<dynamic> connectionGroup in new List<IEnumerable<dynamic>> { subWorldData.EventConnections.Select(v => (dynamic)v), subWorldData.PropertyConnections.Select(v => (dynamic)v), subWorldData.LinkConnections.Select(v => (dynamic)v) })
                foreach (var connection in connectionGroup)
                {
                    if (connection.Target.Type == PointerRefType.External && oldPtrToNew.ContainsKey(connection.Target.External.FileGuid))
                    {
                        connection.Target = new PointerRef(new EbxImportReference { FileGuid = oldPtrToNew[connection.Target.External.FileGuid], ClassGuid = connection.Target.External.ClassGuid });
                    }
                    if (connection.Source.Type == PointerRefType.External && oldPtrToNew.ContainsKey(connection.Source.External.FileGuid))
                    {
                        connection.Source = new PointerRef(new EbxImportReference { FileGuid = oldPtrToNew[connection.Source.External.FileGuid], ClassGuid = connection.Source.External.ClassGuid });
                    }
                }
            ApplyChanges();
        }
        public void ApplyChanges()
        {
            Dictionary<string, EbxAssetEntry> _layerMap = gamemodeLayersByName["DupedGameMode"];
            Dictionary<string, EbxAsset> _layerAssetMap = _layerMap.Values.ToDictionary(e => e.Name, e => App.AssetManager.GetEbx(e));

            int index = 0;

            foreach (var pair in _layerAssetMap)
            {
                index++;
                App.AssetManager.ModifyEbx(pair.Key, pair.Value);
            }

            gamemodeLayersByName.Clear();
            gamemodeLayers.Clear();
            _layerMap.Clear();
            _layerAssetMap.Clear();

            App.Logger.Log("Successfully applied " + index + " assets");
            App.Logger.Log("Please save and restart Frosty to fix the layerData references!");
        }
        
        #region -- Tools --
        public void ClearAllConnections(Blueprint bp, EbxAsset asset)
        {
            //bp.Objects.Clear(); // Not really needed?

            bp.LinkConnections.Clear();
            bp.PropertyConnections.Clear();
            bp.EventConnections.Clear();
            if (bp.Interface.Type == PointerRefType.Null)
            {
                bp.Interface = new PointerRef(asset.AddObject(TypeLibrary.CreateObject(nameof(InterfaceDescriptorData))));
            }
            var _int = bp.Interface.Internal as InterfaceDescriptorData;
            _int.Fields.Clear();
            _int.OutputLinks.Clear();
            _int.OutputEvents.Clear();
            _int.InputLinks.Clear();
            _int.InputEvents.Clear();
            if (asset is not null)
            {
                asset.Update();
            }
        }
        public void ClearAll2015Connections(Blueprint bp, EbxAsset asset)
        {
            bp.LinkConnections.Clear();
            bp.PropertyConnections.Clear();
            bp.EventConnections.Clear();
            if (bp.Interface.Type == PointerRefType.Null)
            {
                bp.Interface = new PointerRef(asset.AddObject(TypeLibrary.CreateObject(nameof(InterfaceDescriptorData))));
            }
            var _int = bp.Interface.Internal as InterfaceDescriptorData;
            _int.Fields.Clear();
            _int.OutputLinks.Clear();
            _int.OutputEvents.Clear();
            _int.InputLinks.Clear();
            _int.InputEvents.Clear();
            if (asset is not null)
            {
                asset.Update();
            }
        }
       
        #endregion
    }
}
