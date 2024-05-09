using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NameField;
    public Button JoinButton;

    public TMP_Text onlinePlayers, gameVersion;

    private static RoomManager _Instance;
    public static RoomManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        MenuManager.Instance.SwitchMenu(0);
        SetSavedName();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    private void SetSavedName()
    {
        if (PlayerPrefs.HasKey("Name"))
        {
            SetName(PlayerPrefs.GetString("Name"));
        }
        else
        {
            SetName("Default");
        }
    }

    public void SaveName(string NewName)
    {
        PlayerPrefs.SetString("Name", NewName);
        SetName(NewName);
    }

    private void Update()
    {
        UpdateLobbyUI();
        onlinePlayers.text = $"Joueurs en ligne : {PhotonNetwork.CountOfPlayers}";
        gameVersion.text = $"Version de jeu : v{Application.version}";
    }

    public void SetName(string Name)
    {
        PhotonNetwork.NickName = Name;
        NameField.text = Name;
    }

    public void UpdateLobbyUI()
    {
        JoinButton.GetComponentInChildren<TMP_Text>().text = OnlineManager.IsConnected ? "Rejoindre" : "Connexion...";
        JoinButton.interactable = OnlineManager.IsConnected;
    }

    public void JoinDevServer()
    {
        RoomOptions options = new RoomOptions();
        options.PlayerTtl = 6000 * 3;
        options.MaxPlayers = 12;
        PhotonNetwork.JoinOrCreateRoom("DevServer", options, TypedLobby.Default);

        AudioManager.PlaySound(Audio.ClickButton);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        string response = GetErrorDescription(returnCode) + "\n" + message;

        List<ModalButton> buttons = new List<ModalButton>
                {
                    new ModalButton("Ok", null),
                };

        PopUpShower.Show("Connection au serveur impossible.", response, buttons.ToArray());
    }

    public void SwitchToLobbyTeamSelection()
    {
        MenuManager.Instance.SwitchMenu(1);
    }
    public void LaunchLobby()
    {
        ExitGames.Client.Photon.Hashtable options = new ExitGames.Client.Photon.Hashtable();
        options["team"] = 0;
        PhotonNetwork.SetPlayerCustomProperties(options);
        SwitchToLobbyTeamSelection();
    }

    public void SwitchToChampionSelection()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PlayerManager.Instance.SwitchToChampionSelection();
        AudioManager.PlaySound(Audio.ClickButton);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnlineManager.Instance.OnPlayerLeftProcedure();
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.SwitchMenu(0);
    }
    public string GetErrorDescription(short returnCode)
    {
        string description;

        description = returnCode switch
        {
            ErrorCode.GameDoesNotExist => "Le jeu n'existe pas.",
            ErrorCode.GameFull => "Partie complète.",
            ErrorCode.ServerFull => "Server complet.",
            ErrorCode.GameIdAlreadyExists => "L'ID du jeu existe déjà.",
            ErrorCode.JoinFailedPeerAlreadyJoined => "L'entrée a échoué, le peer a déjà rejoint.",
            ErrorCode.GameClosed => "La partie est déjà lancée.",
            _ => "Erreur inconnue",
        };
        return description;
    }
}
