using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElixirBar : MonoBehaviour
{
    public List<ElixirSlot> elixirSlots = new List<ElixirSlot>();
    public GameObject elixirValueImage;
    public TMP_Text elixirValueText;

    private static ElixirBar instance;
    public static ElixirBar Instance => instance;

    Entity me;
    bool check = false;

    private void Awake()
    {
        instance = this;
        check = false;
    }

    public void Init(Entity _me)
    {
        me = _me;
        check = true;
    }

    private void Update()
    {
        if (check)
        {
            float elixirValue = me.ElixirValue;
            int elixirFilledSlots = Mathf.FloorToInt(elixirValue);
            float remainingElixirFilledSlots = elixirValue - elixirFilledSlots;

            foreach (ElixirSlot slot in elixirSlots)
            {
                slot.SetValue(0f);
            }

            for (int i = 0; i < elixirFilledSlots; i++)
            {
                elixirSlots[i].SetValue(1f);
            }

            if (elixirFilledSlots < 10f)
            {
                elixirSlots[elixirFilledSlots].SetValue(remainingElixirFilledSlots);
            }

            elixirValueText.text = elixirFilledSlots.ToString();
        }
    }
}

