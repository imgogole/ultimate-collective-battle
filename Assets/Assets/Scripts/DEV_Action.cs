using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DEV_Action : MonoBehaviour
{
    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.U))
        {
            GameObject obj = ObjectPooler.Instance.Spawn("damage", new Vector3(0, 1, -0.75f), Quaternion.identity);
            FloatingTextDamage floatingText = obj.GetComponent<FloatingTextDamage>();
        }
    }
}