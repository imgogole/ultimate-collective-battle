using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas MainCanvas;
    private static UIManager _Instance;
    public static UIManager Instance => _Instance;

    [Header("Stats")]
    public TMP_Text AttackDamageText, AttackSpeedText, MovementSpeedText, ArmorSpeedText, RangeText, DeltaText, HairText, PredatorText, ControlledText, VirusText, FatText, FriendshipText, LoveText, NastinessText;


    [Space(20)]
    public GameObject TipSpellInformation;
    public RectTransform DescriptionRect, TipSpellInformationRect, TipInfoRect;
    public TMP_Text TipSpellTitle;
    public TMP_Text TipSpellDescription;
    public TMP_Text TipSpellCooldown;

    public TMP_Text UIMoneyText;
    public TMP_Text KDAText;

    public TMP_Text UITeamAScore, UITeamBScore, UIAce;

    public GameObject GeneralInfo;
    public TMP_Text GeneralInfoText;
    public Color GeneralInfoColor;

    public TMP_Text GameTimerText;

    public GameObject TipInfoInformation;
    public TMP_Text TipInfoMessage;

    public float HeightMouseYInfoPos;

    public bool IsTipShown;
    public bool IsInfoStatShown;

    public Slider attackSpeedSlider;

    float _TimeGeneralInfo;

    float _AttackSpeedTime = 0;
    bool showAttackSpeedIndicator = false;

    public CanvasGroup canvasGroup;
    Entity me;

    bool isUIHidden = false;
    private void Awake()
    {
        _Instance = this;
    }

    public void Start()
    {
        TipSpellInformation.SetActive(false);
        TipInfoInformation.SetActive(false);

        showAttackSpeedIndicator = false;
        attackSpeedSlider.gameObject.SetActive(false);

        SetShowUI(true);
    }

    private void SetShowUI(bool isShown)
    {
        isUIHidden = !isShown;
        canvasGroup.alpha = isUIHidden ? 0.001f : 1f;
    }

    public void Init(Entity client)
    {
        me = client;
    }

    public void ShowAttackSpeedIndicator(float beginTime)
    {
        _AttackSpeedTime = beginTime;
        showAttackSpeedIndicator = true;
        HandleAttackSpeedIndicator();

        attackSpeedSlider.gameObject.SetActive(true);
    }

    void HandleAttackSpeedIndicator()
    {
        if (showAttackSpeedIndicator && me)
        {
            attackSpeedSlider.value = me.AttackTimeRemaining / _AttackSpeedTime;

            if (me.AttackTimeRemaining <= 0f)
            {
                showAttackSpeedIndicator = false;
                attackSpeedSlider.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateKDA()
    {
        KDAText.text = $"{Mathf.FloorToInt(me.EntityStatistics.GetValue(Statistics.KILLS))}/{Mathf.FloorToInt(me.EntityStatistics.GetValue(Statistics.DEATH))}/{Mathf.FloorToInt(me.EntityStatistics.GetValue(Statistics.ASSISTS))}";
    }

    public void UpdateStatsInformation()
    {
        Entity entity = ClientManager.Instance.Me();
        AttackDamageText.text = entity.TotalAttackDamage.ToString("0.##", CultureInfo.InvariantCulture);
        AttackSpeedText.text = entity.TotalAttackSpeed.ToString("0.00", CultureInfo.InvariantCulture);
        MovementSpeedText.text = entity.TotalMovementSpeed.ToString("0.##", CultureInfo.InvariantCulture);
        ArmorSpeedText.text = (entity.TotalArmor * 100).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        RangeText.text = entity.TotalRange.ToString("0.##", CultureInfo.InvariantCulture);

        bool HasDelta = entity.Delta > 0f;
        DeltaText.gameObject.SetActive(HasDelta);
        if (HasDelta)
        {
            DeltaText.text = Mathf.FloorToInt(entity.Delta).ToString();
        }

        bool HasHair = entity.Hair > 0;
        HairText.gameObject.SetActive(HasHair);
        if (HasHair)
        {
            HairText.text = Mathf.FloorToInt(entity.Hair * 100).ToString() + "%";
        }

        bool HasPredator = entity.Predator > 0;
        PredatorText.gameObject.SetActive(HasPredator);
        if (HasPredator)
        {
            PredatorText.text = entity.Predator.ToString();
        }

        bool IsControlled = entity.IsControlled && entity.InternalFlaw > 0f;
        ControlledText.gameObject.SetActive(IsControlled);
        if (IsControlled)
        {
            ControlledText.text = Mathf.FloorToInt(entity.InternalFlaw * (100f / 1.25f)).ToString() + "%";
        }

        bool HasVirus = entity.VirusCount > 0;
        VirusText.gameObject.SetActive(HasVirus);
        if (HasVirus)
        {
            VirusText.text = entity.VirusCount.ToString();
        }

        bool HasFat = entity.FatValue > 0;
        FatText.gameObject.SetActive(HasFat);
        if (HasFat)
        {
            FatText.text = Mathf.FloorToInt(entity.FatValue).ToString();
        }

        bool HasFriendship = entity.FriendshipValue > 0 && !entity.IsTitouan;
        FriendshipText.gameObject.SetActive(HasFriendship);
        LoveText.gameObject.SetActive(HasFriendship);
        NastinessText.gameObject.SetActive(HasFriendship);
        if (HasFriendship)
        {
            FriendshipText.text = entity.FriendshipValue.ToString("0.#", CultureInfo.InvariantCulture);
            LoveText.text = entity.LoveValue.ToString("0.#", CultureInfo.InvariantCulture);
            NastinessText.text = entity.NastinessValue.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }

    private void Update()
    {
        HandleToolTip();
        HandleGameData();
        HandleGeneralInfo();
        HandleAttackSpeedIndicator();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetShowUI(isUIHidden);
        }
    }

    private void HandleGeneralInfo()
    {
        if (_TimeGeneralInfo > 0f)
        {
            if (_TimeGeneralInfo < 0.5f)
            {
                Color c = GeneralInfoColor;
                c.a = _TimeGeneralInfo * 2f;
                GeneralInfoText.color = c;
            }
            else
            {
                GeneralInfoText.color = GeneralInfoColor;
            }

            _TimeGeneralInfo -= Time.deltaTime;
            if (_TimeGeneralInfo <= 0f)
            {
                _TimeGeneralInfo = 0f;
                GeneralInfo.SetActive(false);
            }
        }
        else
        {
            GeneralInfo.SetActive(false);
        }
    }

    private void HandleGameData()
    {
        // Handle game timer

        float Totaltime = GameManager.Instance.GameTimer;
        string forTime = string.Format("{0:0}:{1:00}", Mathf.Floor(Totaltime / 60), Mathf.Floor(Totaltime % 60));
        GameTimerText.text = forTime;
    }

    private void HandleToolTip()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MainCanvas.transform as RectTransform, Input.mousePosition, MainCanvas.worldCamera, out pos);
        Vector2 offset = new Vector2(-700, HeightMouseYInfoPos + DescriptionRect.sizeDelta.y);
        TipSpellInformationRect.position = MainCanvas.transform.TransformPoint(pos + offset);

        Vector2 pos2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MainCanvas.transform as RectTransform, Input.mousePosition, MainCanvas.worldCamera, out pos2);
        Vector2 offset2 = new Vector2(0, HeightMouseYInfoPos);
        TipInfoRect.position = MainCanvas.transform.TransformPoint(pos2 + offset2);
    }

    public void UpdateScoreUI(int teamA, int teamB, int ace)
    {
        UITeamAScore.text = teamA.ToString();
        UITeamBScore.text = teamB.ToString();
        UIAce.text = "Ace : " + ace.ToString();
    }

    public void ShowStatInfo(string Message)
    {
        TipInfoMessage.text = Message;
        TipInfoInformation.SetActive(true);
        IsInfoStatShown = true;
    }

    public void CloseStatInfo()
    {
        TipInfoInformation.SetActive(false);
        IsInfoStatShown = false;
    }

    /// <summary>
    /// Show a message above the stats player bar. Generally used for fast informations.
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="time"></param>
    public void ShowGeneralInfo(string Message, float time = 3f)
    {
        GeneralInfoText.text = Message;
        _TimeGeneralInfo = time + 0.5f;
        GeneralInfo.SetActive(true);
    }

    public void ShowTip(SpellItem item)
    {
        bool isTitouanStolenUlt = item._SpellType == SpellType.Ultimate && ClientManager.Instance.Me().IsTitouanStolenUlt;
        Champion champ = isTitouanStolenUlt ? ClientManager.Instance.Me().TitouanTarget.BaseChampion : ClientManager.Instance.Me().BaseChampion;
        string spellTitle = item._SpellType switch
        {
            SpellType.Passive => champ.PassiveAbility.Name,
            SpellType.Active => champ.ActiveAbility.Name,
            SpellType.Ultimate => champ.UltimateAbility.Name,
            _ => "Title"
        };
        string TypeText  = item._SpellType switch
        {
            SpellType.Passive => "Passif",
            SpellType.Active => $"Actif [{GameSettings.KeyName(GameSettings.ActiveSpellKey.ToString())}]",
            SpellType.Ultimate => $"Ultime [{GameSettings.KeyName(GameSettings.UltimateSpellKey.ToString())}]",
            _ => "Compétence"
        };
        TipSpellTitle.text = spellTitle + $" <size=26><color=#c4c4c4>{TypeText}</color></size>";
        string DescriptionText = item._SpellType switch
        {
            SpellType.Passive => champ.PassiveAbility.Description,
            SpellType.Active => champ.ActiveAbility.Description,
            SpellType.Ultimate => champ.UltimateAbility.Description,
            _ => "Description"
        };
        if (isTitouanStolenUlt)
        {
            string info = champ.UltimateAbility.TitouanUltInformation;
            if (info != "NONE")
            {
                DescriptionText += $"\n\n<i><color=#dedede>{champ.UltimateAbility.TitouanUltInformation}</i></color>";
            }
        }
        if (item._SpellType == SpellType.Ultimate && GameManager.Instance.CurrentRound < GameManager.Instance.MinimumRoundUltimate)
        {
            DescriptionText += $"\n\n<i><color=#dedede>{string.Format(GameManager.Instance.MinimalRoundUltimateMessage, GameManager.Instance.MinimumRoundUltimate)}</i></color>";
        }
        TipSpellDescription.text = DescriptionText;
        float Cooldown = item._SpellType switch
        {
            SpellType.Passive => champ.PassiveAbility.Cooldown,
            SpellType.Active => champ.ActiveAbility.Cooldown,
            SpellType.Ultimate => champ.UltimateAbility.Cooldown,
            _ => 0f
        };
        TipSpellCooldown.text = Cooldown == 0 ? "" : $"{Cooldown} seconde{(Cooldown < 2f ? "" : "s")}";
        TipSpellInformation.SetActive(true);
        IsTipShown = true;
    }

    public void CloseTip()
    {
        TipSpellInformation.SetActive(false);
        IsTipShown = false;
    }

    public void UpdateMoney(float Value)
    {
        UIMoneyText.text = $"{Value:F2}€";
    }
}

public enum SpellType
{
    Passive,
    Active,
    Ultimate
}
