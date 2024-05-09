using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Lou", order = 1)]
public class Lou_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        actor.ResetAutoAttack();
        actor.SetAndSyncValue(FloatStat.AttackPredatorTime, 10f);
        actor.SetTimeSpellLou();
    }
}
