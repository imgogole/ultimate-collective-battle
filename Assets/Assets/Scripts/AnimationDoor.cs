using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDoor : MonoBehaviour
{
    [Header("Set this transform as the join of the door.")]
    [Space(15)]
    public float ClosedRotation;
    public float OpenedRotation;
    [Space(15)]
    public DoorState StateOnAwake;
    public float Speed = 5f;

    private float _CurrentRotation;
    private float _GoalRotation;

    private void Start()
    {
        if (StateOnAwake == DoorState.Opened)
        {
            _GoalRotation = OpenedRotation;
            ApplyRotation(OpenedRotation);
        }
        else
        {
            _GoalRotation = ClosedRotation;
            ApplyRotation(ClosedRotation);
        }
    }

    private void Update()
    {
        SetDoorRotation(Mathf.Lerp(_CurrentRotation, _GoalRotation, Speed * Time.deltaTime));
        ApplyRotation(_CurrentRotation);
    }

    public void ForceClosing()
    {
        ApplyRotation(ClosedRotation);
    }

    public void OpenDoor()
    {
        _GoalRotation = OpenedRotation;
    }

    public void CloseDoor()
    {
        _GoalRotation = ClosedRotation;
    }

    private void SetDoorRotation(float Rotation)
    {
        _CurrentRotation = Rotation;
    }

    private void ApplyRotation(float Value)
    {
        transform.rotation = Quaternion.Euler(0, Value, 0);
    }
}

public enum DoorState
{
    Opened,
    Closed
}