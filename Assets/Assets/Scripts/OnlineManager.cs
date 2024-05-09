using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    private List<PlayerManager> Players = new List<PlayerManager>();

    private int _CurrentScene;

    private int tentativesReconnection = 0;
    public int CurrentScene
    {
        get
        {
            return _CurrentScene;
        }
    }

    public static bool IsConnected
    {
        get
        {
            return PhotonNetwork.IsConnectedAndReady;
        }
    }

    public static List<PlayerManager> PlayersOnline
    {
        get
        {
            return Instance.Players;
        }
    }

    private static OnlineManager _Instance;
    public static OnlineManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    public void UpdatePlayerList()
    {
        Players = new List<PlayerManager>(FindObjectsOfType<PlayerManager>());
        if (CurrentScene == 0)
        {
            LobbyManager.Instance.RefreshPlayersList();
        }
    }

    public static PlayerManager GetOwner(PhotonView view)
    {
        return PlayersOnline.Find(P => P.view.Owner== view.Owner);
    }

    public List<PlayerManager> GetPlayerManagersOfTeam(int Team)
    {
        return Players.FindAll(p => p.Team == Team);
    }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _Instance = this;
        }
    }

    private void Start()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 40;

        tentativesReconnection = 0;

        ConnectToServers();
    }

    public void ConnectToServers()
    {
        if (!IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion("eu");
        }
    }

    public override void OnConnectedToMaster()
    {
        if (CurrentScene == 0)
        {
            Debug.Log("You have been connected !");
            RoomManager.Instance.UpdateLobbyUI();
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("You have been connected to the lobby !");
    }

    public override void OnJoinedRoom()
    {
        RoomManager.Instance.LaunchLobby();
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("Photon", "Player Manager"), Vector3.zero, Quaternion.identity);
        player.name = "Local Player Manager";
        /*PhotonNetwork.LoadLevel(1);
        _CurrentScene = 1;*/
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerList();
        if (CurrentScene == 0)
        {
            LobbyManager.Instance.RefreshPlayersList();
        }
    }

    public void StartGame()
    {
        ReliableUIManager.Instance.SetNextMethod(delegate { StartGameProcedure(); });
        ReliableUIManager.Instance.FadeBlack(false, true);
        ReliableUIManager.Instance.StartUIProcedure();
    }

    public void StartGameProcedure()
    {
        PhotonNetwork.LoadLevel(1);
        ReliableUIManager.Instance.SetLoadingState(true);
        GameSettings.Instance.CloseSettingsPanel();
    }

    public bool AllPlayersConcluded()
    {
        return !PlayersOnline.Any(p => !p.ChampionConcluded);
    }

    private void RejoinRoom()
    {
        if (PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.Log("Reconnecting to the room...");
        }
        else
        {
            if (tentativesReconnection < 3)
            {
                List<ModalButton> buttons = new List<ModalButton>
                {
                    new ModalButton("Se reconnecter", delegate { TryReconnect(); }),
                    new ModalButton("Revenir au menu", delegate { ReloadGame(); } ),
                };

                PopUpShower.Show("Echec de reconnection", "Impossible de se reconnecter au serveur, voulez-vous retenter ?", buttons.ToArray());
            }
            else
            {
                List<ModalButton> buttons = new List<ModalButton>
                {
                    new ModalButton("Revenir au menu", delegate { ReloadGame(); } ),
                };

                PopUpShower.Show("Echec", "Impossible de se reconnecter au serveur même après 3 tentatives. veuillez retourner au menu et attendre que la partie se termine.", buttons.ToArray());
                tentativesReconnection = 0;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerLeftProcedure();
    }

    public void OnPlayerLeftProcedure()
    {
        UpdatePlayerList();
        if (CurrentScene != 0)
        {
            ClientManager.Instance.InitEntities();
            GameManager.Instance.CheckDoors(0);
            GameManager.Instance.CheckDoors(1);

            GameManager.Instance.CheckDeath();

            GameManager.Instance.CheckEndRound();
        }
    }

    public void ReloadGame()
    {
        PhotonNetwork.LoadLevel(0);
        /*ModalManager.Show("Reconnexion", "Reconnexion en cours...");*/
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from Photon: " + cause);

        if (cause != DisconnectCause.None)
        {
            List<ModalButton> buttons = new List<ModalButton>
            {
                new ModalButton("Se reconnecter", delegate { TryReconnect(); }),
                new ModalButton("Revenir au menu", delegate { ReloadGame(); } ),
            };

            PopUpShower.Show("Déconnexion", "Vous avez été déconnecté du serveur, voulez vous vous reconnecter ?", buttons.ToArray());

            Debug.Log("Error message sended");
        }
    }

    public void TryLeaveGame()
    {
        List<ModalButton> buttons = new List<ModalButton>
            {
                new ModalButton("Rester", null),
                new ModalButton("Quitter la partie", delegate { LeaveGame(); } ),
            };

        PopUpShower.Show("Quitter la partie", "Etes vous sûr de vouloir quitter la partie en cours, vous ne pourrez pas revenir avant que la partie se finisse ?", buttons.ToArray());
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    public void TryReconnect()
    {
        tentativesReconnection++;
        RejoinRoom();
    }

    public override void OnLeftRoom()
    {
        ReloadGame();
    }

    public void OnApplicationQuit()
    {
        ConnectionHandler.AppQuits = true;
    }
}
