using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PingCounter : MonoBehaviourPun
{
    private float _time;
    const string display = "Ping : {0}ms";
    private TMP_Text m_Text;


    private void Start()
    {
        _time = 1f;
        m_Text = GetComponent<TMP_Text>();
    }


    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > 1f)
        {
            m_Text.text = string.Format(display, PhotonNetwork.GetPing());
            _time = 0f;
        }
    }
}
