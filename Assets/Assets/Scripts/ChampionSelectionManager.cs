using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelectionManager : MonoBehaviour
{
    public GameObject ChampionsBar, RunePanel;
    public List<ChampionSelectionItem> ChampionButtons;
    public List<int> AdaptedSelectedRunes;
    public List<string> RuneNames;

    public List<GameObject> ChampionsSplashArt;

    public Color UnselectedColor, SelectedColor;

    public TMP_Text TimerText, InfoText, AlliesPickInfo, EnemiesPickInfo, ChampionSelectedName, RuneNameSelected;
    public List<Image> RuneBackgrounds;
    public Button LockGOButton;
    public ChampionSelectionStat ChampionStat;

    public float StartTime;
    public float FinishTime;

    public float InitialSizeSplashArt;
    public float SpeedSplashArt;

    public bool AllowShowingSplashArt;

    private float _Timer;

    private List<int> lockedChampions = new List<int>();

    bool GameStarted = false;

    private static ChampionSelectionManager _Instance;
    public static ChampionSelectionManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    private void ChoosePreferedRune(int Champion)
    {
        int Rune = AdaptedSelectedRunes[Champion];
        SetRune(Rune);
    }

    public void SetRune(int Rune)
    {
        PlayerManager.Instance.SetRune(Rune);
        RuneNameSelected.text = RuneNames[Rune];
        foreach (Image img in RuneBackgrounds) img.color = UnselectedColor;
        RuneBackgrounds[Rune].color = SelectedColor;
    }

    public void SetRunePanelActive(bool isActive)
    {
        RunePanel.SetActive(isActive);
    }

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        ChampionStat = ChampionSelectionStat.Sleeping;
        UnselectAllChampions();
        HideSplashArts();
        SetRunePanelActive(false);
    }

    public void HideSplashArts()
    {
        foreach (GameObject obj in ChampionsSplashArt) obj.SetActive(false);
    }

    public void ShowSplashArt(int index)
    {
        HideSplashArts();
        StartCoroutine(Coroutine_ShowSplashArt(index));
    }

    public void Init(float Time)
    {
        _Timer = Time;
        lockedChampions = new List<int>();
        ChampionsBar.SetActive(true);
        ChampionStat = ChampionSelectionStat.InChoice;
        OnlineManager.Instance.UpdatePlayerList();
        UnlockAllButtons();
        UpdatePickInfo();
        ClearChampionNameSelected();
    }

    public void SetChampionNameSelected(int Index)
    {
        ChampionSelectedName.text = ((ChampionName)Index).ToString();
    }

    public void ClearChampionNameSelected()
    {
        ChampionSelectedName.text = "";
    }

    private void Update()
    {
        LockGOButton.interactable = ChampionStat == ChampionSelectionStat.ChoosenNotConcluded && !lockedChampions.Contains(PlayerManager.Instance.ChampionSelected);

        if (ChampionStat != ChampionSelectionStat.Sleeping)
        {
            _Timer = Mathf.Max(0, _Timer - Time.deltaTime);
            TimerText.text = _Timer > 0 ? Mathf.CeilToInt(_Timer).ToString() : "Lancement du jeu...";


            if (ChampionStat == ChampionSelectionStat.InChoice || ChampionStat == ChampionSelectionStat.ChoosenNotConcluded)
            {
                InfoText.text = "Choisissez votre champion";
            }
            else
            {
                InfoText.text = "Préparez vous au combat";
            }

            if (_Timer <= 0f)
            {
                if (!GameStarted)
                {
                    GameStarted = true;
                    OnlineManager.Instance.StartGame();
                }
            }
        }
    }

    public void UpdatePickInfo()
    {
        string AllyInfo = "";
        string EnemyInfo = "";

        int AllyTeam = PlayerManager.Me.Team;
        int EnemyTeam = 1 - AllyTeam;

        List<PlayerManager> Allies = OnlineManager.Instance.GetPlayerManagersOfTeam(AllyTeam);
        for (int i = 0; i < Allies.Count; i++)
        {
            PlayerManager pManager = Allies[i];
            AllyInfo += "<b>" + pManager.Name + "</b> <i>";
            if (pManager.ChampionConcluded) AllyInfo += "Prêt";
            else AllyInfo += "En train de choisir...";
            AllyInfo += "</i>";
            if (i < Allies.Count - 1) AllyInfo += "\n";
        }

        List<PlayerManager> Enemies = OnlineManager.Instance.GetPlayerManagersOfTeam(EnemyTeam);
        for (int i = 0; i < Enemies.Count; i++)
        {
            PlayerManager pManager = Enemies[i];

            if (pManager.ChampionConcluded) EnemyInfo += "<i>Prêt";
            else EnemyInfo += "<i>En train de choisir...";
            EnemyInfo += "</i> <b>" + pManager.Name + "</b> <i>";
            if (i < Enemies.Count - 1) EnemyInfo += "\n";
        }

        if (OnlineManager.Instance.AllPlayersConcluded()) _Timer = Mathf.Min(_Timer, FinishTime);

        AlliesPickInfo.text = AllyInfo;
        EnemiesPickInfo.text = EnemyInfo;
    }

    public void SelectButton(int Index)
    {
        UnselectAllChampions();
        ChampionButtons[Index].Background.color = SelectedColor;
    }

    public void UnselectAllChampions()
    {
        foreach (ChampionSelectionItem btn in ChampionButtons)
        {
            btn.Background.color = UnselectedColor;
        }
    }

    public void UnlockAllButtons()
    {
        foreach (ChampionSelectionItem btn in ChampionButtons)
        {
            btn.ChampionButton.interactable = true;
        }   
    }

    public void LockButton(int Index)
    {
        if (Index < ChampionButtons.Count)
        {
            ChampionButtons[Index].ChampionButton.interactable = false;
        }

        lockedChampions.Add(Index);
    }

    public void LockChampion()
    {
        ChampionStat = ChampionSelectionStat.Concluded;
        ChampionsBar.SetActive(false);
        SetChampionNameSelected(PlayerManager.Me.ChampionSelected);
        if (AllowShowingSplashArt) ShowSplashArt(PlayerManager.Me.ChampionSelected);
        PlayerManager.Instance.LockChampion();

        AudioManager.PlaySound(Audio.ClickButton);
    }

    public void ChooseChampion(int Champion)
    {
        SelectButton(Champion);
        ChoosePreferedRune(Champion);
        ChampionStat = ChampionSelectionStat.ChoosenNotConcluded;
        PlayerManager.Instance.SelectChampion(Champion);

        AudioManager.PlaySound(Audio.ClickButton);
    }

    IEnumerator Coroutine_ShowSplashArt(int index)
    {
        GameObject splashArt = ChampionsSplashArt[index];
        splashArt.SetActive(true);
        RectTransform rectTransform = splashArt.GetComponent<RectTransform>();

        //rectTransform.pivot = Vector2.zero;
        Vector2 initialSize = rectTransform.localScale;

       /* Vector2 offset = new Vector2(rectTransform.pivot.x * initialSize.x, rectTransform.pivot.y * initialSize.y);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.anchoredPosition -= offset;*/

        Vector2 size = initialSize * InitialSizeSplashArt;
        rectTransform.localScale = size;

        Image image = splashArt.transform.GetChild(0).GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);

        Debug.Log(image);

        while (!Mathf.Approximately(rectTransform.localScale.x, initialSize.x))
        {
            rectTransform.localScale = Vector2.Lerp(rectTransform.localScale, initialSize, Time.deltaTime * SpeedSplashArt);
            image.color = Color.Lerp(image.color, Color.white, Time.deltaTime * SpeedSplashArt);
            yield return null;
        }
    }
}

public enum ChampionSelectionStat
{
    Sleeping,
    InChoice,
    ChoosenNotConcluded,
    Concluded
}