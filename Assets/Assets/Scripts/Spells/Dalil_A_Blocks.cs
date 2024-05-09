using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dalil_A_Blocks : SpellsEffect
{
    float _Timer = 0f;

    public override void OnCalled()
    {
        _Timer = 3f;
    }

    private void Update()
    {
        if (_Timer > 0f)
        {
            _Timer -= Time.deltaTime;
            if (_Timer <= 0f) gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Crepe")) Destroy(collision.gameObject);
    }
}
