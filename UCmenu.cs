using BepInEx;
using BepInEx.Configuration;
using Fusion;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace CN_CheatMenu
{
    [BepInPlugin("UCModMenu", "Ultra Crack Menu", "1.3.5")]
    public class UltraCrack : BaseUnityPlugin
    {
        private static ConfigEntry<KeyCode> toggleMenuKey;
        

        private void Awake()
        {
            var type = AccessTools.TypeByName("ThermalVision");
            var method = type.GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                Logger.LogError(">>> You need to patch the game code before you can use this menu!");
                Destroy(this);
                return;
            }

            /*
            ImGui.CreateContext();
            ImGui.SetCurrentContext(ImGui.GetCurrentContext());

            var io = ImGui.GetIO();

            ImGui.StyleColorsLight();
            unsafe
            {
                byte* fontData;
                int fontSize = 18;
                ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();
                config.OversampleH = 3;
                config.OversampleV = 1;
                config.RasterizerMultiply = 1.0f;
                io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\msyhbd.ttc", fontSize, config, io.Fonts.GetGlyphRangesChineseFull());
                io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);
            }
            Logger.LogInfo("ImGui init success.");
            */
            toggleMenuKey = Config.Bind("General", "MenuHotkey", KeyCode.Tab, "Hotkey to toggle menu");
            Logger.LogInfo("Ultra Crack Menu Loaded.");
            var harmony = new Harmony("com.cownow.cheatmenu");
            harmony.PatchAll();
        }
        
    }

    public class MenuBehaviour : MonoBehaviour
    {
        internal static bool enableMenu = true;
        internal static Rect windowRect = new Rect(20, 20, 300, 400);
        internal static int currentTab = 0;

        internal static bool NoDamagedLoots;
        internal static bool ToolsAutoFix;
        internal static bool UnlimitedAmmo;
        internal static bool UltraArmor;
        internal static bool BigCash;
        internal static bool BigCart;
        internal static bool OverDraft;
        internal static bool NoFall;
        internal static bool ThermalActive;
        internal static bool ItemCopy;
        internal static bool InvisibilityNoCD;
        internal static bool NoNoise;
        internal static bool MarkLoots;
        internal static bool sendLoot;
        internal static bool NoPolice;
        internal static bool BlowtorchInstantUnlock;
        internal static bool OverFurnished;
        internal static bool Invisibility;
        internal static bool FreeShop;
        internal static bool BulletThroughWall;
        internal static bool InfinityAbility;
        internal static bool XPgain;
        internal static bool GodMode;
        internal static bool SelfRevive;
        internal static bool PickToTruck;

        internal static FPP_Player player;

        [HarmonyPatch(typeof(ThermalVision), "Update")]
        public static class ThermalVision_Update_Patch
        {
            static void Postfix(ThermalVision __instance)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    enableMenu = !enableMenu;
                    Debug.Log("Ultra Crack Menu: " + (enableMenu ? "Show" : "Hide"));
                }
                if (Input.GetKeyDown(KeyCode.V) && ThermalActive)
                {
                    __instance.thermalActive = !__instance.thermalActive;
                    __instance.ThermalOnOff(__instance.thermalActive);
                    __instance.player.ShowMessage("Thermal " + (__instance.thermalActive ? "Activated" : "Deactivated"), 1f);
                }
                // Mark Loots Function
                if (Input.GetKeyDown(KeyCode.F2) && MarkLoots)
                {
                    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Pickupable"))
                    {
                        if (gameObject.GetComponent<Pickupable>() && gameObject.GetComponent<Pickupable>().assignedItem != null && gameObject.GetComponent<Pickupable>().assignedItem.value >= 0)
                        {
                            gameObject.GetComponent<Pickupable>().alwaysSpawnMarker = true;
                            gameObject.GetComponent<Pickupable>().markerColor = Color.yellow;
                            gameObject.GetComponent<Pickupable>().SpawnMarker();
                        }
                    }
                }
                // Init player reference
                if (__instance.player == null && WorldManager.Instance != null)
                {
                    __instance.player = WorldManager.Instance.localPlayer;
                }
                if (__instance.player != null)
                {
                    __instance.player.blockAllNoise = NoNoise;
                    if (Input.GetKeyDown(KeyCode.B) && sendLoot)
                    {
                        __instance.player.inventory.SendLootToTruck();
                    }
                    if (Input.GetKeyDown(KeyCode.I) && Invisibility)
                    {
                        __instance.player.RPC_ActivateInvisibility(true, 99999f);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ItemMarker), "AssignPickup")]
        public static class AssignPickup_Patch
        {
            public static string text;
            static void Postfix(Pickupable pp)
            {
                text = pp.assignedItem.name;
            }
        }

        [HarmonyPatch(typeof(ThermalVision), "OnGUI")]
        public static class ThermalVision_OnGUI_Patch
        {
            static void DrawWindow(int id)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(currentTab == 0, "Plyer", GUI.skin.button)) currentTab = 0;
                if (GUILayout.Toggle(currentTab == 1, "Items", GUI.skin.button)) currentTab = 1;
                if (GUILayout.Toggle(currentTab == 2, "World", GUI.skin.button)) currentTab = 2;
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                switch (currentTab)
                {
                    case 0:
                        GodMode = GUILayout.Toggle(GodMode, " God Mode");
                        UnlimitedAmmo = GUILayout.Toggle(UnlimitedAmmo, " Unlimited Ammo");
                        UltraArmor = GUILayout.Toggle(UltraArmor, " Ultra Armor");
                        NoFall = GUILayout.Toggle(NoFall, " No Fall Damage");
                        NoNoise = GUILayout.Toggle(NoNoise, " No Noise");
                        InvisibilityNoCD = GUILayout.Toggle(InvisibilityNoCD, " Super Invisibility");
                        BulletThroughWall = GUILayout.Toggle(BulletThroughWall, " Pistol Through Wall");
                        InfinityAbility = GUILayout.Toggle(InfinityAbility, " Infinity Ability");
                        XPgain = GUILayout.Toggle(XPgain, " Super XP Gain");
                        ThermalActive = GUILayout.Toggle(ThermalActive, " Thermal Eye [V]");
                        Invisibility = GUILayout.Toggle(Invisibility, " Force Invisibility [I]");
                        SelfRevive = GUILayout.Toggle(SelfRevive, " Self Revive [F3]");
                        break;
                    case 1:
                        ToolsAutoFix = GUILayout.Toggle(ToolsAutoFix, " Tools Auto Fix");
                        NoDamagedLoots = GUILayout.Toggle(NoDamagedLoots, " No Damaged Loots");
                        BlowtorchInstantUnlock = GUILayout.Toggle(BlowtorchInstantUnlock, " Blowtorch Instant Unlock");
                        FreeShop = GUILayout.Toggle(FreeShop, " Free Shop Items");
                        PickToTruck = GUILayout.Toggle(PickToTruck, " Pick Item To Truck");
                        MarkLoots = GUILayout.Toggle(MarkLoots, " Mark All Loots [F2]");
                        sendLoot = GUILayout.Toggle(sendLoot, " Send Inventory to Truck [B]");
                        break;
                    case 2:
                        BigCash = GUILayout.Toggle(BigCash, " Big Cash");
                        NoPolice = GUILayout.Toggle(NoPolice, " No Police");
                        BigCart = GUILayout.Toggle(BigCart, " Shopping Cart No Limit");
                        OverDraft = GUILayout.Toggle(OverDraft, " Over Draft Shopping");
                        OverFurnished = GUILayout.Toggle(OverFurnished, " Over Furnished");
                        // ItemCopy = GUILayout.Toggle(ItemCopy, " Item Copy [F1+G]");
                        break;
                }

                GUI.DragWindow();
            }
            static void Postfix()
            {
                if (!enableMenu) return;

                GUI.skin.label.fontSize = 18;
                GUI.skin.button.fontSize = 18;
                GUI.skin.window.fontSize = 18;
                GUI.skin.toggle.fontSize = 18;

                windowRect = GUI.Window(0, windowRect, DrawWindow, "UC Mod Menu");
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), "PickedItem")]
        public static class PlayerInventory_PickedItem_Patch
        {
            static void Prefix(Pickupable item)
            {
                if (item == null) return;
                if (NoDamagedLoots)
                {
                    item.currentDamage = 0f;
                }
                if (item.assignedItem.isMoney && BigCash)
                {
                    item.addAmount *= 10000;
                }
            }
            static void Postfix(PlayerInventory __instance)
            {
                if (PickToTruck)
                {
                    __instance.SendLootToTruck();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), "AddDamageToCurrentItem")]
        public static class PlayerInventory_AddDamageToCurrentItem_Patch
        {
            static bool Prefix(PlayerInventory __instance, float dmg)
            {
                if (__instance.player.WM.tutorialJobs != null && __instance.player.WM.tutorialJobs.tutorialActive)
                    return false;

                var slot = __instance.itemSlots[__instance.currentItemIndex];
                if (slot == null || slot.assignedItem == null)
                    return false;

                float multi = 1f;
                if (!slot.assignedItem.isLoot)
                    multi = __instance.player.skillManager.toolDamageMulti;

                slot.AddDamage(dmg * multi);

                if (slot.ReturnCurrentDamage() >= 100f)
                {
                    if (ToolsAutoFix)
                    {
                        slot.AddDamage(-100f);
                        __instance.player.ShowMessage("Your tool fixed!", 3f, "good", false);
                        return false;
                    }
                    slot.RemoveFromSlot();
                    __instance.ActivateCurrentSlot();
                    __instance.player.ShowMessage(
                        LocalizationManager.GetTranslation("Your tool broke!", true, 0, true, false, null, null, true),
                        3f, "bad", true
                    );
                    __instance.player.sfxSource.PlayOneShot(__instance.toolBreakClip, 0.75f);

                    return false;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), "AddAmmo")]
        public static class PlayerInventory_AddAmmo_Patch
        {
            static bool Prefix(PlayerInventory __instance, Item ammoItem, int amount = 1)
            {
                if (!UnlimitedAmmo)
                {
                    return true;
                }

                foreach (Weapon weapon in __instance.weaponManager.weapons)
                {
                    if (weapon.ammoTypes.Count > 0)
                    {
                        for (int i = 0; i < weapon.ammoTypes.Count; i++)
                        {
                            if (weapon.ammoTypes[i].ammoItem == ammoItem)
                            {
                                weapon.ammoTypes[i].amountInInventory = 88888888;
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), "InstallArmor")]
        public static class PlayerInventory_InstallArmor_Patch
        {
            static bool Prefix(PlayerInventory __instance)
            {
                if (UltraArmor)
                {
                    __instance.player.hasArmor = true;
                    __instance.player.armorHealth = 99999f;
                    __instance.player.sfxSource.PlayOneShot(__instance.upgradeClip, 0.6f);
                    __instance.player.ShowMessage("Super armor installed", 3f, "good", false);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ComputerStore), "CheckStuff")]
        public static class ComputerStore_CheckStuff_Transpiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 10)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(ComputerStore_CheckStuff_Transpiler), nameof(GetCartLimit)));
                    }

                    if (codes[i].opcode == OpCodes.Ldc_I4_0 &&
                        i + 1 < codes.Count &&
                        codes[i + 1].Calls(typeof(UnityEngine.GameObject).GetMethod("SetActive")))
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(ComputerStore_CheckStuff_Transpiler), nameof(GetOverDraftActive)));
                    }
                }
                return codes;
            }
            public static int GetCartLimit()
            {
                return BigCart ? 99999 : 10;
            }
            public static bool GetOverDraftActive()
            {
                return OverDraft;
            }
        }

        [HarmonyPatch(typeof(InvisibilityTool), "Activate")]
        public static class InvisibilityTool_Activate_Patch
        {
            static void Postfix(InvisibilityTool __instance)
            {
                if (InvisibilityNoCD)
                {
                    __instance.player.RPC_ActivateInvisibility(true, 999f);
                    // Why only this shit is private field?
                    var isOnCooldownField = AccessTools.Field(typeof(InvisibilityTool), "isOnCooldown");
                    isOnCooldownField.SetValue(__instance, false);
                    __instance.cooldownTime = 0f;
                    __instance.ResetColor();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), "SendLootToTruck")]
        public static class PlayerInventory_SendLootToTruck_Patch
        {
            static void Postfix(PlayerInventory __instance)
            {
                var player = __instance.player;
                if (player == null || player.Runner == null) return;

                WeaponManager weaponManager = null;
                try
                {
                    weaponManager = __instance.weaponManager;
                }
                catch
                {
                    var wmField = AccessTools.Field(typeof(PlayerInventory), "weaponManager");
                    weaponManager = wmField?.GetValue(__instance) as WeaponManager;
                }
                if (weaponManager == null) return;
                var carriedItem = weaponManager.carriedItem;
                if (carriedItem == null || !carriedItem.isLoot) return;

                // Find Truck
                var truck = GameObject.FindGameObjectWithTag("SpawnLootTruck");
                if (truck == null) return;

                var netObj = player.Runner.Spawn(
                        carriedItem.itemDropPrefab,
                        new Vector3?(truck.transform.position),
                        new Quaternion?(Quaternion.identity),
                        null, null, (NetworkSpawnFlags)0
                );

                if (netObj != null)
                {
                    netObj.transform.eulerAngles += carriedItem.placementExtraRotation;

                    var pickup = netObj.GetComponent<Pickupable>();
                    if (pickup != null)
                    {
                        __instance.SetDroppedValues(
                            pickup,
                            weaponManager.carriedItemDamage,
                            weaponManager.carriedItemCustomProperty
                        );
                        pickup.SpawnMarker();
                    }
                }
                weaponManager.carriedItemDamage = 0f;
                weaponManager.carriedItemCustomProperty = "";
                weaponManager.carriedItem = null;
                weaponManager.isCarrying = false;
                weaponManager.currentLootItem = "";

                __instance.ActivateCurrentSlot();
            }

        }
        /*
        [HarmonyPatch(typeof(PlayerInventory), "DropItem")]
        public static class PlayerInventory_DropItem_Transpiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                var codes = new List<CodeInstruction>(instructions);
                var removeMethod = AccessTools.Method(typeof(ItemSlot), "RemoveFromSlot");

                // create a label to skip the remove call
                var skipRemove = generator.DefineLabel();

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].Calls(removeMethod))
                    {
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Input), "GetKeyDown", new[] { typeof(KeyCode) })));
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_I4, (int)KeyCode.F1));
                        codes.Insert(i + 2, new CodeInstruction(OpCodes.Brtrue, skipRemove));

                        codes[i + 3].labels.Add(skipRemove);

                        break;
                    }
                }

                return codes;
            }
        }
        
        [HarmonyPatch(typeof(PlayerInventory), "DropHeavyItem")]
        public static class PlayerInventory_DropHeavyItem_Transpiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].Calls(typeof(PlayerInventory).GetMethod("ActivateCurrentSlot")))
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(PlayerInventory_DropHeavyItem_Transpiler), nameof(HandleItemCopy)));
                    }
                }
                return codes;
            }

            public static void HandleItemCopy(PlayerInventory inv)
            {
                if (ItemCopy && Input.GetKey(KeyCode.F1))
                {
                    inv.ActivateCurrentSlot();
                }
                else
                {
                    inv.weaponManager.carriedItemDamage = 0f;
                    inv.weaponManager.carriedItemCustomProperty = "";
                    inv.weaponManager.carriedItem = null;
                    inv.weaponManager.isCarrying = false;
                    inv.weaponManager.currentLootItem = "";
                    inv.ActivateCurrentSlot();
                }
            }
        }
        */
        [HarmonyPatch(typeof(FPP_Player), "CheckMovementDirection")]
        public static class FPP_Player_CheckMovementDirection_Transpiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].Calls(typeof(FPP_Player).GetMethod("RPC_DealDamage")))
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(FPP_Player_CheckMovementDirection_Transpiler), nameof(HandleFallDamage)));
                    }
                }

                return codes;
            }
            public static void HandleFallDamage(FPP_Player player, float dmg)
            {
                if (NoFall)
                {
                    return;
                }
                player.RPC_DealDamage(dmg);
            }
        }

        [HarmonyPatch(typeof(BlowtorchObject), "AddDamage")]
        public static class BlowtorchObject_AddDamage_Patch
        {
            static bool Prefix(BlowtorchObject __instance)
            {
                if (!__instance.destroyed && BlowtorchInstantUnlock)
                {
                    __instance.RPC_DestroyLock();
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(FurnitureManager), "AmountOfFurniture")]
        public static class FurnitureManager_AmountOfFurniture_Patch
        {
            static bool Prefix(ref int __result)
            {
                if (OverFurnished)
                {
                    __result = 0;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ComputerStore), "ReturnItemValue")]
        public static class ComputerStore_ReturnItemValue_Patch
        {
            static bool Prefix(ref int __result)
            {
                if (FreeShop)
                {
                    __result = 0;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SilencedPistol), "ShotRay")]
        public static class SilencedPistol_ShotRay_Patch
        {
            static bool Prefix(SilencedPistol __instance)
            {
                if (BulletThroughWall)
                {
                    Ray ray = new Ray(__instance.shotSensor.transform.position, __instance.shotSensor.transform.forward);
                    RaycastHit[] hits = Physics.RaycastAll(ray, __instance.weaponManager.currentWeapon.attackSensorRange);
                    foreach (var hit in hits)
                    {
                        var npc = hit.collider.GetComponent<NPC_Hitbox>();
                        if (npc != null)
                        {
                            npc.DealDamage(__instance.weaponManager.currentWeapon.attackDamageToNPC, true, true);
                            return false;
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PoliceManager), "PoliceCalled")]
        public static class PoliceManager_PoliceCalled_Patch
        {
            static bool Prefix(PoliceManager __instance, int chaseLevel = 1)
            {
                if (!NoPolice)
                {
                    return true;
                }
                __instance.currentChaseLevel = 0;
                return false;
            }
        }
        
        [HarmonyPatch(typeof(PoliceManager), "TrySpawnPolice")]
        public static class PoliceManager_TrySpawnPolice_Patch
        {
            static bool Prefix(PoliceManager __instance)
            {
                if (!NoPolice)
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(AbilitiesManager), "ActivateAbility")]
        public static class AbilitiesManager_ActivateAbility_Patch
        {
            static void Postfix(string abilityID, AbilitiesManager __instance)
            {
                if (InfinityAbility)
                {
                    foreach (var ability in __instance.abilities)
                    {
                        if (ability.abilityID == abilityID)
                        {
                            ability.usedUp = false;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(AbilitiesManager), "DeactivateAllAbilities")]
        public static class AbilitiesManager_DeactivateAllAbilities_Patch
        {
            static bool Prefix()
            {
                if (!InfinityAbility)
                    return true;

                return false;
            }
        }
        [HarmonyPatch(typeof(AbilitiesSelector), "CheckAbilities")]
        public static class AbilitiesSelector_CheckAbilities_Patch
        {
            static bool Prefix(AbilitiesSelector __instance)
            {
                if (!InfinityAbility)
                    return true;

                for (int i = 0; i < __instance.radialMenu.UltimateRadialButtonList.Count; i++)
                {
                    var ability = __instance.buttons[i].assignedAbility;

                    ability.isUnlocked = true;
                    ability.usedUp = false;
                    ability.blockFromUse = false;

                    __instance.radialMenu.UltimateRadialButtonList[i].UpdateIcon(ability.abilityImage);
                    __instance.radialMenu.UltimateRadialButtonList[i].iconSize = __instance.buttons[i].unlockedImageSize;
                    __instance.radialMenu.UltimateRadialButtonList[i].OnEnable();
                }

                __instance.radialMenu.UpdatePositioning();

                return false;
            }
        }


        [HarmonyPatch(typeof(PlayerSkills), "ReceiveXP")]
        class PlayerSkills_ReceiveXP_Patch
        {
            static void Prefix(ref int amount)
            {
                if (XPgain)
                    amount *= 99999;
            }
        }

        [HarmonyPatch(typeof(FPP_Player), "DealDamageToPlayer")]
        class Patch_DealDamageToPlayer
        {
            static bool Prefix(ref float dmg)
            {
                if (GodMode)
                {
                    dmg = 0f;
                    return false;
                }
                return true;
            }
        }

        // Self resurrect test
        [HarmonyPatch(typeof(FPP_Player), "Update")]
        public static class PlayerSelfRevivePatch
        {
            static void Postfix(FPP_Player __instance)
            {
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    if (__instance.isDead && SelfRevive)
                    {
                        __instance.RPC_RessurectPlayer(false, true);
                    }
                }
            }
        }

    }
}
