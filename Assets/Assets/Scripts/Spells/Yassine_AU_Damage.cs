using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yassine_AU_Damage : MonoBehaviour
{
    public Entity BaseEntity;

    List<Entity> TotalAllies = new List<Entity>();

    private float Heal = 0f;

    private void Start()
    {
        ResetHeal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!BaseEntity.IsClientEntity) return;

        if (other.CompareTag("Entity"))
        {

            Entity entity = other.GetComponent<Entity>();
            if (entity)
            {
                if (ClientManager.Instance.IsEnemy(entity))
                {
                    float Damage;
                    if (!Yassine_AU_Rays.Instance.IsActive && !entity.IsSuccess)
                    {
                        Damage = Mathf.Min(Yassine_AU_Rays.Instance.CurrentDamage, entity.HitPoints - 1f);
                    }
                    else
                    {
                        if (Yassine_AU_Rays.Instance.IsActive)
                        {
                            entity.SetAndSyncValue(FloatStat.Success, 10f);
                        }
                        Damage = Yassine_AU_Rays.Instance.CurrentDamage;
                    }
                    Heal += entity.Attack(Damage, BaseEntity);
                }
                else
                {
                    TotalAllies.Add(entity);
                }
            }
        }
    }

    public void ResetHeal()
    {
        Heal = 0f;
        TotalAllies = new List<Entity>();
    }

    public void ApplyHeal()
    {
        foreach(Entity ally in TotalAllies)
        {
            ally.Heal(Heal * Yassine_AU_Rays.Instance.PercentageHeal, BaseEntity, false);
        }
    }
}
