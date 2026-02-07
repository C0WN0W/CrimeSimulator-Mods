using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("CN_SendTruckToBox", "Send Truck to DeliveryBox", "1.0.0")]
public class MoveLootPlugin : BaseUnityPlugin
{
    public static ConfigEntry<KeyCode> Hotkey;
    public static ConfigEntry<int> MaxItemsPerMove;

    private void Awake()
    {
        Hotkey = Config.Bind("General", "Hotkey", KeyCode.L, "Hotkey");
        MaxItemsPerMove = Config.Bind("General", "MaxItems", 3, "Amounts of items per move");

        var harmony = new Harmony("com.cownow.moveloot");
        harmony.PatchAll();
        Logger.LogInfo("Send Truck to DeliveryBox loaded.");
    }
    public KeyCode GetHotkey() => Hotkey.Value;
    public int GetMaxItems() => MaxItemsPerMove.Value;

}

[HarmonyPatch(typeof(MagicLever), "MoveLootFromTruck")]
public static class MagicLeverPatch
{
    public static void ForceMoveLoot(int maxItems)
    {
        var saver = WorldManager.Instance.truckItemsSaver;
        if (saver == null || saver.sensor == null || saver.sensor.Detections == null) return;

        GameObject spawn = GameObject.FindGameObjectWithTag("DeliveryBoxSpawn");
        if (spawn == null) return;

        int num = 0;
        foreach (GameObject obj in saver.sensor.Detections)
        {
            if (obj == null) continue;
            if (num >= maxItems) break;

            var pickup = obj.GetComponent<Pickupable>();
            if (pickup != null && pickup.assignedItem != null && pickup.assignedItem.isLoot)
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                pickup.RPC_SetPosition(spawn.transform.position + offset);
                pickup.RPC_SetToKinematic(false);
                num++;
            }
        }
    }

}

[HarmonyPatch(typeof(ThermalVision), "Update")]
class ThermalVision_Update_Patch
{
    static void Postfix(ThermalVision __instance)
    {
        if (__instance.player != null && Input.GetKeyDown(MoveLootPlugin.Hotkey.Value))
        {
            MagicLeverPatch.ForceMoveLoot(MoveLootPlugin.MaxItemsPerMove.Value);
        }

    }
}

