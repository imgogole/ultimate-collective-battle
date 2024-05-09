using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yassine_AU_Rays : SpellsEffect
{
    public float Duration;
    public Vector3 Beginning;

    public float ActiveDamage;
    public float UltimateDamage;
    public float PercentageHeal;

    public GameObject LeftRay;
    public GameObject RightRay;

    public List<Yassine_AU_Damage> YassineDamages;

    public float ActiveLength;

    private static Yassine_AU_Rays instance;
    public static Yassine_AU_Rays Instance => instance;

    private void Awake()
    {
        if (Owner.BaseChampion.ID == 13)
        instance = this;
    }

    public List<GameObject> DamageRay = new List<GameObject>();
    public List<GameObject> PreviewRay = new List<GameObject>();

    public float CurrentDamage
    {
        get
        {
            return IsActive ? ActiveDamage : UltimateDamage;
        }
    }

    int _State = 0;

    public bool IsActive => _State == 0;

    public override void OnCalled()
    {
        StartCoroutine(Coroutine_YassineRay());
    }

    public override void OnCancel()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 0 = Active, 1 = Ultimate
    /// </summary>
    /// <param name="state"></param>
    public void SetState(int state)
    {
        _State = state;
    }

    IEnumerator Coroutine_YassineRay()
    {
        Debug.Log($"State is Active : {IsActive}");

        foreach (Yassine_AU_Damage yass in YassineDamages)
        {
            yass.ResetHeal();
        }

        float Value = IsActive ? ActiveLength : 999f;

        for (int i = 0; i < PreviewRay.Count; i++)
        {
            PreviewRay[i].transform.localScale = Beginning;
        }
        for (int i = 0; i < DamageRay.Count; i++)
        {
            DamageRay[i].transform.localScale = Beginning;
        }

        float time = 0f;

        int directionRay = Owner.CurrentDirection == Direction.Left ? 0 : 1;

        if (IsActive)
        {
            LeftRay.SetActive(directionRay == 0);
            RightRay.SetActive(directionRay == 1);
        }
        else
        {
            LeftRay.SetActive(true);
            RightRay.SetActive(true);
        }

        while (time < Duration)
        {
            time += Time.deltaTime;
            for (int i = 0; i < 2; i++)
            {
                if (i == directionRay || !IsActive)
                PreviewRay[i].transform.localScale = new Vector3(time * Value / Duration, 1, 1);
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        time = 0f;
        AudioManager.PlaySound(Audio.Ray, transform.position, Value * 1.5f);

        for (int i = 0; i < 2; i++)
        {
            if (i == directionRay || !IsActive)
                DamageRay[i].transform.localScale = new Vector3(Value, 1, 1);
        }
        yield return new WaitForSeconds(0.3f);

        if (Owner.IsClientEntity)
        {
            foreach (Yassine_AU_Damage yass in YassineDamages)
            {
                yass.ApplyHeal();
            }
        }


        OnCancel();
    }
}
