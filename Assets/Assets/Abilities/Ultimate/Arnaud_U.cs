using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Ultimate/Arnaud", order = 1)]
public class Arnaud_U : Ability
{
    public override void OnActivate(Entity actor)
    {
        Vector3 Position = actor.CurrentDirection == Direction.Left ? actor.LeftAttackPoint.transform.position : actor.RightAttackPoint.transform.position;
        GameObject obj = ClientManager.Instance.CreateObject("Crepe", Position);
        obj.GetComponent<Arnaud_U_Crepe>().Init(actor);
    }
}
