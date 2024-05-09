using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsEffect : MonoBehaviour
{
    public Entity Owner;
    public virtual void OnCalled() { }
    public virtual void OnCancel()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
