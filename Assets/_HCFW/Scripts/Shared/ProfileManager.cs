using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCFW;

public class ProfileManager : MonoBehaviour {

    #region Singleton
    private static ProfileManager _Instance;
    public static ProfileManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<ProfileManager>();
            return _Instance;
        }
    }
    #endregion
}

[System.Serializable]
public class PlayerGameProfile
{
    [Header("Basic Data")]
    public int currentLevel = 1; // current level, 1 by default
    public int bestScore = 0; // 0 by default
    public int coins = 0; // coins or other type of currency that game might use

    [Header("Ad Related Data")]
    public int currentAdCycle = 1;

    [Header("Shop Data")]
    public List<int> unlockedShopItemsType_1 = new List<int>() { 0 }; // list of indexes from shopItems (ShopManager) which are unlocked
    public List<int> unlockedShopItemsType_2 = new List<int>() { 0 }; // list of indexes from shopItems (ShopManager) which are unlocked
    public int activeShopItemIndex = 0; // active "skin" or object

    public int activeShopItemType_1_Index = 0;
    public int activeShopItemType_2_Index = 0;

    [Header("Shop Data")]
    public int activeThemeIndex = 0;

    [Header("Cycle Data")]
    public LevelsCycle currentLevelsCycleData;
    public int progressToReward = 0;
    public int RewardProgress
    {
        get
        {
            return progressToReward;
        }
        set
        {
            progressToReward = value;
        }
    }
    // extend this any way you need, by adding new public variables / properties
    // when changed, it's good to delete/reset json data in location (check ReadJsonToString() for path, e.g. C:/Users/.../AppData/LocalLow/...)

}
