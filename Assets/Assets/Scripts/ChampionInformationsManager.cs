using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static UnityEngine.EventSystems.EventTrigger;

public class ChampionInformationsManager : MonoBehaviour
{
    public List<Champion> Champions;
    
    public TMP_Text attackDamageText, baseHPText, attackSpeedText, armorText, heightJumpText, rangeText;

    public TMP_Text spellTitleText, spellCooldownText, spellDescriptionText;
    public VideoPlayer video;

    public GameObject statsPanel;
    public GameObject spellsPanel;
    public GameObject videoPanel;

    public Image passiveButton, activeButton, ultimateButton;

    int currentChampion = 0;

    private void Start()
    {
        ResetPanels();
    }

    public void ResetPanels()
    {
        statsPanel.SetActive(false);
        spellsPanel.SetActive(false);
        videoPanel.SetActive(false);
    }

    public void OpenStatsChampions(int idchampion)
    {
        currentChampion = idchampion;

        statsPanel.SetActive(true);
        spellsPanel.SetActive(false);
        videoPanel.SetActive(false);
        Champion champion = Champions[idchampion];

        baseHPText.text = champion.HP.ToString("0.#", CultureInfo.InvariantCulture);
        attackDamageText.text = champion.AttackDamage.ToString("0.#", CultureInfo.InvariantCulture);
        attackSpeedText.text = champion.AttackSpeed.ToString("0.00", CultureInfo.InvariantCulture);
        heightJumpText.text = champion.JumpSpeed.ToString("0.#", CultureInfo.InvariantCulture);
        armorText.text = (champion.ArmorPercent * 100).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        rangeText.text = champion.Range.ToString("0.##", CultureInfo.InvariantCulture);

        passiveButton.sprite = champion.PassiveAbility.Icon;
        activeButton.sprite = champion.ActiveAbility.Icon;
        ultimateButton.sprite = champion.UltimateAbility.Icon;
    }

    public void OpenSpell(int spell)
    {
        spellsPanel.SetActive(true);
        videoPanel.SetActive(false);

        Champion champion = Champions[currentChampion];
        Ability ability = spell switch
        {
            0 => champion.PassiveAbility,
            1 => champion.ActiveAbility,
            2 => champion.UltimateAbility,
            _ => champion.PassiveAbility
        };

        spellTitleText.text = ability.Name;
        spellCooldownText.text = $"Cooldown : <b>{ability.Cooldown}</b>s";
        spellDescriptionText.text = ability.Description;

        if (coroutineVideo != null) StopCoroutine(coroutineVideo);
        coroutineVideo = StartCoroutine(Coroutine_VideoPlayer(ability));
    }

    Coroutine coroutineVideo;

    IEnumerator Coroutine_VideoPlayer(Ability ability)
    {
        video.Stop();
        VideoClip clip = ability.video;

        if (clip != null)
        {
            video.clip = clip;
            videoPanel.SetActive(true);
            video.Prepare();

            yield return new WaitUntil(() => video.isPrepared);

            video.Play();
        }
        yield return null;
    }
}
