using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Entity : MonoBehaviourPun, IPunObservable
{
    public bool IsPlayer;
    public bool TestMode;
    public Champion BaseChampion;

    public bool JumpConstraint;

    private float _HitPoints;

    public EntityEffects Effects;
    public EntityHitPoint HitPointsEffect;
    public List<SpellsEffect> SpellsEffects = new List<SpellsEffect>();
    public List<GameObject> LocalEffects = new List<GameObject>();

    public List<GameObject> LocalIconTarget = new List<GameObject>();

    public Collider EntityCollider;
    public GameObject GraveObject;
    public GameObject Renderer;
    public SpriteRenderer GroundGraveObject;

    public List<SpriteRenderer> headSprites;
    public MeshRenderer headRenderer;
    public Outline EntityOutline;

    public Statistics EntityStatistics;

    [Header("UI")]

    public GameObject EntityUI;
    public CanvasGroup CanvasGroupUI;

    public TMP_Text UIName;
    public TMP_Text HPText;
    public RectTransform UIValueHP;
    public Image UIValueImageHP;
    public RectTransform UIRetardValueHP;

    [Header("Icons")]

    public GameObject StunnedIcon;
    public GameObject DodgeIcon;
    public GameObject ControlledIcon;
    public GameObject TryhardingIcon;
    public GameObject ImmuneToCCIcon;
    public GameObject ChannelingIcon;
    public GameObject TrainingIcon;
    public GameObject InvisibilityIcon;
    public GameObject NastinessIcon;

    [Space(5)]
    //Stats

    public GameObject CatchUpIcon;
    public GameObject CritIcon;
    public GameObject PredatorIcon;
    public GameObject VirusIcon;
    public GameObject TitouanTargetIcon;

    [Space(5)]

    public GameObject UINameObject;
    public GameObject UIHitPointsObject;
    public GameObject UIEffectsObject;

    public GameObject UIKeyTeamOne;
    public GameObject UIKeyTeamTwo;

    public Image SkullImage;
    public GameObject DeathObject;

    public Color AllyColor;
    public Color EnemyColor;

    [Header("Infos")]

    public AttackType EntityAttackType;

    public GameObject LeftGOAttackPoint, RigthGOAttackPoint;
    public AttackPoint LeftAttackPoint, RightAttackPoint;

    private Direction _CurrentDirection;

    private float _AttackTimeRemaining;
    private float _PassiveSpellRemaining;
    private float _ActiveSpellRemaining;
    private float _UltimateSpellRemaining;

    public PhotonView View;
    //public PhotonTransformViewClassic TransformView;
    public PlayerManager PlayerManagerParent;
    private PlayerMovement _PlayerMovement;

    private int _Hall;

    List<HistoricAttack> HistoricAttacks = new List<HistoricAttack>();

    /// <summary>
    /// Returns the player name.
    /// </summary>
    public string PlayerName
    {
        get
        {
            return View.Owner.NickName;
        }
    }

    /// <summary>
    /// The current rune used for the entity.
    /// </summary>
    public int Rune
    {
        get
        {
            return PlayerManagerParent.RuneSelected;
        }
    }

    public bool IsAbleToControl
    {
        get
        {
            return
                !IsChanneling &&
                !IsStun &&
                !IsDead &&
                !GameSettings.IsOpen &&
                !GameManager.Instance.IsEntityMustFreeze;
        }
    }

    /// <summary>
    /// Returns the team of the player.
    /// </summary>
    public int Team
    {
        get
        {
            return PlayerManagerParent.Team;
        }
    }

    /// <summary>
    /// The life of the entity
    /// </summary>
    public float HitPoints
    {
        set
        {
            _HitPoints = value;
        }
        get
        {
            return _HitPoints;
        }
    }

    public float HitPointsMax
    {
        get
        {
            return BaseChampion.HP + _HPBonus + _HPRuneBonus;
        }
    }

    /// <summary>
    /// Returns the x position of the entity.
    /// </summary>
    public float Position
    {
        get
        {
            return transform.position.x;
        }
    }

    /// <summary>
    /// Returns the point of the center of the Entity
    /// </summary>
    public Vector3 CenterPoint
    {
        get
        {
            return new Vector3(transform.position.x, transform.position.y + Size + .5f, transform.position.z);
        }
    }

    /// <summary>
    /// The total amount of Attack damage of the entity.
    /// </summary>
    public float TotalAttackDamage
    {
        get
        {
            return Effects.GetValue(BaseChampion.AttackDamage, BehaviourEffects.AttackDamage, BehaviourEffects.PercentageAttackDamage) ;
        }
    }

    /// <summary>
    /// The total movement speed of the entity.
    /// </summary>
    public float TotalMovementSpeed
    {
        get
        {
            return Effects.GetValue(BaseChampion.MovementSpeed, BehaviourEffects.Speed, BehaviourEffects.PercentageSpeed);
        }
    }

    /// <summary>
    /// The total amount of Armor of the entity.
    /// </summary>
    public float TotalArmor
    {
        get
        {
            return Effects.GetValue(BaseChampion.ArmorPercent, BehaviourEffects.Armor, BehaviourEffects.PercentageArmor);
        }
    }

    /// <summary>
    /// The total amount of Armor of the entity.
    /// </summary>
    public float TotalAttackSpeed
    {
        get
        {
            return 1f / Effects.GetValue(1f / BaseChampion.AttackSpeed, BehaviourEffects.AttackSpeed, BehaviourEffects.PercentageAttackSpeed);
        }
    }

    /// <summary>
    /// The total amount of Armor of the entity.
    /// </summary>
    public float TotalRange
    {
        get
        {
            return Effects.GetValue(BaseChampion.Range, BehaviourEffects.Range, BehaviourEffects.PercentageRange);
        }
    }

    /// <summary>
    /// Returns the distance between the Entity and his brother. If this entity doesn't have brother or is in another hall, this value returns infinity.
    /// </summary>
    public float DistanceFromBrother
    {
        get
        {
            if (Brother == null || Brother.Hall != Hall) return float.PositiveInfinity;

            return Vector3.Distance(transform.position, Brother.transform.position);
        }
    }

    /// <summary>
    /// Returns the current size of the entity.
    /// </summary>
    public float Size
    {
        get
        {
            return Mathf.Clamp(0.004f * HitPointsMax + 0.6f, 1f, 1.2f);
        }
    }

    public bool IsTitouan
    {
        get
        {
            return BaseChampion.ID == 11;
        }
    }

    public bool IsTitouanStolenUlt
    {
        get
        {
            return TitouanTarget != null && isUltimateOfTargetValaible > 0f;
        }
    }

    // ------------------------------------------ Stats ----------------------------------------------//

    private bool _InLobby = true;
    private bool _InEnemyLobby = false;

    private ItemShop _LastItemUsed;

    private bool _HadFirstTeamKey;
    private bool _HadSecondTeamKey;

    private float _TimeStandingUp = 0f;

    public bool DidSalto = false;
    private float _DodgingTime = 0f;
    private float _StunTime = 0f;
    private float _ControlledTime = 0f;
    private bool _Training = false;
    private int _Controller = -1;
    private float _TryhardingTime = 0f;
    private float _ImmuneToCC = 0f;
    private float _Success = 0f;
    private float _Delta = 0f;
    private float _MajorationTime = 0f;
    private HistoricAttack _MajorationMaxAttack = HistoricAttack.Null;
    private bool _CanUseResultMajoration = false;
    private float _ChannelingTime = 0f;
    private float _InternalFlaw = 0f;
    private Entity _Brother = null;
    private float _CritAttack = 0f;
    private bool _AlreadySendedLeMettayer = false;
    private bool _AlreadyOpenedClashRoyale = false;

    private float _Hair = 0f;
    private float _UsedSpellLou = 0f;
    private int _Predator = 0;
    private float _AttackPredatorTime = 0f;
    private float _PredatorTimeCheck = 0f;

    private float positionLagPoint = 0f;
    private bool hasLagPoint = false;
    private float hpLagPoint = 0f;
    private int hallLagPoint;

    private Entity lastEntityHitted = null;
    private int timeHittedLastEntity = 0;
    private Entity titouanTarget = null;

    private float isUltimateOfTargetValaible = 0f;

    public bool IsUltimateOfTargetValaible => isUltimateOfTargetValaible > 0f;

    private float _NastinessAttack = 0f;

    private float _softwareRuneTime = 0f;

    private float _InvisibilityTime = 0f;
    public Entity Brother => _Brother;

    private bool _CatchUp = false;

    private Vector3 _LastPosition;

    private bool _CanUseSalto = false;

    private float _NoirrelTime = 0f;

    private float _TargetTime = 0;

    public float TargetTime => _TargetTime;

    public Entity LastEntityHitted => lastEntityHitted;
    public int TimeHittedLastEntity => timeHittedLastEntity;
    public Entity TitouanTarget => titouanTarget;

    public bool IsNoirrel => _NoirrelTime > 0f;
    public bool HasNastinessAttack => _NastinessAttack > 0f;

    public float InternalFlaw => _InternalFlaw;

    public bool IsCatchUp => _CatchUp;
    public float Delta => _Delta;

    public float Hair => _Hair;
    public float UsedSpellLou => _UsedSpellLou;

    public int Predator => _Predator;

    public bool IsInvisible => _InvisibilityTime > 0f;
    public float AttackPredatorTime => _AttackPredatorTime;

    public bool IsStandingUp => _TimeStandingUp > 0f;

    public ItemShop LastItemUsed => _LastItemUsed;

    /// <summary>
    /// Determines whether this entity is capable of basic attacks
    /// </summary>
    public bool IsDodging => _DodgingTime > 0f;
    /// <summary>
    /// Determines whether this entity is able to move, use spells and jump.
    /// </summary>
    public bool IsStun => _StunTime > 0f;

    /// <summary>
    /// Determines whether this entity can do a salto
    /// </summary>
    public bool CanUseSalto => _CanUseSalto;

    public bool IsControlled => _ControlledTime > 0f;
    public bool IsTryharding => _TryhardingTime > 0f;
    public bool IsImmuneToCC => _ImmuneToCC > 0f;
    public PlayerMovement PlayerMovementParent => _PlayerMovement;

    public bool CanUseResultMajoration => _CanUseResultMajoration;

    public bool HadFirstTeamKey => _HadFirstTeamKey;
    public bool HadSecondTeamKey => _HadSecondTeamKey;

    public bool IsChanneling => _ChannelingTime > 0f;
    public bool InLobby => _InLobby;
    public bool InEnemyLobby => _InEnemyLobby;

    public bool IsTraining => _Training;
    public bool HasCritAttack => _CritAttack > 0f;
    public bool HasAttackPredatorTime => _AttackPredatorTime > 0f;

    public bool IsSuccess => _Success > 0f;

    public bool AlreadySendedLeMettayer => _AlreadySendedLeMettayer;

    public bool AlreadyOpenedClashRoyale => _AlreadyOpenedClashRoyale;

    public bool HasLagPoint => hasLagPoint;
    public float LagPointPosition => positionLagPoint;
    public float LagPointHP => hpLagPoint;

    public int LagPointHall => hallLagPoint;

    // ------------------------------------------ Runes  ---------------------------------------------//

    private float _FatValue = 0f;

    public float FatValue
    {
        get
        {
            return Mathf.Max(0f, _FatValue);
        }
    }

    // ------------------------------------------ Stacks ---------------------------------------------//

    private int _VirusCount = 0;
    private float _ElixirValue = 0f;
    private float _FriendshipValue = 0f;
    private float _HPBonus = 0f;
    private float _HPRuneBonus = 0f;
    public int VirusCount => _VirusCount;

    bool _IsDead = false;

    int killStreak = 0;
    float timeKillStreak = 0f;

    public int KillStreak => killStreak;

    public float ElixirValue => _ElixirValue;

    public float FriendshipValue
    {
        get
        {
            if (IsTitouan)
            {
                return 20f;
            }
            return _FriendshipValue;
        }

    }
    public float LoveValue => FriendshipValue * 0.6f;
    public float NastinessValue => FriendshipValue * 0.35f;
    // ----------------------------------------- Constants -------------------------------------------//

    const float GROWHAIRTIME = 30f;

    /// <summary>
    /// The color related of the client entity. If this entity is an ally, it returns green, otherwise, it returns red.
    /// </summary>
    public Color EntityColor
    {
        get
        {
            return ClientManager.Instance.IsAlly(this) ? AllyColor : EnemyColor;
        }
    }

    /// <summary>
    /// Checks if the entity is dead and can no longer play the round. Use this information to stop effects or stacks based on alive entities.
    /// </summary>
    public bool IsDead
    {
        get
        {
            return _IsDead;
        }
    }

    /// <summary>
    /// Returns the number of the hall where the entity is located.
    /// </summary>
    public int Hall
    {
        get
        {
            return _Hall;
        }
    }

    public float AttackTimeRemaining => _AttackTimeRemaining;
    public float RatioPassiveSpellRemaining
    {
        get
        {
            if (BaseChampion.PassiveAbility.Cooldown == 0) return 0;
            return _PassiveSpellRemaining / BaseChampion.PassiveAbility.Cooldown;
        }
    }
    public float RatioActiveSpellRemaining
    {
        get
        {
            if (BaseChampion.ActiveAbility.Cooldown == 0) return 0;
            return _ActiveSpellRemaining / BaseChampion.ActiveAbility.Cooldown;
        }
    }
    public float RatioUltimateSpellRemaining
    {
        get
        {
            if (BaseChampion.UltimateAbility.Cooldown == 0) return 0;
            return _UltimateSpellRemaining / BaseChampion.UltimateAbility.Cooldown;
        }
    }

    public bool IsMajoring
    {
        get
        {
            return _MajorationTime > 0f;
        }
    }

    public float FinalDeltaRayLength
    {
        get
        {
            return _Delta * 0.6f + 5f;
        }
    }

    public float FinalDeltaRayDamage
    {
        get
        {
            return _Delta * 0.3f + 10f;
        }
    }

    public float PassiveSpellRemaining=> _PassiveSpellRemaining;
    public float ActiveSpellRemaining=> _ActiveSpellRemaining;
    public float UltimateSpellRemaining=> _UltimateSpellRemaining;

    public bool IsPassiveConditionOk => BaseChampion.PassiveAbility.OnCondition(this);
    public bool IsActiveConditionOk => IsActiveReady && BaseChampion.ActiveAbility.OnCondition(this);
    public bool IsUltimateConditionOk => IsUltimateReady && BaseChampion.UltimateAbility.OnCondition(this);

    public bool IsActiveReady => _ActiveSpellRemaining == 0;
    public bool IsUltimateReady => _UltimateSpellRemaining == 0;

    public string ChampionName
    {
        get
        {
            if (BaseChampion != null)
            {
                return BaseChampion.Name;
            }
            else
            {
                return "Dummy";
            }
        }
    }
    public float RatioHP
    {
        get
        {
            return HitPoints / HitPointsMax;
        }
    }
    public float RatioMissingHP
    {
        get
        {
            return MissingHP / HitPointsMax;
        }
    }
    public float MissingHP
    {
        get
        {
            return (HitPointsMax - HitPoints);
        }
    }
    public bool CanAttack
    {
        get
        {
            return _AttackTimeRemaining == 0
                && !IsTraining;
        }
    }
    public bool IsClientEntity
    { 
        get
        {
            return  View.IsMine; 
        }
    }
    public Direction CurrentDirection
    {
        get => _CurrentDirection;
    }

    private void Awake()
    {
        LinkPlayer();
    }

    private void Start()
    {
        if (IsClientEntity)
        {
            gameObject.AddComponent<AudioListener>();
        }
    }
    /// <summary>
    /// Set the renderer of the entity.
    /// </summary>
    /// <param name="IsAlive"></param>
    public void SetDeathRenderer(bool IsAlive)
    {
        UIValueImageHP.color = EntityColor;

        _DeathAnimation_Time = IsAlive ? 1f : 0f;

        Renderer.SetActive(IsAlive);
        GraveObject.SetActive(!IsAlive);
        SetUINameActive(IsAlive);
        SetUIHPActive(IsAlive);
        SetEffectsActive(IsAlive);
        SetDeathObjectActive(!IsAlive);
        EntityUI.SetActive(IsAlive);
        //EntityCollider.enabled = IsAlive;

        if (!IsAlive)
        {
            HideAllLocalEffects();
            DesactivateAllSpellsEffects();
        }

        Color c = EntityColor;
        c.a = 0.75f;
        GroundGraveObject.color = c;
    }

    public void SetUINameActive(bool IsActive)
    {
        UINameObject.SetActive(IsActive);
    }
    public void SetUIHPActive(bool IsActive)
    {
        UIHitPointsObject.SetActive(IsActive);
    }
    public void SetEffectsActive(bool IsActive)
    {
        UIEffectsObject.SetActive(IsActive);
    }
    public void SetDeathObjectActive(bool IsActive)
    {
        if (IsActive) SetDeathObjectAnimation(0f);
        DeathObject.SetActive(IsActive);
    }
    public void SetDeathObjectAnimation(float Value)
    {
        Color c = SkullImage.color;
        c.a = 1 - Value;
        SkullImage.color = c;
        float Heigth = 2.5f;
        Vector2 v = DeathObject.GetComponent<RectTransform>().anchoredPosition;
        v.y = 1f + Heigth * Value;
        DeathObject.GetComponent<RectTransform>().anchoredPosition = v;
    }

    /// <summary>
    /// Set if the current entity is dead.
    /// </summary>
    /// <param name="IsDead"></param>
    public void SetDeath(bool __IsDead)
    {
        _IsDead = __IsDead;
        if (IsClientEntity) GameManager.Instance.SetEntityFreeze(__IsDead);
    }

    public void SetLastItemUsed(ItemShop item)
    {
        View.RPC("RPC_SetLastItemUsed", RpcTarget.All, item.IDItem);
    }
    public void Init(int round)
    {
        View.RPC("RPC_InitRound", RpcTarget.All, round);
    }

    private void InitRound(int Round)
    {
        InitRunes(Round);
        SetInitialValues();
        SetDeathRenderer(true);
        SetDeath( false);
        HideAllLocalEffects();
        SetKeyState(0, false);
        SetKeyState(1, false);
        SetLobbyStat(true);
        SetDirection(Direction.Left);
        DesactivateAllSpellsEffects();
        ResetCooldownChampion(0f);
        CleanseAllEffects();
        SetEntityRenderer();
        HandleSize();

        EntityOutline.enabled = ClientManager.Instance.IsEnemy(this);

        if (IsClientEntity)
        {
            GameManager.Instance.CheckSpecialSelection();
            UIManager.Instance.UpdateKDA();
        }
    }

    void InitRunes(int round)
    {
        if (round < 2) return;

        int Round = round - 1;

        if (Rune == 0)
        {
            _HPRuneBonus = 5f * Round;
            if (IsClientEntity)
            {
                AddAndSyncEffect(new Effect(BehaviourEffects.Armor, 0.01f, 0f, true, true, "RUNE_0"));
            }
        }
        else if (Rune == 1)
        {
            if (IsClientEntity)
            {
                AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.015f, 0f, true, true, "RUNE_1"));
                AddAndSyncEffect(new Effect(BehaviourEffects.PercentageAttackSpeed, 0.015f, 0f, true, true, "RUNE_1"));
            }
        }
    }

    private void SetEntityRenderer()
    {
        foreach (SpriteRenderer sRenderer in headSprites)
        {
            sRenderer.sprite = BaseChampion.Icon;
            sRenderer.transform.localRotation = Quaternion.Euler(0, BaseChampion.InverseIcon ? 180f : 0f, 180f);
        }
        headRenderer.material.color = BaseChampion.RepresentativeColor;
    }

    private void SetInitialValues()
    {
        HitPoints = HitPointsMax;

        CanvasGroupUI.alpha = 1;

        _VirusCount = 0;
        _CatchUp = false;
        _Hall = 0;
        _AlreadySendedLeMettayer = false;
        SetClashRoyale(false);
        _InvisibilityTime = 0f;
        _Predator = 0;
        _ElixirValue = 0f;
        hasLagPoint = false;
        HistoricAttacks = new List<HistoricAttack>();

        _TargetTime = 0.01f;
        titouanTarget = null;
        lastEntityHitted = null;
        timeHittedLastEntity = 0;

        killStreak = 0;
        timeKillStreak = 0f;

        SetUltimateTargetValidity(0f);

        _Delta = 0f;
    }

    public void HandleSize()
    {
        float entitySize = Size;
        float entityCompSize = entitySize * 3f;

        Renderer.transform.localScale = Vector3.one * entitySize;

        EntityUI.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, entityCompSize + 0.7f);

        CapsuleCollider collider = (CapsuleCollider)EntityCollider;

        collider.radius = 0.5f * entitySize;
        collider.height = entityCompSize;
        collider.center = new Vector3(0, entityCompSize / 2f, 0);
    }

    private void LinkPlayer()
    {
        Debug.Log($"Link player {View.Owner.NickName}");
        PlayerManagerParent = OnlineManager.GetOwner(View);

        BaseChampion = GameManager.Instance.GetChampion(PlayerManagerParent.ChampionSelected);

        PlayerManagerParent.SetOwnedEntity(this);

        if (IsClientEntity)
        {
            ClientManager.Instance.Init(this);
            _PlayerMovement = GetComponent<PlayerMovement>();
        }
    }

    public void CleanseAllEffects()
    {
        Effects.ClearAllEffects();
    }
    public void HideAllLocalEffects()
    {
        foreach (GameObject effect in LocalEffects)
            effect.SetActive(false);
    }

    public void DesactivateAllSpellsEffects()
    {
        foreach (SpellsEffect s in SpellsEffects)
        {
            s.OnCancel();
        }
    }

    public void ClearLagPoint()
    {
        hasLagPoint = false;
    }

    public void SetLagPoint()
    {
        hasLagPoint = true;
        positionLagPoint = Position;

        hpLagPoint = HitPoints;
        hallLagPoint = Hall;

        GameManager.Instance.ShowLagPoint(positionLagPoint);
    }

    public void SetHall(int hall)
    {
        View.RPC("RPC_SetHall", RpcTarget.All, hall);
    }

    public void SendLeMettayer()
    {
        _AlreadySendedLeMettayer = true;
        ClientManager.Instance.CreateObject("LeMettayer", new Vector3(Position, 0, 0));
    }

    public void TranslateTo(Vector3 direction, float duration)
    {
        StartCoroutine(Coroutine_TranslateTo(direction, duration));
    }

    public void UseStairs(int IDStairs)
    {
        SetChanneling(true);
        StartCoroutine(Coroutine_UseStairs(IDStairs));
    }

    IEnumerator Coroutine_UseStairs(int IDStairs)
    {
        Stairs startStairs = GameManager.Instance.GetStairs(IDStairs);
        Stairs endStairs = GameManager.Instance.GetStairs(startStairs.DestinationID);

        CameraFollowing.Instance.SetFollowing(false);

        float Duration = 1f;
        TranslateTo(startStairs.SidePoint, Duration);
        //yield return new WaitUntil(() => Mathf.Abs((transform.position - startStairs.SidePoint).sqrMagnitude) < 0.1f);
        yield return new WaitForSeconds(Duration + 0.1f);
        Teleport(endStairs.SidePoint);
        yield return new WaitForEndOfFrame();
        CameraFollowing.Instance.ForceAnchorPosition();
        yield return new WaitForEndOfFrame();
        TranslateTo(endStairs.Point, Duration);
        yield return new WaitForSeconds(Duration + 0.1f);
        CameraFollowing.Instance.SetFollowing(true);
        Teleport(endStairs.Point.x);

        View.RPC("RPC_SetHall", RpcTarget.All, startStairs.FinalHall);

        SetChanneling(false);
    }

    public void TeleportToSpawnPoint()
    {
        if (GameManager.Instance.SpawnPoint != null)
        {
            Teleport(GameManager.Instance.SpawnPoint);
        }
    }

    public void SetBrother(Entity brother)
    {
        _Brother = brother;
    }

    public bool CanUseSpell(bool UsableWithCC)
    {
            return
                !IsChanneling &&
                (!IsStun || UsableWithCC) &&
                !IsDead &&
                !IsControlled &&
                !GameManager.Instance.IsEntityMustFreeze;
    }

    public void Teleport(float xPos)
    {
        Teleport(new Vector3(xPos, 0.1f, 0f));
    }

    public void Teleport(Vector3 Pos)
    {
        transform.position = Pos;
        Physics.SyncTransforms();
        CameraFollowing.Instance.ForceAnchorPosition();
    }

    public IEnumerator Coroutine_TranslateTo(Vector3 targetPosition, float duration)
    {
        // THX TO CHATGPT <333

        Vector3 startPosition = transform.position;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null; // Attendre la prochaine frame
        }

        // Assurer que la position finale soit exacte
        Debug.Log($"End translating {ChampionName} to {targetPosition}");
        transform.position = targetPosition;
    }

    public void SendToClosedRoom()
    {
        SetAndSyncValue(BoolStat.CatchUp, false);
        Teleport(GameManager.Instance.ClosedRoomSpawn.position);
        Debug.Log($"Teleport to Closed room : {GameManager.Instance.ClosedRoomSpawn.position}");
    }

    public void CallSpellEffect(int Index)
    {
        View.RPC("RPC_CallSpellEffect", RpcTarget.All, Index);
    }

    public Direction DirectionFrom(Entity Target)
    {
        float Value = (transform.position.x - Target.transform.position.x);

        if (Value == 0) return Direction.Null;
        return Value > 0 ? Direction.Left : Direction.Right;
    }

    public void ResetCooldownChampion(float Delay)
    {
        _PassiveSpellRemaining = Delay + BaseChampion.PassiveAbility.Cooldown;
        _ActiveSpellRemaining = Delay + BaseChampion.ActiveAbility.Cooldown;
        _UltimateSpellRemaining = Delay + BaseChampion.UltimateAbility.Cooldown;
    }

    public void SetClashRoyale(bool alreadyOpened)
    {
        _AlreadyOpenedClashRoyale = alreadyOpened;
    }
    private void Update()
    {
        UpdateUI();

        HandleAttack();
        HandleSpells();
        HandleStats();
        HandleIcons();
        HandleAnimations();

        if (!IsDead)
        {
            HandleProperties();
            HandleActions();
        }

        _LastPosition = transform.position;
        if (Input.GetKeyDown(KeyCode.M))
        {
            List<ModalButton> buttons = new List<ModalButton>
            {
                new ModalButton("Se reconnecter",delegate { Test(); } ),
            };

            PopUpShower.Show("Test", "Ceci est un bouton text. ", buttons.ToArray());
        }
    }

    public void Test()
    {
        Debug.Log("Bouton test appuyé !");
    }

    private void HandleActions()
    {
        if (IsClientEntity && Input.GetKeyDown(KeyCode.X))
        {
            DropKey(-1);
        }
    }


    float _DeathAnimation_Time = 0f;
    float _DeathAnimation_Speed = 2f;
    /// <summary>
    /// Handle the whole animation state of the entity.
    /// </summary>
    private void HandleAnimations()
    {
        // Death
        if (IsDead)
        {
            _DeathAnimation_Time = Mathf.Lerp(_DeathAnimation_Time, 1f, Time.deltaTime * _DeathAnimation_Speed);
            SetDeathObjectAnimation(_DeathAnimation_Time);
        }


    }

    private void HandleIcons()
    {
        // Stats

        StunnedIcon.SetActive(IsStun);
        DodgeIcon.SetActive(IsDodging);
        ControlledIcon.SetActive(IsControlled);
        TryhardingIcon.SetActive(IsTryharding);
        ImmuneToCCIcon.SetActive(IsImmuneToCC);
        ChannelingIcon.SetActive(IsChanneling);
        TrainingIcon.SetActive(IsTraining);
        InvisibilityIcon.SetActive(IsInvisible);

        // Data

        CatchUpIcon.SetActive(IsCatchUp);
        CritIcon.SetActive(HasCritAttack);
        PredatorIcon.SetActive(HasAttackPredatorTime);
        VirusIcon.SetActive(VirusCount >= 3);
        NastinessIcon.SetActive(HasNastinessAttack);

        if (titouanTarget != null && IsUltimateOfTargetValaible)
        {
            TitouanTargetIcon.SetActive(true);
            TitouanTargetIcon.GetComponentInChildren<Image>().sprite = titouanTarget.BaseChampion.Icon;
        }
        else
        {
            TitouanTargetIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Reset the auto attack remaining time. Use it for spells that allow reinforcing next autoattack. Only for melee.
    /// </summary>
    public void ResetAutoAttack()
    {
        if (IsClientEntity && BaseChampion.IsMelee)
        {
            _AttackTimeRemaining = 0f;
        }
    }
    public void SetConsecutiveHit(List<Entity> targets)
    {
        /*
            public Entity LastEntityHitted => lastEntityHitted;
            public int TimeHittedLastEntity => timeHittedLastEntity;
            public Entity TitouanTarget => titouanTarget;
            public float TimeTarget => timeTarget;
         */

        if (lastEntityHitted == null)
        {
            timeHittedLastEntity = 1;
            List<Entity> intersections = ClientManager.Instance.NearestEnemyEntitiesFrom(this).Where(e => targets.Contains(e)).ToList();
            
            if (intersections.Count != 0)
            {
                lastEntityHitted = intersections.First();
            }
        }
        else
        {
            if (targets.Contains(lastEntityHitted))
            {
                timeHittedLastEntity++;
            }
            else
            {
                timeHittedLastEntity = 1;
                lastEntityHitted = null;

                List<Entity> intersections = ClientManager.Instance.NearestEnemyEntitiesFrom(this).Where(e => targets.Contains(e)).ToList();

                if (intersections.Count != 0)
                {
                    lastEntityHitted = intersections.First();
                }
            }
        }
    }

    public void AddPredatorEffect()
    {
        if (Predator < 4)
            _Predator++;
    }

    public void ResetPredatorEffects()
    {
        print("Reset predator effects");
        _Predator = 0;
    }

    public void ResetHair()
    {
        _Hair = 0f;
    }

    public void SetTitouanTarget(Entity entity, bool isUltime)
    {
        if (entity != null)
        {
            entity.SetAndSyncValue(FloatStat.Target, isUltime ? Mathf.Min(20f, entity.BaseChampion.UltimateAbility.Cooldown) : 20f);
        }
        else
        {
            if (titouanTarget != null) titouanTarget.SetAndSyncValue(FloatStat.Target, 0f);
            SetUltimateTargetValidity(0f);
        }
        titouanTarget = entity;
    }

    private void HandleStats()
    {
        if (_DodgingTime > 0)
        {
            _DodgingTime -= Time.deltaTime;
        }
        if (_StunTime > 0)
        {
            _StunTime -= Time.deltaTime;
        }
        if (_ControlledTime > 0)
        {
            _ControlledTime -= Time.deltaTime;
        }
        if (_TryhardingTime > 0)
        {
            _TryhardingTime -= Time.deltaTime;
        }
        if (_ImmuneToCC > 0)
        {
            _ImmuneToCC -= Time.deltaTime;
        }
        if (_ChannelingTime > 0)
        {
            _ChannelingTime -= Time.deltaTime;
        }
        if (_CritAttack > 0)
        {
            _CritAttack -= Time.deltaTime;
        }
        if (_AttackPredatorTime > 0)
        {
            _AttackPredatorTime -= Time.deltaTime;
        }
        if (_NastinessAttack > 0)
        {
            _NastinessAttack -= Time.deltaTime;
        }
        if (_NoirrelTime > 0)
        {
            _NoirrelTime -= Time.deltaTime;
        }
        if (timeKillStreak > 0)
        {
            timeKillStreak -= Time.deltaTime;
            if (timeKillStreak <= 0f)
            {
                killStreak = 0;
            }
        }
        if (_Success > 0)
        {
            _Success -= Time.deltaTime;
            if (_Success <= 0f)
            {
                _Success = 0f;
                SetLocalEffect(1, false);
            }
        }
        if (_TargetTime > 0f)
        {
            _TargetTime -= Time.deltaTime;
            if (_TargetTime <= 0f)
            {
                if (IsClientEntity) SetLocalEffect(2, false);
            }
        }
        if (TitouanTarget != null)
        {
            if (TitouanTarget.TargetTime <= 0f || TitouanTarget.IsDead)
            {
                if (TitouanTarget.IsDead)
                {
                    ReduceCooldown(SpellType.Ultimate, 0.99f);
                }
                SetTitouanTarget(null, false);
            }
        }

        if ((_LastPosition - transform.position).sqrMagnitude < 0.0001f)
        {
            _TimeStandingUp += Time.deltaTime;
        }
        else
        {
            _TimeStandingUp = 0f;
        }
        if (BaseChampion.ID == 6)
        {
            _Hair = Mathf.Clamp(_Hair + (Time.deltaTime / GROWHAIRTIME), 0f, 1f);
        }
        if (_UsedSpellLou > 0f)
        {
            _UsedSpellLou -= Time.deltaTime;
        }

    }

    private void ResetAttackPredatorTime()
    {
        SetAndSyncValue(FloatStat.AttackPredatorTime, 0f);
    }

    public void SetTimeSpellLou()
    {
        if (_Hair == 1f)
        {
            _UsedSpellLou = 10f;
            ResetHair();
        }
    }

    private void ResetSpellLou()
    {
        _UsedSpellLou = 0f;
    }

    public void SetDirection(Direction dir)
    {
        View.RPC("RPC_SetDirection", RpcTarget.All, (int)dir);
    }

    private void HandleSpells()
    {
        if (!IsClientEntity || (!GameManager.Instance.InRound)) return;

        if (_PassiveSpellRemaining > 0)
        {
            _PassiveSpellRemaining -= Time.deltaTime;
            if (_PassiveSpellRemaining <= 0)
            {
                _PassiveSpellRemaining = 0;
            }
        }
        if (_PassiveSpellRemaining == 0)
        {
            if (IsPassiveConditionOk)
            {
                StartPassiveSpell();
                _PassiveSpellRemaining = BaseChampion.PassiveAbility.Cooldown;
            }
        }
       if (_ActiveSpellRemaining >= 0)
        {
            _ActiveSpellRemaining -= Time.deltaTime;
            if (_ActiveSpellRemaining <= 0) _ActiveSpellRemaining = 0;
        }
        if (_UltimateSpellRemaining >= 0 && GameManager.Instance.CurrentRound >= GameManager.Instance.MinimumRoundUltimate)
        {
            _UltimateSpellRemaining -= Time.deltaTime;
            if (_UltimateSpellRemaining <= 0) _UltimateSpellRemaining = 0;
        }

        if (CanUseSpell(BaseChampion.ActiveAbility.UsableWhileCC))
        {
            if (Input.GetKeyDown(GameSettings.ActiveSpellKey)) StartActiveSpell();
        }

        if (CanUseSpell(BaseChampion.UltimateAbility.UsableWhileCC))
        {
            if (Input.GetKeyDown(GameSettings.UltimateSpellKey)) StartUltimateSpell();
        }
    }

    private void HandleProperties()
    {
        // Rune

        if (IsClientEntity)
        {
            if (Rune == 0 && GameManager.Instance.InRound)
            {
                _softwareRuneTime -= Time.deltaTime;

                if (_softwareRuneTime <= 0f)
                {
                    _softwareRuneTime = 5f;

                    List<Entity> enemies = ClientManager.Instance.NearestEnemyEntitiesFrom(this, 1.5f);

                    if (enemies.Count != 0)
                    {
                        foreach (Entity enemy in enemies)
                        {
                            enemy.Attack(0.02f * HitPointsMax, this);
                        }
                    }
                }
            }
        }


        // Majoration

        if (_MajorationTime > 0f)
        {
            _MajorationTime -= Time.deltaTime;

            // End of majoration

            if (_MajorationTime <= 0f)
            {
                if (IsClientEntity)
                {
                    if (_MajorationMaxAttack != HistoricAttack.Null)
                    {
                        _CanUseResultMajoration = true;
                        Entity target = _MajorationMaxAttack.Author;
                        target.SetLocalEffect(0, false);
                        float totalDamage = _MajorationMaxAttack.Amount + _MajorationMaxAttack.Author.HitPointsMax * 0.04f;
                        target.TakeTrueDamage(totalDamage, this);
                    }
                    else
                    {
                        if (BaseChampion.ID == 5) ReduceCooldown(SpellType.Active, 0.4f);
                    }
                }
            }
        }

        // Delta

        if (BaseChampion.ID == 7)
        {
            if (_TimeStandingUp > 8f)
            {
                if (!_Training && IsClientEntity)
                {
                    PlayerMovementParent.DoMomoTrain();
                }

                _Training = true;
                _Delta += Time.deltaTime * 0.5f;
            }
            else
            {
                _Training = false;
            }
        }

        // Controlled

        if (IsControlled)
        {
            if (_TimeStandingUp < 0.1f)
            {
                _InternalFlaw += Time.deltaTime;
            }

            if (_InternalFlaw > 1.25f)
            {
                _InternalFlaw = -100f;
                if (IsClientEntity)
                {
                    SetAndSyncValue(FloatStat.StunTime, 2f);
                    SetAndSyncValue(IntStat.Virus, VirusCount + 1);
                    Attack(20f, ClientManager.Instance.GetEntityFromChampionID(_Controller));
                }
            }
        }

        // Predators

        if (BaseChampion.ID == 6)
        {
            if (_PredatorTimeCheck < 0)
            {
                if (Predator < 4)
                {
                    List<Entity> entites = ClientManager.Instance.NearestEnemyEntitiesFrom(this, 5f);
                    if (entites.Count != 0)
                    {
                        AddPredatorEffect();
                        _PredatorTimeCheck = 3f;
                    }
                }
            }
            else
            {
                if (ActiveSpellRemaining <= 0f)
                    _PredatorTimeCheck -= Time.deltaTime;
            }
        }


        // Invisibility
        if (IsInvisible)
        {
            if (ClientManager.Instance.IsAlly(this))
            {
                CanvasGroupUI.alpha = 0.5f;
                Renderer.SetActive(true);
            }
            else
            {
                CanvasGroupUI.alpha = 0;
                Renderer.SetActive(false);
            }

            _InvisibilityTime -= Time.deltaTime;
            if (_InvisibilityTime <= 0)
            {
                CanvasGroupUI.alpha = 1;
                SetDeathRenderer(!IsDead);
            }
        }

        // Elixir

        if (BaseChampion.ID == 9) // Samuel
        {
            _ElixirValue += Time.deltaTime * GameManager.Instance.ElixirRate;
            _ElixirValue = Mathf.Clamp(_ElixirValue, 0f, 10f);
        }
    }

    private void StartPassiveSpell()
    {
        BaseChampion.PassiveAbility.OnActivate(this);
    }
    private void StartActiveSpell()
    {
        if (IsActiveConditionOk)
        {
            BaseChampion.ActiveAbility.OnActivate(this);
            _ActiveSpellRemaining = BaseChampion.ActiveAbility.Cooldown;

            if (Rune == 1)
            {
                AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.1f, 1f, false, true, "RUNE_1"));
            }
        }
        else
        {
            AudioManager.PlaySound(Audio.Blocked);
            UIManager.Instance.ShowGeneralInfo(GameManager.Instance.AbilityReadyNotUsableMessage);
        }
    }
    private void StartUltimateSpell()
    {
        if (IsUltimateConditionOk)
        {
            _UltimateSpellRemaining = BaseChampion.UltimateAbility.Cooldown;
            BaseChampion.UltimateAbility.OnActivate(this);

            if (Rune == 1)
            {
                AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.1f, 1f, false, true, "RUNE_1"));
            }
        }
        else
        {
            AudioManager.PlaySound(Audio.Blocked);
            UIManager.Instance.ShowGeneralInfo(GameManager.Instance.AbilityReadyNotUsableMessage);
        }
    }

    public void SetUltimateTargetValidity(float timeValid)
    {
        isUltimateOfTargetValaible = timeValid;
    }

    public void AddElixir(float Elixir)
    {
        _ElixirValue += Elixir;
    }

    private void HandleAttack()
    {
        if (_AttackTimeRemaining > 0)
        {
            _AttackTimeRemaining -= Time.deltaTime;
            if (_AttackTimeRemaining <= 0) _AttackTimeRemaining = 0;
        }
    }

    /// <summary>
    /// Reduce the cooldown of a spell by a relative % value (between 0 and 1)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void ReduceCooldown(SpellType type, float value)
    {
        float _value = (1 - value);
        switch (type)
        {
            case SpellType.Passive:
                _PassiveSpellRemaining *= _value;
                break;
            case SpellType.Active:
                _ActiveSpellRemaining *= _value;
                break;
            case SpellType.Ultimate:
                _UltimateSpellRemaining *= _value;
                break;
        }
    }

    private void UpdateUI() 
    {
        // 100% = 0.5; 0% = 2.5
        UIValueHP.offsetMax = Vector2.Lerp(UIValueHP.offsetMax, new Vector2(-(2.5f - RatioHP * 2f), UIValueHP.offsetMax.y), 5f * Time.deltaTime);
        UIName.text = BaseChampion.Name;
        HPText.text = Mathf.RoundToInt(HitPoints).ToString();
        UIManager.Instance.UpdateStatsInformation();
        ClientManager.Instance.UpdateClientStatsUI();
    }

    public void SetMajorationTime(float Value)
    {
        SetAndSyncValue(FloatStat.Majoration, Value);
        _MajorationMaxAttack = HistoricAttack.Null;
        _CanUseResultMajoration = false;
    }

    public void SetChanneling(bool value)
    {
        SetAndSyncValue(FloatStat.Channeling, value ? 9999f : 0f);
    }

    public void SetChanneling(float value)
    {
        SetAndSyncValue(FloatStat.Channeling, Mathf.Clamp(value, 0f, float.MaxValue));
        PlayerMovementParent.StopMovements();
    }

    public void HealFromMajoration()
    {
        _CanUseResultMajoration = false;
        float Amount = _MajorationMaxAttack.Amount + _MajorationMaxAttack.Author.HitPointsMax * 0.04f;
        Amount *= 1.75f;
        Heal(Amount, this, true);
    }

    public bool IsEntityParticipateElimination(Entity Subject)
    {
        return HistoricAttacks.Any(e => e.Author.BaseChampion.ID == Subject.BaseChampion.ID);
    }

    public void AddKillStreak()
    {
        killStreak++;
        timeKillStreak = 20f;

        if (IsClientEntity)
        {
            if (killStreak == 1)
            {
                AudioManager.PlaySound(Audio.OneKill);
            }
            else if (killStreak == 2)
            {
                AudioManager.PlaySound(Audio.DoubleKill);
            }
            else if (killStreak == 3)
            {
                AudioManager.PlaySound(Audio.TripleKill);
            }
            else if (killStreak == 4)
            {
                AudioManager.PlaySound(Audio.QuadraKill);
            }
            else if (killStreak >= 5)
            {
                AudioManager.PlaySound(Audio.PentaKill);
            }
        }

    }

    public void AddBonusHP(float Value)
    {
        SetAndSyncValue(FloatStat.HPBonus, Value);
    }
    public void AddBonusAttackDamage(float Value)
    {
        AddAndSyncEffect(new Effect(BehaviourEffects.AttackDamage, Value, 0f, true, true, "IDRISS_P"));
    }
    public void AddBonusArmor(float Value)
    {
        AddAndSyncEffect(new Effect(BehaviourEffects.Armor, Value, 0f, true, true, "IDRISS_P"));
    }
    private void DeadProcedure(int Author)
    {
        SetDeathRenderer(false);
        SetDeath(true);
        HideAllLocalEffects();

        EntityStatistics.AddValue(Statistics.DEATH, 1);

        Entity me = ClientManager.Instance.Me();
        Entity author = ClientManager.Instance.GetEntityFromChampionID(Author);

        author.AddKillStreak();
        GameAnnoncerManager.Instance.AddEvent(AnnoncementType.Kill, author, this);

        List<Entity> entities = HistoricAttacks.Select(h => h.Author).Distinct().ToList();

        author.EntityStatistics.AddValue(Statistics.KILLS, 1);

        foreach (Entity entity in entities)
        {
            if (entity != author)
            {
                entity.EntityStatistics.AddValue(Statistics.ASSISTS, 1);
            }
        }

        if (IsMajoring)
        {
            _MajorationTime = 0f;
        }

        if (IsClientEntity)
        {
            CameraFollowing.Instance.SetSpectating(true, 0);



            if (_MajorationMaxAttack != HistoricAttack.Null)
            {
                _MajorationMaxAttack.Author.SetLocalEffect(0, false);
            }
        }

        Entity Client = ClientManager.Instance.Me();

        #region IDRISS_CHECKS
        // Checks client-side if Idriss participated into the elimination and give him bonus stats

        if (Client.BaseChampion.ID == 4)
        {
            if (IsEntityParticipateElimination(Client))
            {
                float Bonus = Client.IsImmuneToCC ? 2f : 1f;

                Client.AddBonusHP(4f * Bonus);
                Client.AddBonusAttackDamage(0.8f * Bonus);
                Client.AddBonusArmor(0.03f * Bonus);
            }
            if (Client == this)
            {
                // Idriss just died

                if (Client.IsImmuneToCC)
                {
                    Effects.RemoveEffectsWithTag("IDRISS_P");
                }
            }
        }

        #endregion

        #region SAMUEL_CHECKS
        // Checks client-side if Samuel participated into the elimination and give him elixir.
        if (Client.BaseChampion.ID == 9)
        {
            if (IsEntityParticipateElimination(Client))
            {
                Client.AddElixir(1f);
            }
        }

        #endregion

        GameManager.Instance.CheckDeath();

        UIManager.Instance.UpdateKDA();

        if (author.IsClientEntity && HadFirstTeamKey) GameManager.Instance.PickKey(0);
        if (author.IsClientEntity && HadSecondTeamKey) GameManager.Instance.PickKey(1);
    }

    private void RemoveHP(float Amount, int Author)
    {
        if (IsDead) return;

        if (GameManager.Instance.InRound)
        {
            HitPoints -= Amount;
        }
        if (HitPoints <= 0f)
        {
            HitPoints = 0f;
            DeadProcedure(Author);
        }
    }

    private void HealHP(float Amount)
    {
        HitPoints += Amount;
        if (HitPoints > HitPointsMax)
        {
            HitPoints = HitPointsMax;
        }
    }

    public void TakeDamage(float Amount, int Author, bool isCrit)
    {
        Entity me = ClientManager.Instance.Me();

        AddHistoricDamage(Amount, Author);
        RemoveHP(Amount, Author);
        UIManager.Instance.UpdateStatsInformation();

        #region SEND_PARTICLE

        if (Amount > 0f)
        {
            GameObject obj = ObjectPooler.Instance.Spawn("damage", new Vector3(Position, 1f, -1f), Quaternion.identity);
            FloatingTextDamage floatingText = obj.GetComponent<FloatingTextDamage>();

            floatingText.Init(Amount, isCrit);
        }

        #endregion

        Debug.Log($"{ChampionName} a pris {Amount} points de dégâts en pleine face!");

        if (IsClientEntity)
        {
            ClientManager.Instance.UpdateClientStatsUI();
            ClientManager.Instance.UIColorGradientAnimationHP(Color.red);

            HandleMajoration();
            if (AlreadyOpenedClashRoyale && Samuel_U_ClashRoyale.Instance.IsClashRoyaleLaunched) GameManager.Instance.SetSamuelUlt(false);
        }

        // Give Money Elimination

        if (IsDead)
        {
            if (me.BaseChampion.ID == Author)
            {
                PlayerManager.Me.AddMoney(GameManager.Instance.KillReward);
            }
            else if (IsEntityParticipateElimination(me))
            {
                PlayerManager.Me.AddMoney(GameManager.Instance.EliminationReward);
            }
        }

    }

    public void SetLocalEffect(int Index, bool IsActive)
    {
        View.RPC("RPC_SetLocalEffect", RpcTarget.All, Index, IsActive);
    }

    private void HandleMajoration()
    {
        if (IsMajoring)
        {
            HistoricAttack attack = HistoricAttacks.Last();
            if (_MajorationMaxAttack == HistoricAttack.Null || _MajorationMaxAttack.Amount < attack.Amount)
            {
                if (_MajorationMaxAttack != HistoricAttack.Null) _MajorationMaxAttack.Author.SetLocalEffect(0, false);
                attack.Author.SetLocalEffect(0, true);
                _MajorationMaxAttack = attack;
            }
        }
    }

    public void SetNastinessAttack(float time)
    {
        SetAndSyncValue(FloatStat.NastinessAttack, time);
    }


    public void CallYassineRays(int State)
    {
        View.RPC("RPC_CallYassineRays", RpcTarget.All, State);
    }

    public void TakeHeal(float Amount)
    {
        HealHP(Amount);
        UIManager.Instance.UpdateStatsInformation();

        Debug.Log($"{ChampionName} s'est soigné de {Amount} points!");

        if (IsClientEntity)
        {
            ClientManager.Instance.UpdateClientStatsUI();
            ClientManager.Instance.UIColorGradientAnimationHP(Color.green);
        }
    }

    public void AddHistoricDamage(float Amount, int Author)
    {
        Entity EntityAuthor = ClientManager.Instance.GetEntityFromChampionID(Author);

        HistoricAttack attack = new HistoricAttack(EntityAuthor, Amount);
        HistoricAttacks.Add(attack);

        EntityStatistics.AddValue(Statistics.RECEIVED_DAMAGE, Amount);
        EntityAuthor.EntityStatistics.AddValue(Statistics.INFLICTED_DAMAGE, Amount);
    }

    public float TakeTrueDamage(float Amount, Entity Author)
    {
        View.RPC("RPC_RequestAttack", RpcTarget.All, Amount, Author.BaseChampion.ID, false, true);

        return Amount;
    }

    public void Int(float _Time)
    {
        SetAndSyncValue(FloatStat.ImmuneToCC, _Time);
        PlayerMovementParent.SetUncontrolling(_Time);
    }

    /// <summary>
    /// Heals the entity
    /// </summary>
    /// <param name="Amount"></param>
    public void Heal(float Amount)
    {
        View.RPC("RPC_RequestHeal", RpcTarget.All, Amount);
        if (Amount > 0f) ClientManager.Instance.UpdateClientStatsUI();
    }

    /// <summary>
    /// Heals the entity and proke rune
    /// </summary>
    /// <param name="Amount"></param>
    public void Heal(float Amount, Entity Author, bool ProkeToSelf)
    {
        bool self = ProkeToSelf || (this != Author);

        if (Amount > 0f && self &&(Author.Rune == 2 || Rune == 2))
        {
            Amount += Amount * 0.3f * RatioMissingHP;

            List<Entity> entities = ClientManager.Instance.NearestAllyEntitiesFrom(this, 2f);

            foreach (Entity entity in entities)
            {
                if (entity.Effects.GetEffectsWithTag("RUNE_2_ADD_AD").Count == 0)
                {
                    entity.AddAndSyncEffect(new Effect(BehaviourEffects.PercentageAttackDamage, 0.01f, 10f, false, true, "RUNE_2_ADD_AD"));
                }
            }
        }

        View.RPC("RPC_RequestHeal", RpcTarget.All, Amount);
        ClientManager.Instance.UpdateClientStatsUI();
    }

    public void SetKeyState(int Team, bool Owned)
    {
        if (Team == 0)
        {
            _HadFirstTeamKey = Owned;
        }
        else
        {
            _HadSecondTeamKey = Owned;
        }

        UpdateUIKeys();
    }

    /// <summary>
    /// Drop the key. If team = -1, drop all the keys.
    /// </summary>
    /// <param name="Team"></param>
    public void DropKey(int Team)
    {
        if (HadSecondTeamKey && Team != 0) GameManager.Instance.DropKey(1);
        if (HadFirstTeamKey && Team != 1) GameManager.Instance.DropKey(0);
    }

    public void UpdateUIKeys()
    {
        UIKeyTeamOne.SetActive(_HadFirstTeamKey);
        UIKeyTeamTwo.SetActive(_HadSecondTeamKey);
    }

    public bool SetCanUseSalto(bool Value) => _CanUseSalto = Value;
    public void DoSalto()
    {
        DidSalto = true;

        SetCanUseSalto(false);
        SetAndSyncValue(FloatStat.DodgingTime, 0.55f);

        Effect effect = new Effect(BehaviourEffects.PercentageSpeed, 0.15f, 1f, false, true, "ARNAUD_A");
        AddAndSyncEffect(effect);

        PlayerMovementParent.DoSalto();
    }

    public void AfterSalto()
    {
        if (View.IsMine)
        {
            CallSpellEffect(0);
        }
    }

    public void CleanseAllNegativeEffects()
    {
        View.RPC("RPC_CleanseAllNegativeEffects", RpcTarget.All);
    }
    public void SetLobbyStat(bool InLobby)
    {
        SetAndSyncValue(BoolStat.InLobby, InLobby);
    }

    public void SetEnemyLobbyStat(bool InLobby)
    {
        SetAndSyncValue(BoolStat.InEnemyLobby, InLobby);
    }

    public void PushBack(float Speed, Direction dir)
    {
        _PushBack(Speed, dir);
    }

    public void PushBack(float Speed)
    {
        _PushBack(Speed, _CurrentDirection == Direction.Left ? Direction.Right : Direction.Left);
    }

    private void _PushBack(float Speed, Direction dir)
    {
        int Direction = dir == global::Direction.Left ? -1 : 1;

        Vector3 v = _PlayerMovement.PlayerRigidbody.velocity;
        v.x = Speed * Direction;

        _PlayerMovement.PlayerRigidbody.velocity = v;
    }

    public void StartConsuming(int item)
    {
        StartCoroutine(Coroutine_StartConsuming(item));
    }

    IEnumerator Coroutine_StartConsuming(int item)
    {
        ItemShop _CurrentItem = ShopManager.Instance.ItemShops[item];
        SetLastItemUsed(_CurrentItem);
        //me.SetChanneling(Duration);

        if (_CurrentItem.IsDrinking)
        {
            AudioManager.PlaySound(Audio.OpenSoda);
        }
        else
        {
            AudioManager.PlaySound(Audio.Eat);
        }

        if (IsClientEntity)
        {
            CollectionManager.Instance.HideItem();

            yield return null;
            CallSpellEffect(4);
        }

    }

    /// <summary>
    /// Attack by using autoattack.
    /// </summary>
    public float Attack(Entity Author)
    {
        float MitigedValue = Author.TotalAttackDamage;

        bool hasCrit = false;

        // ORDER :
        // ANYWHERE. EFFECTS
        // 1. ADDITIONAL DAMAGE
        // 2. MULTIPLICATIVE DAMAGE
        // 3. ARMOR
        // 4. ADDITIONAL TRUE DAMAGE
        // 5. MULTIPLICATIVE TRUE DAMAGE
        // 6. IMPACT EFFECTS

        // Attack damage of the target is reduced by 5% if author rune is Medical
        if (Author.Rune == 2)
        {
            if (Effects.GetEffectsWithTag("RUNE_2_RMV_AD").Count < 3)
            {
                AddAndSyncEffect(new Effect(BehaviourEffects.PercentageAttackDamage, 0.01f, 10f, false, false, "RUNE_2_RMV_AD"));
            }
        }

        // Samuel auto-attacks reduce the cooldown of his abilites by 20% of the elixir in percentage.
        if (Author.BaseChampion.ID == 9) // Samuel
        {
            float Value = Author.ElixirValue * 0.02f;

            Author.ReduceCooldown(SpellType.Active, Value);
            Author.ReduceCooldown(SpellType.Ultimate, Value);
        }

        // If Titouan hits 3 times in a row an Entity, he marks him as his Target, then, every 3 next hits, he steals 0.01$
        if (Author.BaseChampion.ID == 11) // Titouan
        {
            if (Author.TimeHittedLastEntity != 0 && Author.TimeHittedLastEntity % 3 == 0)
            {
                if (PlayerManagerParent.Money < 0.01f)
                {
                    Effect effect = new Effect(BehaviourEffects.PercentageSpeed, 0.1f, 5f, false, true, "TITOUAN_P");
                    Author.AddAndSyncEffect(effect);
                }
                else
                {
                    // Here, Titouan steal 0.05€.
                    float moneyStole = 0.05f;
                    PlayerManagerParent.UseMoney(moneyStole);
                    Author.PlayerManagerParent.AddMoney(moneyStole);
                }

                if (Author.TitouanTarget == null && Author.LastEntityHitted == this)
                {
                    Author.SetTitouanTarget(this, false);
                }
            }

        }

        // Yassine marks enemies by Success, Yassine's allies can prok this mark to deals 5HP.
        if (Author.BaseChampion.ID == 13)
        {
            SetAndSyncValue(FloatStat.Success, 10f);
        }
        else
        {
            if (IsSuccess)
            {
                SetAndSyncValue(FloatStat.Success, 0f);
                MitigedValue += 5f;
            }
        }

        // Julien deals 4% of Max HP more damage
        if (Author.BaseChampion.ID == 5)
        {
            float _JulienMoreDamage = 0.04f * HitPointsMax;
            MitigedValue += _JulienMoreDamage;

            Author.Heal(_JulienMoreDamage * 0.25f, Author, true);

            hasCrit = true;
        }

        // Lou deals up to 16% more damage with Hair
        if (Author.BaseChampion.ID == 6)
        {
            float _LouMoreDamage = 0f;

            if (Author.UsedSpellLou > 0f)
            {
                Author.ResetSpellLou();

                SetAndSyncValue(FloatStat.StunTime, 1f);
            }

            if (Author.HasAttackPredatorTime)
            {
                _LouMoreDamage += 5f * Author.Predator;

                Author.ResetAttackPredatorTime();
                Author.ResetPredatorEffects();
            }

            _LouMoreDamage += 0.16f * Author.Hair * MitigedValue;

            MitigedValue += _LouMoreDamage;
        }

        if (Author.IsInvisible)
        {
            Author.SetAndSyncValue(FloatStat.InvisibilityTime, 0f);
            Author.RemoveEffectWithTag("LOU_U");
            GameManager.Instance.SetDefaultAmbiant();

            MitigedValue += 0.45f * MissingHP;
        }

        // Thomas and his brother deals 50% more damage
        if (Author.Brother)
        {
            if (Author.DistanceFromBrother < GameManager.Instance.DistanceFromBrotherExtraDamage)
            {
                MitigedValue *= 1.5f;
            }
        }

        // Tryharding increases Attack damage by 20%. If the target is stunned, it increases by 50%.
        if (Author.IsTryharding)
        {
            float TryhardingAmount = IsStun ? 1.7f : 1.3f;
            MitigedValue *= TryhardingAmount;
        }

        // Dalil deals 30% less damage
        if (Author.BaseChampion.ID == 1)
        {
            if (Author.Effects.GetEffectsWithTag("DALIL_P_ARMOR").Count < 4)
            {
                Author.AddAndSyncEffect(new Effect(BehaviourEffects.Armor, 0.1f, 5f, false, true, "DALIL_P_ARMOR"));
            }

            Author.AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.1f, 0.5f, false, true, "DALIL_P_SPEED"));
            MitigedValue *= 0.7f;
        }

        // Virgil when Noirrel helps him deals 25% of the friendship and the active spell cooldown is reduced by 10%.
        if (Author.IsNoirrel)
        {
            MitigedValue += 0.25f * Author.FriendshipValue;

            Author.ReduceCooldown(SpellType.Active, 0.1f);
        }

        // Armor
        MitigedValue *= (1 - TotalArmor);

        // Virgil can deals 100% of his nastiness if he proke his active.
        if (Author.HasNastinessAttack)
        {
            MitigedValue += Author.NastinessValue;
            List<Entity> allies = ClientManager.Instance.NearestAllyEntitiesFrom(this, 4f);

            foreach (Entity entity in allies)
            {
                entity.AddAndSyncEffect(new Effect(BehaviourEffects.PercentageSpeed, 0.15f, 3f, false, true, "VIRGIL_A"));
            }

            if (BaseChampion.ID == 10)
            {
                MitigedValue += Author.NastinessValue;
                SetAndSyncValue(FloatStat.StunTime, 2f);
            }

            Author.SetAndSyncValue(FloatStat.NastinessAttack, 0f);

            hasCrit = true;
        }

        // Fat reduces damage by 10%. Every use of fat reduce the fat of 1.
        if (FatValue > 0)
        {
            MitigedValue *= 0.9f;
            SetAndSyncValue(FloatStat.Fat, FatValue - 1);
        }

        // Crit attack deals 70% true more damage
        if (Author.HasCritAttack)
        {
            MitigedValue *= 1.7f;
            Author.SetAndSyncValue(FloatStat.CritAttack, 0f);

            hasCrit = true;
        }

        // Entity marked with Catch Up deals less damage
        if (Author.IsCatchUp)
        {
            if (BaseChampion.ID == 8) MitigedValue *= 0.9f;
            else MitigedValue *= 0.95f;
        }

        // 3 virus executes the entity.
        if (VirusCount >= 3)
        {
            MitigedValue += HitPointsMax;
            hasCrit = true;
        }

        // When all effects applied, dodging attacks cancel damages.
        if (IsDodging) MitigedValue = 0f;

        // Virgil gains friendship equivalent to 5% + 3% per near allies of total damage applied.
        if (Author.BaseChampion.ID == 12)
        {
            float percentage = 0.05f;

            List<Entity> allies = new (ClientManager.Instance.NearestAllyEntitiesFrom(this, 4f));
            if (allies.Contains(Author)) allies.Remove(Author);
            
            foreach (Entity entity in allies)
            {
                if (entity.BaseChampion.ID == 13)
                {
                    percentage += 0.10f;
                }
                else
                {
                    percentage += 0.03f;
                }
            }

            float friendShip = MitigedValue * percentage;

            Author.SetAndSyncValue(FloatStat.Friendship, Author.FriendshipValue + friendShip);
        }

        View.RPC("RPC_RequestAttack", RpcTarget.All, MitigedValue, Author.BaseChampion.ID, true, hasCrit);

        return MitigedValue;
    }

    /// <summary>
    /// Attack by using spell.
    /// </summary>
    public float Attack(float Value, Entity Author)
    {
        float MitigedValue;

        bool hasCrit = false;

        MitigedValue = Value * (1 - TotalArmor);
        MitigedValue = Mathf.Clamp(MitigedValue, 0f, float.PositiveInfinity);

        // Entity marked with Catch Up deals less damage
        if (Author.IsCatchUp)
        {
            if (BaseChampion.ID == 8) MitigedValue *= 0.9f;
            else MitigedValue *= 0.95f;
        }

        // Crit attack deals 70% true more damage
        if (Author.HasCritAttack)
        {
            MitigedValue *= 1.7f;
            Author.SetAndSyncValue(FloatStat.CritAttack, 0f);

            hasCrit = true;
        }

        // Civil rune deals 2% max HP and heals 60% of the damage.
        if (Author.Rune == 1)
        {
            if (Hall != Author.Hall || Mathf.Abs(Position - Author.Position) > 5f)
            {
                float runeDamage = 0.02f * HitPointsMax;
                MitigedValue += runeDamage;
                Author.Heal(runeDamage * 0.6f);
            }
        }

        View.RPC("RPC_RequestAttack", RpcTarget.All, MitigedValue, Author.BaseChampion.ID, false, hasCrit);

        return MitigedValue;
    }

    public void UseAutoAttack(bool Direction)
    {
        if (IsClientEntity && IsAbleToControl && CanAttack)
        {
            bool hitSomeone = false;

            if (EntityAttackType == AttackType.Melee)
            {


                if (Direction) // Right
                {
                    hitSomeone = RightAttackPoint.HitAttack();
                }
                else
                {
                    hitSomeone = LeftAttackPoint.HitAttack();
                }
            }

            if (hitSomeone)
            {
                _AttackTimeRemaining = TotalAttackSpeed;
                UIManager.Instance.ShowAttackSpeedIndicator(TotalAttackSpeed);
            }
        }
    }

    private void ChangeFloatValue(FloatStat stat, float Value)
    {
        Debug.Log($"Value {stat} changed for {ChampionName} to {Value}");
        switch (stat)
        {
            case FloatStat.ElixirValue:
                _ElixirValue = Value;
                break;
            case FloatStat.HPBonus:
                _HPBonus += Value;
                HandleSize();
                break;
            case FloatStat.Friendship:
                _FriendshipValue = Value;
                break;
            case FloatStat.StunTime:
                if (!IsImmuneToCC)
                {
                    _StunTime = Value;
                    if (Value > 0f)
                    {
                        AudioManager.PlaySound(Audio.Stun, transform.position, 5f);
                    }
                }
                if (Value > 0f && IsClientEntity)
                {
                    PlayerMovementParent.StopMovements();
                    SetChanneling(false);
                }
                break;
            case FloatStat.DodgingTime:
                _DodgingTime = Value;
                break;
            case FloatStat.ControlledTime:
                _ControlledTime = Value;
                _InternalFlaw = 0f;
                break;
            case FloatStat.Tryharding:
                _TryhardingTime = Value;
                break;
            case FloatStat.ImmuneToCC:
                _ImmuneToCC = Value;
                break;
            case FloatStat.Majoration:
                _MajorationTime = Value;
                break;
            case FloatStat.Channeling:
                _ChannelingTime = Value;
                if (Value == 0f)
                {
                    if (GameManager.Instance.IsState(GameState.EndRound)) _ChannelingTime = 9999f;
                    DesactivateAllSpellsEffects();
                }
                break;
            case FloatStat.Delta:
                _Delta = Value;
                break;
            case FloatStat.CritAttack:
                _CritAttack = Value;
                break;
            case FloatStat.AttackPredatorTime:
                _AttackPredatorTime = Value;
                break;
            case FloatStat.InvisibilityTime:
                _InvisibilityTime = Value;
                if (Value == 0f)
                {
                    CanvasGroupUI.alpha = 1;
                    SetDeathRenderer(!IsDead);
                }
                break;
            case FloatStat.Success:
                _Success = Value;
                SetLocalEffect(1, Value > 0f);
                break;
            case FloatStat.Fat:
                _FatValue = Value;
                break;
            case FloatStat.NastinessAttack:
                _NastinessAttack = Value;
                break;
            case FloatStat.Noirrel:
                _NoirrelTime = Value;
                break;
            case FloatStat.Target:
                _TargetTime = Value;
                if (IsClientEntity)
                {
                    SetLocalEffect(2, _TargetTime > 0f);
                }
                break;
        }
    }

    private void ChangeIntValue(IntStat stat, int Value)
    {
        switch (stat)
        {
            case IntStat.Virus:
                _VirusCount = Value;
                if (Value >= 3 && IsClientEntity)
                {
                    UIManager.Instance.ShowGeneralInfo(GameManager.Instance.VirusCountMaxMessage, 5f);
                    AudioManager.PlaySound(Audio.VirusMaxCount);
                }
                break;
            case IntStat.Controller:
                _Controller = Value;
                break;
        }
    }

    private void ChangeBoolValue(BoolStat stat, bool Value)
    {
        switch (stat)
        {
            case BoolStat.InLobby:
                _InLobby = Value;
                GameManager.Instance.CheckDoors(PlayerManagerParent.Team);
                break;
            case BoolStat.InEnemyLobby:
                _InEnemyLobby = Value;

                if (IsClientEntity && Value)
                {
                    GameManager.Instance.CheckKeyEnemyLobby();
                }
                break;
            case BoolStat.CatchUp:
                _CatchUp = Value;
                if (IsClientEntity && _CatchUp)
                {
                    AudioManager.PlaySound(Audio.CatchUp);
                    UIManager.Instance.ShowGeneralInfo(GameManager.Instance.CatchUpMessage);
                }

                break;
        }
    }

    /// <summary>
    /// Sync a float value for the other clients.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="Value"></param>
    public void SetAndSyncValue(FloatStat stat, float Value)
    {
        View.RPC("RPC_SetAndSyncFloatValue", RpcTarget.All, (int)stat, Value);
    }

    /// <summary>
    /// Sync a int value for the other clients.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="Value"></param>
    public void SetAndSyncValue(IntStat stat, int Value)
    {
        View.RPC("RPC_SetAndSyncIntValue", RpcTarget.All, (int)stat, Value);
    }

    /// <summary>
    /// Sync a int value for the other clients.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="Value"></param>
    public void SetAndSyncValue(BoolStat stat, bool Value)
    {
        View.RPC("RPC_SetAndSyncBoolValue", RpcTarget.All, (int)stat, Value);
    }

    public void AddAndSyncEffect(Effect effect, bool Erase = false)
    {
        View.RPC("RPC_AddAndSyncEffect", RpcTarget.All,
            (int)effect.BEffect,
            effect.Time + (float)PhotonNetwork.Time,
            effect.Value,
            effect.Persistant,
            effect.IsPositiveEffect,
            Erase);
    }

    public void RemoveEffectWithTag(string tag)
    {
        View.RPC("RPC_RemoveEffectWithTag", RpcTarget.All,
            tag);
    }

    public void PreventUltimeStolled()
    {
        View.RPC("RPC_PreventUltimeStolled", RpcTarget.All);
    }

    //   ------------------------------------------------ View -----------------------------------------------------

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_HadFirstTeamKey);
            stream.SendNext(_HadSecondTeamKey);
        }
        else
        {
            _HadFirstTeamKey = (bool)stream.ReceiveNext();
            _HadSecondTeamKey = (bool)stream.ReceiveNext();

            UpdateUIKeys();
        }
    }

    //   ------------------------------------------------ RPC ------------------------------------------------------

    [PunRPC]
    private void RPC_PreventUltimeStolled()
    {
        if (IsClientEntity)
        {
            AudioManager.PlaySound(Audio.UltimeStole);
            UIManager.Instance.ShowGeneralInfo(GameManager.Instance.UltimeStoleMessage);

            _UltimateSpellRemaining = Mathf.Min(BaseChampion.UltimateAbility.Cooldown, _UltimateSpellRemaining + 20f);
        }
    }

    [PunRPC]
    private void RPC_RemoveEffectWithTag(string tag)
    {
        Effects.RemoveEffectsWithTag(tag);
    }

    [PunRPC]
    private void RPC_InitRound(int round)
    {
        InitRound(round);
    }

    [PunRPC]
    private void RPC_SetLastItemUsed(int id)
    {
        ItemShop item = ShopManager.Instance.ItemShops[id];
        if (item) _LastItemUsed = item;
    }

    [PunRPC]
    private void RPC_SetLocalEffect(int index, bool active)
    {
        LocalEffects[index].SetActive(active);
    }

    [PunRPC]
    private void RPC_RequestAttack(float Value, int Author, bool IsAutoAttack, bool isCrit)
    {
        TakeDamage(Value, Author, isCrit);

        if (IsAutoAttack)
        {
            HitPointsEffect.Hit();
        }
    }

    [PunRPC]
    private void RPC_RequestHeal(float Value)
    {
        TakeHeal(Value);
    }

    [PunRPC]
    private void RPC_SetAndSyncFloatValue(int Index ,float Value)
    {
        ChangeFloatValue((FloatStat)Index, Value);
    }

    [PunRPC]
    private void RPC_SetAndSyncIntValue(int Index, int Value)
    {
        ChangeIntValue((IntStat)Index, Value);
    }

    [PunRPC]
    private void RPC_SetAndSyncBoolValue(int Index, bool Value)
    {
        ChangeBoolValue((BoolStat)Index, Value);
    }

    [PunRPC]
    private void RPC_AddAndSyncEffect(int effect_id, float timer, float value, bool persistant, bool peffect, bool Erase)
    {
        Effect effect = new Effect((BehaviourEffects)effect_id, value, timer - (float)PhotonNetwork.Time, persistant, peffect);
        if (Erase) Effects.ClearAllEffects((BehaviourEffects)effect_id);
        Effects.AddEffect(effect);
        Debug.Log($"Effet {effect.BEffect} ajouté");
    }

    [PunRPC]
    private void RPC_CallSpellEffect(int Index)
    {
        SpellsEffects[Index].gameObject.SetActive(true);
        SpellsEffects[Index].OnCalled();
    }

    [PunRPC]
    private void RPC_CleanseAllNegativeEffects()
    {
        Effects.PurgeAllNegativeEffects();
    }

    [PunRPC]
    private void RPC_SetHall(int newHall)
    {
        _Hall = newHall;
    }

    [PunRPC]
    private void RPC_CallYassineRays(int state)
    {
        Yassine_AU_Rays.Instance.SetState(state);
        if (IsClientEntity) CallSpellEffect(5);
    }

    [PunRPC]
    private void RPC_SetDirection(int dir)
    {
        _CurrentDirection = (Direction)dir;
    }
}

public enum AttackType
{
    Melee,
    Range
}

public enum Attack
{
    Autoattack,
    Spell
}

public enum FloatStat
{
    StunTime,
    ElixirValue,
    Friendship,
    HPBonus,
    DodgingTime,
    ControlledTime,
    Tryharding,
    ImmuneToCC,
    Majoration,
    Channeling,
    Delta,
    CritAttack,
    AttackPredatorTime,
    InvisibilityTime,
    Success,
    Fat,
    NastinessAttack,
    Noirrel,
    Target
}

public enum IntStat
{
    Virus,
    Controller
}

public enum BoolStat
{
    InLobby,
    InEnemyLobby,
    CatchUp
}

public enum Direction
{
    Null,
    Left,
    Right
}

public struct HistoricAttack
{
    public Entity Author;
    public float Amount;

    /// <summary>
    /// None attack
    /// </summary>
    public static HistoricAttack Null => new HistoricAttack(null, 0f);

    public HistoricAttack(Entity author, float amount)
    {
        Author = author;
        Amount = amount;
    }

    public static bool operator ==(HistoricAttack a, HistoricAttack b) => a.Amount==b.Amount && a.Author == b.Author;
    public static bool operator !=(HistoricAttack a, HistoricAttack b) => !(a == b);

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return base.ToString();
    }
}