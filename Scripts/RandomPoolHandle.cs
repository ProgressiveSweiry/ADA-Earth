using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using SimpleJSON;

public class RandomPoolHandle : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI Ticker, Name, Stake, Pledge, Blocks;

    [SerializeField]
    Image poolImage;

    [SerializeField]
    ReadPoolsEx RPEX;

    [SerializeField]
    ReadPools RP;

    [SerializeField]
    GameObject Label, StakeL, PledgeL, BlocksL;

    private void OnEnable()
    {
        LoadPool();

    }

    public void LoadPool()
    {

        ReadPoolsEx.ExJson tempPool = RPEX.GetPoolRandom();

        if (tempPool == null)
        {
            try
            {
                Ticker.SetText("");
                Name.SetText("");
                Stake.SetText("");
                Pledge.SetText("");
                Blocks.SetText("");

                Label.SetActive(false);
                StakeL.SetActive(false);
                PledgeL.SetActive(false);
                BlocksL.SetActive(false);

                poolImage.gameObject.SetActive(false);
            

            LoadPool();
            }
            catch (Exception e)
            {

            }

        }
        else if (tempPool.total_stake > 10000000000000f)
        {
            LoadPool();
            
        }
        else if(tempPool.total_stake < 10000000000000f)
        {
            try
            {
                Label.SetActive(true);
                StakeL.SetActive(true);
                PledgeL.SetActive(true);
                BlocksL.SetActive(true);
            
            

            Ticker.SetText(tempPool.db_ticker);
            Name.SetText(tempPool.db_name);
            Stake.SetText(ToM(tempPool.total_stake ) + "₳");
            Pledge.SetText(ToM(tempPool.pledge) + "₳");
            Blocks.SetText(tempPool.blocks_lifetime.ToString());

            }
            catch (Exception e) { }
            /*
             * RP NEEDED TO BE SYNCED TO USE IMAGES!
             * 
            if(RP.LoadFromPrefs()[tempPool.hash_id].extended != null)
            StartCoroutine(GetJson(RP.LoadFromPrefs()[tempPool.hash_id].extended));
           */


        }



    }

    IEnumerator GetJ(string url)
    {
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.certificateHandler = new BypassCertificate();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var N = JSON.Parse(www.downloadHandler.text);
            try
            {
                //WHY WONT IT FIND THE EXTENDED?????
                Debug.Log(JSON.Parse(www.downloadHandler.text)["extended"].Value);
                StartCoroutine(GetJson(N["extended"].Value));
            }
            catch (Exception e)
            {
                Debug.Log("No URL PNG ICON");
            }
        }
    }

    IEnumerator GetJson(string url)
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
            var N = JSON.Parse(www.downloadHandler.text);
            try
            {
                StartCoroutine(DownloadImage(N["info"]["url_png_icon_64x64"].Value));
            }
            catch (Exception e)
            {
                Debug.Log("No URL PNG ICON");
            }
        }
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            poolImage.overrideSprite = null;
            poolImage.gameObject.SetActive(false);
        }
        else
        {
            poolImage.overrideSprite = Sprite.Create(((DownloadHandlerTexture)request.downloadHandler).texture, new Rect(0.0f, 0.0f, ((DownloadHandlerTexture)request.downloadHandler).texture.width, ((DownloadHandlerTexture)request.downloadHandler).texture.height), new Vector2(0.5f, 0.5f), 100f);
            //GOT IMAGE

            poolImage.gameObject.SetActive(true);
        }
    }

    string ToM(long? x)
    {
        float y = (float)x / 1000000f;

        if (y <= 999)
        {
            return y.ToString("F2");
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

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }
}
