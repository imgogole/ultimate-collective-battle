using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUps : MonoBehaviourPun
{
    public PowerUpType Type;
    public float Value;
    public float Cooldown;
    public GameObject Renderer;
    public PhotonView View;
    public float SpeedRotationRenderer;

    bool _Active;
    float _Time;

    public void ResetPowerUp()
    {
        _Time = 0;
        _Active = true;
    }

    public void TakePowerUp(Entity entity)
    {
        if (Type == PowerUpType.Heal)
        {
            entity.Heal(Value * entity.MissingHP);
        }
        else if(Type == PowerUpType.CooldownReduction)
        {
            entity.ResetAutoAttack();
            entity.ReduceCooldown(SpellType.Active, Value);
            entity.ReduceCooldown(SpellType.Ultimate, Value);
        }

        View.RPC("RPC_SetCooldown", RpcTarget.All, Cooldown);
    }

    private void Update()
    {
        if (_Time > 0f)
        {
            _Time -= Time.deltaTime;
            if (_Time < 0f)
            {
                ResetPowerUp();
            }
        }

        Vector3 v = Renderer.transform.eulerAngles;
        v.y += SpeedRotationRenderer * Time.deltaTime;
        Renderer.transform.eulerAngles = v;

        Renderer.SetActive(_Active);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_Active)
        {     
            if (other.CompareTag("Entity"))
            {
                _Active = false;
                Entity entity = other.GetComponent<Entity>();
                if (entity)
                {
                    TakePowerUp(entity);
                }
            }
        }
    }

    [PunRPC]
    private void RPC_SetCooldown(float cooldown)
    {
        _Time = cooldown;
        _Active = false;
    }


}

public enum PowerUpType
{
    Heal,
    CooldownReduction
}