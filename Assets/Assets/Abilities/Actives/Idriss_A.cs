using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Idriss", order = 1)]
public class Idriss_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        actor.ResetAutoAttack();
        actor.SetAndSyncValue(FloatStat.Tryharding, 5f);
    }
}
