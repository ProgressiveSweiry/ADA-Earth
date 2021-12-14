using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ReadProtocolParam : MonoBehaviour
{
    string url = "https://js.adapools.org/protocol.json";

    ParamJson PJ;

    [SerializeField]
    TextMeshProUGUI k, a0, tau, fixedfee, stakefee, poolfee, d;

    private void OnEnable()
    {
        StartCoroutine(getJson());
        
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
            DeserializeJson(www.downloadHandler.text);

        }

    }

    void DeserializeJson(string j)
    {
        PJ = JsonConvert.DeserializeObject<ParamJson>(j);
        try
        {
            SetParams();
        }
        catch (Exception e) {
            e.GetBaseException();
        }
    }

    void SetParams()
    {
        k.SetText("k = " + PJ.nOpt);
        a0.SetText("a0 = " + PJ.a0);
        tau.SetText("tau = " + (PJ.tau * 100) + "%");
        d.SetText("D = " + (PJ.decentralisationParam * 100) + "%");
        fixedfee.SetText("Minimum Fixed Fee = " + PJ.minPoolCost / 1000000f + "₳");
        stakefee.SetText("Minimum Stake Deposit = " + PJ.keyDeposit / 1000000f + "₳");
        poolfee.SetText("Minimum Pool Deposit = " + PJ.poolDeposit / 1000000f + "₳");
    }

    public class ParamJson
    {
        /*
         {
    "poolDeposit": 500000000,
    "protocolVersion": {
        "minor": 0,
        "major": 2
    },
    "minUTxOValue": 1000000,
    "decentralisationParam": 0.32,
    "maxTxSize": 16384,
    "minPoolCost": 340000000,
    "minFeeA": 44,
    "maxBlockBodySize": 65536,
    "minFeeB": 155381,
    "eMax": 18,
    "extraEntropy": {
        "tag": "NeutralNonce"
    },
    "maxBlockHeaderSize": 1100,
    "keyDeposit": 2000000,
    "nOpt": 500,
    "rho": 3.0e-3,
    "tau": 0.2,
    "a0": 0.3
}        
         */


        public long? poolDeposit { get; set; }
        public long? minUTxOValue { get; set; }
        public float decentralisationParam { get; set; }
        public long minPoolCost { get; set; }
        public long? maxBlockBodySize { get; set; }
        public long? keyDeposit { get; set; }
        public int nOpt { get; set; }
        public float tau { get; set; }
        public float a0 { get; set; }


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
