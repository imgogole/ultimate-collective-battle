
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public Entity BaseEntity;

    public EntityAnimation BaseEntityAnimation;

    public PhotonView View;
    
    public Rigidbody PlayerRigidbody;

    [HideInInspector] public bool IsGrounded;

    private int _CurrentMultipleJumpRequest;
    private float _CurrentTimeJumpLeft;

    private bool _DirectionFocus;

    private float _UncontrolledTime = 0f;
    public bool IsUncontrolled => _UncontrolledTime > 0f;

    bool _input = false;
    public bool IsInput => _input;

    public bool IsSpecialAnimation => BaseEntityAnimation.State == CharacterState.Special;

    public void SetUncontrolling(float _Time)
    {
        _UncontrolledTime = _Time;
    }

    private void Start()
    {
        if (!View.IsMine) PlayerRigidbody.isKinematic = true;
        _CurrentMultipleJumpRequest = 0;
    }

    private void Update()
    {
        if (!View.IsMine) return;
        if (!BaseEntity.IsAbleToControl) return;
        MoveHandler();
        JumpHandler();
        AnimationHandler();

        if (Input.GetKeyDown(GameSettings.LeftAttackKey))
        {
            BaseEntity.UseAutoAttack(false);
        }
        if (Input.GetKeyDown(GameSettings.RightAttackKey))
        {
            BaseEntity.UseAutoAttack(true);
        }
    }

    private void FixedUpdate()
    {
        FrictionHandler();
    }

    public bool ForceNonVelocity;

    int _Dir = 0;

    /// <summary>
    /// Set the current velocity to zero.
    /// </summary>
    public void StopMovements()
    {
        PlayerRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// This script provides an help to get friction without disturbing jumping.
    /// </summary>
    private void FrictionHandler()
    {
        if (IsGrounded && !IsInput)
        {
            Vector3 v = PlayerRigidbody.velocity;
            v.x *= 0.7f;
            PlayerRigidbody.velocity = v;
        }
    }

    private void AnimationHandler()
    {
        if (IsSpecialAnimation) return;

        if (!IsGrounded)
        {
            if (PlayerRigidbody.velocity.y > 0)
            {
                SetAnimationState(CharacterState.Jump);
            }
            else if (PlayerRigidbody.velocity.y < 0)
            {
                SetAnimationState(CharacterState.Land);
            }
        }
        else
        {
            if (Mathf.Abs(PlayerRigidbody.velocity.x) > 0.1f)
            {
                SetAnimationState(CharacterState.Walk);
            }
            else
            {
                SetAnimationState(CharacterState.Idle);
            }
        }
    }

    private void MoveHandler()
    {
        if (IsUncontrolled)
        {
            _UncontrolledTime -= Time.deltaTime;
            if (_UncontrolledTime < 0f) _UncontrolledTime = 0f;

            _input = true;
            _Dir = BaseEntity.CurrentDirection == Direction.Right ? 1 : -1;
        }
        else
        {
            _Dir = 0;
            _input = false;

            if (Input.GetKeyDown(GameSettings.LeftKey))
            {
                //HideSpecialEffects();
                BaseEntityAnimation.ResetTime();
                SetRotation(Direction.Left);
                _DirectionFocus = false;
            }
            if (Input.GetKeyDown(GameSettings.RightKey))
            {
                //HideSpecialEffects();
                BaseEntityAnimation.ResetTime();
                SetRotation(Direction.Right);
                _DirectionFocus = true;
            }

            if (_DirectionFocus)
            {
                if (Input.GetKey(GameSettings.RightKey))
                {
                    _input = true;
                    _Dir = 1;
                }
            }
            else
            {
                if (Input.GetKey(GameSettings.LeftKey))
                {
                    _input = true;
                    _Dir = -1;
                }
            }
        }

        Vector3 v = PlayerRigidbody.velocity;
        v.x = _Dir * BaseEntity.TotalMovementSpeed;

        if (_input || ForceNonVelocity)
            PlayerRigidbody.velocity = v;

        if (_Dir == -1)
        {
            BaseEntity.SetDirection(Direction.Left);
        }
        if (_Dir == 1)
        {
            BaseEntity.SetDirection(Direction.Right);
        }
    }

    private void JumpHandler()
    {
        if (Input.GetKeyDown(GameSettings.JumpKey))
        {
            Jump();
        }   
        if (_CurrentTimeJumpLeft > 0)
        {
            _CurrentTimeJumpLeft -= Time.deltaTime;
            if (_CurrentTimeJumpLeft < 0) _CurrentTimeJumpLeft = 0;
        }
    }

    private void Jump()
    {
        bool MultipleJumpControlled = (_CurrentMultipleJumpRequest < BaseEntity.BaseChampion.MultipleJump) && _CurrentTimeJumpLeft == 0;
        if (IsGrounded || MultipleJumpControlled)
        {
            Vector3 v = PlayerRigidbody.velocity;
            v.y = BaseEntity.BaseChampion.JumpSpeed;
            PlayerRigidbody.velocity = v;

            _CurrentMultipleJumpRequest++;
            _CurrentTimeJumpLeft = BaseEntity.BaseChampion.JumpCooldown;

            BaseEntity.SetCanUseSalto(true);
            AudioManager.PlaySound(Audio.Jump);
        }

    }

    public void SetGrounded(bool value)
    {
        IsGrounded = value;
        _CurrentMultipleJumpRequest = 0;

        if (value)
        {
            BaseEntity.SetCanUseSalto(false);
        }

        if (BaseEntity.DidSalto)
        {
            BaseEntity.DidSalto = false;
            BaseEntity.AfterSalto();
        }
    }

    public void SetAnimationState(CharacterState state)
    {
        if (BaseEntityAnimation.State != state)
        {
            BaseEntityAnimation.SetState(state);
        }
    }

    public void SetRotation(Direction dir)
    {
        BaseEntityAnimation.SetRotDirection(dir);
        /*if (BaseEntityAnimation.currentDirection != dir)
        {
            BaseEntityAnimation.SetRotDirection(dir);
        }*/
    }

    public void DoSalto()
    {
        BaseEntityAnimation.DoSalto();
    }
    
    public void DoMomoTrain()
    {
        BaseEntityAnimation.DoMomoTrain();
    }

    public void DoDalilHack()
    {
        BaseEntityAnimation.DoDalilHack();
    }

    public void HideSpecialEffects()
    {
        BaseEntityAnimation.HideSpecialEffects();
    }
}
