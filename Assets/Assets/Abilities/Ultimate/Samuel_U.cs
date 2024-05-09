using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Samuel", order = 1)]
public class Samuel_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return !actor.AlreadyOpenedClashRoyale;
    }

    public override void OnActivate(Entity actor)
    {
        actor.SetClashRoyale(true);
        GameManager.Instance.SetSamuelUlt(true);
    }
}
