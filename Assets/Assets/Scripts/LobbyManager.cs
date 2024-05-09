using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPun
{
    public GameObject LobbyClientItemPrefab;

    public Button LaunchButton;

    public Transform BackgroundTeamA, BackgroundTeamB;

    private List<GameObject> LobbyClientTeamAList = new List<GameObject>();
    private List<GameObject> LobbyClientTeamBList = new List<GameObject>();

    private List<PlayerManager> PlayersInLobby = new List<PlayerManager>();

    private static LobbyManager _Instance;
    public static LobbyManager Instance
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

    public void RefreshList()
    {
        LaunchButton.interactable = PhotonNetwork.IsMasterClient;

        foreach (GameObject item in LobbyClientTeamAList) Destroy(item);
        foreach (GameObject item in LobbyClientTeamBList) Destroy(item);
        LobbyClientTeamAList.Clear();
        LobbyClientTeamBList.Clear();

        foreach (PlayerManager player in PlayersInLobby)
        {
            int Team = player.Team;
            GameObject item;

            if (Team == 0)
            {
                item = Instantiate(LobbyClientItemPrefab, BackgroundTeamA.GetChild(0));
                LobbyClientTeamAList.Add(item);
            }
            else
            {
                item = Instantiate(LobbyClientItemPrefab, BackgroundTeamB.GetChild(0));
                LobbyClientTeamBList.Add(item);
            }

            item.SetActive(true);
            LobbyClientItem lobbyClientItem = item.GetComponent<LobbyClientItem>();
            lobbyClientItem.Init(player.view.Owner.NickName, Team, player.view.IsMine, player.view.Owner.IsMasterClient);
        }
    }

    public void RefreshPlayersList()
    {
        PlayersInLobby = OnlineManager.PlayersOnline;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            RefreshList();
        }
        else
        {
            ClientManager.Instance.InitEntities();
        }
    }
}
