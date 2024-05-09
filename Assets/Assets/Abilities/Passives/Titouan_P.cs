using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Passive/Titouan", order = 1)]
public class Titouan_P : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {

    }
}
