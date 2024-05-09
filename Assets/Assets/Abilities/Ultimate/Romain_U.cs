using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Romain", order = 1)]
public class Romain_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return !actor.AlreadySendedLeMettayer;
    }

    public override void OnActivate(Entity actor)
    {
        actor.SendLeMettayer();

    }
}
