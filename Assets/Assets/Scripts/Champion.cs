using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Champion", order = 1)]
public class Champion : ScriptableObject
{
    public string Name = "Dummy";
    public int ID = 0;

    [Header("Base Stats")]

    public float HP = 100;
    public float AttackDamage = 5;
    public int MultipleJump = 0;
    public float AttackSpeed = 1f;
    public float MovementSpeed = 5f;
    public float JumpSpeed = 5f;
    public float JumpCooldown = 0f;
    public float ArmorPercent = 0f;
    public float Range = 1f;

    public bool IsMelee = true;

    [Header("Spells")]

    public Ability PassiveAbility;
    public Ability ActiveAbility;
    public Ability UltimateAbility;

    [Header("Miscellaneous")]

    public Sprite Icon;
    public Color RepresentativeColor;
    public bool InverseIcon;
}

public enum ChampionName
{
    Arnaud,
    Dalil,
    Ibrahim,
    Idriss,
    Julien,
    Lou,
    Mohammad,
    Romain,
    Samuel,
    Thomas,
    Titouan,
    Virgil,
    Yassine
}
