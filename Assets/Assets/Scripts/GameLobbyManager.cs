using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobbyManager : MonoBehaviour
{
    /*
     * 
     * This script provides a better way to checks if the client is currently in or out of the lobby without
     * passing with box colliders.
     * 
     */


    private static GameLobbyManager instance;
    public static GameLobbyManager Instance
    {
        get
        {
            return instance;
        }
    }

    public List<LobbyRange> LobbiesRange = new List<LobbyRange>();

    Entity _ClientEntity;
    bool _Checking = false;
    private bool isInSideInterval = false;
    private bool isInEnemySideInterval = false;

    public float MinimumSideValue
    {
        get
        {
            return LobbiesRange[_ClientEntity.Team].MinimumRange;
        }
    }

    public float MaximumSideValue
    {
        get
        {
            return LobbiesRange[_ClientEntity.Team].MaximumRange;
        }
    }

    public float MinimumEnemySideValue
    {
        get
        {
            return LobbiesRange[1 - _ClientEntity.Team].MinimumRange;
        }
    }

    public float MaximumEnemySideValue
    {
        get
        {
            return LobbiesRange[1 - _ClientEntity.Team].MaximumRange;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _Checking = false;
        _ClientEntity = null;
    }

    public void Init()
    {
        SetChecking(true);
        _ClientEntity = ClientManager.Instance.Me();
    }

    public void SetChecking(bool checking)
    {
        _Checking = checking;
    }

    private void Update()
    {
        if (_Checking && _ClientEntity)
        {
            if (_ClientEntity.IsChanneling) return;

            float entityPositionX = _ClientEntity.Position;

            // Vérifie si l'entité est dans l'intervalle MinimunSideValue et MaximumSideValue
            if (entityPositionX >= MinimumSideValue && entityPositionX <= MaximumSideValue)
            {
                if (!isInSideInterval)
                {
                    // L'entité est entrée dans l'intervalle, exécute la fonction 1
                    _ClientEntity.SetLobbyStat(true);
                    _ClientEntity.SetEnemyLobbyStat(false);
                    isInSideInterval = true;
                    isInEnemySideInterval = false;
                }
            }
            // Vérifie si l'entité est dans l'intervalle MinimunEnemySideValue et MaximumEnemySideValue
            else if (entityPositionX >= MinimumEnemySideValue && entityPositionX <= MaximumEnemySideValue)
            {
                if (!isInEnemySideInterval)
                {
                    // L'entité est entrée dans l'intervalle, exécute la fonction 2
                    _ClientEntity.SetLobbyStat(false);
                    _ClientEntity.SetEnemyLobbyStat(true);
                    isInEnemySideInterval = true;
                    isInSideInterval = false;
                }
            }
            else
            {
                // L'entité est sortie des deux intervalles, exécute la fonction 3
                if (isInSideInterval || isInEnemySideInterval)
                {
                    _ClientEntity.SetLobbyStat(false);
                    _ClientEntity.SetEnemyLobbyStat(false);
                    isInSideInterval = false;
                    isInEnemySideInterval = false;
                }
            }

            if (GameManager.Instance.IsState(GameState.WaitingForStartingRound) && !isInSideInterval)
            {
                _ClientEntity.TeleportToSpawnPoint();
            }
        }
    }  
}

[System.Serializable]
public struct LobbyRange
{
    public float MinimumRange;
    public float MaximumRange;
}