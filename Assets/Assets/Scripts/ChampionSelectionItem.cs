using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI    ;
using TMPro;

public class ChampionSelectionItem : MonoBehaviour
{
    public Image Background;
    public Button ChampionButton;
    public TMP_Text ChampionText;
    public string ChampionName;

    public void OnValidate()
    {
        name = ChampionName + " Item";
        ChampionText.text = ChampionName;
    }
}
