using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Julien", order = 1)]
public class Julien_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return actor.CanUseResultMajoration;
    }

    public override void OnActivate(Entity actor)
    {
        if (actor.IsTitouan)
        {
            actor.Heal(1.75f * actor.TotalAttackDamage);
        }
        else
        {
            actor.HealFromMajoration();
        }
    }
}
