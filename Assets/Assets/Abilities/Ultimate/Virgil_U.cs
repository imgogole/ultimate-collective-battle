using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Virgil", order = 1)]
public class Virgil_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        actor.CallSpellEffect(8);
    }
}