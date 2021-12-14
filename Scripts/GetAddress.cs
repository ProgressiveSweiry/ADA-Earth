using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using System;

public class GetAddress : MonoBehaviour
{

    private string url = "https://explorer.cardano.org/api/addresses/summary/";
    string completeURL = "";

    public string AddressString;

    [SerializeField]
    TextMeshProUGUI Address, Block, Value, Output, Input, Fee;

    [SerializeField]
    TMP_InputField AddressInput;

    [SerializeField]
    GameObject loadingObject, copyObject;

    // Start is called before the first frame update
    void OnEnable()
    {
        Address.SetText("Enter An Address To Get Info:");
        copyObject.SetActive(false);
        Block.SetText("");
        Value.SetText("");
        Output.SetText("");
        Input.SetText("");
        Fee.SetText("");
        AddressInput.gameObject.SetActive(true);
    }

    public void getAddress()
    {
        loadingObject.SetActive(true);

        AddressString = AddressInput.text;
        completeURL = url + AddressString;
        StartCoroutine(getJson());
    }

    public void getAddressContact(string address)
    {
        loadingObject.SetActive(true);

        AddressInput.gameObject.SetActive(false);

        completeURL = url + address;
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

        if (j["Left"])
        {
            Address.SetText("Wrong Address (Or Server Error)");
        }
        else
        {
            copyObject.SetActive(true);
            Address.SetText("Address: " + j["Right"]["caAddress"]);
            Block.SetText("Transactios: " + j["Right"]["caTxNum"]);
            Value.SetText("Balance: " + ToM(j["Right"]["caBalance"]["getCoin"]) + "₳");
            Output.SetText("Total Output: " + ToM(j["Right"]["caTotalOutput"]["getCoin"]) + "₳");
            Input.SetText("Total Input: " + ToM(j["Right"]["caTotalInput"]["getCoin"]) + "₳");

            loadingObject.SetActive(false);

            float fee = j["Right"]["caTotalFee"]["getCoin"] / 1000000f;
            fee = Mathf.Abs(fee);

            if (fee > 100f)
                fee = fee / 1000000f;
            Debug.Log(fee);

            Fee.SetText("Total Fees: " + fee.ToString() + "₳");
        }
    }

    string ToM(long? x)
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
        else if (y > 999999)
        {
            return (Convert.ToSingle(y) / 1000000f).ToString("F2") + "m";
        }

        return "";
    }

}
