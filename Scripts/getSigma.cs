using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using System;

public class getSigma : MonoBehaviour
{

    private string url = "https://api.crypto2099.io/v1/sigma/";
    string completeURL = "";

    public string AddressString;

    [SerializeField]
    TextMeshProUGUI epoch, d, nonce, total, active, sigma, address, ticker;

    [SerializeField]
    TMP_InputField AddressInput;

    [SerializeField]
    ReadPoolsEx RPEX;

    [SerializeField]
    GameObject loadingObject;

    // Start is called before the first frame update
    void Start()
    {
        epoch.SetText("Enter Pool ID To Calculate Sigma:");
        d.SetText("");
        nonce.SetText("");
        total.SetText("");
        active.SetText("");
        sigma.SetText("");
        address.SetText("");
        ticker.SetText("");
        sigma.gameObject.SetActive(false);
        nonce.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        getSigmaAddress();
    }

    public void getSigmaAddress()
    {
        loadingObject.SetActive(true);

        AddressString = AddressInput.text;
            completeURL = url + AddressString;
            StartCoroutine(getJson());

    }

    IEnumerator getJson()
    {
        UnityWebRequest www;
        Debug.Log(completeURL);
        www = UnityWebRequest.Get(completeURL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error + "!");
        }
        else
        {
            string s = www.downloadHandler.text;


            SetParams(s);
            //var j = JSON.Parse(s);
            //float tempPrice = j["data"]["market_data"]["price_usd"];

        }

    }

    void SetParams(string s)
    {
        var j = JSON.Parse(s);

        epoch.SetText("Epoch: " + j["epoch"] + " (Next Epoch)");
        d.SetText("d: " + j["d"] + "%");
        nonce.SetText("Epoch Nonce: " +j["nonce"]);
        nonce.gameObject.SetActive(true);
        total.SetText("Total Staked: " + ToB(j["total_staked"]) + "₳");

        loadingObject.SetActive(false);

        try
        {
            if (AddressString != "")
            {
                active.SetText("Pool's Active Staked: " + ToB(j["active_stake"]) + "₳");
                sigma.SetText("Sigma: " + j["sigma"]);
                sigma.gameObject.SetActive(true);
                address.SetText("Hash ID: " + RPEX.GetPool(AddressString).hash_id);
                ticker.SetText("Pool Ticker: " + RPEX.GetPool(AddressString).db_ticker);
            }
        }catch(Exception e)
        {
            active.SetText("Can't Find Pool");
        }


    }

    string ToB(long? x)
    {
        float y = (float)x / 1000000f;

        if (y <= 999)
        {
            return y.ToString();
        }
        else if (y > 999 && y <= 999999)
        {
            return (Convert.ToSingle(y) / 1000f).ToString("F2") + "k";
        }
        else if (y > 999999 && y < 99999999)
        {
            return (Convert.ToSingle(y) / 1000000f).ToString("F2") + "m";
        }
        else if(y > 999999999)
        {
            return (Convert.ToSingle(y) / 1000000000f).ToString("F2") + "b";
        }

        return "";
    }

}
