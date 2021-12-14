using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class GetLocation : MonoBehaviour
{
    public ReadPools readPools;

    public ReadPools.PoolsJson[] Rpools;
    public string[] poolsDNS;
    public ReadPools.MetaDataJson[] meta;

    public LocationJson[] poolsLoc;
    public GameObject[] poolsObjects;

    [SerializeField]
    GameObject locationbeacon;

    [SerializeField]
    Transform poolsLocationHolder;

    [SerializeField]
    LoadPoolUI LPUI;

    string poolsI;

    [SerializeField]
    Slider limitGage;


    void Start()
    {

    }

    public void createJson(ReadPools.PoolsJson[] pools)
    {
        Rpools = pools;
        //StartCoroutine(getJson());

        poolsI = "";
        poolsDNS = new string[pools.Length];


        StartCoroutine(DelayCall(pools));

    }

    IEnumerator DelayCall(ReadPools.PoolsJson[] pools)
    {

        foreach (ReadPools.PoolsJson pool in pools)
        {
            if (pool != null && pool.hash_id > 0)
                StartCoroutine(getJsonIndividual(pool, pool.hash_id));
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1f);



        string tempDNS = "";

        foreach (string x in poolsDNS)
        {
            tempDNS += x;
        }

        SaveToPrefs(poolsDNS);
        deserializeJson(poolsDNS);

    }


    IEnumerator getJsonIndividual(ReadPools.PoolsJson pool, int id)
    {

        if (pool != null)
        {
            string url = "";

            try
            {
                if (pool.ipv4 != null)
                    url = "http://freegeoip.app/json/" + pool.ipv4;
                else if (pool.dns_name != null)
                    url = "http://freegeoip.app/json/" + pool.dns_name;

            }
            catch (Exception e)
            {
                Debug.Log(e.GetBaseException());


            }


            UnityWebRequest www = UnityWebRequest.Get(url);
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();


            try
            {
                if (www.isNetworkError || www.isHttpError)
                {
                    //Debug.Log(www.error);
                }
                else
                {
                    string temp = www.downloadHandler.text;
                    poolsI += temp;
                    Debug.Log(poolsI.Length);
                    poolsDNS[id] = temp;

                    if (id == 1164)
                        Debug.Log(pool.hash_id + "-----------------------------------------------------------");

                }
            }
            catch (Exception e) { }

        }



    }


    IEnumerator getJson()
    {

        poolsDNS = new string[Rpools.Length];
        string tempS = "";

        foreach (ReadPools.PoolsJson p in Rpools)
        {
            if (p != null)
            {
                string url = "";

                try
                {
                    if (p.ipv4 != null)
                        url = "http://freegeoip.app/json/" + p.ipv4;
                    else if (p.dns_name != null)
                        url = "http://freegeoip.app/json/" + p.dns_name;
                    else
                        Debug.Log(p.ipv4 + "+" + p.dns_name);

                }
                catch (Exception e)
                {
                    Debug.Log(e.GetBaseException());


                }


                UnityWebRequest www = UnityWebRequest.Get(url);
                www.certificateHandler = new BypassCertificate();
                yield return www.SendWebRequest();


                try
                {
                    if (www.isNetworkError || www.isHttpError)
                    {
                        //Debug.Log(www.error);
                    }
                    else
                    {
                        string temp = www.downloadHandler.text;
                        poolsDNS[p.hash_id] = temp;
                        tempS += temp;
                        Debug.Log(p.hash_id);

                    }
                }
                catch (Exception e) { }

            }
        }



        deserializeJson(poolsDNS);
    }

    void SaveToPrefs(string[] pools)
    {
        Debug.Log("Saving....");
        Debug.Log("Save Size: " + pools.Length);
        int counter = 0;

        foreach (string x in pools)
        {
            PlayerPrefs.SetString(counter.ToString(), x);
            counter++;
        }

        PlayerPrefs.SetInt("counter", counter);

        //LoadFromPrefs();
    }

    public void LoadFromPrefs()
    {
        int counter = PlayerPrefs.GetInt("counter");
        string[] tempA = new string[counter + 1];

        for (int i = 0; i < counter + 1; i++)
        {
            tempA[i] = PlayerPrefs.GetString(i.ToString());
        }


        deserializeJson(tempA);
    }


    void deserializeJson(string[] pools)
    {

        poolsLoc = new LocationJson[pools.Length];
        poolsObjects = new GameObject[pools.Length];

        for (int i = 0; i < poolsLoc.Length; i++) {

            try
            {
                poolsLoc[i] = JsonConvert.DeserializeObject<LocationJson>(pools[i]);
            }
            catch (Exception e) { }
        }



        for (int i = 0; i < poolsLoc.Length; i++)
        {
            LocationJson l = poolsLoc[i];

            try
            {
                if (l.longitude != 0 || l.latitude != 0)
                {
                    if (l.longitude >= 180f)
                        l.longitude = l.longitude / 2f;
                    else if (l.latitude >= 90f)
                        l.latitude = l.latitude / 2f;

                    Quaternion tempR = Quaternion.Euler(new Vector3(l.latitude, (l.longitude * -1f), 0f));
                    GameObject tempBeacon = Instantiate(locationbeacon, poolsLocationHolder.transform.position, locationbeacon.transform.rotation, poolsLocationHolder);

                    poolsObjects[i] = tempBeacon;


                    if (meta[i].ticker != null && Rpools[i].hash_id != 0)
                    {
                        tempBeacon.name = meta[i].ticker;
                        tempBeacon.GetComponent<ParticleFollower>().hash_id = i;
                        tempBeacon.GetComponentInChildren<TMP_Text>().SetText(meta[i].ticker);
                        //tempBeacon.GetComponentInChildren<TextMeshPro>().gameObject.transform.localPosition = new Vector3(tempBeacon.GetComponentInChildren<TextMeshPro>().gameObject.transform.position.x, tempBeacon.GetComponentInChildren<TextMeshPro>().gameObject.transform.position.y - (i % 2) * i * 0.001f, tempBeacon.GetComponentInChildren<TextMeshPro>().gameObject.transform.position.z - i * 0.05f);
                        tempBeacon.GetComponentInChildren<TMP_Text>().gameObject.transform.localPosition = new Vector3(tempBeacon.GetComponentInChildren<TMP_Text>().gameObject.transform.position.x, tempBeacon.GetComponentInChildren<TMP_Text>().gameObject.transform.position.y - (i % 2) * i * 0.001f, tempBeacon.GetComponentInChildren<TextMeshPro>().gameObject.transform.position.z - Rpools[i].pledge * 0.000000000008f);
                        Vector3 tempScale = tempBeacon.transform.localScale;
                        tempBeacon.GetComponentInChildren<TMP_Text>().color = new Color(UnityEngine.Random.Range(0F, 1F), UnityEngine.Random.Range(0, 1F), UnityEngine.Random.Range(0, 1F), 0.85f);
                        tempScale = new Vector3(tempScale.x + Rpools[i].pledge * 0.0000000000005f, tempScale.y + Rpools[i].pledge * 0.0000000000005f, tempScale.z);

                        if (i == 1164)
                        {
                            tempScale = new Vector3(tempScale.x + Rpools[i].pledge * 0.000000001f, tempScale.y + Rpools[i].pledge * 0.000000001f, tempScale.z);
                            tempBeacon.GetComponentInChildren<TMP_Text>().color = new Color(1f, 0.843f, 0f, 0.9f);
                            tempBeacon.GetComponentInChildren<TMP_Text>().gameObject.transform.localPosition = new Vector3(tempBeacon.GetComponentInChildren<TMP_Text>().gameObject.transform.position.x, tempBeacon.GetComponentInChildren<TMP_Text>().gameObject.transform.position.y - (i % 2) * i * 0.001f, tempBeacon.GetComponentInChildren<TextMeshPro>().gameObject.transform.position.z - Rpools[i].pledge * 0.000000000008f * 500f);
                        }

                        tempBeacon.transform.localScale = tempScale;
                        //tempBeacon.GetComponent<ParticleFollower>().marginScale = new Vector3(tempScale.x + Rpools[i].margin * 0.01f, tempScale.y + Rpools[i].margin * 0.01f , tempScale.z);
                        
                    }
                    else
                    {
                        tempBeacon.GetComponentInChildren<TMP_Text>().text = "";
                    }



                    tempBeacon.transform.localRotation = tempR;
                }



            }
            catch (Exception e) {
                e.GetBaseException();
            }
        }

    }

    public void TogglePool(int hashid)
    {
        if (hashid != 0)
        {
            for (int i = 0; i < poolsObjects.Length; i++)
            {
                if (i != hashid)
                {
                    try
                    {
                        poolsObjects[i].SetActive(false);
                    }catch(Exception e)
                    {
                        e.GetBaseException();
                    }
                }
            }
        }
        else
        {
            foreach(GameObject pool in poolsObjects)
            {
                try
                {
                    pool.SetActive(true);
                }catch(Exception e)
                {
                    e.GetBaseException();
                }
            }
        }

    }

    public void TogglePoolPledge()
    {
        StartCoroutine(SetPledgeLimit());
    }

    IEnumerator SetPledgeLimit()
    {
        CameraCtrl.canMove = false;

        


        yield return new WaitForSeconds(0.1f);
        //Disable Pools Objects Below Limit
        for (int i = 0; i < poolsObjects.Length; i++)
        {
            try
            {
                if ((Rpools[i].pledge / 1000000f) - Mathf.Abs(limitGage.maxValue - limitGage.value) > 0)
                {
                    poolsObjects[i].SetActive(false);
                }
                else
                {
                    poolsObjects[i].SetActive(true);
                }
            }
            catch (Exception e) { }
        }

        if (limitGage.value == 0)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject pool in poolsObjects)
            {
                try
                {
                    pool.SetActive(true);
                }
                catch (Exception e)
                {
                    e.GetBaseException();
                }
            }
        }

        

        CameraCtrl.canMove = true;
    }



    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }

    public class LocationJson
    {
        /*{"ip":"176.230.129.18","country_code":"IL","country_name":"Israel",
        "region_code":"M","region_name":"Central District","city":"Petaẖ Tiqwa","zip_code":"","time_zone":"Asia/Jerusalem","latitude":32.0876,"longitude":34.8739,"metro_code":0}
        */

        public string ip { get; set; }
        public string country_name { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string time_zone { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }


    }

}
