using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace CN_Alchemy
{
    [BepInPlugin("CN_Alchemy", "JewelryFurnaceMod", "1.0.1")]
    public class JewelryFurnaceMod : BaseUnityPlugin
    {
        public static ConfigEntry<float> BurnDuration;

        private void Awake()
        {
            var harmony = new Harmony("com.cownow.furnace");
            harmony.PatchAll();

            BurnDuration = Config.Bind("General", "BurnDuration", 10f, "Burn Duration");

            Logger.LogInfo("Alchemy Mod Loaded.");
        }
        public static float BurnDurationValue => BurnDuration.Value;
    }


    [HarmonyPatch(typeof(JewelryFurnace), "Update")]
    class JewelryFurnace_Update_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 10f)
                {
                    codes[i] = new CodeInstruction(OpCodes.Call,
                        AccessTools.PropertyGetter(typeof(JewelryFurnaceMod), nameof(JewelryFurnaceMod.BurnDurationValue)));
                }
            }

            return codes;
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
