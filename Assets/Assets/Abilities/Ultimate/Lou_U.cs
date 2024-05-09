using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Lou", order = 1)]
public class Lou_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        Effect effect = new Effect(BehaviourEffects.PercentageSpeed, 0.6f, 15f, false, true, "LOU_U");
        actor.AddAndSyncEffect(effect);
        actor.SetAndSyncValue(FloatStat.InvisibilityTime, 15f);
        GameManager.Instance.StartLouUltimate();
        actor.SetTimeSpellLou();
    }
}
