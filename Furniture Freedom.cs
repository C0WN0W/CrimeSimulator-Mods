using BepInEx;
using HarmonyLib;
using I2.Loc;
using UnityEngine;

[BepInPlugin("com.cownow.furniturefreedom", "Furniture Freedom", "1.0.0")]
public class FurnitureFreedomPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        var harmony = new Harmony("com.cownow.furniturefreedom");
        harmony.PatchAll();
        Logger.LogInfo("Furniture Freedom loaded!");
    }
}

// No placement restrictions
[HarmonyPatch(typeof(WeaponManager), "FurniturePlacement")]
class Patch_FurniturePlacement
{
    static void Prefix(WeaponManager __instance)
    {
        if (__instance.furnitureMode && __instance.furnitureShadow != null)
        {
            // Always show furniture shadow
            __instance.furnitureShadow.gameObject.SetActive(true);

            // Allow force placement
            __instance.furnitureShadow.canPlace = true;
        }
    }

    static void Postfix(WeaponManager __instance)
    {
        if (__instance.furnitureMode && __instance.furnitureShadow != null)
        {
            if (__instance.player.rewiredPlayer.GetButtonDown("Interact"))
            {
                if (__instance.furnitureShadow.gameObject.activeInHierarchy)
                {
                    __instance.StopCarryFurniture(false);
                }
            }
        }
    }
}

// No furniture limit
[HarmonyPatch(typeof(FurnitureManager), "AmountOfFurniture")]
class Patch_AmountOfFurniture
{
    static bool Prefix(ref int __result)
    {
        __result = 0;
        return false;
    }
}

// Patch for Furniture Ordering - no cost, instant delivery, and achievement unlocking
[HarmonyPatch(typeof(FurnitureWebsite), "OrderFurniture")]
class Patch_OrderFurniture
{
    static bool Prefix(FurnitureWebsite __instance)
    {

        // Access private fields using Harmony's Traverse
        var selectedFurniture = Traverse.Create(__instance).Field("selectedFurniture").GetValue<Furniture>();
        var WM = Traverse.Create(__instance).Field("WM").GetValue<WorldManager>();
        var moneyClip = Traverse.Create(__instance).Field("moneyClip").GetValue<AudioClip>();

        WM.furnitureManager.OrderFurniture(selectedFurniture.prefab.gameObject);

        __instance.gameObject.GetComponent<AudioSource>().PlayOneShot(moneyClip);

        if (selectedFurniture.achievementOnPurchase != "")
        {
            SteamAchievementManager.UnlockAchievement(selectedFurniture.achievementOnPurchase);
        }

        // No cost for furniture
        // WM.RPC_AddMoney(-selectedFurniture.cost);

        // Page transition
        var selectedPage = Traverse.Create(__instance).Field("selectedPage").GetValue<GameObject>();
        var thankYouPage = Traverse.Create(__instance).Field("thankYouPage").GetValue<GameObject>();
        selectedPage.SetActive(true);
        thankYouPage.SetActive(false);

        __instance.CheckReturnButton();

        return false;

    }
}

[HarmonyPatch(typeof(FurnitureWebsite), "SelectFurniture")]
class Patch_SelectFurniture
{
    static void Postfix(FurnitureWebsite __instance)
    {
        // Modify balance display to always show ₡0
        __instance.priceTxt.text = "0₡";
    }
}

[HarmonyPatch(typeof(FurnitureWebsite), "CheckIfCanBuy")]
class Patch_CheckIfCanBuy
{
    static void Postfix(FurnitureWebsite __instance)
    {
        // Can always buy furniture, so set actualPrice to 0 and update UI accordingly
        var field = AccessTools.Field(typeof(FurnitureWebsite), "actualPrice");
        field.SetValue(__instance, 0);

        __instance.orderButton.gameObject.SetActive(true);
        __instance.limitReachedObj.SetActive(false);
        __instance.orderButton.interactable = true;
        __instance.orderTxt.text = LocalizationManager.GetTranslation("order", true, 0, true, false, null, null, true) + " (0₡)"; ;
        __instance.orderTxt.color = Color.white;
    }
}
