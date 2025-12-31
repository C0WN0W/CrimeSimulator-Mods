using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.cownow.stackplus", "Stack Everything", "1.0.0")]
public class StackPatch : BaseUnityPlugin
{
    private void Awake()
    {
        var harmony = new Harmony("com.cownow.stackplus");
        harmony.PatchAll();
        Logger.LogInfo("Stack Everything MOD loaded.");
    }
}

// Still using this shit Class
[HarmonyPatch(typeof(ItemSlot), "RefreshAmount")]
class ItemSlotRefreshPatch
{
    static bool Prefix(ItemSlot __instance)
    {
        if (__instance.assignedItem != null && __instance.inventory.player.skillManager.maxStack > 1)
        {
            __instance.stackUI.SetActive(true);
            __instance.stackTxt.text = __instance.amount.ToString() + "/" + __instance.inventory.skills.maxStack.ToString();
            return false;
        }

        __instance.stackUI.SetActive(false);
        return false;
    }
}
[HarmonyPatch(typeof(PlayerInventory), "AddItem")]
class PlayerInventoryAddItemPatch
{
    static void Prefix(Item item)
    {
        if (item != null)
        {
            item.canStack = true; // force stack
        }
    }
}
[HarmonyPatch(typeof(PlayerInventory), "Update")]
class PlayerInventoryUpdatePatch
{
    static void Postfix(PlayerInventory __instance)
    {
        if (__instance != null && __instance.skills != null)
        {
            __instance.skills.maxStack = 99;
        }
    }
}

