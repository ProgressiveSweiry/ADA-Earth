using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using TMPro;
using System;

public class ReadPrice : MonoBehaviour
{

    string url = "https://data.messari.io/api/v1/assets/ada/metrics";

    [SerializeField]
    TextMeshProUGUI PriceText, PriceText2, MarketCapText, ATHText;

    string s;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loopCall());
        UpdateParams();
    }

    public void UpdateParams()
    {
        StartCoroutine(getJson2());
        StartCoroutine(getJsonATH());
        StartCoroutine(getJsonMarketCap());
    }

    IEnumerator loopCall()
    {

        StartCoroutine(getJson());
        yield return new WaitForSecondsRealtime(100f);
        StartCoroutine(loopCall());
        s = "";


    }

    IEnumerator getJson()
    {
        UnityWebRequest www;

        www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string s = www.downloadHandler.text;
            var j = JSON.Parse(s);
            float tempPrice = j["data"]["market_data"]["price_usd"];
            PriceText.text = tempPrice.ToString("F3") + "$";
        }        

    }

    IEnumerator getJson2()
    {
        UnityWebRequest www;

        www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string s = www.downloadHandler.text;
            var j = JSON.Parse(s);
            float tempPrice = j["data"]["market_data"]["price_usd"];
            PriceText2.text = tempPrice.ToString("F5") + "$";
        }

    }

    IEnumerator getJsonMarketCap()
    {
        UnityWebRequest www;

        www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string s = www.downloadHandler.text;
            var j = JSON.Parse(s);
            float tempPrice = j["data"]["marketcap"]["current_marketcap_usd"];
            MarketCapText.text = ToB(tempPrice) + "$";
        }

    }

    IEnumerator getJsonATH()
    {
        UnityWebRequest www;

        www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string s = www.downloadHandler.text;
            var j = JSON.Parse(s);
            float tempPrice = j["data"]["all_time_high"]["price"];
            ATHText.text = tempPrice.ToString("F2") + "$";
        }

    }

    string ToB(float x)
    {
        float y = (float)x;

        if (y <= 999)
        {
            return y.ToString("F2");
        }
        else if (y > 999 && y <= 999999)
        {
            return (Convert.ToSingle(y) / 1000f).ToString("F2") + "k";
        }
        else if (y > 999999 && y <= 999999999)
        {
            return (Convert.ToSingle(y) / 1000000f).ToString("F2") + "m";
        }
        else if(y > 999999999)
        {
            return (Convert.ToSingle(y) / 1000000000f).ToString("F4") + "b";
        }

        return "";
    }


}
