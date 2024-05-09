using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Dynamic;

public class PlayerManager : MonoBehaviourPun
{
    public int ChampionSelected;
    public int RuneSelected;
    public bool ChampionConcluded;

    float _Money;
    public float Money => _Money;

    private int _Team;
    public int Team
    {
        get
        {
            return _Team;
        }
    }

    public void SetTeam(int newTeam)
    {
        view.RPC("RPC_SetTeam", RpcTarget.AllBuffered, newTeam);
    }

    public PhotonView view;

    [HideInInspector] public GameObject ControllPlayer;

    public Entity _Entity;
    public Entity OwnedEntity
    {
        get
            
        {
            return _Entity;
        }
    }

    private static PlayerManager _Me;

    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get => instance;
    }

    public static PlayerManager Me
    {
        get
        {
            return _Me;
        }
    }

    public string Name
    {
        get
        {
            return view.Owner.NickName;
        }
    }

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            instance = this;
        }
    }

    private void Start()
    {
        OnlineManager.Instance.UpdatePlayerList();
        DontDestroyOnLoad(gameObject);
        ChampionConcluded = false;

        if (view.IsMine)
        {
            _Me = this;
            SetTeam(0);
        }
    }

    public void AddMoney(float Value)
    {
        if (Value > 0)
        {
            float newValue = _Money + Value;
            if (newValue > GameManager.Instance.MaximumBalance) newValue = GameManager.Instance.MaximumBalance;
            SetMoneyValue(newValue);
            AudioManager.PlaySound(Audio.Coins);
        }
    }

    public void UseMoney(float Value)
    {
        float newValue = _Money - Value;
        if (newValue < 0f) newValue = 0f;
        SetMoneyValue(newValue);
    }

    public bool HasEnoughMoney(float Value)
    {
        return Value >= _Money;
    }

    public void SetMoney(float Value)
    {
        SetMoneyValue(Mathf.Clamp(Value, 0, GameManager.Instance.MaximumBalance));
    }

    public void SetInitialBalance()
    {
        SetMoneyValue(GameManager.Instance.InitialBalance);
    }

    private void SetMoneyValue(float Value)
    {
        view.RPC("RPC_SetMoneyValue", RpcTarget.All, Value);
    }

    private void OnDestroy()
    {
        OnlineManager.Instance.UpdatePlayerList();
    }

    public void InitializationGame()
    {
        if (view.IsMine) CreatePlayer();
    }

    public void SetOwnedEntity(Entity entity)
    {
        Debug.Log($"{entity.ChampionName} initialized.");
        _Entity = entity;
    }

    private void CreatePlayer()
    {
        Vector3 SpawnPoint = GameManager.Instance.GetSpawnPoint(Me).position;
        GameManager.Instance.InitSpawnPoint(SpawnPoint);

        GameObject player = PhotonNetwork.Instantiate(Path.Combine("Photon", "Player"), SpawnPoint, Quaternion.identity);
        ControllPlayer = player;
        player.name = "Me";
        Entity entity = player.GetComponent<Entity>();
        if (entity)
        {
            entity.PlayerManagerParent = this;
            _Entity = entity;
        }

        Debug.Log($"[Client] Player of {view.Owner.NickName} instanciated.");
    }

    public void SelectChampion(int _Champion)
    {
        ChampionSelected = _Champion;
        view.RPC("RPC_SelectChampion", RpcTarget.All, _Champion);
    }

    public void SetRune(int _Rune)
    {
        RuneSelected = _Rune;
        view.RPC("RPC_SelectRune", RpcTarget.All, _Rune);
    }

    public void LockChampion()
    {
        view.RPC("RPC_LockChampion", RpcTarget.All, ChampionSelected);
    }

    public void SwitchToChampionSelection()
    {
        view.RPC("RPC_SwitchToChampionSelection", RpcTarget.All, ChampionSelectionManager.Instance.StartTime + (float)PhotonNetwork.Time);
    }

    // RPCs

    [PunRPC]
    private void RPC_SetTeam(int newValue)
    {
        _Team = newValue;
        LobbyManager.Instance.RefreshPlayersList();
    }

    [PunRPC]
    private void RPC_SelectChampion(int _Champion)
    {
        ChampionSelected = _Champion;
    }

    [PunRPC]
    private void RPC_SelectRune(int _Rune)
    {
       RuneSelected = _Rune;
    }

    [PunRPC]
    private void RPC_LockChampion(int _Champion)
    {
        ChampionSelected = _Champion;
        ChampionConcluded = true;
        ChampionSelectionManager.Instance.LockButton(_Champion);
        ChampionSelectionManager.Instance.UpdatePickInfo();
    }

    [PunRPC]
    private void RPC_SwitchToChampionSelection(float time)
    {
        ChampionSelectionManager.Instance.Init(time - (float)PhotonNetwork.Time);
        MenuManager.Instance.SwitchMenu(2);
    }

    [PunRPC]
    private void RPC_SetMoneyValue(float Value)
    {
        _Money = Value;
        if (view.IsMine) UIManager.Instance.UpdateMoney(Value);
    }
}