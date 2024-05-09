using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Active/Thomas", order = 1)]
public class Thomas_A : Ability
{
    public const float DistanceFromBrotherToHeal = 15f;

    public override bool OnCondition(Entity actor)
    {
        return actor.HasLagPoint;
    }

    public override void OnActivate(Entity actor)
    {
        actor.ResetAutoAttack();

        actor.Teleport(actor.LagPointPosition);

        float heal = Mathf.Max(0f, actor.LagPointHP - actor.HitPoints);
        float brotherHeal = heal * 0.6f;

        actor.Heal(heal);
        if (actor.Brother && actor.DistanceFromBrother < DistanceFromBrotherToHeal) actor.Brother.Heal(brotherHeal, actor, false);

        actor.SetHall(actor.LagPointHall);

        AudioManager.PlaySound(Audio.Wifi);
    }
}
