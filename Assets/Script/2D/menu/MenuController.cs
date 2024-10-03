using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("General")]
    public bool menu;
    public bool updated;
    
    [Header("Canvas")]
    public CanvasGroup menuUI;

    [Header("UI")]
    public Text usernameTextUI;
    public Text coinTextUI;
    public Image avatarUI;
    public Image regionIconUI;
    public Text classTextUI;
    public CanvasGroup videoAdsCanvasGroup;
    public Button videoAdsButton;
    public GameObject premiumButton;
    public GameObject premiumMember;
    public Text friendCoinTextUI;

    private PlayerInfo _playerInfo;
    private SignController _signController;
    private ProfileController _profileController;
    private Controller _controller;
    private OptionsController _optionsController;
    private LoadingUpdate _loadingUpdate;
    private BannerAds _unityBannerAds;
    private SearchSystem _searchSystem;
    private OnlineGameServer _onlineGameServer;
    private PlayWithFriends _playWithFriends;
    private RankController _rankController;
    private TournamentController _tournamentController;
    private PopupController _popupController;

    void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _signController = GetComponent<SignController>();
        _profileController = GetComponent<ProfileController>();
        _controller = GetComponent<Controller>();
        _optionsController = GetComponent<OptionsController>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _unityBannerAds = FindObjectOfType<BannerAds>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _playWithFriends = FindObjectOfType<PlayWithFriends>();
        _rankController = FindObjectOfType<RankController>();
        _tournamentController = FindObjectOfType<TournamentController>();
        _popupController = FindObjectOfType<PopupController>();

        menuUI.alpha = 0;
    }
    
    void Update()
    {
        if (!updated && _playerInfo.Updated && !_signController.signing)
        {
            _controller.UpdateUI();
            updated = true;
        }
        
        menu = !_signController.signUp && !_controller.avatar && !_signController.refrral && !_controller.loading && !_profileController.profile && !_optionsController.options && !_loadingUpdate.loading && !_controller.premium;
        _controller.SetCanvas(menuUI, menu, 0, 1);
        _controller.SetCanvas(videoAdsCanvasGroup, videoAdsButton.interactable, 0.5f, 1);

        bool showBannerAd = menu && !_controller.loading && _tournamentController.dataUpdated;
        
        if (showBannerAd && !_unityBannerAds.show)
        {
            _unityBannerAds.ShowBannerAd();
        }
        else if (!showBannerAd && _unityBannerAds.show)
        {
            _unityBannerAds.HideBannerAd();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && ((menu && !_searchSystem.searching && !_searchSystem.opponentFound) || (_signController.signUp)))
        {
            _popupController.OpenPopUp(true, true, false, "Are you sure you want to exit the app?", QuitNo, QuitYes, null);
        }
    }

    public void QuitNo()
    {
        _popupController.show = false;
    }
    
    public void QuitYes()
    {
        Application.Quit();
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "profile":
            {
                _profileController.profile = true;
            }
                break;
            
            case "options":
            {
                _optionsController.options = true;
            }
                break;
            
            case "play":
            {
                _searchSystem.SetupBetItems(_playerInfo.PlayerData.playerClass);
                _searchSystem.playOnline = true;
                
                if (!_onlineGameServer.connected)
                    _onlineGameServer.SocketConnect();
            }
                break;

            case "play with friends":
            {
                _playWithFriends.option = true;
            }
                break;

            case "rank":
            {
                _rankController.UpdateData();
                _rankController.rank = true;
            }
                break;
            
            case "coin":
            {
                _controller.UpdateUI();
                _controller.coin = true;
            }
                break;
        }
    }
}
