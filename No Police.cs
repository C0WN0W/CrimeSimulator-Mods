using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.cownow.nopolice", "No Police", "1.0.0")]
public class noPolicePatch : BaseUnityPlugin
{
    private void Awake()
    {
        var harmony = new Harmony("com.cownow.nopolice");
        harmony.PatchAll();
        Logger.LogInfo("No Police MOD loaded.");
    }
}

// Still using this shit Class
[HarmonyPatch(typeof(ThermalVision), "Update")]
class ThermalVisionUpdatePatch
{
    static void Postfix(ThermalVision __instance)
    {
        if (__instance.player.WM.policeManager != null)
            __instance.player.WM.policeManager.currentChaseLevel = 0;
            
    }
}