using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Titouan", order = 1)]
public class Titouan_A : Ability
{
    public override bool OnCondition(Entity actor)
    {
        List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(actor, 3f);
        return enemies.Count != 0;
    }

    public override void OnActivate(Entity actor)
    {
        actor.CallSpellEffect(9);
    }
}