using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Passive/Virgil", order = 1)]
public class Virgil_P : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {

    }
}
