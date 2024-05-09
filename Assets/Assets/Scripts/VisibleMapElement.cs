using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleMapElement : MonoBehaviour
{
    public bool HideOnCollision;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = !HideOnCollision;    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            meshRenderer.enabled = !HideOnCollision;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            meshRenderer.enabled = HideOnCollision;
        }
    }
}
