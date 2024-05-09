using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "Passive/Thomas", order = 1)]

public class Thomas_P : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return !actor.IsStun && !actor.IsChanneling && !actor.InLobby && !actor.InEnemyLobby;
    }

    public override void OnActivate(Entity actor)
    {
        actor.SetLagPoint();
    }
}
