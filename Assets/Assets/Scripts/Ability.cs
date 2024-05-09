using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Ability : ScriptableObject
{
    public string Name;
    [TextArea]
    public string Description;
    public float Cooldown;
    public bool UsableWhileCC;
    public Sprite Icon;

    public string TitouanUltInformation = "NONE";

    [Space(20)]
    [Header("For informations")]
    public VideoClip video;

    public virtual bool OnCondition(Entity actor) { return true; }
    public virtual void OnActivate(Entity actor) { }
}
