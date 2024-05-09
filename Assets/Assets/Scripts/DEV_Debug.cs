
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DEV_Debug : MonoBehaviour
{
    public TMP_Text debugText;
    bool isDebugShown;
    Entity client;

    private void Start()
    {
        SetDebug(false);
    }

    public void Init(Entity me)
    {
        client = me;
    }

    public void SetDebug(bool isActive)
    {
        isDebugShown = isActive;
        debugText.gameObject.SetActive(isActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetDebug(!isDebugShown);
        }
        if (isDebugShown)
        {
            DebugNow();
        }
    }

    void DebugNow()
    {
        string debug = "Debug :\n";

        List<Entity> entities = ClientManager.Instance.CurrentEntities;
        foreach (Entity entity in entities)
        {
            debug += entity.ChampionName;
            debug += $"\nHas Key 0 : {entity.HadFirstTeamKey}";
            debug += $"\nHas Key 1 : {entity.HadSecondTeamKey}\n";
        }
        debug += $"\nMultiple attack count : {client.TimeHittedLastEntity}";
        if (client.LastEntityHitted != null)
        {
            debug += $"\nMultiple attack entity : {client.LastEntityHitted.ChampionName}";
        }
        if (client.TitouanTarget != null)
        {
            debug += $"\nTitouan target : {client.TitouanTarget.ChampionName}";
            debug += $"\nTitouan target time left : {client.TitouanTarget.TargetTime}";
        }
        debugText.text = debug;
    }
}
