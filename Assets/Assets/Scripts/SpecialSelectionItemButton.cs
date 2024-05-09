using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialSelectionItemButton : MonoBehaviour
{
    public TMP_Text Text;
    public SelectionContext Context;
    public Entity Target;

    public void Init(Entity target, SelectionContext context)
    {
        Target = target;
        Context = context;

        if (target) Text.text = target.ChampionName + $" <i><size=34><color=#c9c9c9>{target.PlayerName}</color></size></i>";
        else Text.text = "Personne";
    }

    public void ExecuteContext()
    {
        if (Target != null)
        {
            if (Context == SelectionContext.ChooseBrother)
            {
                GameManager.Instance.LinkEntitiesAsBrother(ClientManager.Instance.Me(), Target);
            }
            else
            {
                Target.SetAndSyncValue(BoolStat.CatchUp, true);
            }
        }

        SpecialSelectionManager.Instance.ClosePanel();

    }
}
