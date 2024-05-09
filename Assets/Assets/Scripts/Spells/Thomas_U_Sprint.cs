using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thomas_U_Sprint : SpellsEffect
{
    public float Duration = 0.5f;
    public float StunTime = 2f;
    public float Damage = 20f;

    public ParticleSystem pSystem;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_Sprint());
    }

    IEnumerator Coroutine_Sprint()
    {
        pSystem.Play();

        if (Owner.IsClientEntity)
        {
            Owner.SetChanneling(Duration);

            Vector3 target = new Vector3(Owner.Brother.Position, 0.1f, 0f);

            Owner.TranslateTo(target, Duration);

            yield return new WaitForSeconds(Duration);

            if (Owner.DistanceFromBrother < 2f)
            {
                Entity brother = Owner.Brother;
                brother.Heal(0.2f * brother.MissingHP, Owner, false);

                Effect effect = new Effect(BehaviourEffects.AttackDamage, 5f, 10f, false, true, "THOMAS_U");

                brother.AddAndSyncEffect(effect);
            }
        }
        else
        {
            yield return new WaitForSeconds(Duration);
        }


        OnCancel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Owner.IsClientEntity) return;

        if (other.CompareTag("Entity"))
        {
            Entity enemy = other.GetComponent<Entity>();

            if (ClientManager.Instance.IsEnemy(enemy))
            {
                enemy.SetAndSyncValue(FloatStat.StunTime, StunTime);
                enemy.Attack(Damage, Owner);
            }
        }
    }
}
