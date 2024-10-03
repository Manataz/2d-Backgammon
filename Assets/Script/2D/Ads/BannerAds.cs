using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAds : MonoBehaviour, IDisposable
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.
    public bool show;

    void Start()
    {
        // Get the Ad Unit ID for the current platform:
        #if UNITY_IOS
            _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
            _adUnitId = _androidAdUnitId;
        #endif

        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);

        //LoadBanner();
        HideBannerAd();
    }

    private void ToggleBanner()
    {
        ShowBannerAd();
    }

    private void OnUserChanged(object sender, PropertyChangedEventArgs e)
    {
        ToggleBanner();
    }

    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        
        Debug.Log("<color=blue>(AD)</color> <color=white> Banner Ad loading... </color>");
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        Debug.Log("<color=blue>(AD)</color> <color=white> Banner loaded </color>");
        ToggleBanner();
    }

    int retryCount = 0;

    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"<color=blue>(AD)</color> <color=white> Banner Error: {message} </color>");

        // Optionally execute additional code, such as attempting to load another ad.
        retryCount++;
        if (retryCount < 5)
        {
            LoadBanner();
        }
    }

    // Implement a method to call when the Show Banner button is clicked:
    public void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(_adUnitId, options);
        show = true;
        Debug.Log("<color=blue>(AD)</color> <color=white> Banner show </color>");
    }

    public void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
        Debug.Log("<color=blue>(AD)</color> <color=white> Banner hide </color>");
        show = false;
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    public void Dispose()
    {
        //ClientSingleton.Instance.Manager.User.PropertyChanged -= OnUserChanged;
        //UIManager.Instance.MenuChanged.RemoveAllListeners();
    }
}