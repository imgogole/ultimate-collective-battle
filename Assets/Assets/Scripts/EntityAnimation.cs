using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Photon.Pun;

public class EntityAnimation : MonoBehaviour
{
    public Entity Owner;
    public PhotonView View;

    public Transform LeftLegPivot;
    public Transform RightLegPivot;
    public Transform Body;

    public List<GameObject> SpecialEffects = new List<GameObject>();

    public GameObject MomoTrainEffect;
    public float MomoTrainAmplitude;
    public float MomoTrainSpeed;

    public float HeightEntity;

    public Vector3 RotationStandUp
    {
        get
        {
            return new Vector3(0f, Body.localEulerAngles.y, 0f);
        }
    }

    public CharacterState State = CharacterState.Idle;

    public float Amplitude = 10f;
    public float LerpSpeed = 5f;
    public float Speed = 5f;

    public float SaltoTime = 1f;

    public Vector3 InitialLeftLegPosition = new Vector3(0, 0, -0.25f);
    public Vector3 InitialRightLegPosition = new Vector3(0, 0, 0.25f);
    public Vector3 InitialLeftLegRotation = new Vector3(0, 0, -0.25f);
    public Vector3 InitialRightLegRotation = new Vector3(0, 0, 0.25f);

    public Vector3 LeftJumpLegsPosition = new Vector3(-0.5f, 1f, -0.25f);
    public Vector3 LeftJumpLegsRotation = new Vector3(45f, 0f, -0.25f);
    public Vector3 RightJumpLegsPosition = new Vector3(0.5f, 1f, 0.25f);
    public Vector3 RightJumpLegsRotation = new Vector3(45f, 0f, 0.25f);

    public Vector3 LeftLandLegsPosition = new Vector3(-0.5f, -1f, -0.25f);
    public Vector3 LeftLandLegsRotation = new Vector3(-45f, 0f, -0.25f);
    public Vector3 RightLandLegsPosition = new Vector3(0.5f, -1f, 0.25f);
    public Vector3 RightLandLegsRotation = new Vector3(-45f, 0f, 0.25f);

    public Vector3 LeftSaltoLegsPosition = new Vector3(-0.5f, -1f, -0.25f);
    public Vector3 LeftSaltoLegsRotation = new Vector3(-45f, 0f, -0.25f);
    public Vector3 RightSaltoLegsPosition = new Vector3(0.5f, -1f, 0.25f);
    public Vector3 RightSaltoLegsRotation = new Vector3(-45f, 0f, 0.25f);

    public Rigidbody EntityRigidbody;
    public Direction currentDirection;

    public Transform LeftArmPivot;
    public Transform RightArmPivot;

    private bool isLeftArmActive = true;

    float _Time = 0f;
    float _ArmTime = 0f;

    Entity _ArmsTarget;

    Coroutine Coroutine_Salto;


    private void Start()
    {
        EntityRigidbody = GetComponent<Rigidbody>();
        ResetTime();
        HideAllSpecialEffects();
    }

    public void SetState(CharacterState state)
    {
        View.RPC("RPC_SetState", RpcTarget.All, (int)state);
    }

    void Update()
    {
        HandleArms();
        HandleSpecialEffects();

        _Time += Time.deltaTime;

        switch (State)
        {
            case CharacterState.Walk:
                AnimateLegsWalk();
                ShowArms();
                break;

            case CharacterState.Idle:
                SetLegsPositionRotation(InitialLeftLegPosition, InitialLeftLegRotation, InitialRightLegPosition, InitialRightLegRotation);
                ShowArms();
                break;

            case CharacterState.Jump:
                AnimateLegsJump();
                ShowArms();
                break;

            case CharacterState.Land:
                AnimateLegsLand();
                ShowArms();
                break;

            case CharacterState.Special:
                // Ne rien faire, laisser les autres scripts manipuler les transform si besoin
                break;
        }
    }

    void HandleSpecialEffects()
    {
        if (State != CharacterState.Special)
        {
            HideAllSpecialEffects();
        }

        float value = Mathf.Sin(Time.time * MomoTrainSpeed) * MomoTrainAmplitude;
        MomoTrainEffect.transform.localPosition = new Vector3(-0.25f, value, 0f);
    }

    void AnimateLegsWalk()
    {
        float Additional = currentDirection == Direction.Left ? 0f : Mathf.PI;

        float rotationAmount = Mathf.Sin(_Time * Speed + Additional) * Amplitude;
        LeftLegPivot.localRotation = Quaternion.Euler(0f, 0f, rotationAmount);
        RightLegPivot.localRotation = Quaternion.Euler(0f, 0f, -rotationAmount);

        LeftLegPivot.localPosition = InitialLeftLegPosition;
        RightLegPivot.localPosition = InitialRightLegPosition;
    }

    void AnimateLegsJump()
    {
        SetLegsPositionRotation(LeftJumpLegsPosition, LeftJumpLegsRotation, RightJumpLegsPosition, RightJumpLegsRotation);
    }

    void AnimateLegsLand()
    {
        SetLegsPositionRotation(LeftLandLegsPosition, LeftLandLegsRotation, RightLandLegsPosition, RightLandLegsRotation);
    }

    void SetLegsPositionRotation(Vector3 leftPosition, Vector3 leftRotation, Vector3 rightPosition, Vector3 rightRotation)
    {
        // Appliquer l'interpolation aux positions
        LeftLegPivot.localPosition = Vector3.Lerp(LeftLegPivot.localPosition, leftPosition, LerpSpeed * Time.deltaTime);
        RightLegPivot.localPosition = Vector3.Lerp(RightLegPivot.localPosition, rightPosition, LerpSpeed * Time.deltaTime);

        // Appliquer l'interpolation aux rotations
        Quaternion leftTargetRotation = Quaternion.Euler(leftRotation);
        LeftLegPivot.localRotation = Quaternion.RotateTowards(LeftLegPivot.localRotation, leftTargetRotation, LerpSpeed * Time.deltaTime);

        Quaternion rightTargetRotation = Quaternion.Euler(rightRotation);
        RightLegPivot.localRotation = Quaternion.RotateTowards(RightLegPivot.localRotation, rightTargetRotation, LerpSpeed * Time.deltaTime);
    }

    public void DoMomoTrain()
    {
        View.RPC("RPC_DoMomoTrain", RpcTarget.All);
    }

    public void DoDalilHack()
    {
        View.RPC("RPC_DoDalilHack", RpcTarget.All);
    }

    public void DoSalto()
    {
        View.RPC("RPC_DoSalto", RpcTarget.All);
    }

    IEnumerator SaltoCoroutine()
    {
        State = CharacterState.Special; // Met l'état du joueur à Special pendant le salto

        SetLegsPositionRotation(LeftSaltoLegsPosition, LeftSaltoLegsRotation, RightSaltoLegsPosition, RightSaltoLegsRotation);

        float elapsedRotationTime = 0f;

        Body.localRotation = Quaternion.Euler(Vector3.zero);

        Vector3 originalBodyRotation = Vector3.zero;
        //Vector3 targetBodyRotation = originalBodyRotation + new Vector3(0f, 0f, 360f);

        while (elapsedRotationTime < SaltoTime)
        {
            Body.localRotation = Quaternion.Euler(Body.localRotation.eulerAngles.x, Body.localRotation.eulerAngles.y, Mathf.Lerp(0f, 360f, elapsedRotationTime / SaltoTime));
            elapsedRotationTime += Time.deltaTime;
            yield return null;
        }

        Body.localRotation = Quaternion.Euler(Vector3.zero);

        // Réinitialiser l'état du joueur à Idle après le salto
        State = CharacterState.Idle;
    }

    IEnumerator Coroutine_DalilHack()
    {
        State = CharacterState.Special;

        SetLegsPositionRotation(InitialLeftLegPosition, InitialLeftLegRotation, InitialRightLegPosition, InitialRightLegRotation);
        //SetRotDirection(-90);
        HideArms();
        SetSpecialEffects(1, true);
        SpecialEffects[0].transform.localRotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(5);

        SetSpecialEffects(1, false);

        State = CharacterState.Idle;
    }

    public void HideSpecialEffects()
    {
        View.RPC("RPC_HideSpecialEffects", RpcTarget.All);
    }

    public void HideAllSpecialEffects()
    {
        ShowArms();
        for (int i = 0; i < SpecialEffects.Count; i++)
        {
            SetSpecialEffects(i, false);
        }
    }

    public void SetSpecialEffects(int Index, bool IsActive)
    {
        SpecialEffects[Index].SetActive(IsActive);
    }

    public void HideArms()
    {
        LeftArmPivot.gameObject.SetActive(false);
        RightArmPivot.gameObject.SetActive(false);
    }

    public void ShowArms()
    {
        LeftArmPivot.gameObject.SetActive(true);
        RightArmPivot.gameObject.SetActive(true);
    }

    public void SetRotDirection(Direction direction)
    {
        View.RPC("RPC_SetRotDirection", RpcTarget.All, (int)direction);
    }

    // Fonction publique polymorphique pour définir la rotation en fonction d'un float
    public void SetRotDirection(float rotationY)
    {
        transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    public void ResetTime()
    {
        _Time = 0f;
    }

    private void HandleArms()
    {
        float originalScaleX = 1f;
        float targetScaleX = Owner.TotalRange;

        float lerpSpeed = Time.deltaTime * LerpSpeed;

        float leftArmValue;// = originalScaleX;
        float rightArmValue;// = originalScaleX;

        float Angle = GetAngle();

        if (_ArmTime > 0f)
        {
            _ArmTime -= Time.deltaTime;

            if (isLeftArmActive)
            {
                leftArmValue = Mathf.Lerp(LeftArmPivot.localScale.x, targetScaleX, lerpSpeed);
                rightArmValue = Mathf.Lerp(RightArmPivot.localScale.x, originalScaleX, lerpSpeed);
            }
            else
            {
                rightArmValue = Mathf.Lerp(RightArmPivot.localScale.x, targetScaleX, lerpSpeed);
                leftArmValue = Mathf.Lerp(LeftArmPivot.localScale.x, originalScaleX, lerpSpeed);
            }

            _ArmTime = Mathf.Max(_ArmTime, 0f);
        }
        else
        {
            Angle += transform.localEulerAngles.y;

            leftArmValue = originalScaleX;
            rightArmValue = originalScaleX;
        }

        LeftArmPivot.localScale = new Vector3(leftArmValue, 0.2f, 0.2f);
        RightArmPivot.localScale = new Vector3(rightArmValue, 0.2f, 0.2f);

        LeftArmPivot.rotation = Quaternion.Euler(0f, 0f, Angle);
        RightArmPivot.rotation = Quaternion.Euler(0f, 0f, Angle);
    }

    private float GetAngle()
    {
        Vector3 Pos = _ArmsTarget ? _ArmsTarget.CenterPoint : Vector3.zero;

        Vector2 distance = (LeftArmPivot.position - Pos);

        float Angle;

        if (_ArmTime > 0f)
        {
            float angleRadians = Mathf.Atan2(distance.y, distance.x);

            float angleDegrees = Mathf.Rad2Deg * angleRadians;

            Angle = (angleDegrees + 360) % 360;
        }
        else
        {
            Angle = 0f;
        }



        return Angle;

        /* float Angle;
         if (_ArmTime > 0f)
         {
             if (distance.x != 0f)
             {
                 Angle = Mathf.Tan(distance.y / distance.x);
             }
             else
             {
                 Angle = distance.y > 0f ? 90f : -90f;
             }
         }
         else
         {
             Angle = 0f;
         }
         return Angle;*/
    }

    public void HitArm(Entity target)
    {
        View.RPC("RPC_HitArm", RpcTarget.All, target.BaseChampion.ID);
    }

    //////////////////////////////////////////////////// RPCs ////////////////////////////////////////////////////


    [PunRPC]
    public void RPC_HideSpecialEffects()
    {
        HideAllSpecialEffects();
    }

    [PunRPC]
    private void RPC_SetRotDirection(int _direction)
    {
        State = CharacterState.Walk;
        HideAllSpecialEffects();

        Direction direction = (Direction)_direction;

        currentDirection = direction;
        ResetTime();
        switch (direction)
        {
            case Direction.Null:
                // Ne rien faire
                break;

            case Direction.Left:
                SetRotDirection(0f);
                break;

            case Direction.Right:
                SetRotDirection(180f);
                break;
        }
    }

    [PunRPC]
    private void RPC_DoSalto()
    {
        if (Coroutine_Salto != null) StopCoroutine(Coroutine_Salto);
        Coroutine_Salto = StartCoroutine(SaltoCoroutine());
    }

    [PunRPC]
    private void RPC_DoMomoTrain()
    {
        State = CharacterState.Special;

        SetLegsPositionRotation(InitialLeftLegPosition, InitialLeftLegRotation, InitialRightLegPosition, InitialRightLegRotation);
        SetRotDirection(-90);
        HideArms();
        SetSpecialEffects(0, true);
    }

    [PunRPC]
    private void RPC_DoDalilHack()
    {
        StartCoroutine(Coroutine_DalilHack());
    }

    [PunRPC]
    private void RPC_HitArm(int id)
    {
        Entity target = ClientManager.Instance.GetEntityFromChampionID(id);

        _ArmTime = 1f;
        _ArmsTarget = target;

        isLeftArmActive = !isLeftArmActive;
    }

    [PunRPC]
    private void RPC_SetState(int state)
    {
        ResetTime();
        State = (CharacterState)state;
    }
}

public enum CharacterState
{
    Walk,
    Idle,
    Jump,
    Land,
    Special
}