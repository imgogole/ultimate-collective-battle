using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Dalil", order = 1)]
public class Dalil_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        return true;
    }

    public override void OnActivate(Entity actor)
    {
        List<Entity> Enemies = ClientManager.Instance.EnemyEntities;
        foreach (Entity entity in Enemies)
        {
            entity.SetAndSyncValue(FloatStat.ControlledTime, 5f);
            entity.SetAndSyncValue(IntStat.Controller, actor.BaseChampion.ID);
        }
        actor.SetChanneling(5f);
        actor.PlayerMovementParent.DoDalilHack();
    }
}
