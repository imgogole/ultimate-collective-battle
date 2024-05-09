using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using System.Globalization;

public class SpellItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpellType _SpellType;
    public Image SpellTimer, SpellIcon;
    public GameObject SpellCooldownForeground, NotReadyForeground, WaitImage, LockedImage;
    public TMP_Text CooldownTimer;

    public Color NotReadyColor, ReadyColor;

    Entity _Entity;
    Entity _EntityForUltimate => (_SpellType == SpellType.Ultimate && _Entity.IsTitouanStolenUlt) ? _Entity.TitouanTarget : _Entity;

    bool _Init = false;

    public void Init(Entity entity)
    {
        _Entity = entity;
        _Init = true;
        RefreshIcon();
    }

    bool Ready;

    private void Update()
    {
        if (_Init)
        {
            RefreshIcon();
            if (_SpellType == SpellType.Ultimate && GameManager.Instance.CurrentRound < GameManager.Instance.MinimumRoundUltimate)
            {
                CooldownTimer.gameObject.SetActive(false);
                WaitImage.SetActive(false);
                LockedImage.SetActive(true);
                NotReadyForeground.SetActive(false);
                SpellCooldownForeground.SetActive(true);
                SpellTimer.color = NotReadyColor;
            }
            else
            {
                if (GameManager.Instance.InRound)
                {
                    CooldownTimer.gameObject.SetActive(true);
                    WaitImage.SetActive(false);

                    SpellTimer.fillAmount = _SpellType switch
                    {
                        SpellType.Passive => 1 - _Entity.RatioPassiveSpellRemaining,
                        SpellType.Active => 1 - _Entity.RatioActiveSpellRemaining,
                        SpellType.Ultimate => 1 - _Entity.RatioUltimateSpellRemaining,
                        _ => 0
                    };

                    CooldownTimer.text = _SpellType switch
                    {
                        SpellType.Passive => _Entity.PassiveSpellRemaining.ToString("0.#", CultureInfo.InvariantCulture),
                        SpellType.Active => _Entity.ActiveSpellRemaining.ToString("0.#", CultureInfo.InvariantCulture),
                        SpellType.Ultimate => _Entity.UltimateSpellRemaining.ToString("0.#", CultureInfo.InvariantCulture),
                        _ => ""
                    };

                    if (SpellTimer.fillAmount == 1)
                    {
                        Ready = _SpellType switch
                        {
                            SpellType.Passive => _Entity.IsPassiveConditionOk,
                            SpellType.Active => _Entity.IsActiveConditionOk,
                            SpellType.Ultimate => _Entity.IsUltimateConditionOk,
                            _ => true
                        };
                        SpellTimer.color = Ready ? ReadyColor : NotReadyColor;
                        NotReadyForeground.SetActive(!Ready);
                        SpellCooldownForeground.SetActive(false);
                    }
                    else
                    {
                        SpellTimer.color = NotReadyColor;
                        NotReadyForeground.SetActive(false);
                        SpellCooldownForeground.SetActive(true);
                    }
                }
                else
                {
                    CooldownTimer.gameObject.SetActive(false);
                    WaitImage.SetActive(true);
                    LockedImage.SetActive(false);
                    NotReadyForeground.SetActive(false);
                    SpellCooldownForeground.SetActive(true);
                    SpellTimer.color = NotReadyColor;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowTip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.CloseTip();
    }

    public void RefreshIcon()
    {
        SpellIcon.sprite = _SpellType switch
        {
            SpellType.Passive => _Entity.BaseChampion.PassiveAbility.Icon,
            SpellType.Active => _Entity.BaseChampion.ActiveAbility.Icon,
            SpellType.Ultimate => _EntityForUltimate.BaseChampion.UltimateAbility.Icon,
            _ => null
        };
    }
}
