using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Passive/Dummy", order = 1)]
public class Dummy_P : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return actor.RatioHP < 0.5f;
    }

    public override void OnActivate(Entity actor)
    {   
        actor.Heal( Mathf.FloorToInt((actor.RatioMissingHP) * 0.5f * actor.BaseChampion.HP));
        Debug.Log($"{actor.ChampionName} a récupéré 50% de ses PV manquants !");
    }
}
