using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class FloatingTextDamage : MonoBehaviour, IPoolObject
{
    public Rigidbody rb;
    public float force;
    public TMP_Text damageText;

    public float normalSize;
    public float criticalSize;

    public Color normalDamage;
    public Color criticalDamage;

    public void OnSpawn()
    {
        rb.velocity = Vector3.zero;
    }

    public void Init(float damage, bool isCritic)
    {
        damageText.text = damage.ToString("0.#", CultureInfo.InvariantCulture);
        damageText.color = isCritic ? criticalDamage : normalDamage;
        damageText.fontSize = isCritic ? criticalSize : normalSize;

        rb.AddForce(Random.Range(-0.2f, 0.2f) * force, force, 0f);

        Invoke(nameof(Unactive), 2f);
    }

    private void Unactive()
    {
        gameObject.SetActive(false);
    }
}
