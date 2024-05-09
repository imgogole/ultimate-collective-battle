using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Virgil", order = 1)]
public class Virgil_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        actor.ResetAutoAttack();
        actor.SetNastinessAttack(5f);
    }
}