using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Passive/Dalil", order = 1)]
public class Dalil_P : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        
    }
}
