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
            new AssetTypeDefinition("WSSoldierCustomizationKitAsset", "WSSoldierCustomizationKitAsset", "WSSoldierCustomizationKitAsset"),
            new AssetTypeDefinition("WSSoldierCustomizationKitAsset_SP", "WSSoldierCustomizationKitAsset_SP", "WSSoldierCustomizationKitAsset_SP"),
            new AssetTypeDefinition("WSBuddyCustomizationKitAsset_SP", "WSBuddyCustomizationKitAsset_SP", "WSBuddyCustomizationKitAsset_SP"),
            new AssetTypeDefinition("WSVehicleCustomizationKitAsset", "WSVehicleCustomizationKitAsset", "WSVehicleCustomizationKitAsset"),
            new AssetTypeDefinition("WSVehicleCustomizationKitAsset_SP", "WSVehicleCustomizationKitAsset_SP", "WSVehicleCustomizationKitAsset_SP"),
            new AssetTypeDefinition("WSSoldierCustomizationGameplayAsset", "WSSoldierCustomizationGameplayAsset", "WSSoldierCustomizationGameplayAsset"),
            new AssetTypeDefinition("WSVehicleCustomizationGameplayAsset", "WSVehicleCustomizationGameplayAsset", "WSVehicleCustomizationGameplayAsset"),
            new AssetTypeDefinition("WSSoldierCustomizationClassAsset", "WSSoldierCustomizationClassAsset", "WSSoldierCustomizationClassAsset"),
            new AssetTypeDefinition("WSVehicleCustomizationClassAsset", "WSVehicleCustomizationClassAsset", "WSVehicleCustomizationClassAsset"),
            new AssetTypeDefinition("UnlockAsset", "UnlockAsset", "UnlockAsset"),
            new AssetTypeDefinition("ValueUnlockAsset", "ValueUnlockAsset", "ValueUnlockAsset"),
            new AssetTypeDefinition("PlayerAbilityAsset", "PlayerAbilityAsset", "PlayerAbilityAsset"),
            new AssetTypeDefinition("PassivePlayerAbilityAsset", "PassivePlayerAbilityAsset", "PassivePlayerAbilityAsset"),
            new AssetTypeDefinition("BasicPlayerAbilityAsset", "BasicPlayerAbilityAsset", "BasicPlayerAbilityAsset"),
            new AssetTypeDefinition("CharacterStatePlayerAbilityAsset", "CharacterStatePlayerAbilityAsset", "CharacterStatePlayerAbilityAsset"),
            new AssetTypeDefinition("InactiveSoldierWeaponPlayerAbilityAsset", "InactiveSoldierWeaponPlayerAbilityAsset", "InactiveSoldierWeaponPlayerAbilityAsset"),
            new AssetTypeDefinition("EmoteCharacterStatePlayerAbilityAsset", "EmoteCharacterStatePlayerAbilityAsset", "EmoteCharacterStatePlayerAbilityAsset"),
            new AssetTypeDefinition("EmoteAsset", "EmoteAsset", "EmoteAsset"),
            new AssetTypeDefinition("SoldierWeaponUnlockAsset", "SoldierWeaponUnlockAsset", "SoldierWeaponUnlockAsset"),
            new AssetTypeDefinition("SoldierWeaponCustomizationModifier", "SoldierWeaponCustomizationModifier", "SoldierWeaponCustomizationModifier"),
            new AssetTypeDefinition("SoldierWeaponPlayerAbilityAsset", "SoldierWeaponPlayerAbilityAsset", "SoldierWeaponPlayerAbilityAsset"),
            new AssetTypeDefinition("AltFireSoldierWeaponPlayerAbilityAsset", "AltFireSoldierWeaponPlayerAbilityAsset", "AltFireSoldierWeaponPlayerAbilityAsset"),
            new AssetTypeDefinition("MaxHealthAffectorAsset", "MaxHealthAffectorAsset", "MaxHealthAffectorAsset"),
            new AssetTypeDefinition("SoldierHealthRegenerarionAffectorAsset", "SoldierHealthRegenerarionAffectorAsset", "SoldierHealthRegenerarionAffectorAsset"),
            new AssetTypeDefinition("DamageMultiplierAffectorAsset", "DamageMultiplierAffectorAsset", "DamageMultiplierAffectorAsset"),
            new AssetTypeDefinition("CharacterStateAffectorAsset", "CharacterStateAffectorAsset", "CharacterStateAffectorAsset"),
            new AssetTypeDefinition("RemoveAffectorAsset", "RemoveAffectorAsset", "RemoveAffectorAsset"),
            new AssetTypeDefinition("DamageAffectorAsset", "DamageAffectorAsset", "DamageAffectorAsset"),
            new AssetTypeDefinition("BasicAffectorAsset", "BasicAffectorAsset", "BasicAffectorAsset"),
            new AssetTypeDefinition("AbilityInputMappingAffectorAsset", "AbilityInputMappingAffectorAsset", "AbilityInputMappingAffectorAsset"),
            new AssetTypeDefinition("HealingAffectorAsset", "HealingAffectorAsset", "HealingAffectorAsset"),
            new AssetTypeDefinition("FlashAffectorAsset", "FlashAffectorAsset", "FlashAffectorAsset"),
            new AssetTypeDefinition("DamageShieldAffectorAsset", "DamageShieldAffectorAsset", "DamageShieldAffectorAsset"),
            new AssetTypeDefinition("RechargeAbilitiesAffectorAsset", "RechargeAbilitiesAffectorAsset", "RechargeAbilitiesAffectorAsset"),
            new AssetTypeDefinition("OverheatAffectorAsset", "OverheatAffectorAsset", "OverheatAffectorAsset"),
            new AssetTypeDefinition("HealingBlockAffectorAsset", "HealingBlockAffectorAsset", "HealingBlockAffectorAsset"),
            new AssetTypeDefinition("AffectorImmunityAffectorAsset", "AffectorImmunityAffectorAsset", "AffectorImmunityAffectorAsset"),
            new AssetTypeDefinition("WSFactionAsset", "WSFactionAsset", "WSFactionAsset"),
            new AssetTypeDefinition("InputActionMapsData", "InputActionMapsData", "InputActionMapsData"),
            //new AssetTypeDefinition("WSInputConfigurationAsset", "WSInputConfigurationAsset", "WSInputConfigurationAsset"),
            new AssetTypeDefinition("VehicleWeaponPlayerAbilityAsset","VehicleWeaponPlayerAbilityAsset","VehicleWeaponPlayerAbilityAsset"),
            new AssetTypeDefinition("VehicleSlowAffectorAsset","VehicleSlowAffectorAsset","VehicleSlowAffectorAsset"),
            new AssetTypeDefinition("UIDataTag","UIDataTag","UIDataTag"),
            new AssetTypeDefinition("ProfileValueCounter","ProfileValueCounter","ProfileValueCounter"),
            new AssetTypeDefinition("ProfileValueFlags","ProfileValueFlags","ProfileValueFlags"),
        };
    }
}
