using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class LobbyClientItem : MonoBehaviour
{
    public GameObject SwitchToTeamB, SwitchToTeamA, HostImage;
    public TMP_Text NameText;
    public Color OtherColor, MineColor;
    [HideInInspector] public string Name;

    public void Init(string name, int Team, bool IsMine, bool IsHost)
    {
        Name = name;
        NameText.text = name;
        NameText.color = IsMine ? MineColor : OtherColor;
        HostImage.SetActive(IsHost);
        SwitchToTeamA.SetActive(Team == 0 && IsMine);
        SwitchToTeamB.SetActive(Team == 1 && IsMine);
    }

    public void Switch(int Team)
    {
        PlayerManager.Instance.SetTeam(Team);
    }
}
