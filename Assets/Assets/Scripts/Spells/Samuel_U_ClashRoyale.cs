using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Samuel_U_ClashRoyale : MonoBehaviour
{
    public List<int> troopCost;

    public List<Button> troopSelectionButton;

    public GameObject LoadingScreen, GameScreen;
    public VideoPlayer videoPlayer;

    public Button buttonLaunchTroopFirstHall, buttonLaunchTroopSecondHall;

    public int SelectedTroop = -1;

    public bool IsClashRoyaleLaunched => _IsClashRoyaleLaunched;

    private bool _IsClashRoyaleLaunched;

    private static Samuel_U_ClashRoyale instance;
    public static Samuel_U_ClashRoyale Instance => instance;

    void Awake()
    {
        _IsClashRoyaleLaunched = false;
        instance = this;
    }

    private void Update()
    {
        if (_IsClashRoyaleLaunched)
        {
            UpdateLaunchButtons();
        }
    }

    public void LaunchTroop(int hall)
    {
        int Cost = troopCost[SelectedTroop];

        Entity me = ClientManager.Instance.Me();

        me.AddElixir(-Cost);

        float posiiton = GameManager.Instance.Halls[hall].x;
        GameObject troopGameObject = ClientManager.Instance.CreateObject("Troop", new Vector3(posiiton, 0f));
        Samuel_U_Troop troop = troopGameObject.GetComponent<Samuel_U_Troop>();

        troop.Init(hall, me.BaseChampion.ID, SelectedTroop);

        foreach (Button btn in troopSelectionButton) btn.interactable = true;
        SelectedTroop = -1;
    }

    private void UpdateLaunchButtons()
    {
        bool Condition = SelectedTroop != -1
            && troopCost[SelectedTroop] < ClientManager.Instance.Me().ElixirValue;

        buttonLaunchTroopFirstHall.interactable = Condition;
        buttonLaunchTroopSecondHall.interactable = Condition;
    }

    public void SelectTroop(int troop)
    {
        SelectedTroop = troop;

        foreach (Button btn in troopSelectionButton) btn.interactable = true;
        troopSelectionButton[troop].interactable = false;
    }

    public void SetState(bool isLaunched)
    {
        _IsClashRoyaleLaunched = isLaunched;
        LoadingScreen.SetActive(isLaunched);
        GameScreen.SetActive(false);
    }

    public void Init()
    {
        SetState(true);
        StartCoroutine(Coroutine_Init());
    }


    IEnumerator Coroutine_Init()
    {
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();
        AudioManager.PlaySound(Audio.ClashRoyale);

        yield return new WaitForSeconds((float)videoPlayer.length);
        LoadingScreen.SetActive(false);
        GameScreen.SetActive(true);
    }
}

public struct ShadowTracking
{
    public Entity Owner;
    public GameObject Shadow;
}
