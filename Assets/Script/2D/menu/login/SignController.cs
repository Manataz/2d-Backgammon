using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using UnityEngine.Networking;

public class SignController : MonoBehaviour
{
    [Header("General")]
    public bool signUp;
    public bool refrral;
    public bool updated;
    public bool loading;
    public bool newAccount;

    [Header("Status")]
    public bool signing;

    [Header("Canvas")]
    public CanvasGroup signUpUI;
    public CanvasGroup refrralUI;

    private Authentication _authentication;
    private PlayerInfo _playerInfo;
    private Controller _controller;
    private LoadingUpdate _loadingUpdate;
    private LoadExcel _loadExcel;
    private SearchSystem _searchSystem;
    private OnlineGameServer _onlineGameServer;
    private BannerAds _unityBannerAds;
    private RewardAds _rewardedAds;

    [Header("Error")]
    public GameObject signUpError;

    [Header("Avatar")] 
    public int avatarId;
    public Image[] avatar;
    public Image[] avatarFrameImage;
    public Image[] avatarVertexImage;
    public Color[] avatarVertexColor;

    void Start()
    {
        _authentication = FindObjectOfType<Authentication>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _controller = FindObjectOfType<Controller>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _unityBannerAds = FindObjectOfType<BannerAds>();
        _rewardedAds = FindObjectOfType<RewardAds>();
        
        signUpError.SetActive(false);
        signUpUI.alpha = 0;

        avatar[0].sprite = _playerInfo.avatars[0];
        avatar[1].sprite = _playerInfo.avatars[1];
    }

    void Update()
    {
        if (_playerInfo.Updated && _loadingUpdate.loaded && !updated)
        {
            signUp = _playerInfo.PlayerData.token == "" && !AuthenticationService.Instance.IsSignedIn;

            if (signUp)
            {
                _authentication.SignOut();
                signing = false;
            }
            else if (!AuthenticationService.Instance.IsSignedIn)
            {
                signing = true;
                _authentication.TrySignIn();
            }

            updated = true;
        }
        
        SetupAvatar();

        _controller.SetCanvas(signUpUI, signUp, 0, 1);
        _controller.SetCanvas(refrralUI, refrral, 0, 1);
    }

    public void  LoggedIn()
    {
        loading = true;
        _playerInfo.GetRemoteKeys();
        _unityBannerAds.LoadBanner();
        StartCoroutine(_rewardedAds.LoadAdRetry());
    }

    void SetupAvatar()
    {
        float speed = 3 * Time.deltaTime;
        if (avatarId == 0)
        {
            avatarFrameImage[0].transform.localScale = Vector2.Lerp(avatarFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            avatarFrameImage[1].transform.localScale = Vector2.Lerp( avatarFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            avatarVertexImage[0].color = avatarVertexColor[1];
            avatarVertexImage[1].color = avatarVertexColor[0];
        }
        else if (avatarId == 1)
        {
            avatarFrameImage[1].transform.localScale = Vector2.Lerp(avatarFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            avatarFrameImage[0].transform.localScale = Vector2.Lerp(avatarFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            avatarVertexImage[1].color = avatarVertexColor[1];
            avatarVertexImage[0].color = avatarVertexColor[0];
        }

        avatarFrameImage[0].enabled = avatarId == 0;
        avatarFrameImage[1].enabled = avatarId == 1;
    }

    public void AvatarClick(int id)
    {
        avatarId = id;
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "sign as guest":
            {
                signUpError.SetActive(false);
                int randomNumber = UnityEngine.Random.Range(100, 1000);
                _authentication.nickname = "Guest " + randomNumber.ToString();
                _authentication.avatarId = avatarId;
                _authentication.state = "";
                _authentication.birthdate = new []{-1, -1, -1};
                _authentication.coin = 150; //Debug
                newAccount = true;
                
                _authentication.ClickSignInAsGuest();
                    signing = true;
            }
                break;
            
            case "sign with google":
            {
                signUpError.SetActive(false);
                _authentication.ClickSignInWithGoogle();
                signing = true;
            }
                break;

            case "refrral":
            {
                refrral = true;
            }
                break;
            
            case "avatar":
            {
                _controller.avatar = true;
                _controller.UpdateUI();
            }
                break;
            
            case "back":
            {
                if (refrral)
                {
                    refrral = false;
                }else 
                if (signUp)
                {
                    signUp = false;
                }
            }
                break;

            case "terms":
            {
                string url = _loadingUpdate.gameData.terms_of_use;
                Application.OpenURL(url);
            }
                break;
            
            case "privacy":
            {
                string url = _loadingUpdate.gameData.privacy_policy;
                Application.OpenURL(url);
            }
                break;
        }
        
    }

    public void InputChanged()
    {
        signUpError.SetActive(false);
    }
}
