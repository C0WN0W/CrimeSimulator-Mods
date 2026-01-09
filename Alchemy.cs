using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CN_Alchemy
{
    [BepInPlugin("com.cownow.furnace", "Alchemy Mod", "1.0.0")]
    public class FurnaceAlwaysGoldPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony("com.cownow.furnace");
            harmony.PatchAll();
            Logger.LogInfo("Alchemy Mod Loaded.");
        }
    }

    [HarmonyPatch(typeof(JewelryFurnace), "CheckItems")]
    public static class JewelryFurnace_CheckItems_Patch
    {
        static bool Prefix(JewelryFurnace __instance)
        {
            if (__instance.itemSensor == null || __instance.itemSensor.Detections.Count == 0)
            {
                return false;
            }

            __instance.canvasObj.SetActive(true);
            __instance.circleUI.SetActive(true);

            __instance.working = true;
            __instance.circleImage.fillAmount = 0f;

            __instance.createType = (Random.value < 0.5f) ? 0 : 1;

            return false;
        }
    }


}

