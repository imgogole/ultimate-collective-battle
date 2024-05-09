using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samuel_A_Screams : SpellsEffect
{
    public ParticleSystem pSystem;
    public float ScreamDuration;
    public float ScreamRange;
    public float ScreamDamageMaxHP;
    public float ElixirGainPercentage;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_SamuelScream());
    }

    public override void OnCancel()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    IEnumerator Coroutine_SamuelScream()
    {
        pSystem.Stop();
        pSystem.Play();

        float elixirGain = 0f;

        if (Owner.IsClientEntity)
        {
            //Owner.SetChanneling(ScreamDuration);

            List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(Owner, ScreamRange);

            foreach (Entity enemy in enemies)
            {
                float Damage = enemy.HitPointsMax * ScreamDamageMaxHP;
                elixirGain += enemy.Attack(Damage, Owner);

                Debug.Log($"Samuel hits an enemy : {enemy.ChampionName} with {Damage}HP damaged (Current elixir gain : {elixirGain})");
            }

            Debug.Log($"Total elixir gain : {elixirGain * ElixirGainPercentage}");
            Owner.AddElixir(elixirGain * ElixirGainPercentage);
        }

        AudioManager.PlaySound(Audio.SamuelScream);

        yield return new WaitForSeconds(ScreamDuration);

        OnCancel();
    }
}
