using System.Collections.Generic;

namespace SearchIdentifier.Common
{
    public class AssetTypeDefinition
    {
        public string DisplayName;   
        public string EbxType;       
        public string CacheSuffix;   

        public AssetTypeDefinition(string displayName, string ebxType, string cacheSuffix)
        {
            DisplayName = displayName;
            EbxType = ebxType;
            CacheSuffix = cacheSuffix;
        }
        public override string ToString() => DisplayName; 
    }

    public static class AssetTypeRegistry
    {
        public static readonly List<AssetTypeDefinition> Types = new List<AssetTypeDefinition>
        {
            new AssetTypeDefinition("Kit", "WSSoldierCustomizationKitAsset", "Kit"),
            new AssetTypeDefinition("Kit_SP", "WSSoldierCustomizationKitAsset_SP", "Kit_SP"),
            new AssetTypeDefinition("Kit_SP_Buddy", "WSBuddyCustomizationKitAsset_SP", "Kit_SP_Buddy"),
            new AssetTypeDefinition("Kit_Vehicle", "WSVehicleCustomizationKitAsset", "Kit_Vehicle"),
            new AssetTypeDefinition("Kit_SP_Vehicle", "WSVehicleCustomizationKitAsset_SP", "Kit_SP_Vehicle"),
            new AssetTypeDefinition("GP", "WSSoldierCustomizationGameplayAsset", "GP"),
            new AssetTypeDefinition("GP_Vehicle", "WSVehicleCustomizationGameplayAsset", "GP_Vehicle"),
            new AssetTypeDefinition("Class", "WSSoldierCustomizationClassAsset", "Class"),
            new AssetTypeDefinition("Class_Vehicle", "WSVehicleCustomizationClassAsset", "Class_Vehicle"),
            new AssetTypeDefinition("Unlock", "UnlockAsset", "Unlock"),
            new AssetTypeDefinition("ValueUnlock", "ValueUnlockAsset", "ValueUnlock"),
            new AssetTypeDefinition("Ability", "PlayerAbilityAsset", "Ability"),
            new AssetTypeDefinition("PassivePlayerAbility", "PassivePlayerAbilityAsset", "PassivePlayerAbility"),
            new AssetTypeDefinition("BasicPlayerAbility", "BasicPlayerAbilityAsset", "BasicPlayerAbility"),
            new AssetTypeDefinition("CharacterStatePlayerAbility", "CharacterStatePlayerAbilityAsset", "CharacterStatePlayerAbility"),
            new AssetTypeDefinition("InactiveSoldierWeaponPlayerAbility", "InactiveSoldierWeaponPlayerAbilityAsset", "InactiveSoldierWeaponPlayerAbility"),
            new AssetTypeDefinition("EmoteCharacterStatePlayerAbility", "EmoteCharacterStatePlayerAbilityAsset", "EmoteCharacterStatePlayerAbility"),
            new AssetTypeDefinition("Emote", "EmoteAsset", "Emote"),
            new AssetTypeDefinition("SoldierWeaponUnlock", "SoldierWeaponUnlockAsset", "SoldierWeaponUnlock"),
            new AssetTypeDefinition("SoldierWeaponCustomization", "SoldierWeaponCustomizationModifier", "SoldierWeaponCustomization"),
            new AssetTypeDefinition("SoldierWeaponPlayerAbility", "SoldierWeaponPlayerAbilityAsset", "SoldierWeaponPlayerAbility"),
            new AssetTypeDefinition("AltFireSoldierWeaponPlayerAbility", "AltFireSoldierWeaponPlayerAbilityAsset", "AltFireSoldierWeaponPlayerAbility"),
            new AssetTypeDefinition("MaxHealthAffector", "MaxHealthAffectorAsset", "MaxHealthAffector"),
            new AssetTypeDefinition("SoldierHealthRegenerarionAffector", "SoldierHealthRegenerarionAffectorAsset", "SoldierHealthRegenerarionAffector"),
            new AssetTypeDefinition("DamageMultiplierAffector", "DamageMultiplierAffectorAsset", "DamageMultiplierAffector"),
            new AssetTypeDefinition("CharacterStateAffector", "CharacterStateAffectorAsset", "CharacterStateAffector"),
            new AssetTypeDefinition("RemoveAffector", "RemoveAffectorAsset", "RemoveAffector"),
            new AssetTypeDefinition("DamageAffector", "DamageAffectorAsset", "DamageAffector"),
            new AssetTypeDefinition("BasicAffector", "BasicAffectorAsset", "BasicAffector"),
            new AssetTypeDefinition("AbilityInputMappingAffector", "AbilityInputMappingAffectorAsset", "AbilityInputMappingAffector"),
            new AssetTypeDefinition("HealingAffector", "HealingAffectorAsset", "HealingAffector"),
            new AssetTypeDefinition("FlashAffector", "FlashAffectorAsset", "FlashAffector"),
            new AssetTypeDefinition("DamageShieldAffector", "DamageShieldAffectorAsset", "DamageShieldAffector"),
            new AssetTypeDefinition("RechargeAbilitiesAffector", "RechargeAbilitiesAffectorAsset", "RechargeAbilitiesAffector"),
            new AssetTypeDefinition("OverheatAffector", "OverheatAffectorAsset", "OverheatAffector"),
            new AssetTypeDefinition("HealingBlockAffector", "HealingBlockAffectorAsset", "HealingBlockAffector"),
            new AssetTypeDefinition("AffectorImmunityAffector", "AffectorImmunityAffectorAsset", "AffectorImmunityAffector"),
            new AssetTypeDefinition("WSFaction", "WSFactionAsset", "WSFaction"),
            //new AssetTypeDefinition("SchematicChannel", "SchematicChannelAsset", "SchematicChannel"),
            //new AssetTypeDefinition("DataContainer", "DataContainerAsset", "DataContainer"),
            new AssetTypeDefinition("InputActionMaps", "InputActionMapsData", "InputActionMaps"),
            //new AssetTypeDefinition("WSInputConfiguration", "WSInputConfigurationAsset", "WSInputConfiguration"),
        };
    }
}