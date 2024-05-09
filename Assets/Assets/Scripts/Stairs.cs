using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public int ID;
    public int DestinationID;
    public int FinalHall;

    [Header("1 if top and -1 if bot")]
    public int Floor;

    public Vector3 Point
    {
        get
        {
            return new Vector3(transform.position.x, 0.1f, 0);
        }
    }

    public Vector3 SidePoint
    {
        get
        {
            float Y = Floor * 8f;
            float Z = Floor * 10f;
            return new Vector3(transform.position.x, Y, Z);
        }
    }

    public Outline _Outline;

    float _ClientCooldown = 0f;
    bool _IsTrigger = false;
    bool _IsInRange = false;

    private void Start()
    {
        SetTrigger(false);
    }

    private void OnMouseEnter()
    {
        SetTrigger(true);
    }

    private void OnMouseExit()
    {
        SetTrigger(false);
    }

    private void Update()
    {
        if (_ClientCooldown > 0f)
        {
            _ClientCooldown -= Time.deltaTime;
            if (_ClientCooldown < 0f) _ClientCooldown = 0f;
        }

        if (GameManager.Instance.InRound)
        {
            if (_IsTrigger)
            {
                CheckRange();

                _Outline.enabled = true;
                if (_IsInRange && _ClientCooldown <= 0f)
                {
                    _Outline.OutlineColor = Color.green;

                }
                else
                {
                    _Outline.OutlineColor = Color.red;
                }

                if (ClientManager.Instance.Me().IsAbleToControl && Input.GetMouseButtonDown(0))
                {
                    if (!_IsInRange)
                    {
                        AudioManager.PlaySound(Audio.Blocked);
                        UIManager.Instance.ShowGeneralInfo(GameManager.Instance.NotInRangeStairs);
                    }
                    else if (_ClientCooldown > 0f)
                    {
                        AudioManager.PlaySound(Audio.Blocked);
                        UIManager.Instance.ShowGeneralInfo(string.Format(GameManager.Instance.OnCooldownStairs, Mathf.CeilToInt(_ClientCooldown)));
                    }
                    else
                    {
                        AudioManager.PlaySound(Audio.Stairs);
                        ClientManager.Instance.Me().UseStairs(ID);
                        GameManager.Instance.ForceStairsCooldown();
                    }
                }
            }
            else
            {
                _Outline.enabled = false;
            }
        }
    }

    public void ForceCooldown(float time)
    {
        _ClientCooldown = time;
    }

    private void CheckRange()
    {
        _IsInRange = Mathf.Abs(transform.position.x - ClientManager.Instance.Me().Position) < GameManager.Instance.MaxRangeStairs;
    }

    private void SetTrigger(bool isTrigger)
    {
        if (GameManager.Instance.InRound)
        {
            _IsTrigger = isTrigger;
            CheckRange();
        }
        else
        {
            _IsTrigger = false;
        }
    }
}
