using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> Menus = new List<GameObject>();

    private static MenuManager _Instance;
    public static MenuManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    private void Awake()
    {
        _Instance = this;
    }

    public void SwitchMenu(int Index)
    {
        foreach (GameObject menu in Menus)
        {
            menu.SetActive(false);
        }
        Menus[Index].SetActive(true);
    }
}
