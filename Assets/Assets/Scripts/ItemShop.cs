using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int IDItem;
    public float PriceValue;
    public Image ItemImage;

    [Space(20)]
    [Header("Rune")]
    public float FatAmount;

    public bool IsDrinking;

    public string Price
    {
        get
        {
            return PriceValue.ToString("0.00", CultureInfo.InvariantCulture) + "€";
        }
    }

    [TextArea]
    public string ItemDescription;

    private bool IsTriggered;

    private float LastTimeClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetTrigger(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetTrigger(false);
    }

    void SetTrigger(bool isTriggered)
    {
        IsTriggered = isTriggered;
        ItemImage.color = isTriggered ? Color.white : new Color(1f, 1f, 1f, 0.55f);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetTrigger(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTriggered)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (DoubleClick())
                {
                    ShopManager.Instance.TryBuy(IDItem);
                }
                else
                {
                    ShopManager.Instance.OpenItemInfo(this);
                }

                LastTimeClick = Time.time;
            }
        }
    }

    private bool DoubleClick()
    {
        return Time.time - LastTimeClick < 0.4f;
    }
}
