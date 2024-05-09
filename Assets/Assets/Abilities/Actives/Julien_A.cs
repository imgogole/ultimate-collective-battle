using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Julien", order = 1)]
public class Julien_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        actor.SetMajorationTime(5f);
    }
}
