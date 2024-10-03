using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class PlayWithFriends : MonoBehaviour
{
    [Header("General")] public bool option;
    public bool inviteFriend;
    public bool joinMatch;
    private bool inviteFriend_new;
    private bool updated;
    public string searchType = "playWithFriend";
    public int coin;

    [Header("Canvas")] public CanvasGroup optionUI;
    public CanvasGroup inviteFriendUI;
    public CanvasGroup joinMatchUI;

    [Header("Access")] public Image playWithFriendsButton;
    public Text shareCodeInfoTextUI;
    public GameObject shareButton;
    public Text[] searchInfoTextUI;
    public GameObject clearButton;
    public GameObject error;
    public GameObject playButton;
    public Text shareCodeTextUI;
    public GameObject copiedText;

    [Header("UI")] public Sprite[] playWithFriendsSprite;

    [Header("Refrral")] public int friendRefferalCode;
    public InputField[] refrralInput;
    public Image[] refrralImage;
    public Sprite[] refralSprite;
    private int presentInput;

    private Controller _controller;
    private LoadExcel _loadExcel;
    private PlayerInfo _playerInfo;
    private SearchSystem _searchSystem;
    private OnlineGameServer _onlineGameServer;



    private void Start()
    {
        _controller = FindObjectOfType<Controller>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();

        error.SetActive(false);
        copiedText.SetActive(false);
    }

    private void Update()
    {
        if (!updated && _loadExcel.updated && _playerInfo.Updated)
        {
            shareCodeInfoTextUI.text = _loadExcel.itemDatabase_book[0].shareCode;
            updated = true;
        }

        if (inviteFriend && _onlineGameServer.connected && !inviteFriend_new)
        {
            friendRefferalCode = UnityEngine.Random.Range(10000, 99999);
            shareCodeTextUI.text = friendRefferalCode.ToString();
            inviteFriend_new = true;
            _searchSystem.Search(searchType, false);
        }

        if (!_onlineGameServer.connected)
            shareCodeTextUI.text = "-----";

        SetCanvas();
        SetSearchInfo(searchInfoTextUI[0], false);
        SetSearchInfo(searchInfoTextUI[1], true);
        RefrralCodeSetup();

        clearButton.SetActive(refrralInput[0].text.Length > 0 || refrralInput[1].text.Length > 0 ||
                              refrralInput[2].text.Length > 0 || refrralInput[3].text.Length > 0 ||
                              refrralInput[4].text.Length > 0);

        if (option)
            playWithFriendsButton.sprite = playWithFriendsSprite[1];
        else
            playWithFriendsButton.sprite = playWithFriendsSprite[0];

        shareButton.SetActive( /*!_searchSystem.searching &&*/
            !_searchSystem.opponentFound && _onlineGameServer.connected);
        playButton.SetActive(!_searchSystem.searching && !_searchSystem.opponentFound && _onlineGameServer.connected);
    }

    void SetSearchInfo(Text text, bool showSearch)
    {
        if (!_onlineGameServer.connected)
            text.text = "Connecting...";
        else if (_searchSystem.opponentFound)
            text.text = "<color=yellow>Your friend joined, starting the game...</color>";
        else if (_searchSystem.searching && showSearch)
            text.text = "Waiting for your friend...";
        else
            text.text = "";
    }

    void SetCanvas()
    {
        _controller.SetCanvas(optionUI, option, 0, 1);
        _controller.SetCanvas(inviteFriendUI, inviteFriend, 0, 1);
        _controller.SetCanvas(joinMatchUI, joinMatch, 0, 1);
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "join match":
            {
                joinMatch = true;
                option = false;
            }
                break;

            case "join match back":
            {
                joinMatch = false;
                _searchSystem.ButtonClick("back");
                error.SetActive(false);
            }
                break;

            case "invite friend":
            {
                copiedText.SetActive(false);
                inviteFriend = true;
                option = false;
            }
                break;

            case "option back":
            {
                option = false;
            }
                break;

            case "invite friend back":
            {
                inviteFriend = false;
                inviteFriend_new = false;
                _searchSystem.ButtonClick("back");
            }
                break;

            case "play":
            {
                error.SetActive(false);

                if (!CheckRefrralInputErrors())
                {
                    friendRefferalCode = int.Parse(refrralInput[0].text + refrralInput[1].text + refrralInput[2].text +
                                                   refrralInput[3].text + refrralInput[4].text);
                    _searchSystem.Search(searchType, false);
                }
                else
                {
                    error.SetActive(true);
                }
            }
                break;
        }
    }

    public bool IsNumeric(string str)
    {
        foreach (char c in str)
        {
            if (!Char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    bool CheckRefrralInputErrors()
    {
        bool error = false;
        foreach (InputField refrral in refrralInput)
        {
            if (refrral.text.Length == 0)
                error = true;

            if (!IsNumeric(refrral.text))
                error = true;
        }

        if (refrralInput[0].text == "0")
            error = true;

        return error;
    }

    void RefrralCodeSetup()
    {
        /*if (Input.inputString.Length > 0 && Input.inputString[0] == '\b')
        {
            for (int i = refrralInput.Length-1;i>=0;i--)
            {
                if (refrralInput[i].text.Length > 0)
                {
                    refrralInput[i].text = "";
                    break;
                }
            }
        }*/

        for (int i = 0; i < refrralInput.Length; i++)
        {
            if (refrralInput[i].text.Length > 1 && i != 4)
            {
                int targetIndex = -1;

                if (refrralInput.Length > i + 1 && refrralInput[i + 1].text.Length == 0)
                    targetIndex = i + 1;
                else if (refrralInput.Length > i + 2 && refrralInput[i + 2].text.Length == 0)
                    targetIndex = i + 2;
                else if (refrralInput.Length > i + 3 && refrralInput[i + 3].text.Length == 0)
                    targetIndex = i + 3;
                else if (refrralInput.Length > i + 4 && refrralInput[i + 4].text.Length == 0)
                    targetIndex = i + 4;

                if (i != targetIndex && targetIndex != -1)
                {
                    refrralInput[targetIndex].text = refrralInput[i].text[1].ToString();
                    string updatedText = refrralInput[i].text.Remove(1, 1);
                    refrralInput[i].text = updatedText;
                }
            }

            if (refrralInput[0].text.Length == 0)
                presentInput = 0;
            else
            {
                if (i > 0 && refrralInput[i - 1].text.Length > 0 && refrralInput[i].text.Length == 0)
                    presentInput = i;
            }

            if (i != presentInput)
            {
                if (refrralInput[i].text.Length == 0)
                    refrralImage[i].sprite = refralSprite[0];
                else if (refrralInput[i].text.Length > 0)
                    refrralImage[i].sprite = refralSprite[1];
            }
            else if (refrralInput[i].text.Length == 0)
                refrralImage[i].sprite = refralSprite[2];
            else
                refrralImage[i].sprite = refralSprite[1];
        }

        foreach (InputField input in refrralInput)
        {
            if (input.text.Length > 1)
            {
                string updatedText = input.text.Remove(1, 1);
                input.text = updatedText;
            }
        }
    }

    public void ClearRefrralCodeClick()
    {
        for (int i = refrralInput.Length - 1; i >= 0; i--)
            refrralInput[i].text = "";
    }

    public void InputSetPresent(int i)
    {
        presentInput = i;
        error.SetActive(false);
    }

    public void CopyToClipboard()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject clipboardManager =
                currentActivity.Call<AndroidJavaObject>("getSystemService", "clipboard");

            AndroidJavaObject clipData = new AndroidJavaObject("android.content.ClipData", "label", "text/plain",
                new AndroidJavaObject("android.content.ClipData$Item", friendRefferalCode.ToString()));
            clipboardManager.Call("setPrimaryClip", clipData);
        }
        else
        {
            GUIUtility.systemCopyBuffer = friendRefferalCode.ToString();
        }
        
        copiedText.SetActive(true);
    }
    
    public void Share()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            string actionSend = intentClass.GetStatic<string>("ACTION_SEND");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent", actionSend);

            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", friendRefferalCode.ToString());

            currentActivity.Call("startActivity", intentObject);
        }
        else
        {
            Debug.Log("Sharing is only available on Android.");
        }
        
        copiedText.SetActive(false);
    }
}

