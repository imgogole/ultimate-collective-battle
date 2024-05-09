 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Romain_U_Joke : SpellsEffect
{
    public GameObject Progress;
    public Vector3 GoalSize;
    public float Duration;

    public float MoveSpeedBonus;
    public float Damage;
    public float StunTime;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_Joke());
    }

    public override void OnCancel()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void Update()
    {
        Vector3 v = transform.position;
        v.y = 00.1f;
        transform.position = v;
    }

    IEnumerator Coroutine_Joke()
    {
        if (Owner.IsClientEntity) Owner.SetChanneling(Duration);

        Progress.transform.localScale = Vector3.zero;

        float time = 0f;

        while (time < Duration)
        {
            time += Time.deltaTime;
            Progress.transform.localScale = (time / Duration) * GoalSize;
            yield return null;
        }

        // Apply effects

        if (Owner.IsClientEntity)
        {
            List<Entity> Allies = ClientManager.Instance.NearestAllyEntitiesFrom(Owner, GoalSize.x / 2f);
            List<Entity> Enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(Owner, GoalSize.x / 2f);

            foreach (Entity ally in Allies)
            {
                int ID = ally.BaseChampion.ID;
                float buff = (ID == 9) ? 2f : 1f;

                Effect allyEffect = new Effect(BehaviourEffects.PercentageSpeed, MoveSpeedBonus * buff, 5f, false, true);

                ally.AddAndSyncEffect(allyEffect);
            }

            foreach (Entity enemy in Enemies)
            {
                int ID = enemy.BaseChampion.ID;
                float nerf = (ID == 9) ? 2f : ((ID == 13) ? 3f : 1f);

                enemy.SetAndSyncValue(FloatStat.StunTime, StunTime * nerf);
                enemy.Attack(Damage * nerf, Owner);
            }
        }

        yield return null;
        gameObject.SetActive(false);
    }
}
