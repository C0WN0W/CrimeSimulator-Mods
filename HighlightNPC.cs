using BepInEx;
using HarmonyLib;
using HighlightPlus;
using MinimapRadar;
using UnityEngine;

namespace CN_HighlightNPC
{
    [BepInPlugin("CN_HighlightNPC", "Highlight NPC", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony("com.cownow.highlightnpc");
            harmony.PatchAll();
            Logger.LogInfo("Highlight NPC plugin loaded.");
        }
    }

    [HarmonyPatch(typeof(NPC), "Update")]
    class NPC_Patch
    {
        static readonly AccessTools.FieldRef<NPC, HighlightEffect> highlightRef =
            AccessTools.FieldRefAccess<NPC, HighlightEffect>("highlight");

        static readonly AccessTools.FieldRef<NPC, MinimapItem> minimapRef =
            AccessTools.FieldRefAccess<NPC, MinimapItem>("minimapIcon");

        static void Postfix(NPC __instance)
        {
            __instance.isHighlighted = true;
            __instance.highlightTime = float.MaxValue;

            var highlight = highlightRef(__instance);
            if (highlight != null)
                highlight.highlighted = true;

            var minimap = minimapRef(__instance);
            if (minimap != null)
                minimap.enabled = true;
        }
    }
}
