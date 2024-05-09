using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Dummy", order = 1)]
public class Dummy_U : Ability
{
    public override void OnActivate(Entity actor)
    {
        Debug.Log($"{actor.ChampionName} a activé sa capacité ultime !");
    }
}
