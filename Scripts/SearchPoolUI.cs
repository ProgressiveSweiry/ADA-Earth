using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.Globalization;
using SimpleJSON;
using UnityEngine.Networking;

public class SearchPoolUI : MonoBehaviour
{
    [SerializeField]
    ReadPools RPMain;

    [SerializeField]
    GameObject listViewPort;

    [SerializeField]
    GameObject poolContent;

    [SerializeField]
    ReadPoolsEx RPEX;

    private List<GameObject> ObjList;

    [SerializeField]
    TMP_InputField searchInput;

    [SerializeField]
    ReadPools ReadPoolsHandler;

    bool oneTimeInit = false;

    string s = "";

    public Dictionary<string, ReadPoolsEx.ExJson> Dic;

    CultureInfo culture;

    // Start is called before the first frame update
    void Start()
    {
        culture = CultureInfo.CurrentCulture;
    }

    private void OnEnable()
    {
        if(!oneTimeInit)
        InitList();

        CameraCtrl.canMove = false;
        oneTimeInit = true;
    }

    private void OnDisable()
    {
        ObjList = new List<GameObject>();
        CameraCtrl.canMove = true;
    }

    public void InitList()
    {
        StartCoroutine(InitListE());
    }



    IEnumerator InitListE()
    {

        yield return new WaitForFixedUpdate();

        try
        {
            foreach (GameObject g in ObjList)
            {
                Destroy(g);
            }
        }
        catch (Exception e) { }

        foreach (Transform child in listViewPort.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        ObjList = new List<GameObject>();


        int count = Dic.Count;

        if (s == "" || s == null)
        {
            count = 25;


            //ADD ADAPI AT TOP OF THE LIST
            GameObject myPool = Instantiate(poolContent) as GameObject;
            myPool.name = "1164";

            myPool.transform.SetParent(listViewPort.transform, false);

            ObjList.Add(myPool);


            foreach (Transform t in myPool.transform)
            {
                try
                {
                    switch (t.tag)
                    {
                        case "Ticker":
                            t.GetComponent<TextMeshProUGUI>().SetText(Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].db_ticker);
                            break;

                        case "Saturation":
                            t.GetComponent<TextMeshProUGUI>().SetText("Saturation: " + (Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].saturated * 100f).ToString("F1") + "%");
                            break;

                        case "ROA":
                            t.GetComponent<TextMeshProUGUI>().SetText("ROA(Lifetime): " + Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].roa_lifetime.ToString("F3") + "%");
                            break;

                        case "Blocks":
                            t.GetComponent<TextMeshProUGUI>().SetText("Blocks: " + (Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].blocks_lifetime + Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].blocks_epoch).ToString("F0"));
                            break;

                        case "Name":
                            t.GetComponent<TextMeshProUGUI>().SetText(Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].db_name);
                            break;

                        case "Pledge":
                            t.GetComponent<TextMeshProUGUI>().SetText(ToM(Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].pledge) + "₳");
                            break;

                        case "Margin":
                            t.GetComponent<TextMeshProUGUI>().SetText((Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].tax_ratio * 100f).ToString() + "%");
                            break;


                        case "Stake":
                            t.GetComponent<TextMeshProUGUI>().SetText(ToM(Dic["1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36"].total_stake) + "₳");
                            break;
                    }
                }
                catch (Exception e) { }
            }
        }

        
        for (int i = 0; i < count; i++)
        {
            if (culture.CompareInfo.IndexOf(Dic.Values.ElementAt(i).db_ticker, s, CompareOptions.IgnoreCase) >= 0 || s == "")
            {
                GameObject pool = Instantiate(poolContent) as GameObject;
                pool.name = Dic.Values.ElementAt(i).hash_id.ToString();

                pool.transform.SetParent(listViewPort.transform, false);

                ObjList.Add(pool);


                foreach (Transform t in pool.transform)
                {
                    switch (t.tag)
                    {
                        case "Ticker":
                            t.GetComponent<TextMeshProUGUI>().SetText(Dic.Values.ElementAt(i).db_ticker);
                            break;

                        case "Saturation":
                            t.GetComponent<TextMeshProUGUI>().SetText("Saturation: " + (Dic.Values.ElementAt(i).saturated * 100f).ToString("F1") + "%");
                            break;

                        case "ROA":
                            t.GetComponent<TextMeshProUGUI>().SetText("ROA(Lifetime): " + Dic.Values.ElementAt(i).roa_lifetime.ToString("F1") + "%");
                            break;

                        case "Blocks":
                            t.GetComponent<TextMeshProUGUI>().SetText("Blocks: " + (Dic.Values.ElementAt(i).blocks_lifetime + Dic.Values.ElementAt(i).blocks_epoch).ToString("F0"));
                            break;

                        case "Name":
                            t.GetComponent<TextMeshProUGUI>().SetText(Dic.Values.ElementAt(i).db_name);                           
                            break;

                        case "Pledge":
                            t.GetComponent<TextMeshProUGUI>().SetText(ToM(Dic.Values.ElementAt(i).pledge) + "₳");
                            break;

                        case "Margin":
                            t.GetComponent<TextMeshProUGUI>().SetText((Dic.Values.ElementAt(i).tax_ratio * 100f).ToString() + "%");
                            break;


                        case "Stake":
                            t.GetComponent<TextMeshProUGUI>().SetText(ToM(Dic.Values.ElementAt(i).total_stake) + "₳");
                            break;
                    }
                }

            }

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

    public void UpdateSearch()
    {
        s = searchInput.text;
        InitList();
    }

    public void ReturnToMenu()
    {
        RPMain.Menu();
    }

    public void OpenAdapools()
    {
        Application.OpenURL("https://adapools.org/");
    }

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }

}
