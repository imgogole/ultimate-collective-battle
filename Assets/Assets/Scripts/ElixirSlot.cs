using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ElixirSlot : MonoBehaviour
{
    public Color filledColor, unfilledColor;
    public Image slotImage;
    public float slotSize;

    private float slotHeight = 30f;

    private float value;

    private void Start()
    {
        value = 0f;
    }

    public void SetValue(float _value)
    {
        value = Mathf.Clamp01(_value);

        if (value == 1f)
        {
            slotImage.color = filledColor;
        }
        else
        {
            slotImage.color = unfilledColor;
        }

        slotImage.rectTransform.sizeDelta = new Vector2(value * slotSize, slotHeight);
    }
}
