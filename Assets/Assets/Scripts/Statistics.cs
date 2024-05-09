using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    private Dictionary<string, float> statisticValues = new Dictionary<string, float>();
    private Entity Owner;

    /// <summary>
    /// Damage sended to other entities.
    /// </summary>
    public const string INFLICTED_DAMAGE = "INFLICTED_DAMAGE";

    /// <summary>
    /// Damage received from other entities.
    /// </summary>
    public const string RECEIVED_DAMAGE = "RECEIVED_DAMAGE";

    /// <summary>
    /// Amount of entities killed.
    /// </summary>
    public const string KILLS = "KILLS";

    /// <summary>
    /// Amount of entities killed with the help of the client.
    /// </summary>
    public const string ASSISTS = "ASSISTS";

    /// <summary>
    /// Amount of client entity death.
    /// </summary>
    public const string DEATH = "DEATH";

    public void Init(Entity client)
    {
        statisticValues = new Dictionary<string, float>();
        Owner = client;
    }

    public void SetValue(string tag, float value)
    {
        if (statisticValues.ContainsKey(tag))
        {
            statisticValues[tag] = value;
        }
        else
        {
            statisticValues.Add(tag, value);
        }
    }

    public void AddValue(string tag, float value)
    {
        if (statisticValues.ContainsKey(tag))
        {
            statisticValues[tag] += value;
        }
        else
        {
            statisticValues.Add(tag, value);
        }
    }

    public float GetValue(string tag)
    {
        if (statisticValues.TryGetValue(tag, out float value))
        {
            return value;
        }
        return 0;
    }
}
