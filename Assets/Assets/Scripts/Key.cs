using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public int Team;
    public float PickCooldown;
    private float _PickTimeLeft;
    public float PickTimeLeft => _PickTimeLeft;

    private void Update()
    {
        if (_PickTimeLeft > 0f)
        {
            _PickTimeLeft -= Time.deltaTime;
            if (_PickTimeLeft < 0f) _PickTimeLeft = 0f;
        }
    }

    public void OnDrop()
    {
        _PickTimeLeft = PickCooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entity"))
        {
            Entity entity = other.GetComponent<Entity>();
            if (entity.IsDead) return;
            if (entity.IsClientEntity && PickTimeLeft == 0f)
            {
                GameManager.Instance.PickKey(Team);
            }
        }
    }
}
