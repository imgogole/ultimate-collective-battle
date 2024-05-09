using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class Romain_U_LeMettayerBot : MonoBehaviour
{
    public PhotonView View;
    public Entity Owner;
    public float MinimumSide;
    public float MaximumSide;
    public float Range;
    public float Speed;

    public ParticleSystem Fireworks;

    bool _Active = false;
    int _Hall;
    float Objective;

    public float PosX
    {
        get
        {
            return transform.position.x;
        }
    }

    public float Direction
    {
        get
        {
            return Objective == MaximumSide ? 1 : -1;
        }
    }

    private void Start()
    {
        Fireworks.gameObject.SetActive(false);
        if (View.IsMine)
        {
            Entity me = ClientManager.Instance.Me();
            Init(me);
        }
    }

    public void Init(Entity me)
    {
        View.RPC("RPC_Init", RpcTarget.All, me.BaseChampion.ID, me.Hall, (int)me.CurrentDirection);
    }

    private void Update()
    {
        if (_Active)
        {
            if (PosX > MaximumSide) Objective = MinimumSide;
            if (PosX < MinimumSide) Objective = MaximumSide;

            if (Owner.IsClientEntity)
            {
                Vector3 v = Direction * Speed * Time.deltaTime * Vector3.right;

                transform.position += v;

                List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(transform.position, Range);
                Entity enemy = enemies.Find(e => e.IsCatchUp);
                if (enemy != null)
                {
                    enemy.Attack(0f, Owner);

                    _Active = false;

                    View.RPC("RPC_Destroy", RpcTarget.All, enemy.BaseChampion.ID);

                    // Give buff to Romain and their allies

                    List<Entity> allies = ClientManager.Instance.AllyEntities;

                    foreach (Entity ally in allies)
                    {
                        if (ally == Owner)
                        {
                            PlayerManager.Me.AddMoney(0.5f);
                            Effect romainADbonus = new Effect(BehaviourEffects.AttackDamage,
                                5f,
                                30f,
                                false,
                                true);
                            Effect romainMovebonus = new Effect(BehaviourEffects.PercentageSpeed,
                                0.2f,
                                30f,
                                false,
                                true);
                            Effect romainRangebonus = new Effect(BehaviourEffects.PercentageRange,
                                0.25f,
                                30f,
                                false,
                                true);
                            ally.AddAndSyncEffect(romainADbonus);
                            ally.AddAndSyncEffect(romainMovebonus);
                            ally.AddAndSyncEffect(romainRangebonus);
                            // Romain get better bufff
                        }
                        else
                        {
                            Effect otherADbonus = new Effect(BehaviourEffects.AttackDamage,
                                2f,
                                15f,
                                false,
                                true);
                            Effect otherMovebonus = new Effect(BehaviourEffects.PercentageSpeed,
                                0.1f,
                                15f,
                                false,
                                true);

                            ally.AddAndSyncEffect(otherADbonus);
                            ally.AddAndSyncEffect(otherMovebonus);
                        }
                    }
                }
            }

        }
    }

    private void OnDestroy()
    {
        if (View != null && View.IsMine)
        {
            PhotonNetwork.LocalCleanPhotonView(View);
            //PhotonNetwork.RemoveRPCs(View);
        }
    }

    public void DestroyLeMettayer()
    {
        if (Owner.IsClientEntity)
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }

    IEnumerator Coroutine_Destroy(int ID)
    {
        Fireworks.gameObject.SetActive(true);
        Fireworks.Play();

        Entity me = ClientManager.Instance.Me();

        if (me.BaseChampion.ID == ID)
        {
            me.SendToClosedRoom();
        }

        AudioManager.PlaySound(Audio.DestroyedLeMettayer, transform.position, 10f);

        yield return new WaitForSeconds(2f);
        DestroyLeMettayer();
    }

    /// RPC


    [PunRPC]
    private void RPC_Init(int author, int Hall, int direction)
    {
        Owner = ClientManager.Instance.GetEntityFromChampionID(author);
        _Active = true;
        _Hall = Hall;

        Vector2 sides = GameManager.Instance.Halls[_Hall];

        MinimumSide = sides.x;
        MaximumSide = sides.y;

        Objective = direction == 1 ? MinimumSide : MaximumSide;
    }

    [PunRPC]
    private void RPC_Destroy(int ID)
    {
        StartCoroutine(Coroutine_Destroy(ID));
    }


}
