using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;


public class AdManager : MonoBehaviour
{
    //Should Display Ads?
    public bool UnderStake = false;

    private string adUnitIdTest = "ca-app-pub-3940256099942544/1033173712";

    private void Start()
    {
        InitAds();
    }

    public void InitAds()
    {
        MobileAds.Initialize(initStatus => { });
        RequestInterstitial();
    }

    private InterstitialAd interstitial;

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3714165391574154/8817099185";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3714165391574154/9256917039";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void ShowAd()
    {
        if (this.interstitial.IsLoaded() && UnderStake)
        {
            this.interstitial.Show();
        }

        RequestInterstitial();
    }
}
