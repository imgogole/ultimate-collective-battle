using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momow_U_DeltaRay : SpellsEffect
{
    public float Duration;
    public Vector3 Beginning;

    public List<GameObject> DamageRay = new List<GameObject>();
    public List<GameObject> PreviewRay = new List<GameObject>();

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_DeltaRay());
    }

    public override void OnCancel()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    IEnumerator Coroutine_DeltaRay()
    {
        float Value = Owner.FinalDeltaRayLength;

        for (int i = 0; i < PreviewRay.Count; i++)
        {
            PreviewRay[i].transform.localScale = Beginning;
        }
        for (int i = 0; i < DamageRay.Count; i++)
        {
            DamageRay[i].transform.localScale = Beginning;
        }

        float time = 0f;

        while (time < Duration)
        {
            time += Time.deltaTime;
            for (int i = 0; i < PreviewRay.Count; i++) PreviewRay[i].transform.localScale = new Vector3(time * Value / Duration, 1, 1);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        time = 0f;
        AudioManager.PlaySound(Audio.Ray, transform.position, Value * 1.5f);

        while (time < Duration)
        {
            time += Time.deltaTime;
            for (int i = 0; i < DamageRay.Count; i++) DamageRay[i].transform.localScale = new Vector3( time * Value / Duration, 1, 1);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
