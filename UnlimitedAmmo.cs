using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CN_UnlimitedAmmo
{
    [BepInPlugin("CN_UnlimitedAmmo", "UnlimitedAmmo", "1.0")]
    public class UnlimitedAmmoMod : BaseUnityPlugin
    {
        public static ConfigEntry<bool> Crossbow;
        public static ConfigEntry<bool> Pistol;
        public static ConfigEntry<bool> StunGun;

        private void Awake()
        {
            var harmony = new Harmony("com.cownow.ammomod");
            harmony.PatchAll();

            Crossbow = Config.Bind("General", "Crossbow", true, "Enable Crossbow unlimited ammo");
            Pistol = Config.Bind("General", "Pistol", false, "Enable Pistol unlimited ammo");
            StunGun = Config.Bind("General", "StunGun", true, "Enable StunGun unlimited ammo");

            Logger.LogInfo("Unlimited Ammo Mod Loaded.");
        }
        public static bool EnabledCrossbow => Crossbow.Value;
        public static bool EnabledPistol => Pistol.Value;
        public static bool EnabledStunGun => StunGun.Value;
    }

    // Taser
    [HarmonyPatch(typeof(StunGun), "TriggerAttack")]
    class Patch_StunGun_TriggerAttack
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                if (instr.opcode == OpCodes.Sub)
                {
                    // replace the method
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Patch_StunGun_TriggerAttack), nameof(ReplaceAmmoOp)));
                }
                else
                {
                    yield return instr;
                }
            }
        }

        // Check config at runtime to decide whether to add or subtract ammo
        public static int ReplaceAmmoOp(int ammo, int value)
        {
            if (UnlimitedAmmoMod.EnabledStunGun)
                return ammo + value; // mod
            return ammo - value;     // original
        }
    }

    // Pistol
    [HarmonyPatch(typeof(SilencedPistol), "TriggerAttack")]
    class Patch_SilencedPistol_TriggerAttack
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                if (instr.opcode == OpCodes.Sub)
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Patch_SilencedPistol_TriggerAttack), nameof(ReplaceAmmoOp)));
                }
                else
                {
                    yield return instr;
                }
            }
        }

        public static int ReplaceAmmoOp(int ammo, int value)
        {
            if (UnlimitedAmmoMod.EnabledPistol)
                return ammo + value; 
            return ammo - value;    
        }
    }


    // Crossbow
    [HarmonyPatch(typeof(Crossbow), "TriggerShot")]
    class Patch_Crossbow_TriggerShot
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                if (instr.opcode == OpCodes.Sub)
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Patch_Crossbow_TriggerShot), nameof(ReplaceAmmoOp)));
                }
                else
                {
                    yield return instr;
                }
            }
        }

        public static int ReplaceAmmoOp(int ammo, int value)
        {
            if (UnlimitedAmmoMod.EnabledCrossbow)
                return ammo + value;
            return ammo - value;
        }
    }


}
