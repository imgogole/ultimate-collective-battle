using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Arnaud", order = 1)]
public class Arnaud_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return actor.CanUseSalto;
    }

    public override void OnActivate(Entity actor)
    {
        Debug.Log($"{actor.ChampionName} a fait un salto et esquive les attaques !");
        actor.DoSalto();
    }
}
