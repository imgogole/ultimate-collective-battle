using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialSelectionManager : MonoBehaviour
{
    public GameObject SpecialSelectionPanel;
    public TMP_Text TextContext;

    public Transform BackgroundFiller;
    public GameObject ChampionButtonPrefab;

    private static SpecialSelectionManager instance;
    public static SpecialSelectionManager Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    public void CreateButton(Entity target, SelectionContext context)
    {
        GameObject button = Instantiate(ChampionButtonPrefab, BackgroundFiller);
        SpecialSelectionItemButton ssitem = button.GetComponent<SpecialSelectionItemButton>();

        if (ssitem)
        {
            ssitem.Init(target, context);
        }
    }

    public void OpenPanel(SelectionContext context, bool isTitouan)
    {
        SpecialSelectionPanel.SetActive(true);

        if (isTitouan)
        {
            TextContext.text = context switch
            {
                SelectionContext.ChooseBrother => GameManager.Instance.ChooseBrotherTitouanContextMessage,
                SelectionContext.ChooseCatchUp => GameManager.Instance.ChooseCatchUpTitouanContextMessage,
                _ => ""
            };
        }
        else
        {
            TextContext.text = context switch
            {
                SelectionContext.ChooseBrother => GameManager.Instance.ChooseBrotherContextMessage,
                SelectionContext.ChooseCatchUp => GameManager.Instance.ChooseCatchUpContextMessage,
                _ => ""
            };
        }

        Clear();
        CreateButton(null, context);

        List<Entity> Targets = new List<Entity>();

        if (context == SelectionContext.ChooseBrother)
        {
            Targets = ClientManager.Instance.AllyEntities;
            Targets.Remove(ClientManager.Instance.Me());
        }
        else if (context == SelectionContext.ChooseCatchUp)
        {
            Targets = ClientManager.Instance.EnemyEntities;
        }
        print($"Targets : {Targets.Count}, context : {context}, Enemies : {ClientManager.Instance.EnemyEntities.Count}");

        foreach (Entity target in Targets)
        {
            CreateButton(target, context);
        }
    }

    public void ClosePanel()
    {
        SpecialSelectionPanel.SetActive(false);
    }

    private void Clear()
    {
        foreach (Transform child in BackgroundFiller) Destroy(child.gameObject);
    }
}

public enum SelectionContext
{
    ChooseBrother,
    ChooseCatchUp
}