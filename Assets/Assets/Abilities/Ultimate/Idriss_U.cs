using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Idriss", order = 1)]
public class Idriss_U : Ability
{
    public override void OnActivate(Entity actor)
    {
        Debug.Log("Inting...");
        actor.AddAndSyncEffect(new Effect(BehaviourEffects.Armor, 0.3f, 5f, false, true, "IDRISS_U"));
        actor.AddAndSyncEffect(new Effect(BehaviourEffects.PercentageAttackSpeed, 0.3f, 5f, false, true, "IDRISS_U"));
        actor.AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.3f, 5f, false, true, "IDRISS_U"));
        actor.Int(5f);
    }
}
