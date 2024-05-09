using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraFollowing : MonoBehaviour
{
    private bool _FocusTarget = false;

    private Entity _TargetEntity;
    public float SpeedFollowing = 0.125f;
    public float MaxHeight;
    public float MinHeight;
    public Vector3 CameraOffset;

    public GameObject SpectatingPanel;

    public TMP_Text nameText, nicknameText;

    private Vector3 finalPos;
    private bool isSpectating;
    private int spectatingTeam;
    private List<Entity> spectatingEntities = new List<Entity>();

    private int spectatingIndex = 0;

    private static CameraFollowing _Instance;
    public static CameraFollowing Instance
    {
        get
        {
            return _Instance;
        }
    }

    public bool IsSpectating => isSpectating;

    bool _Following;

    private void Awake()
    {
        _Instance = this;
        SetFollowing(true);
    }

    public void AddIndex(int amount)
    {
        spectatingIndex += amount;
        spectatingIndex = (spectatingIndex + spectatingEntities.Count) % spectatingEntities.Count;

        SetTarget(spectatingEntities[spectatingIndex]);
    }

    /// <summary>
    /// Set if the game is currently spectating. 0 is for ally, 1 is for enemy and 2 representing both.
    /// </summary>
    /// <param name="_isfollowing"></param>
    /// <param name="_team"></param>
    public void SetSpectating(bool isSpec, int _team = 2)
    {
        isSpectating = isSpec;
        SpectatingPanel.SetActive(isSpec);
        spectatingTeam = _team;

        Entity me = ClientManager.Instance.Me();

        if (IsSpectating)
        {
            if (spectatingTeam == 0)
            {
                spectatingEntities = new List<Entity>(ClientManager.Instance.AllyEntities);
            }
            else if (spectatingTeam == 1)
            {
                spectatingEntities = new List<Entity>(ClientManager.Instance.EnemyEntities);
            }
            else
            {
                spectatingEntities = new List<Entity>(ClientManager.Instance.CurrentEntities);
            }

            if (spectatingEntities.Contains(me))
            {
                int index = spectatingEntities.IndexOf(me);
                spectatingIndex = index;
            }
            else
            {
                spectatingIndex = 0;
            }

            SetTarget(spectatingEntities[spectatingIndex]);
        }
        else
        {
            SetTarget(me);
        }
    }

    public void SetTarget(Entity entity)
    {
        _TargetEntity = entity;
        _FocusTarget = true;
        SetFollowing(true);

        if (isSpectating)
        {
            nicknameText.text = entity.View.Owner.NickName;
            nameText.text = entity.ChampionName;
        }
    }

    public void SetFollowing(bool _isfollowing)
    {
        _Following = _isfollowing;
    }

    /// <summary>
    /// Force the camera to be at the player position for the next frame.
    /// </summary>
    public void ForceAnchorPosition()
    {
        Vector3 pos = FinalPos();
        transform.position = pos;
    }

    private Vector3 FinalPos()
    {
        Vector3 pos = _TargetEntity.transform.position + CameraOffset;
        pos.y = Mathf.Clamp(pos.y, MinHeight, MaxHeight);
        pos.z = CameraOffset.z;
        return pos;
    }

    private void Update()
    {
        if (_FocusTarget && _Following && _TargetEntity)
        {
            finalPos = FinalPos();

            transform.position = Vector3.Lerp(transform.position, finalPos, (SpeedFollowing * Time.deltaTime)); ;
        }
    }
}
