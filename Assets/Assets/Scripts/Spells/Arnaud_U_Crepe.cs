using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class Arnaud_U_Crepe : MonoBehaviourPun
{
    public float DistanceMax;
    public float Speed;
    public float PushBackSpeed;
    public float PushBackPlayerSpeed;
    public float Damage;
    public float StunTime;

    public Outline outline;

    public Collider col;

    PhotonView _View;
    public Entity Owner
    {
        get
        {
            return _Owner;
        }
    }

    Rigidbody _rb;
    Entity _Owner;

    bool _IsGrounded;

    private void Awake()
    {
        _View = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        _IsGrounded = false;
    }

    public void Init(Entity owner)
    {
        _View.RPC("RPC_Init", RpcTarget.All, owner.BaseChampion.ID);
    }

    private void OnInit(Entity owner)
    {
        _Owner = owner;

        outline.OutlineColor = Owner.EntityColor;

        GameManager.Instance.AddCrepe(this);

        List<Entity> allies = ClientManager.Instance.CurrentEntities;
        foreach (Entity ally in allies)
        {
            if (ClientManager.Instance.AreInTheSameTeam(ally, Owner))
            Physics.IgnoreCollision(col, ally.EntityCollider);
        }


        if (_View.IsMine)
        {
            List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(Owner, DistanceMax);
            Direction dir;
            if (enemies.Count != 0)
            {
                Entity Target = enemies.First();
                dir = Owner.DirectionFrom(Target);
            }
            else
            {
                dir = Owner.CurrentDirection;
            }
            int _dir = dir == Direction.Left ? -1 : 1;

            Vector3 force = new Vector3(_dir * Speed, 0, 0);

            _rb.AddForce(force, ForceMode.VelocityChange);
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            //_rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Somethings touched the crepe.");

        GameObject target = collision.gameObject;

        if (target.CompareTag("Floor") || target.CompareTag("Crepe"))
        {
            _IsGrounded = true;
        }

        if (target.CompareTag("Entity"))
        {
            Debug.Log("An entity touched the crepe.");
            Entity entity = target.GetComponent<Entity>();
            if (ClientManager.Instance.AreInTheSameTeam(entity, Owner)) return;
            Debug.Log("The crepe owner is an enemy.");
            if (_IsGrounded)
            {
                Debug.Log("An entity touched the crepe while it is on the floor.");
                // The crepe only stun for 0.5 secondes and push back
                if (entity.IsClientEntity)
                {
                    Debug.Log("The entity is the client");
                    entity.SetAndSyncValue(FloatStat.StunTime, StunTime);
                    entity.PushBack(PushBackPlayerSpeed, entity.CurrentDirection);

                    _View.RPC("RPC_DestroyCrepe", RpcTarget.All);
                }
                if (_View.IsMine)
                {
                    entity.Attack(0f, Owner);

                    DestroyCrepe();
                }
            }
            else
            {
                if (_View.IsMine)
                {
                    entity.Attack(Damage, Owner);

                    DestroyCrepe();
                }
            }


        }
    }

    public void DestroyCrepe()
    {
        if (_View.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    public void PushBack()
    {
        float _Value = (transform.position.x - Owner.transform.position.x);
        float _dir = _Value / Mathf.Abs(_Value);

        Vector3 force = new Vector3(_dir * PushBackSpeed, 0, 0);

        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    [PunRPC]
    private void RPC_DestroyCrepe()
    {
        DestroyCrepe();
    }

    [PunRPC]
    private void RPC_Init(int id)
    {
        OnInit(ClientManager.Instance.GetEntityFromChampionID(id));
    }
}
