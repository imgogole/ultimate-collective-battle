using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Passive/Lou", order = 1)]
public class Lou_P : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
    }
}