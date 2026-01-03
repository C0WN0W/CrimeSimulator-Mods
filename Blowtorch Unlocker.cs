using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.cownow.blowtorch", "Blowtorch Unlocker", "1.0.0")]
public class BlowtorchPatchPlugin : BaseUnityPlugin
{
    void Awake()
    {
        var harmony = new Harmony("com.cownow.blowtorch");
        harmony.PatchAll();
        Logger.LogInfo("Blowtorch Patch loaded.");
    }
}

[HarmonyPatch(typeof(BlowtorchObject), "AddDamage")]
class Blowtorch
{
    static bool Prefix(BlowtorchObject __instance)
    {
        if (!__instance.destroyed)
        {
            __instance.RPC_DestroyLock();
        }
        return false;
    }

}
