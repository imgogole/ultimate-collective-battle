using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Momow", order = 1)]
public class Momow_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        actor.CleanseAllNegativeEffects();
        actor.SetAndSyncValue(FloatStat.StunTime, 0f);
        actor.SetAndSyncValue(FloatStat.ImmuneToCC, 3f);

        GameManager.Instance.PlaySoundThroughServer(Audio.HewewSimple);
    }
}