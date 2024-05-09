using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableEffect : SpellsEffect
{
    public List<ParticleSystem> pSystems;
    public ParticleSystem onDricking;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_StartEffect());
    }

    private void CloseAllEffects()
    {
        onDricking.gameObject.SetActive(false);
        foreach (ParticleSystem p in pSystems)
        {
            p.gameObject.SetActive(false);
        }
    }

    private void ShowEffect(int ID)
    {
        pSystems[ID].gameObject.SetActive(true);
        pSystems[ID].Clear();
        pSystems[ID].Play();
    }

    IEnumerator Coroutine_StartEffect()
    {
        CloseAllEffects();

        onDricking.gameObject.SetActive(true);
        onDricking.Clear();
        onDricking.Play();

        ItemShop item = Owner.LastItemUsed;
        int itemID = item.IDItem;

        Effect effect;

        switch (itemID)
        {
            case 0:
                if (Owner.IsClientEntity)
                {
                    effect = new Effect(BehaviourEffects.PercentageAttackSpeed,
                        0.8f,
                        20f,
                        false,
                        true);
                    Owner.AddAndSyncEffect(effect);
                }

                break;
            case 1:
                if (Owner.IsClientEntity)
                {
                    Owner.Heal(20f);
                    effect = new Effect(BehaviourEffects.Armor,
                        0.1f,
                        10,
                        false,
                        true);
                    Owner.AddAndSyncEffect(effect);
                }
                break;
            case 2:
                if (Owner.IsClientEntity)
                {
                    effect = new Effect(BehaviourEffects.PercentageAttackDamage,
                    0.2f,
                    20f,
                    false,
                    true);
                    Owner.AddAndSyncEffect(effect);
                }
                break;
            case 3:
                if (Owner.IsClientEntity)
                { 
                    float Duration = 3f;
                    for (int i = 0; i < 10; i++)
                    {
                        effect = new Effect(BehaviourEffects.PercentageSpeed,
                        0.1f,
                        Duration /( i + 1),
                        false,
                        true);
                        Owner.AddAndSyncEffect(effect);
                    }
                }
                break;
            case 4:
                ShowEffect(0);
                if (Owner.IsClientEntity)
                {
                    List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(Owner, 7.5f);
                    List<Entity> allies = ClientManager.Instance.NearestAllyEntitiesFrom(Owner, 7.5f);
                    float TotalDamage = 0;
                    foreach (Entity enemy in enemies)
                    {
                        enemy.TakeTrueDamage(enemy.HitPointsMax * 0.1f, Owner);
                        TotalDamage += Mathf.Min(enemy.HitPointsMax * 0.1f, enemy.HitPoints);
                    }
                    float TotalHeal = TotalDamage * 0.66f;
                    foreach (Entity ally in allies)
                    {
                        ally.Heal(TotalHeal);
                    }
                }
                break;
            case 5:
                ShowEffect(1);
                if (Owner.IsClientEntity)
                {
                    List<Entity> enemies2 = ClientManager.Instance.NearestEnemyEntitiesFrom(Owner, 7.5f);
                    foreach (Entity enemy in enemies2)
                    {
                        Effect effect2 = new Effect(BehaviourEffects.PercentageSpeed,
                            0.4f,
                            10f,
                            false,
                            false);
                        enemy.AddAndSyncEffect(effect2);
                    }
                }
                break;
            case 6:
                if (Owner.IsClientEntity) Owner.SetAndSyncValue(FloatStat.CritAttack, 5f);
                break;
            case 7:
                if (Owner.IsClientEntity)
                {
                    Owner.CleanseAllNegativeEffects();
                    Owner.Heal(Owner.MissingHP * 0.25f);
                }
                break;
        }

        yield return new WaitForSeconds(2f);
        onDricking.gameObject.SetActive(false);

        // Software rune give fat.
        if (Owner.IsClientEntity && Owner.Rune == 0)
        {
            Owner.SetAndSyncValue(FloatStat.Fat, Owner.FatValue + item.FatAmount);
        }

        OnCancel();
    }
}
