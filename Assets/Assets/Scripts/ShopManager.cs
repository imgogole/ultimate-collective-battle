using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject UIShop, UIInfoItem;
    public List<GameObject> ImagesItemInfo;
    public List<GameObject> ImagesItemAnimation;
    public List<ItemShop> ItemShops;
    public TMP_Text ItemDescription, TextButton;

    bool _ShopOpen;
    bool _ShopTriggered;

    private ItemShop _SelectedItem;

    const float StartY = 0f;

    public static bool IsShopOpen => instance._ShopOpen;
    public static bool IsShopTriggered => !instance._ShopOpen && instance._ShopTriggered;

    private static ShopManager instance;
    public static ShopManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        CloseShop();
        instance = this;
    }

    // Depreacated
 /*   public void Init()
    {
        if (PlayerManager.Instance.Team == TeamOwner) instance = this;
        else Destroy(this);
    }*/

    public void OpenShop()
    {
        _SelectedItem = null;
        UIInfoItem.SetActive(false);
        _ShopOpen = true;
        GameManager.Instance.SetEntityFreeze(true);
        UIShop.SetActive(true);
        AudioManager.PlaySound(Audio.ClickButton);
    }

    public void SetShopTriggered(bool IsTriggered)
    {
        _ShopTriggered = IsTriggered;
    }

    public void CloseShop()
    {
        _ShopOpen = false;
        GameManager.Instance.SetEntityFreeze(false);
        UIShop.SetActive(false);
        UIInfoItem.SetActive(false);
    }

    public void TryBuyCurrentItem()
    {
        TryBuy(_SelectedItem.IDItem);
    }

    public void TryBuy(int IDItem)
    {
        float Price = ItemShops[IDItem].PriceValue;
        if (PlayerManager.Me.Money >= Price)
        {
            Buy(IDItem);
            PlayerManager.Me.UseMoney(Price);
            CollectionManager.Instance.ShowItem(ItemShops[IDItem]);
        }
    }

    public void Buy(int IDItem)
    {
        AudioManager.PlaySound(Audio.ClickButton);
        AudioManager.PlaySound(Audio.ObjectBought);
        AnimateBuy(IDItem);
        Debug.Log($"Item {IDItem} acheté.");
    }

    public void OpenItemInfo(ItemShop Item)
    {
        ShowImageItemInfo(Item.IDItem);
        SetDescription(Item.ItemDescription);
        UIInfoItem.SetActive(true);

        TextButton.text = $"Acheter {Item.Price}";
        _SelectedItem = Item;
    }

    public void SetDescription(string description)
    {
        ItemDescription.text = description;
    }

    public void ShowImageItemInfo(int ID)
    {
        foreach (GameObject obj in ImagesItemInfo) obj.SetActive(false);
        ImagesItemInfo[ID].SetActive(true);
    }

    public void AnimateBuy(int index)
    {
        /*StopAllCoroutines();
        GetBack();*/
        StartCoroutine(Coroutine_AnimateBuy(index));
    }

    private void GetBack()
    {
        foreach (GameObject img in ImagesItemAnimation)
        {
            img.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            img.SetActive(false);
            Vector2 ImgPos = img.GetComponent<RectTransform>().anchoredPosition;
            ImgPos.y = StartY;
            img.GetComponent<RectTransform>().anchoredPosition = ImgPos;
        }
    }

    IEnumerator Coroutine_AnimateBuy(int index)
    {

        GameObject img = ImagesItemAnimation[index];
        img.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        img.SetActive(true);
        Vector2 ImgPos = img.GetComponent<RectTransform>().anchoredPosition;
        ImgPos.y = StartY;
        img.GetComponent<RectTransform>().anchoredPosition = ImgPos;

        float t = 0f;

        while (img.GetComponent<RectTransform>().position.y > -1000f)
        {
            t += Time.deltaTime;

            Vector2 newPos = ImgPos;
            newPos.y = StartY - (1500f * t * t);

            img.GetComponent<RectTransform>().anchoredPosition = newPos;
            img.GetComponent<Image>().color = new Color(1f, 1f, 1f, t * 100f);

            yield return new WaitForEndOfFrame();
        }

        img.SetActive(false);
        ImgPos = img.GetComponent<RectTransform>().anchoredPosition;
        ImgPos.y = StartY;
        img.GetComponent<RectTransform>().anchoredPosition = ImgPos;
    }
}
