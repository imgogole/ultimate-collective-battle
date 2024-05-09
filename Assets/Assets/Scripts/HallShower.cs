using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HallShower : MonoBehaviour
{
    const string display = "Hall : {0}";
    private TMP_Text m_Text;
    Entity client;

    private bool _Checking;


    private void Start()
    {
        m_Text = GetComponent<TMP_Text>();
        _Checking = false;
    }

    public void Init(Entity Client)
    {
        _Checking = true;
        client = Client;
    }


    private void Update()
    {
        if (_Checking)
        {
            m_Text.text = string.Format(display, (HallName)client.Hall);
        }

    }
}

public enum HallName
{
    PremierEtage,
    RezDeChaussee
}
