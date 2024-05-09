using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yassine_U_Tower : MonoBehaviour
{
    public Material redLightColor, greenLightColor;
    public ParticleSystem lightOpenParticleSystem, negativeParticleSystem;

    public MeshRenderer poleSphere;

    bool isOpen;
    float negativeEffectTime = 0f;

    public bool IsLightOpen => isOpen;

    private void Start()
    {
        negativeEffectTime = 0f;
    }

    private void Update()
    {
        if (negativeEffectTime > 0f)
        {
            negativeEffectTime -= Time.deltaTime;

            if (negativeEffectTime <= 0f)
            {

            }
        }
    }

    public void SetLight(bool isOpen)
    {
        this.isOpen = isOpen;
        poleSphere.material = isOpen ? greenLightColor : redLightColor;
        negativeParticleSystem.Stop();
        if (isOpen)
        {
            lightOpenParticleSystem.Play();
        }
    }

    public void ProkeNegativeEffect(float time)
    {
        negativeParticleSystem.Play();
        negativeEffectTime = time;
    }
}
