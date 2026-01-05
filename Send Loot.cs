using BepInEx;
using Fusion;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.cownow.sendloot", "Send Inventory to Truck", "1.0.0")]
public class SendLootPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        var harmony = new Harmony("com.cownow.sendloot");
        harmony.PatchAll();
        Logger.LogInfo("SITT Load.");
    }
}

// Patch for heavy items
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

// Hijack to add keybind
[HarmonyPatch(typeof(ThermalVision), "Update")]
class ThermalVision_Update_Patch
{
    static void Postfix(ThermalVision __instance)
    {
        if (__instance.player != null && Input.GetKeyDown(KeyCode.B))
            __instance.player.inventory.SendLootToTruck();

    }
}