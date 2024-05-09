using UnityEngine;

public class ShopArea : MonoBehaviour
{
    public string Message;
    public Outline outline;

    public int TeamOwner;

    public bool AccessPermitted
    {
        get
        {
            return TeamOwner == PlayerManager.Me.Team;
        }
    }

    private void Start()
    {
        TriggerShop(false);
    }

    private void Update()
    {
        if (AccessPermitted)
        {
            TriggerCast();
        }

    }

    public void OnMouseEnter()
    {
        OnCursorEnterShop();
    }

    public void OnMouseExit()
    {
        OnCursorExitShop();
    }

    public void OnCursorEnterShop()
    {
        if (AccessPermitted)
        {
            TriggerShop(!ShopManager.IsShopOpen);
        }
    }

    public void OnCursorExitShop()
    {
        if (AccessPermitted)
        {
            TriggerShop(false);
        }
    }

    private void TriggerCast()
    {
        if (!ClientManager.Instance.Me().IsAbleToControl) return;

        if (ShopManager.IsShopTriggered && Input.GetMouseButtonDown(0))
        {
            ShopManager.Instance.OpenShop();
            TriggerShop(false);
        }
    }

    private void TriggerShop(bool IsTriggered)
    {
        if (IsTriggered) UIManager.Instance.ShowStatInfo(Message);
        else UIManager.Instance.CloseStatInfo();
        outline.enabled = IsTriggered;
        ShopManager.Instance.SetShopTriggered(IsTriggered);
    }
}
