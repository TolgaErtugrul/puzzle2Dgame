using UnityEngine;

public static class ShopManager
{
    // Eşya Fiyatları
    public const int EXTRA_TIME_PRICE = 10;
    public const int CANCEL_BOMB_PRICE = 15;

    public static bool BuyExtraTime()
    {
        int stars = PlayerPrefs.GetInt("TotalStars", 0);
        if (stars >= EXTRA_TIME_PRICE)
        {
            PlayerPrefs.SetInt("TotalStars", stars - EXTRA_TIME_PRICE);
            // Satın alınan miktar
            int currentAmount = PlayerPrefs.GetInt("Inventory_ExtraTime", 0);
            PlayerPrefs.SetInt("Inventory_ExtraTime", currentAmount + 1);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public static bool BuyCancelBomb()
    {
        int stars = PlayerPrefs.GetInt("TotalStars", 0);
        if (stars >= CANCEL_BOMB_PRICE)
        {
            PlayerPrefs.SetInt("TotalStars", stars - CANCEL_BOMB_PRICE);
            int currentAmount = PlayerPrefs.GetInt("Inventory_CancelBomb", 0);
            PlayerPrefs.SetInt("Inventory_CancelBomb", currentAmount + 1);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
