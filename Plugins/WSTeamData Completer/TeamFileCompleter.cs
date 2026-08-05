using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CrossEraPlugin
{
    public class TeamFileCompleter : MenuExtension
    {
        public override string TopLevelMenuName => "Tools";

        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Team File Completer";

        public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Database.png") as ImageSource;

        public class KitData
        {
            public string KitName;
            public UInt32 GPIdentifier;
            public UInt32 ClassIdentifier;
        }

        public static List<KitData> SavedKitData = new List<KitData>();
        public static string KitSearch(EbxAssetEntry KitEntry)
        {
            string AssetName = KitEntry.Name;
            int Idx = SavedKitData.FindIndex(f => f.KitName == KitEntry.Name);
            if (Idx == -1)
            {
                EbxAsset KitAsset = App.AssetManager.GetEbx(KitEntry);
                dynamic KitRoot = KitAsset.RootObject;
                dynamic GPRoot = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(KitRoot.Gameplay.External.FileGuid)).RootObject;
                UInt32 GPIdentifier = GPRoot.Identifier;
                dynamic ClassRoot = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(GPRoot.Class.External.FileGuid)).RootObject;
                UInt32 ClassIdentifier = ClassRoot.Identifier;
                SavedKitData.Add(new KitData {KitName = KitEntry.Name, GPIdentifier = GPIdentifier, ClassIdentifier = ClassIdentifier});
            }
            return AssetName;
        }

        public class FactionData
        {
            public string KitName;
            public EbxAssetEntry FactionEntry;
        }
        public static List<FactionData> SavedFactionData = new List<FactionData>();
        public static EbxAssetEntry FactionSearch(EbxAssetEntry KitEntry)
        {
            int Idx = SavedFactionData.FindIndex(f => f.KitName == KitEntry.Name);
            EbxAssetEntry FactionEntry = null;
            if (Idx == -1)
            {
                EbxAsset KitAsset = App.AssetManager.GetEbx(KitEntry);
                dynamic KitRoot = KitAsset.RootObject;
                FactionEntry = App.AssetManager.GetEbxEntry(KitRoot.WSFaction.External.FileGuid);
                SavedFactionData.Add(new FactionData { KitName = KitEntry.Name, FactionEntry = FactionEntry });
                Idx = SavedFactionData.Count - 1;
            }
            else
            {
                FactionEntry = SavedFactionData[Idx].FactionEntry;
            }

            return FactionEntry;
        }
        public EbxImportReference GetAssetGUID(string AssetName)
        {
            EbxAssetEntry AssetEntry = App.AssetManager.GetEbxEntry(AssetName);
            EbxAsset AssetAsset = App.AssetManager.GetEbx(AssetEntry);
            dynamic AssetRoot = AssetAsset.RootObject;
            AssetClassGuid AssetGuid = AssetRoot.GetInstanceGuid();
            EbxImportReference AssetReference = new EbxImportReference()
            {
                FileGuid = AssetAsset.FileGuid,
                ClassGuid = AssetGuid.ExportedGuid
            };
            return AssetReference;
        }

        public PointerRef ReferenceAdder(string AssetName)
        {
            EbxAssetEntry AssetEntry = App.AssetManager.GetEbxEntry(AssetName);
            EbxAsset AssetAsset = App.AssetManager.GetEbx(AssetEntry);
            dynamic AssetRoot = AssetAsset.RootObject;
            AssetClassGuid AssetGuid = AssetRoot.GetInstanceGuid();
            EbxImportReference AssetReference = new EbxImportReference()
            {
                FileGuid = AssetAsset.FileGuid,
                ClassGuid = AssetGuid.ExportedGuid
            };
            PointerRef AssetRef = new PointerRef(AssetReference);
            return AssetRef;
        }

        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            FrostyTaskWindow.Show("Completing Team Files", "", (task) =>
            {
                List<Guid> KitLists = new List<Guid>();
                List<EbxAssetEntry> TeamFiles = new List<EbxAssetEntry>();
                foreach (EbxAssetEntry KitListEntry in App.AssetManager.EnumerateEbx("WSSoldierCustomizationKitList"))
                {
                    if (KitListEntry.HasModifiedData)
                    {
                        KitLists.Add(KitListEntry.Guid);
                    }
                }
                foreach (EbxAssetEntry KitListEntry in App.AssetManager.EnumerateEbx("WSVehicleCustomizationKitList"))
                {
                    if (KitListEntry.HasModifiedData)
                    {
                        KitLists.Add(KitListEntry.Guid);
                    }
                }
                foreach (EbxAssetEntry TeamEntry in App.AssetManager.EnumerateEbx("WSTeamData"))
                {
                    if (TeamEntry.HasModifiedData)
                    {
                        TeamFiles.Add(TeamEntry);
                    }
                    else
                    {
                        foreach (Guid KitList in KitLists)
                        {
                            if (TeamEntry.ContainsDependency(KitList))
                            {
                                TeamFiles.Add(TeamEntry);
                                break;
                            }
                        }
                    }
                }

                foreach (EbxAssetEntry TeamEntry in TeamFiles)
                {
                    EbxAsset TeamAsset = App.AssetManager.GetEbx(TeamEntry);
                    task.Update(TeamEntry.Name);
                    dynamic TeamRoot = TeamAsset.RootObject;
                    string OutputLogString = string.Format("EBX:{0}, ", TeamEntry.Name);


                    EbxAssetEntry SoldierListEntry = App.AssetManager.GetEbxEntry(TeamRoot.Soldiers.External.FileGuid);
                    EbxAssetEntry FactionEntry = null;
                    try
                    {
                        if (SoldierListEntry != null)
                        {
                            EbxAsset SoldierListAsset = App.AssetManager.GetEbx(SoldierListEntry);
                            dynamic SoldierListRoot = SoldierListAsset.RootObject;
                            if (SoldierListRoot.Kits.Count > 0)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(SoldierListRoot.Kits[0].External.FileGuid);
                                FactionEntry = FactionSearch(KitEntry);
                            }
                        }
                        else
                        {
                            if (TeamRoot.Soldiers.Internal.Kits.Count > 0)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(TeamRoot.Soldiers.Internal.Kits[0].External.FileGuid);
                                FactionEntry = FactionSearch(KitEntry);
                            }
                        }
                    }
                    catch
                    {
                    }
                    if (FactionEntry != null)
                    {
                        TeamRoot.WSFaction = ReferenceAdder(FactionEntry.Name);
                        TeamAsset.AddDependency(GetAssetGUID(FactionEntry.Name).FileGuid);
                        OutputLogString += string.Format("Faction:{0}, ", FactionEntry.DisplayName);
                    }
                    else
                    {
                        OutputLogString += "Faction: Null, ";
                    }

                    EbxAssetEntry HeroListEntry = App.AssetManager.GetEbxEntry(TeamRoot.Heroes.External.FileGuid);
                    List<string> HeroEntries = new List<string>();
                    try
                    {
                        if (HeroListEntry != null)
                        {
                            EbxAsset HeroListAsset = App.AssetManager.GetEbx(HeroListEntry);
                            dynamic HeroListRoot = HeroListAsset.RootObject;
                            foreach(PointerRef HeroRef in HeroListRoot.Kits)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(HeroRef.External.FileGuid);
                                HeroEntries.Add(KitSearch(KitEntry));
                            }
                        }
                        else
                        {
                            foreach (PointerRef HeroRef in TeamRoot.Heroes.Internal.Kits)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(HeroRef.External.FileGuid);
                                HeroEntries.Add(KitSearch(KitEntry));
                            }
                        }
                    }
                    catch
                    {
                    }
                    if (HeroEntries.Count > 0)
                    {
                        TeamRoot.HeroIdCollection.Internal.Characters.Clear();
                        TeamRoot.HeroClassIdCollection.Internal.CharacterClasses.Clear();
                        int HeroCount = 0;
                        foreach (string KitName in HeroEntries)
                        {
                            HeroCount++;
                            dynamic characterIdData = TypeLibrary.CreateObject("CharacterIdData");
                            dynamic characterClassIdData = TypeLibrary.CreateObject("CharacterClassIdData");
                            int Idx = SavedKitData.FindIndex(f => f.KitName == KitName);
                            characterIdData.CharacterId = SavedKitData[Idx].GPIdentifier;
                            characterIdData.CharacterClassId = SavedKitData[Idx].ClassIdentifier;
                            characterClassIdData.CharacterClassId = SavedKitData[Idx].ClassIdentifier;
                            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                            characterIdData.SetInstanceGuid(guid);
                            TeamRoot.HeroIdCollection.Internal.Characters.Add(new PointerRef(characterIdData));
                            TeamAsset.AddObject(characterIdData, true);
                            AssetClassGuid guid2 = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                            characterClassIdData.SetInstanceGuid(guid2);
                            TeamRoot.HeroClassIdCollection.Internal.CharacterClasses.Add(new PointerRef(characterClassIdData));
                            TeamAsset.AddObject(characterClassIdData, true);
                        }
                        OutputLogString += string.Format("Heroes:{0}, ", HeroCount);
                    }
                    else
                    {
                        OutputLogString += string.Format("Heroes:0, ");
                    }
                    EbxAssetEntry SpecialListEntry = App.AssetManager.GetEbxEntry(TeamRoot.SpecialSoldiers.External.FileGuid);
                    List<string> SpecialEntries = new List<string>();
                    try
                    {
                        if (SpecialListEntry != null)
                        {
                            EbxAsset SpecialListAsset = App.AssetManager.GetEbx(SpecialListEntry);
                            dynamic SpecialListRoot = SpecialListAsset.RootObject;
                            foreach (PointerRef SpecialRef in SpecialListRoot.Kits)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(SpecialRef.External.FileGuid);
                                SpecialEntries.Add(KitSearch(KitEntry));
                            }
                        }
                        else
                        {
                            foreach (PointerRef SpecialRef in TeamRoot.SpecialSoldiers.Internal.Kits)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(SpecialRef.External.FileGuid);
                                SpecialEntries.Add(KitSearch(KitEntry));
                            }
                        }
                    }
                    catch
                    {
                    }
                    if (SpecialEntries.Count > 0)
                    {
                        TeamRoot.SpecialSoldiersIdCollection.Internal.Characters.Clear();
                        TeamRoot.SpecialClassIdCollection.Internal.CharacterClasses.Clear();
                        int SpecialCount = 0;
                        List<UInt32> AddedSpecialClassIDs = new List<UInt32>();
                        foreach (string KitName in SpecialEntries)
                        {
                            SpecialCount++;
                            dynamic characterIdData = TypeLibrary.CreateObject("CharacterIdData");
                            dynamic characterClassIdData = TypeLibrary.CreateObject("CharacterClassIdData");
                            int Idx = SavedKitData.FindIndex(f => f.KitName == KitName);
                            characterIdData.CharacterId = SavedKitData[Idx].GPIdentifier;
                            characterIdData.CharacterClassId = SavedKitData[Idx].ClassIdentifier;
                            characterClassIdData.CharacterClassId = SavedKitData[Idx].ClassIdentifier;
                            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                            characterIdData.SetInstanceGuid(guid);
                            TeamRoot.SpecialSoldiersIdCollection.Internal.Characters.Add(new PointerRef(characterIdData));
                            TeamAsset.AddObject(characterIdData, true);
                            if (AddedSpecialClassIDs.Contains(SavedKitData[Idx].ClassIdentifier) == false)
                            {
                                AssetClassGuid guid2 = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                                characterClassIdData.SetInstanceGuid(guid2);
                                TeamRoot.SpecialClassIdCollection.Internal.CharacterClasses.Add(new PointerRef(characterClassIdData));
                                AddedSpecialClassIDs.Add(SavedKitData[Idx].ClassIdentifier);
                            }
                            TeamAsset.AddObject(characterClassIdData, true);
                        }
                        OutputLogString += string.Format("Specials:{0}, ", SpecialCount);
                    }
                    else
                    {
                        OutputLogString += string.Format("Specials:0, ");
                    }
                    EbxAssetEntry VehicleListEntry = App.AssetManager.GetEbxEntry(TeamRoot.Vehicles.External.FileGuid);
                    List<string> VehicleEntries = new List<string>();
                    try
                    {
                        if (VehicleListEntry != null)
                        {
                            EbxAsset VehicleListAsset = App.AssetManager.GetEbx(VehicleListEntry);
                            dynamic VehicleListRoot = VehicleListAsset.RootObject;
                            foreach (PointerRef VehicleRef in VehicleListRoot.Kits)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(VehicleRef.External.FileGuid);
                                VehicleEntries.Add(KitSearch(KitEntry));
                            }
                        }
                        else
                        {
                            foreach (PointerRef VehicleRef in TeamRoot.Vehicles.Internal.Kits)
                            {
                                EbxAssetEntry KitEntry = App.AssetManager.GetEbxEntry(VehicleRef.External.FileGuid);
                                VehicleEntries.Add(KitSearch(KitEntry));
                            }
                        }
                    }
                    catch
                    {
                    }
                    if (VehicleEntries.Count > 0)
                    {
                        TeamRoot.VehicleIdCollection.Internal.Characters.Clear();
                        TeamRoot.VehicleClassIdCollection.Internal.CharacterClasses.Clear();
                        int VehicleCount = 0;
                        foreach (string KitName in VehicleEntries)
                        {
                            VehicleCount++;
                            dynamic characterIdData = TypeLibrary.CreateObject("CharacterIdData");
                            dynamic characterClassIdData = TypeLibrary.CreateObject("CharacterClassIdData");
                            int Idx = SavedKitData.FindIndex(f => f.KitName == KitName);
                            characterIdData.CharacterId = SavedKitData[Idx].GPIdentifier;
                            characterIdData.CharacterClassId = SavedKitData[Idx].ClassIdentifier;
                            characterClassIdData.CharacterClassId = SavedKitData[Idx].ClassIdentifier;
                            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                            characterIdData.SetInstanceGuid(guid);
                            TeamRoot.VehicleIdCollection.Internal.Characters.Add(new PointerRef(characterIdData));
                            TeamAsset.AddObject(characterIdData, true);
                            AssetClassGuid guid2 = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                            characterClassIdData.SetInstanceGuid(guid2);
                            TeamRoot.VehicleClassIdCollection.Internal.CharacterClasses.Add(new PointerRef(characterClassIdData));
                            TeamAsset.AddObject(characterClassIdData, true);
                        }
                        OutputLogString += string.Format("Vehicles:{0}, ", VehicleCount);
                    }
                    else
                    {
                        OutputLogString += string.Format("Vehicles:0, ");
                    }
                    App.AssetManager.ModifyEbx(TeamEntry.Name, TeamAsset);
                    App.Logger.Log(OutputLogString);
                    SavedFactionData.Clear();


                    //foreach (EbxAssetEntry TeamEntry in TeamFiles)
                    //{
                    //    try
                    //    {
                    //        EbxAsset TeamAsset = App.AssetManager.GetEbx(TeamEntry);
                    //        dynamic TeamRoot = TeamAsset.RootObject;

                    //        List<EbxAssetEntry> HeroKits = new List<EbxAssetEntry>();
                    //        List<EbxAssetEntry> SpecialKits = new List<EbxAssetEntry>();

                    //        try
                    //        {
                    //            PointerRef heroRef = new PointerRef();
                    //            heroRef = TeamRoot.Heroes;

                    //            EbxAssetEntry HeroEntry = App.AssetManager.GetEbxEntry(heroRef.External.FileGuid);
                    //            EbxAsset HeroAsset = App.AssetManager.GetEbx(HeroEntry);
                    //            dynamic HeroRoot = HeroAsset.RootObject;

                    //            foreach (PointerRef NewRefEntry in HeroRoot.Kits)
                    //            {
                    //                HeroKits.Add(App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid));
                    //                task.Update(string.Format("Hero: {0}", App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid).Name));
                    //            }
                    //        }
                    //        catch
                    //        {
                    //            foreach (PointerRef NewRefEntry in TeamRoot.Heroes.Internal.Kits)
                    //            {
                    //                HeroKits.Add(App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid));
                    //                task.Update(string.Format("Hero: {0}", App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid).Name));
                    //            }
                    //        }
                    //        //task.Update(string.Format("1"));
                    //        try
                    //        {
                    //            //task.Update(string.Format("2"));
                    //            PointerRef specialRef = new PointerRef();
                    //            specialRef = TeamRoot.SpecialSoldiers;

                    //            EbxAssetEntry SpecialEntry = App.AssetManager.GetEbxEntry(specialRef.External.FileGuid);
                    //            EbxAsset SpecialAssetTemp = App.AssetManager.GetEbx(SpecialEntry);
                    //            dynamic SpecialRoot = SpecialAssetTemp.RootObject;
                    //            //task.Update(string.Format("3"));
                    //            foreach (PointerRef NewRefEntry in SpecialRoot.Kits)
                    //            {
                    //                SpecialKits.Add(App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid));
                    //                task.Update(string.Format("Special: {0}", App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid).Name));
                    //            }
                    //            //task.Update(string.Format("4"));
                    //        }
                    //        catch
                    //        {
                    //            //task.Update(string.Format("5"));
                    //            foreach (PointerRef NewRefEntry in TeamRoot.SpecialSoldiers.Internal.Kits)
                    //            {
                    //                SpecialKits.Add(App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid));
                    //                task.Update(string.Format("Special: {0}", App.AssetManager.GetEbxEntry(NewRefEntry.External.FileGuid).Name));
                    //            }
                    //            //task.Update(string.Format("6"));
                    //        }
                    //        //task.Update(string.Format("7"));
                    //        try
                    //        {
                    //            TeamRoot.HeroIdCollection.Internal.Characters.Clear();
                    //        }
                    //        catch
                    //        {
                    //            dynamic CharacterIdCollection = TypeLibrary.CreateObject("CharacterIdCollection");
                    //            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            CharacterIdCollection.SetInstanceGuid(guid);
                    //            TeamRoot.HeroIdCollection = (new PointerRef(CharacterIdCollection));
                    //            TeamAsset.AddObject(CharacterIdCollection, true);
                    //        }
                    //        try
                    //        {
                    //            TeamRoot.HeroClassIdCollection.Internal.CharacterClasses.Clear();
                    //        }
                    //        catch
                    //        {
                    //            dynamic CharacterClassIdCollection = TypeLibrary.CreateObject("CharacterClassIdCollection");
                    //            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            CharacterClassIdCollection.SetInstanceGuid(guid);
                    //            TeamRoot.HeroClassIdCollection = (new PointerRef(CharacterClassIdCollection));
                    //            TeamAsset.AddObject(CharacterClassIdCollection, true);
                    //        }
                    //        try
                    //        {
                    //            TeamRoot.SpecialSoldiersIdCollection.Internal.Characters.Clear();
                    //        }
                    //        catch
                    //        {
                    //            dynamic CharacterIdCollection = TypeLibrary.CreateObject("CharacterIdCollection");
                    //            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            CharacterIdCollection.SetInstanceGuid(guid);
                    //            TeamRoot.SpecialSoldiersIdCollection = (new PointerRef(CharacterIdCollection));
                    //            TeamAsset.AddObject(CharacterIdCollection, true);
                    //        }
                    //        try
                    //        {
                    //            TeamRoot.SpecialClassIdCollection.Internal.CharacterClasses.Clear();
                    //        }
                    //        catch
                    //        {
                    //            dynamic CharacterClassIdCollection = TypeLibrary.CreateObject("CharacterClassIdCollection");
                    //            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            CharacterClassIdCollection.SetInstanceGuid(guid);
                    //            TeamRoot.SpecialClassIdCollection = (new PointerRef(CharacterClassIdCollection));
                    //            TeamAsset.AddObject(CharacterClassIdCollection, true);
                    //        }
                    //        //task.Update(string.Format("8"));
                    //        foreach (EbxAssetEntry Hero in HeroKits)
                    //        {
                    //            //App.Logger.Log(string.Format("Added {0} to team", Hero.Name));
                    //            EbxAsset HeroAsset = App.AssetManager.GetEbx(Hero);
                    //            dynamic characterIdData = TypeLibrary.CreateObject("CharacterIdData");
                    //            dynamic characterClassIdData = TypeLibrary.CreateObject("CharacterClassIdData");

                    //            foreach (Guid ReferenceGuid in HeroAsset.Dependencies)
                    //            {
                    //                EbxAssetEntry GPEntry = App.AssetManager.GetEbxEntry(ReferenceGuid);
                    //                if (GPEntry.Type == "WSSoldierCustomizationGameplayAsset")
                    //                {
                    //                    EbxAsset GPAsset = App.AssetManager.GetEbx(GPEntry);
                    //                    dynamic GPRoot = GPAsset.RootObject;
                    //                    UInt32 GPIdentifier = GPRoot.Identifier;
                    //                    foreach (Guid GPReferenceGuid in GPAsset.Dependencies)
                    //                    {
                    //                        EbxAssetEntry ClassEntry = App.AssetManager.GetEbxEntry(GPReferenceGuid);
                    //                        if (ClassEntry.Type == "WSSoldierCustomizationClassAsset")
                    //                        {
                    //                            EbxAsset ClassAsset = App.AssetManager.GetEbx(ClassEntry);
                    //                            dynamic ClassRoot = ClassAsset.RootObject;
                    //                            UInt32 ClassIdentifier = ClassRoot.Identifier;
                    //                            characterIdData.CharacterClassId = ClassIdentifier;
                    //                            characterIdData.CharacterId = GPIdentifier;
                    //                            characterClassIdData.CharacterClassId = ClassIdentifier;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            characterIdData.SetInstanceGuid(guid);
                    //            TeamRoot.HeroIdCollection.Internal.Characters.Add(new PointerRef(characterIdData));
                    //            TeamAsset.AddObject(characterIdData, true);
                    //            AssetClassGuid guid2 = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            characterClassIdData.SetInstanceGuid(guid2);
                    //            TeamRoot.HeroClassIdCollection.Internal.CharacterClasses.Add(new PointerRef(characterClassIdData));
                    //            TeamAsset.AddObject(characterClassIdData, true);
                    //        }
                    //        List<UInt32> AddedSpecialClasses = new List<UInt32>() { };
                    //        foreach (EbxAssetEntry Special in SpecialKits)
                    //        {
                    //            //App.Logger.Log(string.Format("Added Special {0} to team", Special.Name));
                    //            EbxAsset SpecialAsset = App.AssetManager.GetEbx(Special);
                    //            dynamic characterIdData = TypeLibrary.CreateObject("CharacterIdData");
                    //            dynamic characterClassIdData = TypeLibrary.CreateObject("CharacterClassIdData");
                    //            bool dontadd = false;

                    //            foreach (Guid ReferenceGuid in SpecialAsset.Dependencies)
                    //            {
                    //                EbxAssetEntry GPEntry = App.AssetManager.GetEbxEntry(ReferenceGuid);
                    //                if (GPEntry.Type == "WSSoldierCustomizationGameplayAsset")
                    //                {
                    //                    EbxAsset GPAsset = App.AssetManager.GetEbx(GPEntry);
                    //                    dynamic GPRoot = GPAsset.RootObject;
                    //                    UInt32 GPIdentifier = GPRoot.Identifier;
                    //                    foreach (Guid GPReferenceGuid in GPAsset.Dependencies)
                    //                    {
                    //                        EbxAssetEntry ClassEntry = App.AssetManager.GetEbxEntry(GPReferenceGuid);
                    //                        if (ClassEntry.Type == "WSSoldierCustomizationClassAsset")
                    //                        {
                    //                            EbxAsset ClassAsset = App.AssetManager.GetEbx(ClassEntry);
                    //                            dynamic ClassRoot = ClassAsset.RootObject;
                    //                            UInt32 ClassIdentifier = ClassRoot.Identifier;
                    //                            characterIdData.CharacterClassId = ClassIdentifier;
                    //                            characterIdData.CharacterId = GPIdentifier;
                    //                            characterClassIdData.CharacterClassId = ClassIdentifier;
                    //                            if (AddedSpecialClasses.Contains(ClassIdentifier))
                    //                            {
                    //                                dontadd = true;
                    //                            }
                    //                            else
                    //                            {
                    //                                AddedSpecialClasses.Add(ClassIdentifier);
                    //                            }
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //            AssetClassGuid guid = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //            characterIdData.SetInstanceGuid(guid);
                    //            TeamRoot.SpecialSoldiersIdCollection.Internal.Characters.Add(new PointerRef(characterIdData));
                    //            TeamAsset.AddObject(characterIdData, true);
                    //            if (dontadd == false)
                    //            {
                    //                AssetClassGuid guid2 = new AssetClassGuid(Utils.GenerateDeterministicGuid(TeamAsset.Objects, (Type)TeamAsset.GetType(), TeamAsset.FileGuid), -1);
                    //                characterClassIdData.SetInstanceGuid(guid2);
                    //                TeamRoot.SpecialClassIdCollection.Internal.CharacterClasses.Add(new PointerRef(characterClassIdData));
                    //                TeamAsset.AddObject(characterClassIdData, true);
                    //            }
                    //        }
                    //        App.AssetManager.ModifyEbx(TeamEntry.Name, TeamAsset);
                    //        App.Logger.Log(string.Format("Completed: {0}", TeamEntry.Name));
                    //    }
                    //    catch
                    //    {
                    //        App.Logger.Log(string.Format("Failed: {0}", TeamEntry.Name));
                    //    }

                    //}
                }

            });
        });

    }
}
