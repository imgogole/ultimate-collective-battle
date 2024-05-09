using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Linq;

public class EntityEffects : MonoBehaviour
{
    public List<Effect> Effects = new List<Effect>();
    public Entity BaseEntity;

    private void Update()
    {
        if (Effects.Count != 0)
        {
            foreach (Effect effect in Effects)
            {
                effect.Next(Time.deltaTime);
            }
            Effects.RemoveAll(e => e.IsEnded && !e.Persistant);
        }
    }

    /// <summary>
    /// Get the final value of a entity state, included flat and percentage effects the entity is affected. Returns the initial value if no effects are found.
    /// </summary>
    /// <param name="InitValue"></param>
    /// <param name="flatEffect"></param>
    /// <param name="percentageEffect"></param>
    /// <returns></returns>
    public float GetValue(float InitValue, BehaviourEffects flatEffect, BehaviourEffects percentageEffect)
    {
        return Mathf.Clamp((InitValue + GetValueFlat(flatEffect)) * GetValuePercentage(percentageEffect), 0, float.MaxValue);
    }

    public float GetValuePercentage(BehaviourEffects effect)
    {
        float Value = 1f;
        foreach (Effect e in Effects)
        {
            if (e.BEffect == effect)
            {
                int Modifier = e.IsPositiveEffect ? 1 : -1;
                Value += Modifier * e.Value;
            }
        }
        return Value;
    }

    public float GetValueFlat(BehaviourEffects effect)
    {
        float Value = 0f;
        foreach (Effect e in Effects)
        {
            if (e.BEffect == effect)
            {
                int Modifier = e.IsPositiveEffect ? 1 : -1;
                Value += Modifier * e.Value;
            }
        }
        return Value;
    }

    public void AddEffect(Effect effect)
    {
        Effects.Add(effect);
    }

    /// <summary>
    /// Return if the entity has the effect.
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool HasEffect(BehaviourEffects effect)
    {
        return Effects.Any(e => e.BEffect == effect);
    }

    public void PurgeAllNegativeEffects()
    {
        Effects.RemoveAll(e => !e.IsPositiveEffect);
    }

    public void ClearAllEffects(BehaviourEffects bEffects)
    {
        Effects.RemoveAll(e => e.BEffect == bEffects);
    }

    public void ClearAllEffects()
    {
        Effects.RemoveAll(e => !e.Persistant);
    }

    public List<Effect> GetAllEffectsOfType(BehaviourEffects bEffect)
    {
        return Effects.FindAll(e => e.BEffect == bEffect);
    }

    public void RemoveEffectsWithTag(string tag)
    {
        Effects.RemoveAll(e => e.Tag == tag);
    }

    public List<Effect> GetEffectsWithTag(string tag)
    {
        return Effects.FindAll(e => e.Tag == tag);
    }

    /// <summary>
    /// Return if the entity has negative effects.
    /// </summary>
    public bool HasNegativeEffects()
    {
        return Effects.Any(e => !e.IsPositiveEffect);
    }
}

public class Effect
{
    public BehaviourEffects BEffect;
    public float Value;
    public float Time;
    public bool Persistant;
    public bool IsPositiveEffect;
    public string Tag;

    public bool IsEnded
    {
        get
        {
            return Time <= 0f;
        }
    }

    public Effect(BehaviourEffects bEffect, float value, float time, bool persistant, bool positive, string tag = "")
    {
        BEffect = bEffect;
        Value = value;
        Time = time;
        Persistant = persistant;
        IsPositiveEffect = positive;
        Tag = tag;
    }

    public void Next(float time)
    {
        Time -= time;
    }
}

public enum BehaviourEffects
{
    Speed,
    AttackDamage,
    Armor,
    AttackSpeed,
    PercentageAttackDamage,
    PercentageArmor,
    PercentageSpeed,
    PercentageAttackSpeed,
    Range,
    PercentageRange
}