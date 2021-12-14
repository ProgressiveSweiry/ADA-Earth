using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class ReadPools : MonoBehaviour
{
    string url = "https://ada-pi.online/j/pools.json";
    string[] s;

    private List<PoolsJson> pools;
    public PoolsJson[] temp;

    [SerializeField]
    public MetaDataJson[] metaArray;

    public bool requestLocation = false;

    [SerializeField]
    Animator anim;

    [SerializeField]
    Text loadingText;

    string loadingPercent = "";

    public bool loading = false;

    [SerializeField]
    GameObject SearchPoolUI, ParamsUI;

    [SerializeField]
    GameObject ExplorerPanel, HideButton;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        pools = new List<PoolsJson>();
        requestLocation = PlayerPrefsX.GetBool("req");

        string tmp = ReadString();

        if(tmp.Length < 10)
        {
            StartCoroutine(getJson());
            Debug.Log("Read From Online JSON");
        }
        else
        {
            deserializeJson(tmp.Split('\n'));
            Debug.Log("Read From File");
        }


        loading = true;
    }


    IEnumerator getJson()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.certificateHandler = new BypassCertificate();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string temp = www.downloadHandler.text;
            s = temp.Split('\n');


            WriteString(temp);
            deserializeJson(s);
        }
    }

    void deserializeJson(string[] sArray)
    {
        temp = null;
        int counter = 1;

        foreach (string x in sArray)
        {
            try
            {
                pools.Add(JsonConvert.DeserializeObject<PoolsJson>(x));
            }catch(Exception e)
            {}
        }

        while (temp == null)
        try
        {
            temp = new PoolsJson[pools[pools.Count - counter].hash_id];
        }catch(Exception e)
        {
                counter++; 
        }



        foreach (PoolsJson p in pools)
        {
            try
            {
                temp[p.hash_id] = p;
            }
            catch (Exception e) { }
        }



        int empty = 0;

        foreach(PoolsJson x in temp)
        {
            if(x == null)
            {
                empty++;
            }
        }

        metaArray = new MetaDataJson[temp.Length - empty];


        StartCoroutine(DelayCall(empty));
       

    }

    IEnumerator DelayCall(int empty)
    {
        yield return new WaitForSeconds(0.1f);
        GetLocation getLocation = gameObject.GetComponent<GetLocation>();
        if (requestLocation)
        {
            getLocation.createJson(temp);
            foreach (PoolsJson x in temp)
            {
                if (x != null && x.url != null)
                    StartCoroutine(GetMetaJson(x.url, x.hash_id));
                yield return new WaitForSeconds(0.05f);
                try
                {
                    loadingPercent = (((float)x.hash_id / (float)temp.Length) * 100f).ToString("F0");
                }
                catch (Exception e) { }
            }
            yield return new WaitForSeconds(0.1f);

            SaveToPrefs();
        }
        else
        {
            LoadFromPrefs();
        }

        //Debug.Log("Active Pools: " + (temp.Length - empty));

       
        getLocation.meta = metaArray;
        getLocation.readPools = this;
        if (requestLocation)
        {
            
        }
        else
        {
            getLocation.Rpools = temp;
            getLocation.LoadFromPrefs();
        }

        loadingPercent = "";
        requestLocation = false;
        loading = false;
        PlayerPrefsX.SetBool("req", requestLocation);

    }

    void SaveToPrefs()
    {
        Debug.Log("Saving Meta: " + metaArray.Length);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/meta.gd");
        bf.Serialize(file, metaArray);
        file.Close();

        LoadFromPrefs();

    }

    public MetaDataJson[] LoadFromPrefs()
    {
        //loading = true;

        if (File.Exists(Application.persistentDataPath + "/meta.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/meta.gd", FileMode.Open);
            metaArray = (MetaDataJson[])bf.Deserialize(file);
            file.Close();
        }
        else
        {
            Debug.Log("No Meta Data Saved...");
            return null;
        }



        return metaArray;

    }

    public void toggleRequest()
    {
        requestLocation = !requestLocation;
        PlayerPrefsX.SetBool("req", requestLocation);
        StartCoroutine(getJson());
    }

    public void MakeMetaData(string pool, int position)
    {
        try
        {
            metaArray[position] = JsonConvert.DeserializeObject<MetaDataJson>(pool);
            Debug.Log("Name: " + metaArray[position].name + " Ticker: " + metaArray[position].ticker);
        }catch(Exception e)
        {
            Debug.Log("Couldn't get meta data " + position);
        }

        
    }

    
    IEnumerator GetMetaJson(string url, int position)
    {

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.certificateHandler = new BypassCertificate();
        yield return www.SendWebRequest();
        yield return new WaitForSeconds(0.02f);

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string temp = www.downloadHandler.text;
            MakeMetaData(temp, position);
            
        }
    }

    private void LateUpdate()
    {
        anim.SetBool("isLoading", requestLocation);

        if(!requestLocation)
            loadingText.text = "Sync";
        else if(requestLocation)
            loadingText.text = "Syncing " + loadingPercent + "%";
        else if(loading)
            loadingText.text = "Loading...";


    }

    public int IDToHashID(string id)
    {
        if(metaArray != null)
        foreach(PoolsJson p in pools)
        {
            if(id == p.view && p != null)
            {
                return p.hash_id;
            }
            else if(id == "" || id == " ")
            {
                return 1;
            }
        }

        return 1;
    }

    public void Menu()
    {
        SearchPoolUI.SetActive(false);
        HideButton.SetActive(true);
        ExplorerPanel.SetActive(true);
        ParamsUI.SetActive(false);
    }


    public void StartSearchUI()
    {
        SearchPoolUI.SetActive(true);
        HideButton.SetActive(false);
        ExplorerPanel.SetActive(false);
    }

    public void StartParamsUI()
    {
        SearchPoolUI.SetActive(false);
        HideButton.SetActive(false);
        ExplorerPanel.SetActive(false);
        ParamsUI.SetActive(true);
    }

    public void ToggleExplorerPanel()
    {
        ExplorerPanel.SetActive(!ExplorerPanel.activeSelf);

    }

    public class PoolsJson
    {
        /*{"hash_id":1,"pledge":450000000000,"reward_addr_id":61,"active_epoch_no":210,"meta_id":1,"margin":0.015,"fixed_cost":340000000,"hash_raw":"\\\\x153806dbcd134ddee69a8c5204e38ac80448f62342f8c23cfe4b7edf","view":"pool1z5uqdk7dzdxaae5633fqfcu2eqzy3a3rgtuvy087fdld7yws0xt","url":"https://raw.githubusercontent.com/Octalus/cardano/master/p.json",
         * "update_id":1,"ipv4":"54.220.20.40","ipv6":null,"dns_name":null,"dns_srv_name":null,"port":3002,"pool_hash_id":1} */
        public int hash_id { get; set; }
        public long pledge { get; set; }
        public int reward_addr_id { get; set; }
        public int active_epoch_no { get; set; }
        public int meta_id { get; set; }
        public float margin { get; set; }
        public long fixed_cost { get; set; }
        public string hash_raw { get; set; }
        public string view { get; set; }
        public string url { get; set; }
        public int update_id { get; set; }
        public string ipv4 { get; set; }
        public string ipv6 { get; set; }
        public string dns_name { get; set; }
        public string dns_srv_name { get; set; }
        public int port { get; set; }
        public int pool_hash_id { get; set; }
    }

    [System.Serializable]
    public class MetaDataJson
    {
        /*{"name":"ADAPI Future Pool","ticker":"ADAPI","description":"Using The Power Of The RPI For The Best!","homepage":"https://ada-pi.online","nonce":"1602346099",
         * "extended":"https://ada-pi.online/extended.json"}
          */
        public string name { get; set; }
        public string ticker { get; set; }
        public string description { get; set; }
        public string homepage { get; set; }
        public string extended { get; set; }


    }

  

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }

    void WriteString(string t)
    {
        //string path = "Assets/Resources/pools.txt";
        string path = Application.persistentDataPath + "/pools.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(t);
        writer.Close();

    }

    string ReadString()
    {
        string path = Application.persistentDataPath + "/pools.txt";

        FileStream file = File.Open(path , FileMode.OpenOrCreate, FileAccess.ReadWrite);
        string s;

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(file);
        s = reader.ReadToEnd();
        reader.Close();

        return s;
    }



}
