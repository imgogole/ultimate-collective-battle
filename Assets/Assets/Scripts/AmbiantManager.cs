using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AmbiantManager : MonoBehaviour
{
    private AmbientOcclusion GameAmbientOcclusion;
    public PostProcessVolume GamePostProcess;
    public Light GameLight;

    public float Speed;

    public Color DefaultLightColor;
    public Color LouUltimateLightColor;

    public Color DefaultPPLightColor;
    public Color LouUltimatePPLightColor;

    public float DefaultIntensityAmbiantOcclusion;
    public float LouUltimateIntensityAmbiantOcclusion;

    Color Target_LightColor;
    Color Target_PPLightColor;
    float Target_IntensityAmbiantOcclusion;

    AmbiantGoal _Goal;
    bool _Check;

    private void Start()
    {
        _Check = GamePostProcess.profile.TryGetSettings(out GameAmbientOcclusion);
        SetTarget(AmbiantGoal.Default);

        Debug.Log($"Initialisation des Ambient Occlusion : {_Check}");
    }

    /// <summary>
    /// Set the preset of the current light.
    /// </summary>
    /// <param name="goal"></param>
    public void SetTarget(AmbiantGoal goal)
    {
        _Goal = goal;
        if (goal == AmbiantGoal.Default)
        {
            Target_LightColor = DefaultLightColor;
            Target_PPLightColor = DefaultPPLightColor;
            Target_IntensityAmbiantOcclusion = DefaultIntensityAmbiantOcclusion;
        }
        else if (goal == AmbiantGoal.LouUltimate)
        {
            Target_LightColor = LouUltimateLightColor;
            Target_PPLightColor = LouUltimatePPLightColor;
            Target_IntensityAmbiantOcclusion = LouUltimateIntensityAmbiantOcclusion;
        }
    }

    public void Update()
    {
        if (_Check)
        {
            if (GameManager.Instance.InRound)
            {
                float timeValue = Time.deltaTime * Speed;
                GameLight.color = Color.Lerp(GameLight.color, Target_LightColor, timeValue);
                GameAmbientOcclusion.intensity.value = Mathf.Lerp(GameAmbientOcclusion.intensity.value, Target_IntensityAmbiantOcclusion, timeValue);
                GameAmbientOcclusion.color.value = Color.Lerp(GameAmbientOcclusion.color.value, Target_PPLightColor, timeValue);
            }
            else
            {
                SetTarget(AmbiantGoal.Default);

                GameLight.color = DefaultLightColor;
                GameAmbientOcclusion.intensity.value = DefaultIntensityAmbiantOcclusion;
                GameAmbientOcclusion.color.value = DefaultPPLightColor;
            }
        }
    }
}

public enum AmbiantGoal
{
    Default,
    LouUltimate
}

