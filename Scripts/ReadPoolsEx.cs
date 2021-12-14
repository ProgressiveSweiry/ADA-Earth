using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using UnityEngine.UI;

public class ReadPoolsEx : MonoBehaviour
{
    
    public Dictionary<string, ExJson> tempJson;
    public ExJson[] tempJsonHash;
    public ExJson currentPool;

    [SerializeField]
    ReadPools RP;

    [SerializeField]
    SearchPoolUI SPUI;

    [SerializeField]
    Button SearchPoolButton;

    [SerializeField]
    GameObject poolButton;

    string temp;

    public bool isReady = false;

    //HASH RAW:
    public string poolID = "1fd1a22c53e08de6bae42053ce40d5c0f7e0b52b33eb728867505a36";


    private void Awake()
    {
        StartCoroutine(GetJson()); 
    }

    private void Start()
    {
        SearchPoolButton.gameObject.SetActive(false);
        poolButton.SetActive(false);
    }


    IEnumerator GetJson()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://js.adapools.org/pools.json");
        www.certificateHandler = new BypassCertificate();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            temp = www.downloadHandler.text;
            DeserializeJson(temp, www);
        }     
    }

    void DeserializeJson(string json, UnityWebRequest contentData)
    {
        tempJson = JsonConvert.DeserializeObject<Dictionary<string, ExJson>>(contentData.downloadHandler.text);

        Debug.Log(tempJson.Count);
        tempJsonHash = new ExJson[10000];

        foreach(KeyValuePair< string, ExJson > t in tempJson)
        {
            tempJsonHash[t.Value.hash_id] = t.Value;
        }

        StartCoroutine(WaitForLoad());
        SPUI.Dic = tempJson;
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(0.3f);

        if (RP.loading)
            StartCoroutine(WaitForLoad());
        else
        {
            SearchPoolButton.gameObject.SetActive(true);
            poolButton.SetActive(true);
            isReady = true;
        }
        

    }

    public String GetTwitter()
    {
        return "";
    }

    public ExJson GetPoolHash(int hash)
    {
        try
        {
            currentPool = tempJsonHash[hash];
        }
        catch (Exception e)
        {
            return null;
        }

        return currentPool;

    } 

    public ExJson GetPool(string ID)
    {

        try
        {
            currentPool = tempJson[ID];
        }
        catch (Exception e) {
            return null;
        }

        return currentPool;
    }

    public ExJson GetPoolRandom()
    {
        try
        {
            ExJson temp = GetPoolHash(UnityEngine.Random.Range(0, tempJson.Count));

            return temp;
        }catch(Exception e)
        {
            return null;
        }
    }

    [Serializable]
    public class ExJson
    {
        /* {"f5b2ef0d7db63c8d00446cd7d9ce9cdb9e73023ffaa5e806decceb66":
         * {"id":"1543","pool_id":"f5b2ef0d7db63c8d00446cd7d9ce9cdb9e73023ffaa5e806decceb66","db_ticker":"BLOOM","db_name":"Bloom Three","db_url":"https:\/\/bloompool.io\/","total_stake":"49726419058065","rewards_epoch":"232","tax_ratio":"0.04",
         * "tax_fix":"340000000","roa":"2.677","blocks_epoch":"16","blocks_lifetime":"42","blocks_est_lifetime":"40","stamp_strike":"0","hist_roa":"[{\"val\":null,\"time\":\"1605614004\",\"e\":229},{\"val\":null,\"time\":\"1605995340\",\"e\":230},
         * {\"val\":4.47,\"time\":\"1606427350\",\"e\":231},{\"val\":6.239,\"time\":\"1606859375\",\"e\":232}]","hist_bpe":"[{\"val\":\"0\",\"time\":\"1605614004\",\"e\":229},{\"val\":\"0\",\"time\":\"1605995340\",\"e\":230},{\"val\":\"16\",\"time\":\"1606427350\",\"e\":231},{\"val\":\"26\",\"time\":\"1606859375\",\"e\":232}]",
         * "pledge":"1250000000000","hash_id":"1543","ticker_orig":"BLOOM","metric":1766.9044436184,"delegators":"276","pledged":"1252018813385","roa_lifetime":"5.41","group_basic":"BLOOM","tax_ratio_old":"0.05",
         * "tax_fix_old":"340000000","tax_real":4.9596032732238,"active_stake":"37221983672641","active_blocks":"16","direct":false,"saturated":0.78338786749564,"rank":1,
         * "handles":{"tw":"bigpeyYT","tg":"bigpey","fb":"BloomPool.io","yt":"bigpey","tc":"","di":"R8hy3YC","gh":"","icon":"https:\/\/static.adapools.org\/pool_logo\/f5b2ef0d7db63c8d00446cd7d9ce9cdb9e73023ffaa5e806decceb66.png"},"blocks_estimated":25,"stake_x_deleg":180168184992.99}
    */

        public int? id { get; set; }
        public string pool_id { get; set; }
        public string db_ticker { get; set; }
        public string db_name { get; set; }
        public string db_url { get; set; }
        public long total_stake { get; set; }
        public int? rewards_epoch { get; set; }
        public float tax_ratio { get; set; }
        public long? tax_fix { get; set; }
        public float roa { get; set; }
        public float blocks_epoch { get; set; }
        public float blocks_lifetime { get; set; }
        public float? blocks_est_lifetime { get; set; }
        public long? pledge { get; set; }
        public int hash_id { get; set; }
        public float? metric { get; set; }
        public int? delegators { get; set; }
        public long? pledged { get; set; }
        public float roa_lifetime { get; set; }
        public float? tax_real { get; set; }
        public long? active_stake { get; set; }
        public int? active_blocks { get; set; }
        public float saturated { get; set; }
        public int? rank { get; set; }
        public string icon { get; set; }

        public Dictionary<string, string> handles;
    }

    public class MonoBehaviourOrScriptableObject : ScriptableObject
    {
        [SerializeReference] ExJson data;
    }

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }

   public string FilterAscii(string s)
    {

        List<char> tempC = new List<char>();

        foreach (char c in s)
        {
            int i = c;
            if(i < 178)
            {
                tempC.Add(c);
            }
        }

        var tempS = new String(tempC.ToArray());

        return tempS;
    }


}
