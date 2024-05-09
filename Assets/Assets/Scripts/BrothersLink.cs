using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrothersLink : MonoBehaviour
{
    private Entity pointA;
    private Entity pointB;
    public LineRenderer lineRenderer;

    bool rendering;
    bool Rendering
    {
        get
        {
            return rendering;
        }
        set
        {
            rendering = value;
            lineRenderer.gameObject.SetActive(value);
        }
    }

    public void SetLink(Entity entityA, Entity entityB)
    {
        pointA = entityA;
        pointB = entityB;
        Rendering = true;
    }


    private void Update()
    {
        if (Rendering)
        {
            if (pointA && pointB)
            {
                lineRenderer.SetPosition(0, pointA.CenterPoint);
                lineRenderer.SetPosition(1, pointB.CenterPoint);

                float distance = Vector3.Distance(pointA.transform.position, pointB.transform.position);
                float maxDistanceBeforeBlend = 3f;
                float maxDistanceAfterBlend = 6f;

                float opacity = Mathf.InverseLerp(maxDistanceAfterBlend, maxDistanceBeforeBlend, distance);

                Color color = lineRenderer.material.color;
                color.a = opacity;
                lineRenderer.material.color = color;

                lineRenderer.enabled = distance <= maxDistanceAfterBlend;
            }
            else
            {
                Rendering = false;
            }
        }
    }
}
