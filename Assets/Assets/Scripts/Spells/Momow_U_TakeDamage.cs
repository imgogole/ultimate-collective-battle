using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momow_U_TakeDamage : MonoBehaviour
{
    public Entity BaseEntity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entity"))
        {
            Entity entity = other.GetComponent<Entity>();
            if (entity && BaseEntity.IsClientEntity && ClientManager.Instance.IsEnemy(entity))
            {
                entity.Attack(BaseEntity.FinalDeltaRayDamage, BaseEntity);
            }
        }
    }
}
