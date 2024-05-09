using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] private PlayerMovement _PlayerMovement;

    private void OnTriggerEnter(Collider collision)
    {
        if (!_PlayerMovement.BaseEntity.IsClientEntity) return;
        if (collision.gameObject.CompareTag("Floor") && !_PlayerMovement.IsGrounded) AudioManager.PlaySound(Audio.OnGround);
    }
    private void OnTriggerStay(Collider collision)
    {
        if (!_PlayerMovement.BaseEntity.IsClientEntity) return;
        if (collision.gameObject.CompareTag("Floor")) _PlayerMovement.SetGrounded(true);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!_PlayerMovement.BaseEntity.IsClientEntity) return;
        if (collision.gameObject.CompareTag("Floor")) _PlayerMovement.SetGrounded(false);
    }
}
