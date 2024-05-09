using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    private static CollectionManager instance;
    public static CollectionManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public GameObject ItemPanel;
    public List<GameObject> Items;
    public Image CooldownImage;

    private ItemShop _CurrentItem;
    public ItemShop CurrentItem => _CurrentItem;

    private bool _CanConsume;
    Entity me;
    bool _Checking = false;

    private void Start()
    {
        _CurrentItem = null;
        HideItem();
    }

    public void Init()
    {
        me = ClientManager.Instance.Me();
        _Checking = true;
    }

    public void HideItem()
    {
        ItemPanel.SetActive(false);
    }

    public void ShowItem(ItemShop item)
    {
        ItemPanel.SetActive(true);
        _CurrentItem = item;
        SetItem(item.IDItem);
        _CanConsume = true;
        CooldownImage.fillAmount = 0;
    }

    private void SetItem(int id)
    {
        foreach (GameObject item in Items) item.SetActive(false);
        Items[id].SetActive(true);
    }

    private void Update()
    {
        if (_Checking)
        {
            if (!me.IsAbleToControl) return;

            if (Input.GetKeyDown(GameSettings.ItemKey))
            {
                if (GameManager.Instance.InRound)
                {
                    if (_CurrentItem != null)
                    {
                        if (_CanConsume)
                        {
                            GameManager.Instance.UseItem(_CurrentItem.IDItem);
                            _CanConsume = false;
                            _CurrentItem = null;
                        }
                        else
                        {
                            AudioManager.PlaySound(Audio.Blocked);
                            UIManager.Instance.ShowGeneralInfo(GameManager.Instance.AlreadyUsingItemMessage);
                        }
                    }
                    else
                    {
                        AudioManager.PlaySound(Audio.Blocked);
                        UIManager.Instance.ShowGeneralInfo(GameManager.Instance.NoItemMessage);
                    }
                }
                else
                {
                    AudioManager.PlaySound(Audio.Blocked);
                    UIManager.Instance.ShowGeneralInfo(GameManager.Instance.CantUseItemOutRoundMessage);
                }
            }
        }

    }
}
