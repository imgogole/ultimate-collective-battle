using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class Virgil_U_Noirrel : SpellsEffect
{
    public GameObject noirrelObject;
    public ParticleSystem pSystem;

    public float canalisationTime = 1.5f;
    public float ultimateDuration = 30f;
    public float healDuration = 15f;

    bool called = false;
    float _time = 0f;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_Noirrel());

    }

    public void Update()
    {
        pSystem.gameObject.transform.position = new Vector3(Owner.Position, 0.01f, 0f);
        if (Owner.IsClientEntity && called && Owner.IsNoirrel && healDuration > 0f)
        {
            if (_time > 0f)
            {
                _time -= Time.deltaTime;
                healDuration -= Time.deltaTime;
                if (_time <= 0f)
                {
                    _time = 0.5f;

                    Owner.Heal(Owner.LoveValue * 0.9f / healDuration, Owner, true);
                }
            }
        }

    }

    IEnumerator Coroutine_Noirrel()
    {
        called = true;
        healDuration = 15f;

        noirrelObject.SetActive(false);
        pSystem.gameObject.SetActive(true);

        pSystem.Clear();
        pSystem.Play();

        if (Owner.IsClientEntity)
        {
            Owner.SetChanneling(canalisationTime);
        }


        AudioManager.PlaySound(Audio.Noirrel, Owner.CenterPoint, 3f);

        yield return new WaitForSeconds(canalisationTime);

        noirrelObject.SetActive(true);

        _time = 1f;
        if (Owner.IsClientEntity)
        {
            Owner.AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.1f, ultimateDuration, false, true, "VIRGIL_U"));
            Owner.SetAndSyncValue(FloatStat.Noirrel, ultimateDuration);
        }

        yield return new WaitForSeconds(ultimateDuration);

        called = false;
        OnCancel();
    }
}
