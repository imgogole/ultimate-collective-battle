using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Titouan", order = 1)]
public class Titouan_U : Ability
{
    public override bool OnCondition(Entity actor)
    {
        bool result;
        if (actor.TitouanTarget != null)
        {
            if (actor.IsUltimateOfTargetValaible)
            {
                result = actor.TitouanTarget.BaseChampion.UltimateAbility.OnCondition(actor);
            }
            else
            {
                result = true;
            }
        }
        else
        {
            result = false;
        } 
        return result;
    }

    public override void OnActivate(Entity actor)
    {
        if (actor.IsUltimateOfTargetValaible) // Titouan already stolen the ultimate, apply it.
        {
            AudioManager.PlaySound(Audio.LaughUltimeStole, actor.CenterPoint, 5f);

            actor.TitouanTarget.BaseChampion.UltimateAbility.OnActivate(actor);
            actor.SetTitouanTarget(null, false);
        }
        else
        {
            actor.SetTitouanTarget(actor.TitouanTarget, true);
            actor.SetUltimateTargetValidity(20f);
            actor.TitouanTarget.PreventUltimeStolled();
            actor.ReduceCooldown(SpellType.Ultimate, 0.99f);
        }
    }
}