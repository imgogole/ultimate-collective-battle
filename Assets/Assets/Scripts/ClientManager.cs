using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviourPun
{
    public Camera MainCamera;

    public DEV_Debug debugDev;

    [Header("UI")]

    [SerializeField] private TMP_Text HP_Text, HPMax_Text;
    [SerializeField] private Image HP_Background;

    [SerializeField] private UIManager _UIManager;

    [SerializeField] private List<SpellItem> SpellsInfoItems = new List<SpellItem>();

    public Color UIBackgroundColor;

    private List<Entity> _CurrentEntities = new List<Entity>();
    private List<Entity> _AllyEntities = new List<Entity>();
    private List<Entity> _EnemyEntities = new List<Entity>();

    public List<Entity> CurrentEntities => _CurrentEntities;
    public List<Entity> AllyEntities => _AllyEntities;
    public List<Entity> EnemyEntities => _EnemyEntities;

    private static ClientManager _Instance;
    public static ClientManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    /// <summary>
    /// Returns the game camera.
    /// </summary>
    public static Camera Camera => Instance.MainCamera;

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        PlayerManager.Me.InitializationGame();
        //SpawnPlayer();
    }

    private void Update()
    {
        HP_Background.color = Color.Lerp(HP_Background.color, UIBackgroundColor, 10f * Time.deltaTime);
    }

    public void InitEntities()
    {
        int team = PlayerManager.Me.Team;

        if (team == 2) team = 0;

        int AllyTeam = PlayerManager.Me.Team;
        int EnemyTeam = 1 - AllyTeam;

        _CurrentEntities = FindObjectsOfType<Entity>().ToList();
        _AllyEntities = _CurrentEntities.FindAll(p => p.PlayerManagerParent.Team == AllyTeam).ToList();
        _EnemyEntities = _CurrentEntities.FindAll(p => p.PlayerManagerParent.Team == EnemyTeam).ToList();
    }

    public void Init(Entity entity)
    {
        //Entity entity = client.GetComponent<Entity>();
        entity_me = entity;
        debugDev.Init(entity);


        CameraFollowing.Instance.SetSpectating(false);
        //CameraFollowing.Instance.SetTarget(entity);
        _UIManager.UpdateStatsInformation();

        ResetSpellsInfo(entity);
    }

    public void ResetSpellsInfo(Entity entity)
    {
        foreach (SpellItem item in SpellsInfoItems)
        {
            item.Init(entity);
        }
    }

    public void UIColorGradientAnimationHP(Color color)
    {
        HP_Background.color = color;
    }

    public void UpdateClientStatsUI()
    {
        //print($"hp : {entity_me.HitPoints} de {entity_me.ChampionName} avec {entity_me.HitPointsMax} max");
        HP_Text.text = FormattedHPText(entity_me.HitPoints, entity_me.RatioHP);
        HPMax_Text.text = "/" + entity_me.HitPointsMax.ToString();
    }

    private Entity entity_me;
    
    /// <summary>
    /// Return the entity owned by the client
    /// </summary>
    /// <returns></returns>
    public Entity Me()
    {
        return entity_me;
    }

    private string FormattedHPText(float Hp, float Hpr)
    {
        string color;
        if (Hpr < 0.1f) color = "#b81200";
        else if (Hpr < 0.2f) color = "#d40700";
        else if (Hpr < 0.3f) color = "#fa8100";
        else if (Hpr < 0.4f) color = "#fab300";
        else if (Hpr < 0.5f) color = "#fae100";
        else if (Hpr < 0.6f) color = "#e9fa00";
        else if (Hpr < 0.7f) color = "#ccfa00";
        else if (Hpr < 0.8f) color = "#a2fa00";
        else if (Hpr < 0.9f) color = "#43fa00";
        else color = "#39D400";
        
        return $"<color={color}>{Hp.ToString("0.#", CultureInfo.InvariantCulture)}</color>";
    }


    /// <summary>
    /// Instanciate an object through the server. This object is owned by the client who execute this method.
    /// </summary>
    /// <param name="Name"></param>
    public GameObject CreateObject(string Name, Vector3 Position)
    {
        return PhotonNetwork.Instantiate(Path.Combine("Photon", Name), Position, Quaternion.identity);
    }

    // --------------------------------- Utilities ------------------------------------------


    /// <summary>
    /// Returns true if every enemies are dead.
    /// </summary>
    /// <returns></returns>
    public bool IsEveryEnemiesDead()
    {
        return !EnemyEntities.Any(e => !e.IsDead);
    }

    /// <summary>
    /// Returns true if an enemy with this ID is currently playing.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public bool IsEnemyExists(int ID)
    {
        bool result = _EnemyEntities.Any(e => e.BaseChampion.ID == ID);
        Debug.Log($"Searching if enemy {ID} is here, result : {result}");
        return result;
    }

    /// <summary>
    /// Returns true is both entities are in the same team.
    /// </summary>
    /// <param name="eOne"></param>
    /// <param name="eTwo"></param>
    /// <returns></returns>
    public bool AreInTheSameTeam(Entity eOne, Entity eTwo)
    {
        return eOne.PlayerManagerParent.Team == eTwo.PlayerManagerParent.Team;
    }

    /// <summary>
    /// Returns true if this entity is in the same team as the client entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsAlly(Entity entity)
    {
        return entity.PlayerManagerParent.Team == entity_me.PlayerManagerParent.Team;
    }

    /// <summary>
    /// Returns true if this entity is not in the same team as the client entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsEnemy(Entity entity)
    {
        return !IsAlly(entity);
    }

    /// <summary>
    /// Get the current entity with the champion ID.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public Entity GetEntityFromChampionID(int ID)
    {
        return CurrentEntities.Find(e => e.BaseChampion.ID == ID);
    }

    /// <summary>
    /// Returns a list of the nearest enemy entities.
    /// </summary>
    /// <param name="Target"></param>
    /// <returns></returns>
    public List<Entity> NearestEnemyEntitiesFrom(Entity entity, float MaxDistance = float.PositiveInfinity)
    {
        List<Entity> enemies = new (EnemyEntities);
        enemies.RemoveAll(e => e.Hall != entity.Hall);
        enemies.RemoveAll(e => e.IsDead);
        enemies.RemoveAll(e => (e.transform.position - entity.CenterPoint).sqrMagnitude > Mathf.Pow(MaxDistance, 2));
        enemies.OrderBy(e => (e.transform.position - entity.CenterPoint).sqrMagnitude).ToList();
        return enemies;
    }

    /// <summary>
    /// Returns a list of the nearest enemy entities.
    /// </summary>
    /// <param name="Target"></param>
    /// <returns></returns>
    public List<Entity> NearestEnemyEntitiesFrom(Vector3 Pos, float MaxDistance = float.PositiveInfinity)
    {
        List<Entity> enemies = new (EnemyEntities);
        enemies.RemoveAll(e => e.IsDead);
        enemies.RemoveAll(e => (e.transform.position - Pos).sqrMagnitude > Mathf.Pow(MaxDistance, 2));
        enemies.OrderBy(e => (e.transform.position - Pos).sqrMagnitude).ToList();
        return enemies;
    }

    /// <summary>
    /// Returns a list of the nearest ally entities.
    /// </summary>
    /// <param name="Target"></param>
    /// <returns></returns>
    public List<Entity> NearestAllyEntitiesFrom(Entity entity, float MaxDistance = float.PositiveInfinity)
    {
        List<Entity> allies = new (AllyEntities);
        allies.RemoveAll(e => e.Hall != entity.Hall);
        allies.RemoveAll(e => e.IsDead);
        allies.RemoveAll(e => (e.transform.position - entity.transform.position).sqrMagnitude > Mathf.Pow(MaxDistance, 2));
        allies.OrderBy(e => (e.transform.position - entity.transform.position).sqrMagnitude).ToList();
        return allies;
    }

    /// <summary>
    /// Make a collider ignoring collisions with every entities.
    /// </summary>
    /// <param name="col"></param>
    public void IgnoreEveryEntityCollisions(Collider col)
    {
        foreach (Entity e in CurrentEntities)
        {
            Physics.IgnoreCollision(col, e.GetComponent<Collider>());
        }
    }
}
