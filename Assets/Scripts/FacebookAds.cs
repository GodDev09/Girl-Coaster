using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using AudienceNetwork;

public class FacebookAds : MonoBehaviour
{
    public bool testAds;
    public string bannerId;
    public string interstitialId;
    public string rewardedVideoId;
    //public bool directedForChildren;
    
    private const float reloadTime = 60;
    private const int maxRetryCount = 10;

    private AdView bannerAd;
    private InterstitialAd interstitialAd;
    private RewardedVideoAd rewardedVideoAd;
    
    private UnityAction OnInterstitialClosed;
    private UnityAction OnRewardedVideoCloed;

    private int currentRetryInterstitial;
    private int currentRetryRewardedVideo;

    private bool interstitialIsLoaded;
    private bool bannerUsed;
    private bool rewardedVideoisLoaded;
    private bool triggerCompleteMethod;
    
    public static FacebookAds instance;
    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        InitializeAds();
    }
    
    /// <summary>
    /// Initializing Audience Network
    private void InitializeAds()
    {
        //AdSettings.SetMixedAudience(directedForChildren);
        
        AudienceNetworkAds.Initialize();
        
        //add Device ID test
        //AdSettings.AddTestDevice("ef8ac37dae9828fc");

        StartCoroutine(WaitingConsent());
    }

    IEnumerator WaitingConsent()
    {
        Debug.Log("Waiting to Initialize Ads");
        yield return new WaitForSeconds(3f);
        Debug.Log("Initialize Ads");
        if (!string.IsNullOrEmpty(interstitialId))
        {
            LoadInterstitial();
        }
        
        if (!string.IsNullOrEmpty(rewardedVideoId))
        {
            LoadRewardedVideo();
        }

        ShowBanner();
    }


    #region Banner Implementation
    
    public void ShowBanner()
    {
        LoadBanner();
    }

    
    public void HideBanner()
    {
        if (bannerAd)
        {
            bannerAd.Dispose();
        }
    }
    
    private void LoadBanner()
    {
        if (bannerAd)
        {
            bannerAd.Dispose();
        }

        AdSize bannerSize;
        bannerSize = AdSize.BANNER_HEIGHT_50;

        var bannerID = testAds ? $"IMG_16_9_APP_INSTALL#{bannerId}" : bannerId;
        bannerAd = new AdView(bannerID, bannerSize);
        bannerAd.Register(gameObject);
        Debug.Log("bannerID " + bannerID);
        // Set delegates
        bannerAd.AdViewDidLoad += BannerLoadSuccess;
        bannerAd.AdViewDidFailWithError = BannerLoadFailed;
        bannerAd.AdViewWillLogImpression = BannerAdWillLogImpression;
        bannerAd.AdViewDidClick = BannerAdDidClick;
        
        bannerAd.LoadAd();
    }
    
    private void BannerAdDidClick()
    {
        Debug.Log(this + " " + "Banner ad clicked.");
    }
    
    private void BannerAdWillLogImpression()
    {
        Debug.Log(this + " " + "Banner ad logged impression.");
    }
    
    private void BannerLoadFailed(string error)
    {
        Debug.Log(this + " " + "Banner Failed To Load " + error);
    }

    
    private void BannerLoadSuccess()
    {
        Debug.Log(this + " " + "Banner Loaded");
        
        bannerAd.Show(AdPosition.BOTTOM);
    }

    #endregion
    
    #region Interstitial Implementation
    
    public bool IsInterstitialAvailable()
    {
        return interstitialIsLoaded;
    }

    public void ShowInterstitial()
    {
        Debug.Log("Click ShowInterstitial");
        
        LoadInterstitial();
        UnityAction action = () =>
        {
            Debug.LogError("=== ShowInterstitial Done");
        };
        ShowInterstitial(action);
    }


    /// <summary>
    /// Show Facebook interstitial
    /// </summary>
    /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
    public void ShowInterstitial(UnityAction _InterstitialClosed)
    {
        if (IsInterstitialAvailable())
        {
            OnInterstitialClosed = _InterstitialClosed;
            interstitialAd.Show();
            interstitialIsLoaded = false;
        }
    }
    
    private void LoadInterstitial()
    {
        interstitialAd?.Dispose();

        interstitialIsLoaded = false;

        var interID = testAds ? $"IMG_16_9_LINK#{interstitialId}" : interstitialId;
        Debug.Log("interID " + interID);
        interstitialAd = new InterstitialAd(interID);
        interstitialAd.Register(gameObject);

        interstitialAd.InterstitialAdDidLoad += InterstitialLoaded;
        interstitialAd.InterstitialAdDidFailWithError += InterstitialFailed;
        interstitialAd.InterstitialAdWillLogImpression += InterstitialAdWillLogImpression;
        interstitialAd.InterstitialAdDidClick += InterstitialAdDidClick;
        interstitialAd.InterstitialAdDidClose += InterstitialClosed;

#if UNITY_ANDROID
        /*
         * Only relevant to Android.
         * This callback will only be triggered if the Interstitial activity has
         * been destroyed without being properly closed. This can happen if an
         * app with launchMode:singleTask (such as a Unity game) goes to
         * background and is then relaunched by tapping the icon.
         */
        interstitialAd.interstitialAdActivityDestroyed = delegate()
        {
            Debug.Log(this + " " + "Interstitial activity destroyed without being closed first.");
            InterstitialClosed();
        };
#endif
        interstitialAd.LoadAd();
    }
    
    private void InterstitialAdDidClick()
    {
        Debug.Log(this + " " + "Interstitial ad clicked.");
    }
    
    private void InterstitialAdWillLogImpression()
    {
        Debug.Log(this + " " + "Interstitial ad logged impression.");
    }
    
    private void InterstitialClosed()
    {
        Debug.Log(this + " " + "Reload Interstitial");

        interstitialAd?.Dispose();

        //reload interstitial
        LoadInterstitial();

        //trigger complete event
        CompleteMethodInterstitial();
    }
    
    private void CompleteMethodInterstitial()
    {
        if (OnInterstitialClosed != null)
        {
            OnInterstitialClosed();
            OnInterstitialClosed = null;
        }
    }
    
    private void InterstitialFailed(string error)
    {
        Debug.Log(this + " " + "Interstitial Failed To Load: " + error);


        //try again to load a rewarded video
        if (currentRetryInterstitial < maxRetryCount)
        {
            currentRetryInterstitial++;

            Debug.Log(this + " " + "RETRY " + currentRetryInterstitial);
            
            Invoke("LoadInterstitial", reloadTime);
        }
    }
    
    private void InterstitialLoaded()
    {
        if (interstitialAd.IsValid())
        {
            interstitialIsLoaded = true;


            Debug.Log(this + " " + "Interstitial Loaded");


            currentRetryInterstitial = 0;
        }
        else
        {
            Debug.Log(this + " " + "Interstitial Loaded but is invalid");


            //try again to load an interstitial video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;

                Debug.Log(this + " " + "RETRY " + currentRetryInterstitial);


                Invoke("LoadInterstitial", reloadTime);
            }
        }
    }

    #endregion
    
    #region Rewarded Video
    
    public bool IsRewardVideoAvailable()
    {
        return rewardedVideoisLoaded;
    }

    public void ShowRewardVideo()
    {
        Debug.Log("Click ShowRewardVideo");
        LoadRewardedVideo();
        UnityAction action = () =>
        {
            Debug.LogError("=== ShowRewardVideo Done");
        };
        ShowRewardVideo(action);
    }

    /// <summary>
    /// Show Facebook rewarded video
    /// </summary>
    /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
    public void ShowRewardVideo(UnityAction _CompleteMethod)
    {
        if (IsRewardVideoAvailable())
        {
            rewardedVideoisLoaded = false;
            triggerCompleteMethod = true;
            OnRewardedVideoCloed = _CompleteMethod;
            rewardedVideoAd.Show();
        }
    }

    /// <summary>
    /// Load a Facebook rewarded video and add the required listeners
    /// </summary>
    private void LoadRewardedVideo()
    {
        rewardedVideoAd?.Dispose();

        var rewardedID = testAds ? $"VID_HD_9_16_39S_APP_INSTALL#{rewardedVideoId}" : rewardedVideoId;
        rewardedVideoAd = new RewardedVideoAd(rewardedID);
        rewardedVideoAd.Register(gameObject);
        Debug.Log("rewardedID " + rewardedID);
        rewardedVideoAd.RewardedVideoAdDidLoad += RewardedVideoLoaded;
        rewardedVideoAd.RewardedVideoAdDidFailWithError += RewardedVideoFailed;
        rewardedVideoAd.RewardedVideoAdWillLogImpression += RewardedVideoAdWillLogImpression;
        rewardedVideoAd.RewardedVideoAdDidClick += RewardedVideoAdDidClick;
        rewardedVideoAd.RewardedVideoAdComplete += RewardedVideoWatched;
        rewardedVideoAd.RewardedVideoAdDidClose += RewardedVideoAdClosed;

#if UNITY_ANDROID
        /*
         * Only relevant to Android.
         * This callback will only be triggered if the Rewarded Video activity
         * has been destroyed without being properly closed. This can happen if
         * an app with launchMode:singleTask (such as a Unity game) goes to
         * background and is then relaunched by tapping the icon.
         */
        rewardedVideoAd.RewardedVideoAdActivityDestroyed = delegate()
        {
            Debug.Log("Rewarded video activity destroyed without being closed first.");
            Debug.Log("Game should resume. User should not get a reward.");
            RewardedVideoAdClosed();
        };
#endif
        rewardedVideoAd.LoadAd();
    }
    
    private void RewardedVideoAdClosed()
    {
        rewardedVideoAd?.Dispose();

        Debug.Log(this + " " + "OnAdClosed");


        //reload
        LoadRewardedVideo();

        //if complete method was not already triggered, trigger complete method with ad skipped param
        if (triggerCompleteMethod == true)
        {
            CompleteMethodRewardedVideo();
        }
    }
    
    private void RewardedVideoWatched()
    {
        Debug.Log(this + " " + "Rewarded Video Watched");


        triggerCompleteMethod = false;
        CompleteMethodRewardedVideo();
    }
    
    private void CompleteMethodRewardedVideo()
    {
        if (OnRewardedVideoCloed != null)
        {
            OnRewardedVideoCloed();
            OnRewardedVideoCloed = null;
        }
    }
    
    private void RewardedVideoAdDidClick()
    {
        Debug.Log(this + " " + "Rewarded video ad clicked.");
    }
    
    private void RewardedVideoAdWillLogImpression()
    {
        Debug.Log(this + " " + "Rewarded video ad logged impression.");
    }

    
    private void RewardedVideoFailed(string error)
    {
        Debug.Log(this + " " + "Rewarded Video Failed To Load: " + error);


        //try again to load a rewarded video
        if (currentRetryRewardedVideo < maxRetryCount)
        {
            currentRetryRewardedVideo++;

            Debug.Log(this + " " + "RETRY " + currentRetryRewardedVideo);


            Invoke("LoadRewardedVideo", reloadTime);
        }
    }
    
    private void RewardedVideoLoaded()
    {
        if (rewardedVideoAd.IsValid())
        {
            Debug.Log(this + " " + "Rewarded Video Loaded");


            rewardedVideoisLoaded = true;
            currentRetryRewardedVideo = 0;
        }
        else
        {
            Debug.Log(this + " " + "Rewarded Video Loaded but is invalid");


            //try again to load a rewarded video
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                currentRetryRewardedVideo++;

                Debug.Log(this + " " + "RETRY " + currentRetryRewardedVideo);


                Invoke("LoadRewardedVideo", reloadTime);
            }
        }
    }

    #endregion

    private void OnApplicationFocus(bool focus)
    {
        if (focus == true)
        {
            if (IsInterstitialAvailable() == false)
            {
                if (currentRetryInterstitial == maxRetryCount)
                {
                    LoadInterstitial();
                }
            }

            if (IsRewardVideoAvailable() == false)
            {
                if (currentRetryRewardedVideo == maxRetryCount)
                {
                    LoadRewardedVideo();
                }
            }
        }
    }
}