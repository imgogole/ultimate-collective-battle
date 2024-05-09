using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Samuel_U_BabyDragon : SpellsEffect
{
    public ParticleSystem pSystem;
    public float Damage;
    public float Radius;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_BabyDragon());
    }

    private IEnumerator Coroutine_BabyDragon()
    {
        pSystem.Play();

        yield return null;
    }
}
