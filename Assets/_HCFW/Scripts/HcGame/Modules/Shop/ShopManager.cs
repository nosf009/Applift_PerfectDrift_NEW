using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HCFW
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Global Control")]
        public bool isShopEnabled = false;

        [Header("References")]
        public ShopUiItem itemUi;
        public ScrollRect scrollArea;
        public Button shopTab_1_Btn;
        public Button shopTab_2_Btn;
        public Button rvButton;
        public ShopItemType activeShopItemType;

        [Header("Integration config")]
        public Transform localShopView;
        public Button localShopButton;
        public Button localCloseButton;

        // Shop items for inspector configuration
        [Header("Item config")]
        public List<ShopItem> shopItems_1 = new List<ShopItem>();
        public List<ShopItem> shopItems_2 = new List<ShopItem>();

        public List<ShopUiItem> shopUiItems = new List<ShopUiItem>();

        private void Start()
        {
#if SHOP_MODULE
            if (isShopEnabled)
            {
                Debug.Log("SHOP_MODULE Start()");
                GameManager.Instance.MenuManager.shopView = localShopView;
                localShopButton.onClick.AddListener(OpenShopOnTap);
                localCloseButton.onClick.AddListener(CloseShopOnTap);
                GameObject canvasMain = GameObject.FindGameObjectWithTag("HCFW_Canvas");
                localShopView.SetParent(canvasMain.transform);
                localShopView.SetAsLastSibling();
                localShopButton.transform.SetParent(GameManager.Instance.MenuManager.mainView);
                localShopButton.transform.SetAsLastSibling();
                localShopButton.gameObject.SetActive(true);
            }
#endif
        }

        public void Init()
        {
            if (shopTab_1_Btn != null)
            {
                shopTab_1_Btn.onClick.RemoveAllListeners();
                shopTab_1_Btn.onClick.AddListener(OnShopTab_1_Clicked);
            }

            if (shopTab_2_Btn != null)
            {
                shopTab_2_Btn.onClick.RemoveAllListeners();
                shopTab_2_Btn.onClick.AddListener(OnShopTab_2_Clicked);
            }

            if (activeShopItemType == ShopItemType.ShopItemType1)
            {
                int index = 0;
                CleanUpPreviousInit();
                SyncShopWithSavedData();

             

                foreach (ShopItem item in shopItems_1)
                {

                    if (item.shopItemType == ShopItemType.ShopItemType1)
                    {
                        item.itemPrefab = GameManager.Instance.tcv.GetCarGameObjectByIndex(item.itemId);
                        GameObject uiGo = Instantiate(itemUi.gameObject, scrollArea.content);
                        ShopUiItem uiItem = uiGo.GetComponent<ShopUiItem>();
                        item.itemPic = RenderTextureCamera.Instance.createdSpritesFromGameObjects[index];
                        uiItem.Init(item, index);
                        shopUiItems.Add(uiItem);
                        index++;
                    }
                }
            }

            if (activeShopItemType == ShopItemType.ShopItemType2)
            {
                int index = 0;
                CleanUpPreviousInit();
                SyncShopWithSavedData();

                foreach (ShopItem item in shopItems_2)
                {
                    if (item.shopItemType == ShopItemType.ShopItemType2)
                    {
                        GameObject uiGo = Instantiate(itemUi.gameObject, scrollArea.content);
                        ShopUiItem uiItem = uiGo.GetComponent<ShopUiItem>();
                        uiItem.Init(item, index);
                        shopUiItems.Add(uiItem);

                        index++;
                    }
                }
            }

            rvButton.onClick.RemoveAllListeners();
            rvButton.onClick.AddListener(GetCoins);

        }

        void CloseShopOnTap()
        {
            GameManager.Instance.MenuManager.EnableView(GameManager.Instance.MenuManager.mainView);
        }

        void OpenShopOnTap()
        {
            Init();
            GameManager.Instance.MenuManager.EnableView(GameManager.Instance.MenuManager.shopView);
        }

        public void OnShopTab_1_Clicked()
        {
            activeShopItemType = ShopItemType.ShopItemType1;
            Init();
        }

        public void OnShopTab_2_Clicked()
        {
            activeShopItemType = ShopItemType.ShopItemType2;
            Init();
        }

        void GetCoins()
        {
#if SERVICES_MODULE
            if (ServicesManager.Instance)
            {
                ServicesManager.Instance.ShowRewardedVideo(GiveRewardOnComplete);
            }
#endif
        }

        void GiveRewardOnComplete()
        {
            rvButton.interactable = false;
            GameManager.Instance.activeProfile.coins += 150;
            DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile;
            GameManager.Instance.MenuManager.UpdateMainMenuLabels();
            Init(); // refresh UI
        }

        void CleanUpPreviousInit()
        {
            if (scrollArea.content.childCount > 0)
            {
                foreach (Transform t in scrollArea.content)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        void SyncShopWithSavedData()
        {
            if (activeShopItemType == ShopItemType.ShopItemType1)
            {
                foreach (int index in GameManager.Instance.activeProfile.unlockedShopItemsType_1)
                {
                    shopItems_1[index].isUnlocked = true;
                }
            }

            if (activeShopItemType == ShopItemType.ShopItemType2)
            {
                foreach (int index in GameManager.Instance.activeProfile.unlockedShopItemsType_2)
                {
                    shopItems_2[index].isUnlocked = true;
                }
            }
        }

        public void BuyItem(ShopItem item)
        {
            item.isUnlocked = true;

            if (item.shopItemType == ShopItemType.ShopItemType1)
            {
                GameManager.Instance.activeProfile.unlockedShopItemsType_1.Add(item.itemId);
            }

            if (item.shopItemType == ShopItemType.ShopItemType2)
            {
                GameManager.Instance.activeProfile.unlockedShopItemsType_2.Add(item.itemId);
            }

            GameManager.Instance.activeProfile.coins -= item.price;
            DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile;
            GameManager.Instance.MenuManager.UpdateMainMenuLabels();
            //Init(); // refresh UI
        }

    }

    // The exact definition of this class will have to be sorted according to each game
    // For example: one game might have Materials in shop, other could have GameObjects, it's impossible to predict
    // Example uses GameObject/prefab as unlockable
    [System.Serializable]
    public class ShopItem
    {
        public ShopItemType shopItemType;
        public int price;
        public int itemId;
        public Sprite itemPic; // ui representation
        public GameObject itemPrefab; // actual item
        public bool isUnlocked = false; // can be set to true for item #1 via inspector but is UNLOCKED BY DEFAULT (check GameManager InitGameData()) 
    }

    public enum ShopItemType { ShopItemType1, ShopItemType2 }
}


