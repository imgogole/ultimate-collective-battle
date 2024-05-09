using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Samuel_U_Troop : MonoBehaviour
{
    public PhotonView View;

    public List<GameObject> troopRenderers;
    public int currentTroop;

    private float MaximumSide;

    public float Speed;
    public int Hall;

    [Header("Effects")]
    public float pekkaDamageMaxHP;

    bool moving = false;
    Entity Owner;

    List<Entity> touchedEntity = new List<Entity>();

    public void Init(int hall, int idOwner, int troop)
    {
        View.RPC("RPC_Init", RpcTarget.All, hall, idOwner, troop);
    }

    private void Update()
    {
        if (moving && View.IsMine)
        {
            Vector3 v = Speed * Time.deltaTime * Vector3.right;

            transform.position += v;

            if (transform.position.x > MaximumSide)
            {
                DestroyTroop();
            }
        }
    }

    public void HideAllTroopRenderers()
    {
        foreach (GameObject troop in troopRenderers)
        {
            troop.SetActive(false);
        }
    }

    public void ShowTroopRenderer(int index)
    {
        HideAllTroopRenderers();
        troopRenderers[index].SetActive(true);
    }

    public void DestroyTroop()
    {
        if (View.IsMine)
        {
            moving = false;
            PhotonNetwork.Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    private void SetEffect(Entity target)
    {
        touchedEntity.Add(target);

        if (currentTroop == 0)
        {
            GameObject fireball = ClientManager.Instance.CreateObject("Fireball", target.transform.position);
            Samuel_U_Fireball fireball_component = fireball.GetComponent<Samuel_U_Fireball>();
            fireball_component.Init(Owner.BaseChampion.ID);
        }
        else if (currentTroop == 1)
        {
            target.Attack(target.HitPointsMax * pekkaDamageMaxHP, Owner);
        }
        else if (currentTroop == 2)
        {
            target.Attack(0f, Owner);
            target.SetAndSyncValue(FloatStat.StunTime, 2f);
            DestroyTroop();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject target = collision.gameObject;

        if (target.CompareTag("Entity"))
        {
            Entity entity = target.GetComponent<Entity>();

            bool IsAlly = ClientManager.Instance.AreInTheSameTeam(entity, Owner);
            bool AlreadyTouched = touchedEntity.Contains(entity);
            if (IsAlly) return;
            if (AlreadyTouched) return;
            if (entity.IsDead) return;

            if (Owner.IsClientEntity)
            {
                SetEffect(entity);
            }
        }
    }

    ////////////////////////////////////// RPCs //////////////////////////////////////////


    [PunRPC]
    private void RPC_Init(int hall, int idOwner, int troop)
    {
        currentTroop = troop;
        ShowTroopRenderer(currentTroop);

        Owner = ClientManager.Instance.GetEntityFromChampionID(idOwner);

        Hall = hall;
        moving = true;
        Vector2 sides = GameManager.Instance.Halls[hall];

        MaximumSide = sides.y;
    }
}
