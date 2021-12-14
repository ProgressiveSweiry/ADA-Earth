using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PoolPanelManager : MonoBehaviour
{

    [SerializeField]
    ReadPoolsEx RPEX;

    ReadPoolsEx.ExJson ADAPI;

    [SerializeField]
    Slider stakeSlider;

    [SerializeField]
    TextMeshProUGUI stakeText;

    [SerializeField]
    GameObject adapiPanel, explorer, info, viewButton, exitButton;

    public float stake;

    [SerializeField]
    AdManager AM;

    private void Awake()
    {
        exitButton.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //ShowSupportPanel();
        StartCoroutine(WaitForLoad());
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(0.3f);

        if (RPEX.isReady)
        {
            ADAPI = RPEX.GetPool("1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36");
            SetStake();
        }
        else
        {
            StartCoroutine(WaitForLoad());
        }

    }

    void SetStake()
    {
        float stake = (float)ADAPI.total_stake / 1000000f;
        stakeSlider.value = stake / 2000000f;


        stakeText.SetText("Live Stake: " + ToM((long)stake).ToString() + "₳ / 2M₳");

        if(stake >= 2000000f)
        {
            ShowMenu();
        }
        else
        {
            AM.UnderStake = true;           
        }

        exitButton.SetActive(true);

    }


    string ToM(long? x)
    {
        float y = (float)x;

        if (y <= 999)
        {
            return y.ToString();
        }
        else if (y > 999 && y <= 999999)
        {
            return (Convert.ToSingle(y) / 1000f).ToString("F2") + "k";
        }
        else if (y > 999999)
        {
            return (Convert.ToSingle(y) / 1000000f).ToString("F2") + "m";
        }

        return "";
    }

    public void ShowMenu()
    {
        explorer.SetActive(true);
        info.SetActive(true);
        viewButton.SetActive(true);
        adapiPanel.SetActive(false);


    }

    void ShowSupportPanel()
    {
        explorer.SetActive(false);
        info.SetActive(false);
        viewButton.SetActive(false);
        adapiPanel.SetActive(true);
    }

}
