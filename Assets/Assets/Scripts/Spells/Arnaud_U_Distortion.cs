using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arnaud_U_Distortion : SpellsEffect
{
    public float Size;
    public float TimeToLive;
    public float Damage;

    public ParticleSystem pSystem;

    float _Time = 0f;

    bool Active = false;

    public override void OnCalled()
    {
        Active = true;
        if (Owner.IsClientEntity)
        {
            List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(Owner, Size /2f);
            foreach (Entity e in enemies)
            {
                Debug.Log($"{e.ChampionName} a pris {Damage} points de dégâts à cause de l'onde de choc d'Arnaud.");
                e.Attack(Damage, Owner);
            }
            List<Arnaud_U_Crepe> crepes = FindObjectsOfType<Arnaud_U_Crepe>().ToList();
            foreach (Arnaud_U_Crepe crepe in crepes)
            {
                if (Vector3.Distance(crepe.transform.position, transform.position) < 4f)
                    crepe.PushBack();
            }
        }
        pSystem.Play();
    }

    private void Update()
    {
        if (Active)
        {
            _Time += Time.deltaTime;
            Vector3 WorldPos = transform.position;
            WorldPos.y = 0.75f;
            transform.position = WorldPos;
            if (_Time > TimeToLive)
            {
                _Time = 0f;
                Active = false;
                pSystem.Stop();
                gameObject.SetActive(false);
            }
        }
    }
}
