using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public List<GameObject> runeImages;

    Entity me;

    private static RuneManager instance;
    public static RuneManager Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    public void Init(Entity client)
    {
        me = client;
        ShowRune(me.Rune);
    }

    public void ShowRune(int rune)
    {
        foreach (GameObject runeImage in runeImages)
        {
            runeImage.SetActive(false);
        }
        runeImages[rune].SetActive(true);
    }
}
