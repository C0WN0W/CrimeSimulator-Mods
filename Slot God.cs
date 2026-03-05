using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

[BepInPlugin("CN_SlotGod", "SlotMachine Mod", "1.0.0")]
public class SlotMachineMod : BaseUnityPlugin
{
    public static ConfigEntry<int> MaxUsesPerDay;
    public static ConfigEntry<bool> Force777;

    private void Awake()
    {
        MaxUsesPerDay = Config.Bind("General", "MaxUsesPerDay", 999,
            "Times can use per day");

        Force777 = Config.Bind("General", "Force777", true,
            "Force win 777");

        var harmony = new Harmony("com.cownow.slotmachine");
        Logger.LogInfo("Slot God Loaded.");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(SlotMachine), "LoadMachine")]
class Patch_SlotMachine_LoadMachine
{
    static void Postfix(SlotMachine __instance)
    {
        __instance.remainingUses = SlotMachineMod.MaxUsesPerDay.Value;
    }
}

[HarmonyPatch(typeof(SlotMachine), "Update")]
class Patch_SlotMachine_Update
{
    static void Postfix(SlotMachine __instance)
    {
        if (SlotMachineMod.Force777.Value && __instance.checkWin)
        {
            foreach (var slot in __instance.slots)
            {
                if (slot.spriteIndexes.Count > 0)
                {
                    slot.spriteIndexes[slot.spriteIndexes.Count - 1] = 0;
                }
            }
        }
    }
}
