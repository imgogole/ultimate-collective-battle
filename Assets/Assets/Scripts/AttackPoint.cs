using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    public Entity BaseEntity;

    /// <summary>
    /// Search every enemy entities and hit them. Return true if at least one enemy was hitted.
    /// </summary>
    /// <returns></returns>
    public bool HitAttack()
    {
        List<Entity> currentCollisions = GetEveryEntities();
        currentCollisions.RemoveAll(e => e.IsDead || e.BaseChampion.ID == BaseEntity.BaseChampion.ID || ClientManager.Instance.IsAlly(e));
        foreach (Entity entity in currentCollisions)
        {
            entity.Attack(BaseEntity);
        }

        if (currentCollisions.Count != 0)
        {
            BaseEntity.PlayerMovementParent.BaseEntityAnimation.HitArm(currentCollisions[0]);
            BaseEntity.SetConsecutiveHit(currentCollisions);
        }

        return currentCollisions.Count != 0;
    }

    private List<Entity> GetEveryEntities()
    {
        List<Collider> Results = new List<Collider>(Physics.OverlapSphere(transform.position, BaseEntity.TotalRange));
        return Results.FindAll(c => IsEntity(c)).Select(c => c.GetComponent<Entity>()).ToList();
    }

    private bool IsEntity(Collider other)
    {
        return other.gameObject.CompareTag("Entity");
    }
}
