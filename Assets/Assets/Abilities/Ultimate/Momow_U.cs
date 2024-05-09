using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Momow", order = 1)]
public class Momow_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        GameManager.Instance.PlaySoundThroughServer(Audio.Hewew);
        actor.CallSpellEffect(2);
        actor.SetChanneling(1.3f);
    }
}
