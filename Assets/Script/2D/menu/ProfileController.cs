using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    [Header("General")]
    public bool profile;
    public int avatarId;
    
    [Header("Canvas")]
    public CanvasGroup profileUI;

    [Header("UI")]
    public Text usernameTextUI;
    public Text ageTextUI;
    public Text emailTextUI;
    public Text phoneTextUI;
    public Image avatarUI;
    public Image regionIconUI;
    public Text scoreTextUI;
    public Text classTextUI;
    public Text winTextUI;
    public Text loseTextUI;
    public GameObject premiumStatusButton;

    private PlayerInfo _playerInfo;
    private Controller _controller;
    private PopupController _popupController;
    private Authentication _authentication;
    private LoadExcel _loadExcel;
    private SignController _signController;
    private TournamentController _tournamentController;

    public float usernameTextPosY_default;

    private void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _controller = FindObjectOfType<Controller>();
        _popupController = FindObjectOfType<PopupController>();
        _authentication = FindObjectOfType<Authentication>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _signController = FindObjectOfType<SignController>();
        _tournamentController = FindObjectOfType<TournamentController>();

        profileUI.alpha = 0;
        usernameTextPosY_default = usernameTextUI.transform.position.y;
    }

    void Update()
    {
        premiumStatusButton.SetActive(_playerInfo.PlayerData.premium && _playerInfo.Updated);
        SetCanvas(profileUI, profile);
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "back":
            {
                profile = false;
            }
                break;

            case "avatar":
            {
                _controller.avatar = true;
                _controller.UpdateUI();
            }
                break;
            
            case "nickname":
            {
                _controller.nickname = true;
                _controller.UpdateUI();
            }
                break;
            
            case "region":
            {
                _controller.region = true;
                _controller.UpdateUI();
                _controller.regionData = _playerInfo.PlayerData.region;
                _controller.SetupStateList();
            }
                break;
            
            case "logout":
            {
                string message = _loadExcel.itemDatabase_book[0].logout;
                _popupController.OpenPopUp(true, true, false, message, PopUpHide, LogOutYes, null);
            }
                break;
            
            case "delete account":
            {
                string message = _loadExcel.itemDatabase_book[0].deleteAccount;
                _popupController.OpenPopUp(true, true, false, message, PopUpHide, DeleteAccountYes, null);
            }
                break;

            case "premium status":
            {
                if (_playerInfo.PlayerData.premiumTime != "")
                    _popupController.OpenPopUp(false,false,true,"You are premium: since " + String.Format("{0}", _tournamentController.ConvertTimeToDateTime(_playerInfo.PlayerData.premiumTime).ToString("MMMM")) , null, null, PopUpHide);
            }
                break;
        }
    }

    public void PopUpHide()
    {
        _popupController.show = false;
    }
    
    public void LogOutYes()
    {
        _playerInfo.PlayerData.token = "";
        _playerInfo.SaveGame();
        _authentication.SignOut();
        _signController.signUp = true;
        _popupController.show = false;
        profile = false;
    }
    
    public void DeleteAccountYes()
    {
        _playerInfo.PlayerData.token = "";
        _playerInfo.SetDefault();
        _controller.UpdateDataFromLocal();
        _authentication.DeleteAccount();
        _authentication.SignOut();
        _signController.signUp = true;
        _popupController.show = false;
        profile = false;
    }

    public void UpdateStatus()
    {
        scoreTextUI.text = _playerInfo.PlayerData.coin + " Score";
        classTextUI.text = "Class (" + _playerInfo.PlayerData.playerClass + ")";
        winTextUI.text = _playerInfo.PlayerData.winCount + " Win";
        loseTextUI.text = _playerInfo.PlayerData.loseCount + " Lose";
    }

    void SetCanvas(CanvasGroup canvas, bool active)
    {
        float speed = 10 * Time.deltaTime;
        
        if (active)
        {
            canvas.gameObject.SetActive(true);

            if (canvas.alpha < 1)
                canvas.alpha += speed;
        }else
        {
            if (canvas.alpha > 0)
                canvas.alpha -= speed;
            
            if (canvas.alpha <= 0)
                canvas.gameObject.SetActive(false);
        }
    }
}
