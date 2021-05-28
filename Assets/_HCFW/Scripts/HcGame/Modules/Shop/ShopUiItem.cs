using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HCFW
{
    public class ShopUiItem : MonoBehaviour
    {
        [Header("References")]
        public Image itemImage;
        public Text itemPrice;

        public Color lockedColor = Color.red;
        public Color unlockedColor = Color.cyan;
        public Color activeColor = Color.green;

        Button b;

        public void Init(ShopItem item, int itemIndex)
        {
            //Debug.Log("itemIndex: " + itemIndex);
            b = GetComponent<Button>();
            b.interactable = true;
            /*
            RenderTextureCamera.Instance.SpawnToRender(item.itemPrefab);
            Sprite sprite = Sprite.Create(RenderTextureCamera.Instance.SnapshotImage(), new Rect(0.0f, 0.0f, 256f, 256f), new Vector2(0.5f, 0.5f), 100.0f);
            item.itemPic = sprite;
            */

            itemImage.sprite = item.itemPic;
            itemPrice.text = " " + item.price;

            if (item.isUnlocked)
            {
                itemPrice.text = "";
                GetComponent<Image>().color = unlockedColor;

                if (item.shopItemType == ShopItemType.ShopItemType1)
                {
                    if (itemIndex == GameManager.Instance.activeProfile.activeShopItemType_1_Index)
                    {
                        itemPrice.text = "";
                        GetComponent<Image>().color = activeColor;
                    }
                }
                else if (item.shopItemType == ShopItemType.ShopItemType2)
                {
                    if (itemIndex == GameManager.Instance.activeProfile.activeShopItemType_2_Index)
                    {
                        itemPrice.text = "";
                        GetComponent<Image>().color = activeColor;
                    }
                }

            }

            if (b != null && !item.isUnlocked)
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(delegate { BuyIfCanAfford(item); });
            }
            else
            {
                b.onClick.AddListener(delegate { OnSelectButtonTapped(item); });
            }

            this.gameObject.SetActive(true);
        }

        void BuyIfCanAfford(ShopItem item)
        {
            if (GameManager.Instance.CanBuy(item.price))
            {
                GameManager.Instance.ShopManager.BuyItem(item);
                OnSelectButtonTapped(item);
                HCFW.GameManager.Instance.ShopManager.Init();
            }
        }

        public void OnSelectButtonTapped(ShopItem item)
        {

            if (item.shopItemType == ShopItemType.ShopItemType1)
            {
                //*
                //Logic of actually applying the shop item goes here
                //*

                GameManager.Instance.activeProfile.activeShopItemType_1_Index = item.itemId;
                DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile;

                GameManager.Instance.tcv.EnableCarMesh(item.itemId);
            }

            if (item.shopItemType == ShopItemType.ShopItemType2)
            {
                //*
                //Logic of actually applying the shop item goes here
                //*

                GameManager.Instance.activeProfile.activeShopItemType_2_Index = item.itemId;
                DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile;
            }

            HCFW.GameManager.Instance.ShopManager.Init();
        }

    }
}


