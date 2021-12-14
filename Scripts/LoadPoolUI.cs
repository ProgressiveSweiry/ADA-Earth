using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

public class LoadPoolUI : MonoBehaviour
{

    [SerializeField]
    Camera MainCamera;

    [SerializeField]
    GetLocation LocationHandler;

    [SerializeField]
    ReadPools ReadPoolsHandler;

    public TextMeshProUGUI PoolID;
    public TextMeshProUGUI PoolTicker;
    public TextMeshProUGUI PoolName;
    public TextMeshProUGUI PoolLiveStake;
    public TextMeshProUGUI PoolActiveStake;
    public TextMeshProUGUI PoolPledge;
    public TextMeshProUGUI PoolROA;
    public TextMeshProUGUI PoolMargin;
    public TextMeshProUGUI PoolFixedFee;
    public TextMeshProUGUI PoolBlocks;
    public TextMeshProUGUI PoolEpochBlocks;
    public TextMeshProUGUI PoolCountry;
    public TextMeshProUGUI Delegators;
    public TextMeshProUGUI PoolStatus;
    public GameObject Divider;
    public Button exit;

    public Image PoolImage, AwardImage;

    [SerializeField]
    Sprite B1, B10, B100, B1000;

    public TextMeshProUGUI PoolLink;

    public GameObject ExplorerPanel;

    //ADD MORE ACCORDING TO EXJSON


    ReadPoolsEx ex;

    public bool Search = false;

    ReadPools RP;

    public ReadPoolsEx.ExJson exJ;
    private int HashID;

    Color tmpcolor;

    bool isVisible = false;

    Image panel;

    int LastHash;

    string extendedJsonText;

    [SerializeField]
    GameObject HideButton;

    [SerializeField]
    GameObject SearchPoolUI;

    string twitterHandler, telegramHandler, youtubeHandler;

    [SerializeField]
    GameObject TwButton, TeButton, YoButton;

    [SerializeField]
    AdManager adManager;

    void Start()
    {
        tmpcolor = Color.black;
        tmpcolor.a = 0.7f;
        RP = GameObject.FindGameObjectWithTag("PoolsHandle").GetComponent<ReadPools>();
        ex = GameObject.Find("PoolsHandle").GetComponent<ReadPoolsEx>();
        panel = GetComponent<Image>();
        panel.enabled = false;
    }

    public void SearchPool(int hash)
    {
        SearchPoolUI.SetActive(false);

        

            Debug.Log(hash);
            exJ = ex.GetPoolHash(hash);
        

        HashID = hash;

        twitterHandler = null;
        telegramHandler = null;
        youtubeHandler = null;

        try
        {
            //Get Twitter And Telegram From ExJson
            twitterHandler = exJ.handles["tw"];
        }catch(Exception e)
        {
            e.GetBaseException();
        }

        try
        {
            //Get Twitter And Telegram From ExJson
            telegramHandler = exJ.handles["tg"];
        }
        catch (Exception e)
        {
            e.GetBaseException();
        }

        try
        {
            //Get Twitter And Telegram From ExJson
            youtubeHandler = exJ.handles["yt"];
        }
        catch (Exception e)
        {
            e.GetBaseException();          
        }

        if (!isVisible && exJ != null)
            TogglePanel(hash);  

    }

    private void LateUpdate()
    {
        try
        {
            if (Search && Convert.ToSingle(exJ.total_stake) != 0)
            {
                LoadFromExJson(exJ);
            }
        }
        catch (Exception e) { }
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
            string temp = www.downloadHandler.text;
            var N = JSON.Parse(temp);
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

    void LoadFromExJson(ReadPoolsEx.ExJson exJson)
    {

        //ALSO EQUIP THE IMAGE:
        //StartCoroutine(DownloadImage());
        //ReadPoolsHandler.metaArray[exJson.hash_id].extended

        float? blocksLifetiem = exJson.blocks_epoch + exJson.blocks_lifetime;

        PoolID.SetText(exJson.pool_id);
        PoolTicker.SetText(exJson.db_ticker);
        PoolName.SetText(exJson.db_name);
        PoolLiveStake.SetText("Live Stake: " + ToM(exJson.total_stake) + "₳");
        PoolActiveStake.SetText("Active Stake: " + ToM(exJson.active_stake) + "₳");
        PoolPledge.SetText("Pledge: " + ToM(exJson.pledge) + "₳");
        PoolROA.SetText("ROA(lifetime): " + exJson.roa_lifetime.ToString("F3") + "%");
        PoolMargin.SetText("Margin: " + (exJson.tax_ratio * 100).ToString("F2") + "%");
        PoolFixedFee.SetText("Fixed Fee: " + ToM(exJson.tax_fix) + "₳");
        PoolBlocks.SetText("Blocks Lifetime: " + blocksLifetiem.ToString());
        PoolEpochBlocks.SetText("Blocks Epoch: " + exJson.blocks_epoch.ToString());
        PoolLink.SetText(exJson.db_url);



        if (blocksLifetiem >= 1 && blocksLifetiem < 10)
        {
            AwardImage.overrideSprite = B1;
        }
        else if (blocksLifetiem > 10 && blocksLifetiem < 100)
        {
            AwardImage.overrideSprite = B10;
        }
        else if (blocksLifetiem >= 100 && blocksLifetiem < 1000)
        {
            AwardImage.overrideSprite = B100;
        }
        else if (blocksLifetiem >= 1000)
        {
            AwardImage.overrideSprite = B1000;
        }
        else
        {
            AwardImage.overrideSprite = null;
            AwardImage.gameObject.SetActive(false);
        }



        try
        {
            PoolCountry.SetText("" + LocationHandler.GetComponent<GetLocation>().poolsLoc[HashID].country_name);
        }
        catch (Exception e) {

        }
        Delegators.SetText("Delegators: " + exJson.delegators.ToString());

        string dns = "";

        try
        {
            if (ReadPoolsHandler.temp[exJson.hash_id].dns_name != null)
            {
                try
                {

                    var hostEntry = Dns.GetHostAddresses(ReadPoolsHandler.temp[exJson.hash_id].dns_name)[0];


                    //you might get more than one ip for a hostname since 
                    //DNS supports more than one record


                    dns = hostEntry.ToString();
                }
                catch (Exception e)
                {
                    e.GetBaseException();
                }

            }
            else if (ReadPoolsHandler.temp[exJson.hash_id].ipv4 != null && ReadPoolsHandler.temp[exJson.hash_id].ipv4 != "")
            {
                dns = ReadPoolsHandler.temp[exJson.hash_id].ipv4;
            }
            else if (ReadPoolsHandler.temp[exJson.hash_id].ipv6 != null)
            {
                dns = ReadPoolsHandler.temp[exJson.hash_id].ipv6;
            }
        }
        catch (Exception e) { }

        Debug.Log("DNS: " + dns);

        try
        {
            StartCoroutine(GetJson(ReadPoolsHandler.metaArray[exJson.hash_id].extended));
        }
        catch (Exception) { }

        try
        {
            //CHECK WITH DNS / IPV4 / IPV6
            switch (PortInUse(dns, ReadPoolsHandler.temp[exJson.hash_id].port))
            {

                case 2:
                    PoolStatus.SetText("Online");
                    PoolStatus.color = Color.green;
                    break;

                default:
                    PoolStatus.SetText("");
                    break;
            }
        }
        catch (Exception e) { }

        Search = false;
    }

    string ToM(long? x)
    {
        float y = (float)x / 1000000f;

        if(y <= 999)
        {
            return y.ToString();
        }
        else if(y > 999 && y <= 999999)
        {
            return (Convert.ToSingle(y) / 1000f).ToString("F2") + "k";
        }else if(y > 999999)
        {
            return (Convert.ToSingle(y) / 1000000f).ToString("F2") + "m";
        }

        return "";
    }


    public void TogglePanel(int hash)
    {
        isVisible = !isVisible;
        CameraCtrl.canMove = !isVisible;

        HideButton.SetActive(!isVisible);

        if (isVisible)
        {
            try
            {
                panel.enabled = true;
                if (hash != 0 && LocationHandler.poolsObjects[hash].GetComponent<ParticleFollower>().getCamera() != null)
                {
                    MainCamera.gameObject.SetActive(false);
                    LocationHandler.poolsObjects[hash].GetComponent<ParticleFollower>().getCamera().enabled = true;
                    Vector3 tempV = new Vector3(0, 0, -5f - LocationHandler.poolsObjects[hash].transform.localScale.x * 15f);
                    LocationHandler.poolsObjects[hash].GetComponent<ParticleFollower>().getCamera().transform.localPosition = tempV;
                }
                else
                {
                }

                //Disable Other Pools
                LocationHandler.TogglePool(hash);
            }catch(Exception e)
            {
                e.GetBaseException();
            }
        }
        else
        {
            panel.enabled = false;
            MainCamera.gameObject.SetActive(true);
            if (hash != 0)
                LocationHandler.poolsObjects[hash].GetComponent<ParticleFollower>().getCamera().enabled = false;
            else
            {
                try
                {
                    LocationHandler.poolsObjects[LastHash].GetComponent<ParticleFollower>().getCamera().enabled = false;
                }
                catch (Exception e) { }

                }

            //Enable Other Pools
            LocationHandler.TogglePool(0);

        }

        LastHash = hash;

        ExplorerPanel.SetActive(!isVisible);

        PoolID.gameObject.SetActive(isVisible);
        PoolTicker.gameObject.SetActive(isVisible);
        PoolName.gameObject.SetActive(isVisible);
        PoolLiveStake.gameObject.SetActive(isVisible);
        PoolActiveStake.gameObject.SetActive(isVisible);
        PoolPledge.gameObject.SetActive(isVisible);
        PoolROA.gameObject.SetActive(isVisible);
        PoolMargin.gameObject.SetActive(isVisible);
        PoolFixedFee.gameObject.SetActive(isVisible);
        PoolBlocks.gameObject.SetActive(isVisible);
        PoolEpochBlocks.gameObject.SetActive(isVisible);
        PoolCountry.gameObject.SetActive(isVisible);
        Delegators.gameObject.SetActive(isVisible);
        PoolLink.gameObject.SetActive(isVisible);
        Divider.SetActive(isVisible);
        exit.gameObject.SetActive(isVisible);

        PoolStatus.gameObject.SetActive(isVisible);

            AwardImage.overrideSprite = null;
            AwardImage.gameObject.SetActive(isVisible);

        PoolCountry.SetText("");


        if (twitterHandler != null && twitterHandler != "")
            TwButton.SetActive(isVisible);

        if (telegramHandler != null && telegramHandler != "")
            TeButton.SetActive(isVisible);

        if (youtubeHandler != null && youtubeHandler != "")
            YoButton.SetActive(isVisible);

        Debug.Log("yt Handler: " + youtubeHandler);

        if (!isVisible)
        {
            PoolImage.gameObject.SetActive(false);
            adManager.ShowAd();
        }

        Search = true;

        


    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            PoolImage.overrideSprite = null;
            PoolImage.gameObject.SetActive(false);
        }
        else
        {
            PoolImage.overrideSprite = Sprite.Create(((DownloadHandlerTexture)request.downloadHandler).texture, new Rect(0.0f,0.0f, ((DownloadHandlerTexture)request.downloadHandler).texture.width, ((DownloadHandlerTexture)request.downloadHandler).texture.height)  , new Vector2(0.5f, 0.5f), 100f);
            //GOT IMAGE

            PoolImage.gameObject.SetActive(isVisible);
        }
    }

    public void LaunchTwitter()
    {
        Application.OpenURL("https://twitter.com/" + twitterHandler);
    }

    public void LaunchTelegram()
    {
        Application.OpenURL("https://t.me/" + telegramHandler);
    }

    public void LaunchYoutube()
    {
        Application.OpenURL("https://www.youtube.com/" + youtubeHandler);
    }

    public void LaunchLink()
    {
        if(PoolLink.text != "" && PoolLink.text != null)
        Application.OpenURL(PoolLink.text);
    }

    public void LaunchLinkAwards()
    {
        if (PoolID.text != "" && PoolID.text != null)
            Application.OpenURL("https://pooltool.io/pool/" + PoolID.text + "/awards");
    }

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }


    public int PortInUse(string ip, int port)
    {
        using (TcpClient tcpClient = new TcpClient())
        {
            try
            {
                tcpClient.ConnectAsync(ip, port).Wait(500);

                if (tcpClient.Connected)
                    return 2;
                else
                    return 1;
                
            }
            catch (Exception e)
            {
                Debug.Log(e.GetBaseException());
                return 0;
            }

        }
    }
}
