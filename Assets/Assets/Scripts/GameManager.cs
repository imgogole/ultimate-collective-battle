using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class GameManager : MonoBehaviourPun
{
    public PhotonView View;

    public List<Vector2> Halls;

    public List<Champion> ChampionsList = new List<Champion>();
    public List<Transform> SpawnPointsFirstTeam = new List<Transform>();
    public List<Transform> SpawnPointsSecondTeam = new List<Transform>();

    public List<AnimationDoor> Doors;
    public Key FirstTeamKey;
    public Key SecondTeamKey;
    public Transform FirstTeamKeySpawn;
    public Transform SecondTeamKeySpawn;
    public Transform ClosedRoomSpawn;
    public HallShower hallShower;

    GameState _State;
    
    public GameState CurrentGameState => _State;

    public CanvasGroup DescriptionRoundCanvasGroup;
    public TMP_Text DescriptionRoundText;
    public AmbiantManager GameAmbiantManager;
    public RuneManager runeManager;

    public List<Stairs> GameStairs = new List<Stairs>();
    public List<PowerUps> GamePowerUps = new List<PowerUps>();

    [Header("Champions specials")]
    public GameObject elixirBar;
    public GameObject elixirValueImage;
    public Samuel_U_ClashRoyale samuelU;
    public GameObject samuelClashRoyale;
    public GameObject BrotherLink;
    public GameObject lagPoint;

    public List<Yassine_U_Tower> yassineTowers;

    [Header("Rules")]

    public float RoundTime;

    public float ElixirRate;

    public float DistanceFromBrotherExtraDamage;

    public float MaximumBalance;
    public float InitialBalance;

    public float MaxRangeStairs;
    public float CooldownStairs;

    public float SyncTime;

    public float KillReward;
    public float EliminationReward;
    public float WinReward;
    public float LooseReward;
    public float AceReward;

    public float StartRoundDuration;

    [Header("Messages")]

    public string DescriptionRoundMessage;
    public string BeginRoundMessage;
    public string MinimalRoundUltimateMessage;
    public string AbilityReadyNotUsableMessage;
    public string GetAllyKeyMessage;
    public string GetEnemyKeyMessage;
    public string NotInRangeStairs;
    public string OnCooldownStairs;
    public string EnemyDoorOpened;
    public string AllyDoorOpened;
    public string ThirtySeconds;
    public string EndRoundMessage;
    public string ChooseBrotherContextMessage;
    public string ChooseCatchUpContextMessage;
    public string ChooseBrotherTitouanContextMessage;
    public string ChooseCatchUpTitouanContextMessage;
    public string CatchUpMessage;
    public string CantUseItemOutRoundMessage;
    public string NoItemMessage;
    public string AlreadyUsingItemMessage;
    public string ClosedRoomGoodAnswer;
    public string ClosedRoomWrongAnswer;
    public string VirusCountMaxMessage;
    public string BrotherMessage;
    public string UltimeStoleMessage;

    private int _FirstTeamScore;
    private int _SecondTeamScore;
    private int _AceTeamScore;

    private bool _IsEntityMustFreeze;
    public bool IsEntityMustFreeze => _IsEntityMustFreeze;

    public int FirstTeamScore => _FirstTeamScore;
    public int SecondTeamScore => _SecondTeamScore;
    public int AceTeamScore => _AceTeamScore;

    private float _GameTimer;
    public float GameTimer => _GameTimer;
    public bool GameTimerConcluded => _GameTimer <= 0f;

    private int _Round;
    public int CurrentRound => _Round;

    public int MinimumRoundUltimate;


    private bool _LockedFirstDoor = false;

    private bool _LockedSecondDoor = false;

    public bool LockedFirstDoor => _LockedFirstDoor;

    public bool LockedSecondDoor => _LockedSecondDoor;

    Entity me;
    public Entity Me => me;

    /// <summary>
    /// Checks if the round begins, this allows players to fight and uses abilities.
    /// </summary>
    public bool InRound => _State == GameState.InRound;

    private bool _IsThirtySecondsAnnonced = false;

    Vector3 _SpawnPoint;
    public Vector3 SpawnPoint => _SpawnPoint;

    List<Arnaud_U_Crepe> _Crepes = new List<Arnaud_U_Crepe>();


    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    public Vector3 TeamSpawnOutLobby
    {
        get
        {
            if (PlayerManager.Me.Team == 0) return TeamASpawnOutLobby;
            if (PlayerManager.Me.Team == 1) return TeamBSpawnOutLobby;
            return Vector3.zero;
        }
    }

    public Vector3 TeamASpawnOutLobby
    {
        get
        {
            return FirstTeamKeySpawn.position;
        }
    }

    public Vector3 TeamBSpawnOutLobby
    {
        get
        {
            return SecondTeamKeySpawn.position;
        }
    }

    public void SetState(GameState State)
    {
        _State = State;
    }

    public bool IsState(GameState state)
    {
        return _State == state;
    }

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        ReliableUIManager.Instance.SetLoadingState(false);
        SpecialSelectionManager.Instance.ClosePanel();
        SetState(GameState.WaitingForEveryoneSpawned);
        SetDescriptionRoundCanvasGroup(0);
        SetEntityFreeze(false);
    }

    public void InitSpawnPoint(Vector3 point)
    {
        _SpawnPoint = point;
    }

    /// <summary>
    /// Determine if the client can do some actions like moving, use spells or stairs.
    /// </summary>
    /// <param name="isFreeze"></param>
    public void SetEntityFreeze(bool isFreeze)
    {
        _IsEntityMustFreeze = isFreeze;
    }

    public Champion GetChampion(int Index)
    {
        return ChampionsList[Index];
    }

    private void Update()
    {
        // Handle parameters
        HandleTimer();
        HandleGameState();

        if (Input.GetKeyDown(KeyCode.U) && PhotonNetwork.IsMasterClient)
        {
            int round = GetLobbyState(true);
            if (round != -1)
            {
                List<ModalButton> buttons = new List<ModalButton>
                {
                    new ModalButton("Ok.", null),
                };

                PopUpShower.Show("Réussite", "Vous avez forcer la vérification des portes.", buttons.ToArray());

                SendEndRound(round);
            }
        }
    }

    public void SetYassineActive(bool isActive)
    {
        View.RPC("RPC_SetYassineActive", RpcTarget.All, isActive);
    }

    public void ResetPowersUps()
    {
        foreach (PowerUps powerUp in GamePowerUps) powerUp.ResetPowerUp();
    }

    public Stairs GetStairs(int ID)
    {
        return GameStairs[ID];
    }

    private void HandleGameState()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (IsState(GameState.WaitingForEveryoneSpawned))
        {
            if (IsEveryoneSpawned())
            {
                ChangeGameStateToEveryone(0);
            }
        }
        if (IsState(GameState.WaitingForStartingRound))
        {
            if (GameTimerConcluded)
            {
                ChangeGameStateToEveryone(1);
                //Init_StartRound();
            }
        }
        if (IsState(GameState.InRound))
        {
            if (GameTimer <= 30f && !_IsThirtySecondsAnnonced)
            {
                ChangeGameStateToEveryone(2);

            }
            if (GameTimerConcluded)
            {
                SendEndRound(GetLobbyState(true));
                SetState(GameState.EndRound);
            }
        }
        if (IsState(GameState.EndRound))
        {
            if (GameTimerConcluded)
            {
                ChangeGameStateToEveryone(3);
            }
        }

        if (_SyncTime <= 0f)
        {
            SyncGameTime();
            _SyncTime = SyncTime;
        }
        _SyncTime -= Time.deltaTime;

    }

    float _SyncTime = 0f;

    private void ChangeGameStateToEveryone(int Index)
    {
        View.RPC("RPC_ChangeGameStateToEveryone", RpcTarget.All, Index, PhotonNetwork.Time);
    }

    public void ReloadGame(double Latence)
    {
        SetRound(CurrentRound + 1);
        Init_WaitingForStartingRound(false, Latence);
        CloseDoors();
    }

    public void EndRound(int value)
    {
        SetState(GameState.EndRound);
        ClearMap();
        ShowDescriptionRound(GetEndRoundMessage(value));
        SetGameTimer(7f);
        me.SetChanneling(true);
        CheckWinners(value);
        GameLobbyManager.Instance.SetChecking(false);
        Romain_U_DSMaths.Instance.ClosePaper();
        SetSamuelUlt(false);
        GameAnnoncerManager.Instance.HideAndClear();
        if (PhotonNetwork.IsMasterClient) SetYassineActive(false);

        int Team = me.Team;

        if (value == 2)
        {
            PlayerManager.Me.AddMoney(AceReward);
        }
        else if (Team == value)
        {
            PlayerManager.Me.AddMoney(WinReward);
        }
        else
        {
            PlayerManager.Me.AddMoney(LooseReward);
        }
    }

    private string GetEndRoundMessage(int Winner)
    {
        bool isClientWinner = me.Team == Winner;
        string message = EndRoundMessage + " ";

        if (Winner == 2) message += "<color=#3d86e0>Ace</color>";
        else if (isClientWinner) message += "<color=#70ff45>Victoire</color>";
        else message += "<color=#ff4019>Défaite</color>";

        return message;
    }

    public void ClearMap()
    {
        // Destroy All LeMettayers
        List<GameObject> leMettayer = new List<GameObject>(GameObject.FindGameObjectsWithTag("LeMettayer"));
        foreach (GameObject obj in leMettayer)
        {
            Romain_U_LeMettayerBot bot = obj.GetComponent<Romain_U_LeMettayerBot>();
            if (bot) bot.DestroyLeMettayer();
        }
    }

    public void CheckWinners(int value)
    {
        AddPoint(value);
    }

    public int GetLobbyState(bool IsEndRound)
    {
        int AmountInLobbyTeamA = 0;
        int AmountInLobbyTeamB = 0;

        float RatioA = 0f;
        float RatioB = 0f;

        int Result;

        if (!LockedSecondDoor)
        {
            List<PlayerManager> teamAManagers = new List<PlayerManager>(OnlineManager.Instance.GetPlayerManagersOfTeam(0));
            teamAManagers.RemoveAll(p => p.OwnedEntity.IsDead);

            if (teamAManagers.Count != 0)
            {
                foreach (PlayerManager pManager in teamAManagers)
                {
                    if (pManager.OwnedEntity.InEnemyLobby) AmountInLobbyTeamA += 1;
                }

                RatioA = (float)AmountInLobbyTeamA / teamAManagers.Count;
            }
        }

        if (!LockedFirstDoor)
        {
            List<PlayerManager> teamBManagers = new List<PlayerManager>(OnlineManager.Instance.GetPlayerManagersOfTeam(1));
            teamBManagers.RemoveAll(p => p.OwnedEntity.IsDead);

            if (teamBManagers.Count != 0)
            {
                foreach (PlayerManager pManager in teamBManagers)
                {
                    if (pManager.OwnedEntity.InEnemyLobby) AmountInLobbyTeamB += 1;
                }

                RatioB = (float)AmountInLobbyTeamB / teamBManagers.Count;
            }
        }

        if (IsEndRound)
        {
            if (RatioA > RatioB) Result = 0;
            else if (RatioB > RatioA) Result = 1;
            else Result = 2;
        }
        else
        {
            if (RatioA == 1f) Result = 0;
            else if (RatioB == 1f) Result = 1;
            else Result = -1;     
        }
        return Result;
    }

    public void ForceStairsCooldown()
    {
        foreach (Stairs stair in GameStairs)
        {
            stair.ForceCooldown(CooldownStairs);
        }
    }

    public void DestroyKey(int Team)
    {
        if (Team == 0) _LockedFirstDoor = false;
        else _LockedSecondDoor = false;

        // View.RPC("RPC_DestroyKey", RpcTarget.All, Team);
    }

    public void SendDestroyKey(int Team)
    {
        View.RPC("RPC_SendDestroyKey", RpcTarget.All, Team, me.BaseChampion.ID);
    }

    public void SetSamuelUlt(bool active)
    {
        me.SetChanneling(active);
        samuelClashRoyale.SetActive(active);
        if (active) samuelU.Init();
    }

    public void UseItem(int Item)
    {
        View.RPC("RPC_UseItem", RpcTarget.All, Item, me.BaseChampion.ID);
    }

    /// <summary>
    /// This scripts is executed when an entity is approching the enemy lobby. If this entity owns the key of this lobby, the door will be opened.
    /// </summary>
    /// <param name="author"></param>
    public void CheckKeyEnemyLobby()
    {
        if (me.Team == 0)
        {
            if (me.HadSecondTeamKey)
            {
                SendDestroyKey(0);
            }
        }
        else
        {
            if (me.HadFirstTeamKey)
            {
                SendDestroyKey(1);
            }
        }

        CheckEndRound();
    }

    /// <summary>
    /// Checks if the round must be ended.
    /// </summary>
    public void CheckEndRound()
    {
        int lobbyState = GetLobbyState(false);

        if (IsState(GameState.InRound) && lobbyState != -1)
        {
            SendEndRound(lobbyState);
        }
    }

    /// <summary>
    /// Play a sound to everyone.
    /// </summary>
    /// <param name="audio"></param>
    public void PlaySoundThroughServer(Audio audio)
    {
        View.RPC("RPC_PlaySound", RpcTarget.All, (int)audio);
    }

    private void SendEndRound(int value)
    {
        View.RPC("RPC_SendEndRound", RpcTarget.All, value);
    }

    /// <summary>
    /// Checks if every players of the Team is out of the lobby. In this case, the door will be closed and locked for the rest of the round.
    /// </summary>
    /// <param name="Team"></param>
    public void CheckDoors(int Team)
    {
        if (Team == 0 && LockedFirstDoor) return;
        if (Team == 1 && LockedSecondDoor) return;

        List<PlayerManager> playerManagers = OnlineManager.Instance.GetPlayerManagersOfTeam(Team);
        bool IsEveryoneOutOfLobby = true;
        foreach (PlayerManager pManager in playerManagers)
        {
            if (pManager.OwnedEntity.InLobby) IsEveryoneOutOfLobby = false;
        }
        if (IsEveryoneOutOfLobby)
        {
            CloseDoor(Team);
            if (Team == 0) _LockedFirstDoor = true;
            if (Team == 1) _LockedSecondDoor = true;
        }
    }

    /// <summary>
    /// Handle if the timer must be decrease or not in the game
    /// </summary>
    private void HandleTimer()
    {
        float lastTime = _GameTimer;
        _GameTimer -= Time.deltaTime;

        _GameTimer = Mathf.Max(0, _GameTimer);

        if (IsState(GameState.WaitingForStartingRound) && lastTime >= 4f && _GameTimer <= 4f)
        {
            AudioManager.PlaySound(Audio.Countdown);
        }
    }

    /// <summary>
    /// Set the timer of the game, printed in the middle center of the UI.
    /// </summary>
    /// <param name="_Time"></param>
    public void SetGameTimer(float _Time)
    {
        _GameTimer = _Time;
    }

    /// <summary>
    /// Set the round of the game.
    /// </summary>
    /// <param name="round"></param>
    public void SetRound(int round)
    {
        _Round = round;
    }

    private void Init_WaitingForStartingRound(bool FirstTime, double Latence)
    {
        if (FirstTime)
        {
            SetRound(1);
        }
        ClientManager.Instance.InitEntities();
        if (FirstTime)
        {
            ReliableUIManager.Instance.FadeBlack(true);
            InitGame();
            runeManager.Init(me);
            UIManager.Instance.Init(me);
        }
        Me.Init(CurrentRound);
        SetState(GameState.WaitingForStartingRound);
        ForceCloseDoor();
        Me.SetChanneling(false);
        if (FirstTime)
        {
            GameLobbyManager.Instance.Init();
            PlayerManager.Me.SetInitialBalance();
        }
        GameLobbyManager.Instance.SetChecking(true);
        GameAmbiantManager.SetTarget(AmbiantGoal.Default);
        SetGameTimer((FirstTime ? StartRoundDuration * 2f : StartRoundDuration) - (float)Latence);
        ShowDescriptionRound(DescriptionRoundMessage);
        InitKeys();
        SetSamuelUlt(false);
        Me.Teleport(SpawnPoint);
        ResetPowersUps();
        CheckElixir();
        ResetCrepes();
        HideLagPoint();
        Me.ClearLagPoint();
        Romain_U_DSMaths.Instance.ClosePaper();
        CameraFollowing.Instance.SetSpectating(PlayerManager.Me.Team == 2);
        if (PhotonNetwork.IsMasterClient)
        {
            SetYassineActive(ClientManager.Instance.CurrentEntities.Any(e => e.BaseChampion.ID == 13));
        }
    }

    private void ResetCrepes()
    {
        List<Arnaud_U_Crepe> crepes = new List<Arnaud_U_Crepe>(FindObjectsOfType<Arnaud_U_Crepe>());
        foreach (Arnaud_U_Crepe crepe in crepes)
        {
            crepe.DestroyCrepe();
        }

        _Crepes = new List<Arnaud_U_Crepe>();
    }

    private void CheckElixir()
    {
        bool IsSamuel = me.BaseChampion.ID == 9;
        elixirBar.SetActive(IsSamuel);
        elixirValueImage.SetActive(IsSamuel);

        if (IsSamuel)
        {
            ElixirBar.Instance.Init(me);
        }
    }

    public void HideLagPoint()
    {
        lagPoint.SetActive(false);
    }

    public void ShowLagPoint(float pos)
    {
        View.RPC("RPC_ShowLagPoint", RpcTarget.All, pos);
    }

   public void CheckSpecialSelection()
    {
        int ID = me.BaseChampion.ID;
        if (ID == 8) // Romain
        {
            SpecialSelectionManager.Instance.OpenPanel(SelectionContext.ChooseCatchUp, false);
        }
        else if (ID == 10) // Thomas
        {
            if (me.Brother == null)
            {
                SpecialSelectionManager.Instance.OpenPanel(SelectionContext.ChooseBrother, false);
            }
        }
        else if (ID == 11) // Titouan
        {
            if (ClientManager.Instance.IsEnemyExists(8)) // romain
            {
                SpecialSelectionManager.Instance.OpenPanel(SelectionContext.ChooseCatchUp, true);
            }
            else if (ClientManager.Instance.IsEnemyExists(10)) //thomas
            {
                SpecialSelectionManager.Instance.OpenPanel(SelectionContext.ChooseBrother, true);
            }
            else
            {
                SpecialSelectionManager.Instance.ClosePanel();
            }
        }
        else
        {
            SpecialSelectionManager.Instance.ClosePanel();
        }
    }

    /// <summary>
    /// Checks whenever every entities are dead, in this case, the round ends with Ace.
    /// </summary>
    public void CheckDeath()
    {
        List<Entity> entities = ClientManager.Instance.CurrentEntities;

        if(! entities.Any(e => !e.IsDead))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SendEndRound(2);
            }
        }
    }

    public void StartLouUltimate()
    {
        View.RPC("RPC_StartLouUltimate", RpcTarget.All);
    }

    private void ForceCloseDoor()
    {
        foreach (AnimationDoor door in Doors) door.ForceClosing();
    }

    private void Init_StartRound()
    {
        ClientManager.Instance.InitEntities();
        SetState(GameState.InRound);
        SetGameTimer(RoundTime);
        OpenDoors();
        ShowDescriptionRound(string.Format(BeginRoundMessage, CurrentRound));
        _IsThirtySecondsAnnonced = false;
    }

    public Transform GetSpawnPoint(PlayerManager playerManager)
    {
        List<int> IDs = OnlineManager.Instance.GetPlayerManagersOfTeam(playerManager.Team).Select(p => p.ChampionSelected).ToList();
        IDs.Sort();
        int IndexOf = IDs.IndexOf(playerManager.ChampionSelected);
        return playerManager.Team == 0 ? SpawnPointsFirstTeam[IndexOf] : SpawnPointsSecondTeam[IndexOf];
    }

    private void InitGame()
    {
        Debug.Log("Init Game");
        me = ClientManager.Instance.Me();
        CollectionManager.Instance.Init();
        hallShower.Init(me);
        RemoveCollisions();
        //SetShopManagers();
        //ClientManager.Instance.InitEntities();
    }

    public void RemoveCollisions()
    {
        List<Entity> entities = ClientManager.Instance.CurrentEntities;
        foreach (Entity entity in entities)
        {
            if (me.BaseChampion.ID != entity.BaseChampion.ID)
            {
                Physics.IgnoreCollision(entity.EntityCollider, me.EntityCollider);
            }
        }
    }

    public void AddCrepe(Arnaud_U_Crepe crepe)
    {
        _Crepes.Add(crepe);
    }

    /// <summary>
    /// Link and prevent everyone that both entities become brothers
    /// </summary>
    /// <param name="firstEntity"></param>
    /// <param name=""></param>
    public void LinkEntitiesAsBrother(Entity firstEntity, Entity secondEntity)
    {
        View.RPC("RPC_LinkEntitiesAsBrother", RpcTarget.All, firstEntity.BaseChampion.ID, secondEntity.BaseChampion.ID);
    }

    /// <summary>
    /// Checks if the amount of Entities match with the players count.
    /// </summary>
    /// <returns></returns>
    public bool IsEveryoneSpawned()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == FindObjectsOfType<Entity>().Length;
    }

    /// <summary>
    /// Open every doors with animations
    /// </summary>
    public void OpenDoors()
    {
        foreach (AnimationDoor door in Doors) door.OpenDoor();
        AudioManager.PlaySound(Audio.DoorOpened);

        _LockedFirstDoor = false;
        _LockedSecondDoor = false;
    }

    /// <summary>
    /// Close every doors with animations
    /// </summary>
    public void CloseDoors()
    {
        foreach (AnimationDoor door in Doors) door.CloseDoor();
    }

    /// <summary>
    /// Close the specific door with animations
    /// </summary>
    public void CloseDoor(int Team)
    {
        Doors[Team].CloseDoor();
        AudioManager.PlaySound(Audio.DoorClosed, Doors[Team].transform.position, 5f);
    }

    /// <summary>
    /// Open the specific door with animations
    /// </summary>
    public void OpenDoor(int Team)
    {
        Doors[Team].OpenDoor();
        AudioManager.PlaySound(Audio.DoorOpened, Doors[Team].transform.position, 5f);
    }

    /// <summary>
    /// Allow everyone in the room to get the exact value of GameTime as the MasterClient.
    /// </summary>
    public void SyncGameTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            View.RPC("RPC_SyncGameTime", RpcTarget.All, GameTimer, PhotonNetwork.Time);
        }
    }

    public void AddPoint(int Team)
    {
        if (Team == 0) _FirstTeamScore++;
        if (Team == 1) _SecondTeamScore++;
        if (Team == 2) _AceTeamScore++;

        UIManager.Instance.UpdateScoreUI(_FirstTeamScore, _SecondTeamScore, _AceTeamScore);
    }

    private void SetDescriptionRoundCanvasGroup(float Value)
    {
        DescriptionRoundCanvasGroup.alpha = Value;
        DescriptionRoundCanvasGroup.gameObject.SetActive(Value > 0.01f);
    }

    public void ShowDescriptionRound(string Message)
    {
        DescriptionRoundText.text = Message;
        StopAllCoroutines();
        StartCoroutine(Coroutine_AnimateDescriptionRound());
    }

    public void PickKey(int Team)
    {
        int ID =  me.BaseChampion.ID;
        UIManager.Instance.ShowGeneralInfo(me.PlayerManagerParent.Team == Team ? GetAllyKeyMessage : GetEnemyKeyMessage);
        AudioManager.PlaySound(Audio.GetKey);
        View.RPC("RPC_PickKeyOwner", RpcTarget.All, ID, Team);
    }

    public void DropKey(int Team)
    {
        int ID = me.BaseChampion.ID;
        
        AudioManager.PlaySound(Audio.GetKey);
        View.RPC("RPC_DropKey", RpcTarget.All, ID, me.transform.position.x + UnityEngine.Random.Range(-1f, 1f), Team);
    }

    /// <summary>
    /// Set keys to their spawns and dispo every entities from their keys.
    /// </summary>
    public void InitKeys()
    {
        FirstTeamKey.transform.position = TeamASpawnOutLobby;
        SecondTeamKey.transform.position = TeamBSpawnOutLobby;
        FirstTeamKey.gameObject.SetActive(true);
        SecondTeamKey.gameObject.SetActive(true);

        me.SetKeyState(0, false);
        me.SetKeyState(1, false);
    }

    IEnumerator Coroutine_AnimateDescriptionRound()
    {
        float time = 0.3f;
        SetDescriptionRoundCanvasGroup(0);
        while (time > 0f)
        {
            SetDescriptionRoundCanvasGroup(1 - time);

            time -= Time.deltaTime;
            yield return null;
        }
        SetDescriptionRoundCanvasGroup(1);
        yield return new WaitForSeconds(4f);
        time = 1f;
        while (time > 0f)
        {
            SetDescriptionRoundCanvasGroup(time);

            time -= Time.deltaTime;
            yield return null;
        }
        SetDescriptionRoundCanvasGroup(0);
    }

    IEnumerator Coroutine_StartLouUltimate()
    {
        GameAmbiantManager.SetTarget(AmbiantGoal.LouUltimate);
        AudioManager.PlaySound(Audio.LouUltimate);

        yield return new WaitForSeconds(15f);

        GameAmbiantManager.SetTarget(AmbiantGoal.Default);
    }

    public void SetDefaultAmbiant()
    {
        View.RPC("RPC_SetDefaultAmbiant", RpcTarget.All);
    }


    //   ------------------------------------------------ RPC ------------------------------------------------------

    [PunRPC]
    private void RPC_SetYassineActive(bool isActive)
    {
        foreach (Yassine_U_Tower tower in yassineTowers)
        {
            tower.gameObject.SetActive(isActive);
            tower.SetLight(false);
        }
    }

    [PunRPC]
    private void RPC_ShowLagPoint(float pos)
    {
        lagPoint.SetActive(true);
        lagPoint.transform.position = new Vector3(pos, 0f, 0f);
    }

    [PunRPC]
    private void RPC_LinkEntitiesAsBrother(int fid, int sid)
    {
        Entity firstEntity = ClientManager.Instance.GetEntityFromChampionID(fid);
        Entity secondEntity = ClientManager.Instance.GetEntityFromChampionID(sid);

        firstEntity.SetBrother(secondEntity);
        secondEntity.SetBrother(firstEntity);

        if (firstEntity.IsClientEntity)
        {
            AudioManager.PlaySound(Audio.Brother);
            UIManager.Instance.ShowGeneralInfo(string.Format(BrotherMessage, secondEntity.ChampionName));

            GameObject link = Instantiate(BrotherLink, Vector3.zero, Quaternion.identity);
            BrothersLink bLink = link.GetComponent<BrothersLink>();
            bLink.SetLink(firstEntity, secondEntity);
        }
        else if (secondEntity.IsClientEntity)
        {
            AudioManager.PlaySound(Audio.Brother);
            UIManager.Instance.ShowGeneralInfo(string.Format(BrotherMessage, firstEntity.ChampionName));

            GameObject link = Instantiate(BrotherLink, Vector3.zero, Quaternion.identity);
            BrothersLink bLink = link.GetComponent<BrothersLink>();
            bLink.SetLink(secondEntity, firstEntity);
        }
    }

    [PunRPC]
    private void RPC_UseItem(int item, int id)
    {
        Entity entity = ClientManager.Instance.GetEntityFromChampionID(id);
        if (entity)
        {
            entity.StartConsuming(item);
        }
    }

    [PunRPC]
    private void RPC_SetDefaultAmbiant()
    {
        GameAmbiantManager.SetTarget(AmbiantGoal.Default);
    }

    [PunRPC]
    private void RPC_ChangeGameStateToEveryone(int Index, double LastTime)
    {
        double Latence = PhotonNetwork.Time - LastTime;

        if (Index == 0)
        {
            Init_WaitingForStartingRound(true, Latence);
        }
        else if (Index == 1)
        {
            Init_StartRound();
        }
        else if (Index == 2)
        {
            _IsThirtySecondsAnnonced = true;
            ShowDescriptionRound(ThirtySeconds);
        }
        else if (Index == 3)
        {
            ReloadGame(Latence);
        }
    }

    [PunRPC]
    private void RPC_PlaySound(int audio)
    {
        AudioManager.PlaySound((Audio)audio);
    }

    [PunRPC]
    private void RPC_SendEndRound(int value)
    {
        EndRound(value);
    }

    [PunRPC]
    private void RPC_SendDestroyKey(int Team, int author)
    {
        DestroyKey(1 - Team);
        OpenDoor(1 - Team);
        me.SetKeyState(Team, false);

        GameAnnoncerManager.Instance.AddEvent(AnnoncementType.DoorOpened, ClientManager.Instance.GetEntityFromChampionID( author));
    }


    [PunRPC]
    private void RPC_SyncGameTime(float time, double lastTime)
    {
        double Latency = PhotonNetwork.Time - lastTime;
        _GameTimer = time - (float)Latency;
    }

    [PunRPC]
    private void RPC_DropKey( int ChampionID,float xPos, int Team)
    {
        GameObject targetKey = Team == 0 ? FirstTeamKey.gameObject : SecondTeamKey.gameObject;

        targetKey.transform.position = new Vector3(xPos, 0, 0);
        targetKey.SetActive(true);

        Entity entity = ClientManager.Instance.GetEntityFromChampionID(ChampionID);

        if (entity)
        {
            if (Team == 0 || Team == -1)
            {
                if (entity.IsClientEntity) FirstTeamKey.OnDrop();
            }
            if (Team == 1 || Team == -1)
            {
                if (entity.IsClientEntity) SecondTeamKey.OnDrop();
            }

            List<Entity> allEntities = ClientManager.Instance.CurrentEntities;

            foreach (Entity ent in allEntities)
            {
                ent.SetKeyState(Team, false);
            }
        }
    }

    [PunRPC]
    private void RPC_PickKeyOwner(int ChampionID, int Team)
    {
        View.RPC("RPC_PickKey", RpcTarget.All, ChampionID, Team);
    }

    [PunRPC]
    private void RPC_PickKey(int ChampionID, int Team)
    {
        GameObject targetKey = Team == 0 ? FirstTeamKey.gameObject : SecondTeamKey.gameObject;

        targetKey.SetActive(false);
        Entity entity = ClientManager.Instance.GetEntityFromChampionID(ChampionID);

        List<Entity> allEntities = ClientManager.Instance.CurrentEntities;

        if (entity)
        {
            foreach (Entity ent in allEntities)
            {
                ent.SetKeyState(Team, ent == entity);
            }
        }
    }

    [PunRPC]
    private void RPC_DestroyKey(int Team)
    {
        if (Team == 0) _LockedFirstDoor = false;
        if (Team == 1) _LockedSecondDoor = false;
    }

    [PunRPC]
    private void RPC_StartLouUltimate()
    {
        StartCoroutine(Coroutine_StartLouUltimate());
    }
}

public enum GameState
{
    WaitingForEveryoneSpawned,
    WaitingForStartingRound,
    InRound,
    EndRound
}
