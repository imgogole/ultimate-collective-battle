using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titouan_A_Scream : SpellsEffect
{
    public ParticleSystem pSystem;
    public float pushBackSpeed;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_OnCalled());
    }

    IEnumerator Coroutine_OnCalled()
    {
        pSystem.Play();

        //AudioManager.PlaySound(Audio.TitouanScream); //à mettre

        yield return null;

        Entity me = ClientManager.Instance.Me();

        if (!me.IsChanneling && ClientManager.Instance.IsEnemy(Owner))
        {
            me.SetAndSyncValue(FloatStat.StunTime, 0.75f);
            yield return null;
            me.PushBack(pushBackSpeed, Owner.DirectionFrom(me));
        }
    }
}
