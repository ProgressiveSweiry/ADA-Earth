using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class ReadBlock : MonoBehaviour
{
    string url = "https://ada-pi.online/j/block.json";
    string url2 = "https://api.adaex.org/blocks.json";
    string s;

    [SerializeField]
    bool isUsingURL2;

    ReadPools RP;

    [SerializeField]
    ProgressController PC;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loopCall());
        RP = GameObject.Find("PoolsHandle").GetComponent<ReadPools>();
    }

    IEnumerator loopCall()
    {

        StartCoroutine(getJson());        
        yield return new WaitForSecondsRealtime(10f);
        s = "";
        StartCoroutine(loopCall());
        

    }

    IEnumerator getJson()
    {
        UnityWebRequest www;

        if(isUsingURL2)
             www = UnityWebRequest.Get(url2);
        else
             www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            
        }
        else
        {
            deserializeJson(www.downloadHandler.text);

        }

    }

    void deserializeJson(string j)
    {

        BlockExJson b2;
        BlockJson b;

        if (isUsingURL2 && RP.metaArray != null)
        {
            string[] temp = j.Split('}');
            string tempS = temp[0] + "}";
            string[] tempS2 = tempS.Split('[');


            try
            {
                b2 = JsonConvert.DeserializeObject<BlockExJson>(tempS2[1].ToString());
                PC.SetSlot(b2.slotNo, b2.epochNo);
            }
            catch (Exception e) { }
        }
        else
        {
            b = JsonConvert.DeserializeObject<BlockJson>(j);
            Debug.Log(b.epoch_no + " + " + b.slot_no + " + " + b.block_no + " + " + b.slot_leader_id + " + " + b.time);
        }


    }

    private class BlockJson
    {
        //{"epoch_no":229,"slot_no":13695283,"block_no":4944989,"slot_leader_id":73,"time":"2020-11-13T09:59:34"}

        public int epoch_no { get; set; }
        public int slot_no { get; set; }
        public int block_no { get; set; }
        public int slot_leader_id { get; set; }
        public string time { get; set; }

    }

    private class BlockExJson
    {
        /*{"id":"760086a4b2094fbfb73fa2ed0950de7a5baf194e0dd51b18da4b1720e024e264","epochNo":"230","slotNo":"401198","createdAt":"2020-11-21 13:11:29","transactionsCount":"1",
        "output":"10210627585","fees":"172937","createdBy":"\\x43a7cf27c04747c306e744e3c51c01bb01c64e5a8e7e46c46f9cdbdc",
        "createdByInfo":"https:\/\/adapools.org\/pool\/43a7cf27c04747c306e744e3c51c01bb01c64e5a8e7e46c46f9cdbdc","size":"396",
        "pool_id":"43a7cf27c04747c306e744e3c51c01bb01c64e5a8e7e46c46f9cdbdc"}
        */

        public string id { get; set; }
        public int epochNo { get; set; }
        public int slotNo { get; set; }
        public string createdAt { get; set; }
        public int transactionsCount { get; set; }
        public long output { get; set; }
        public int fees { get; set; }
        public string createdBy { get; set; }
        public int size { get; set; }
        public string pool_id { get; set; }

    }



    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

}
