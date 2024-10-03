using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardAds : MonoBehaviour, IDisposable, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField]
    Button _rewardAdsButton;

    private readonly string _androidAdUnitId = "Rewarded_Android";
    private readonly string _iOSAdUnitId = "Rewarded_iOS";

    string _adUnitId = null; // This will remain null for unsupported platforms

    bool readyToLoadNewAd = true;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        #if UNITY_IOS
            _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
                _adUnitId = _androidAdUnitId;
        #endif

        // Disable the button until the ad is ready to show:
        _rewardAdsButton.interactable = false;
    }

    public IEnumerator LoadAdRetry()
    {
        while (true)
        {
            LoadAd();
            
            // Wait for half an hour (1800 seconds)
            yield return new WaitForSeconds(60);
        }
    }

    public void ClickRewardAdsButton()
    {
        // Disable the button:
        _rewardAdsButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {
        if (readyToLoadNewAd)
        {
            readyToLoadNewAd = false;
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("<color=blue>(AD)</color> <color=white> Reward Ad Loading... </color>  (" + _adUnitId + ")");
            Advertisement.Load(_adUnitId, this);
        }
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("<color=blue>(AD)</color> <color=white> Reward Ad Loaded: " + adUnitId + "</color>");

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _rewardAdsButton.onClick.AddListener(ClickRewardAdsButton);
            // Enable the button for users to click:
            _rewardAdsButton.interactable = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"<color=blue>(AD)</color> <color=white> Error loading Ad Unit {adUnitId}: {error.ToString()} - {message} </color>");
        // Use the error details to determine whether to try to load another ad. 

        readyToLoadNewAd = true;
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"<color=blue>(AD)</color> <color=white> Error showing Ad Unit {adUnitId}: {error.ToString()} - {message} </color>");
        // Use the error details to determine whether to try to load another ad.

        readyToLoadNewAd = true;
    }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public async void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("<color=blue>(AD)</color> <color=white> Unity Ads Rewarded Ad Completed </color>");

            _rewardAdsButton.gameObject.SetActive(false);
            readyToLoadNewAd = true;

            // Grant a reward using matchplayuser and leader board
            //await ClientSingleton.Instance.Manager.User.IncreaseCoinAndStore(LocalizationManager.Instance.RewardAdsValue);

            /*if (LocalizationManager.Instance.RewardAdsValue * 2 <= LocalizationManager.Instance.RewardAdsMaxValue)
            {
                LocalizationManager.Instance.RewardAdsValue *= 2;
            }*/
        }
    }

    public void Dispose()
    {
        //LocalizationManager.Instance.RewardAdsValueChanged -= OnRewardAdsValueChanged;
    }
}