using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Passive/Yassine", order = 1)]
public class Yassine_P : Ability
{
    List<Entity> NearestAllies;

    public override bool OnCondition(Entity actor)
    {
        List<Entity> Allies = ClientManager.Instance.NearestAllyEntitiesFrom(actor, 10f);
        Allies.Remove(actor);
        Allies.RemoveAll(e => e.RatioHP == 1f);
        NearestAllies = Allies;
        return NearestAllies.Count != 0;
    }

    public override void OnActivate(Entity actor)
    {
        foreach (Entity entity in NearestAllies)
        {
            entity.Heal(1f, actor, false);
        }
    }
}
