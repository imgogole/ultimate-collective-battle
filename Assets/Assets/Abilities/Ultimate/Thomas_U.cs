using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Thomas", order = 1)]
public class Thomas_U : Ability
{
    private const float DISTANCE_FROM_BROTHER_TO_DASH = 20f;

    public override bool OnCondition(Entity actor)
    {
        return actor.Brother && !actor.Brother.IsDead && actor.Brother.Hall == actor.Hall && actor.DistanceFromBrother < DISTANCE_FROM_BROTHER_TO_DASH;
    }

    public override void OnActivate(Entity actor)
    {
        actor.SetLagPoint();
        actor.CallSpellEffect(7);
    }
}
