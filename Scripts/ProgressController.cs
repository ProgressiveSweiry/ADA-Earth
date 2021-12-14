using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressController : MonoBehaviour
{

    [SerializeField]
    Image fillImage;

    [SerializeField]
    TMP_Text percentText, epochText, slotText;


    // Start is called before the first frame update
    void Start()
    {
        fillImage.fillAmount = 0f;
        percentText.SetText("");

    }

    public void SetSlot(int slot, int epoch)
    {
        if (epoch != 0)
        {
            float amount = 0f;

            amount = slot / 432000f;
            fillImage.fillAmount = amount;

            amount = amount * 100f;

            percentText.SetText(amount.ToString("F0") + "%");
            epochText.SetText("Epoch: " + epoch.ToString());
            slotText.SetText("Slot: " + slot.ToString());
        }
        else
        {
            epochText.SetText("Loading...");
            slotText.SetText("");
        }

    }
}
