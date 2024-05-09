using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samuel_U_Fireball : MonoBehaviourPun
{
    public ParticleSystem fireBallParticle;
    public float Distance;
    public float Damage;

    public void Init(int ID)
    {
        fireBallParticle.Stop();
        photonView.RPC("RPC_Init", RpcTarget.All, ID);
    }

    IEnumerator Coroutine_Init(Entity owner)
    {

  
        fireBallParticle.Play();

        if (owner.IsClientEntity)
        {
            List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(transform.position, Distance);
            foreach (Entity enemy in enemies)
            {
                enemy.Attack(Damage, owner);
            }
        }

        yield return new WaitForSeconds(fireBallParticle.main.duration);

        if (owner.IsClientEntity)
        {
            PhotonNetwork.Destroy(photonView);
        }
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_Init(int ID)
    {
        StartCoroutine(Coroutine_Init(ClientManager.Instance.GetEntityFromChampionID(ID)));
    }
}
