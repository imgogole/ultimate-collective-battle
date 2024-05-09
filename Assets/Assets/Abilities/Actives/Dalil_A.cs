using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Dalil", order = 1)]
public class Dalil_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        Effect effect = new Effect(BehaviourEffects.Armor,
            0.8f,
            3f,
            false,
            true);
        actor.AddAndSyncEffect(effect);
        actor.SetAndSyncValue(FloatStat.DodgingTime, 3f);
        actor.CallSpellEffect(1);
    }
}
