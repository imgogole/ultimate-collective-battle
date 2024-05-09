using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Dummy", order = 1)]
public class Dummy_A : Ability
{
    public override void OnActivate(Entity actor)
    {
        Debug.Log("Compétence active activée !");
    }
}
