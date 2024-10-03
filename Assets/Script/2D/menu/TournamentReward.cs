using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentReward : MonoBehaviour
{
    [Header("General")]
    public bool updated;
    public bool tournamentReward;
    public bool goPremium;
    public bool inviteFriends;
    public bool completeInfo;
    public bool award;
    public int completeLevel;
    public TimeSpan differentTime;
    private bool haveAward;
    public bool editInfo;
    public bool isActive;

    [Header("Canvas")]
    public CanvasGroup tournamentRewardUI;
    public CanvasGroup goPremiumUI;
    public CanvasGroup inviteFriendsUI;
    public CanvasGroup completeInfoUI;
    public CanvasGroup nicknameCanvasUI;
    public CanvasGroup phoneNumberCanvasUI;
    public CanvasGroup addressCanvasUI;
    public CanvasGroup verifyCanvasUI;
    public CanvasGroup doneCanvasUI;
    public CanvasGroup awardUI;
    public CanvasGroup awardButtonUI;
    public CanvasGroup editInfoUI;

    [Header("Access")]
    public Text premiumInfoTextUI;
    public Text inviteFriendsInfoTextUI;
    public Text nicknameHolderTextUI;
    public List<CompleteLevelClass> completeLevels;
    public GameObject nicknameErrorIcon;
    public GameObject phoneNumberErrorIcon;
    public GameObject addressErrorIcon;
    public GameObject continueButton;
    public GameObject verifyButton;
    public GameObject mainMenuButton;
    public Text completeDoneTextUI;
    public GameObject awardTimeBox;
    public GameObject prizeTimeBox;
    public GameObject prizeUI;
    public GameObject[] yes;
    public GameObject[] no;
    public Image claimButton;
    // Edit Info
    public Text nameTextUI;
    public Text phoneTextUI;
    public Text addressTextUI;

    [Header("Time UI")]
    public Text[] hoursTextUI;
    public Text[] minTextUI;
    public Text awardTimeTextUI;
    public Text prizeTimeTextUI;

    [Header("Input")]
    public InputField nicknameInput;
    public InputField phoneNumberInput;
    public InputField addressInput;
    public Text nicknameErrorTextUI;
    public Text phoneNumberErrorTextUI;
    public Text addressErrorTextUI;

    [Header("UI")]
    public Color normalColor;
    public Color doneColor;
    public Sprite[] claimButtonSprite;

    [Serializable]
    public class CompleteLevelClass
    {
        public Image level;
        public Image vertex;
        public Image line;
        public GameObject tick;
    }

    private Controller _controller;
    private PlayerInfo _playerInfo;
    private LoadExcel _loadExcel;
    private TournamentController _tournamentController;

    private void Start()
    {
        _controller = FindObjectOfType<Controller>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _tournamentController = FindObjectOfType<TournamentController>();

        tournamentRewardUI.alpha = 0;
        goPremiumUI.alpha = 0;
        inviteFriendsUI.alpha = 0;
        completeInfoUI.alpha = 0;
        awardUI.alpha = 0;

        nicknameErrorTextUI.text = "";
        phoneNumberErrorTextUI.text = "";
        addressErrorTextUI.text = "";

        awardButtonUI.interactable = false;
        prizeUI.SetActive(false);
    }

    private void Update()
    {
        if (!updated && _loadExcel.updated && _playerInfo.Updated && _tournamentController.timeUpdated)
        {
            completeLevel = _playerInfo.PlayerData.completeLevel;
            premiumInfoTextUI.text = _loadExcel.itemDatabase_book[0].tournamentPremiumInfo;
            inviteFriendsInfoTextUI.text = _loadExcel.itemDatabase_book[0].tournamentInviteFriendsInfo;
            nicknameHolderTextUI.text = _loadExcel.itemDatabase_book[0].nicknameHolder;
            completeDoneTextUI.text = _loadExcel.itemDatabase_book[0].tournamentCompleteDone;

            if (_playerInfo.PlayerData.premium)
                claimButton.sprite = claimButtonSprite[1];
            else
                claimButton.sprite = claimButtonSprite[0];

            SetCase();
            UpdateCompleteLevels();
            CheckRewardTime();
            
            updated = true;
        }
        
        haveAward = differentTime.TotalHours <= 49 && differentTime.TotalSeconds > 0 && differentTime.Hours != 0;
        
        SetUI();
        SetCanvas();
        SetCompleteButton();
        SetErrorIcon();
    }

    void SetUI()
    {
        if (haveAward && updated)
        {
            hoursTextUI[0].text = (differentTime.TotalHours-1).ToString("F0");
            minTextUI[0].text = differentTime.Minutes.ToString("F0");
            hoursTextUI[1].text = (differentTime.TotalHours-1).ToString("F0");
            minTextUI[1].text = differentTime.Minutes.ToString("F0");
            
            if (_playerInfo.PlayerData.premium && _playerInfo.PlayerData.friendsInvited >= 2)
            {
                prizeUI.SetActive(true);
                prizeTimeTextUI.text = (differentTime.TotalHours-1).ToString("F0") + ":" + differentTime.Minutes.ToString("F0");
                SetAwardUI(false);
            }
            else
            {
                SetAwardUI(true);
                awardTimeTextUI.text = (differentTime.TotalHours-1).ToString("F0") + ":" + differentTime.Minutes.ToString("F0");
            }
        }
        else
        {
            hoursTextUI[0].text = "";
            minTextUI[0].text = "";
            hoursTextUI[1].text = "";
            minTextUI[1].text = "";
            
            SetAwardUI(false);
            prizeUI.SetActive(false);
        }
    }

    void SetAwardUI(bool active)
    {
        if (active)
        {
            awardButtonUI.alpha = 1f;
            awardButtonUI.interactable = true;
            awardTimeBox.SetActive(true);
        }
        else
        {
            awardButtonUI.alpha = 0.5f;
            awardButtonUI.interactable = false;
            awardTimeBox.SetActive(false);
        }
    }

    void SetCanvas()
    {
        _controller.SetCanvas(tournamentRewardUI, tournamentReward, 0, 1);
        _controller.SetCanvas(goPremiumUI, goPremium, 0, 1);
        _controller.SetCanvas(inviteFriendsUI, inviteFriends, 0, 1);
        _controller.SetCanvas(completeInfoUI, completeInfo, 0, 1);
        _controller.SetCanvas(nicknameCanvasUI, completeInfo && completeLevel == 0, 0, 1);
        _controller.SetCanvas(phoneNumberCanvasUI, completeInfo && completeLevel == 1, 0, 1);
        _controller.SetCanvas(addressCanvasUI, completeInfo && completeLevel == 2 && !editInfo, 0, 1);
        _controller.SetCanvas(verifyCanvasUI, completeInfo && completeLevel == 3, 0, 1);
        _controller.SetCanvas(doneCanvasUI, completeInfo && completeLevel == 4, 0, 1);
        _controller.SetCanvas(awardUI, award, 0, 1);
        _controller.SetCanvas(editInfoUI, editInfo && completeLevel == 2, 0, 1);
    }

    void SetCase()
    {
        yes[0].SetActive(_playerInfo.PlayerData.premium);
        no[0].SetActive(!yes[0].activeSelf);
            
        yes[1].SetActive(_playerInfo.PlayerData.completeLevel >= 4);
        no[1].SetActive(!yes[1].activeSelf);
            
        yes[2].SetActive(_playerInfo.PlayerData.friendsInvited >= 2);
        no[2].SetActive(!yes[2].activeSelf);
    }

    void SetCompleteButton()
    {
        continueButton.SetActive(completeInfo && completeLevel < 3 && !editInfo);
        verifyButton.SetActive(completeInfo && completeLevel == 3);
        mainMenuButton.SetActive(completeInfo && completeLevel == 4);
    }

    void SetErrorIcon()
    {
        nicknameErrorIcon.SetActive(nicknameErrorTextUI.text.Length > 0);
        phoneNumberErrorIcon.SetActive(phoneNumberErrorTextUI.text.Length > 0);
        addressErrorIcon.SetActive(addressErrorTextUI.text.Length > 0);
    }

    public void CheckRewardTime()
    {
        DateTime rewardTime = _tournamentController.StringToDateTime(_playerInfo.PlayerData.rewardTime);

        if (rewardTime > _tournamentController.lastTime)
        {
            differentTime = rewardTime - _tournamentController.lastTime;
            
            if (differentTime.TotalHours <= 49)
            {
                Debug.Log("<color=yellow> Tournament reward is active </color>");
                isActive = true;
                StartCoroutine(CountdownCoroutine());
            }
            else
            {
                tournamentReward = false;
                isActive = false;
                Debug.Log("<color=yellow> Tournament reward is end </color>");
            }
        }
        else
        {
            tournamentReward = false;
            isActive = false;
            Debug.Log("<color=yellow> Tournament reward is end </color>");
        }
    }
    
    IEnumerator CountdownCoroutine()
    {
        while (differentTime.TotalSeconds > 0)
        {
            differentTime = differentTime.Subtract(TimeSpan.FromSeconds(1));
            yield return new WaitForSeconds(1);
        }

        tournamentReward = false;
        Debug.Log("<color=red> Tournament reward time out </color>");
    }

    public void Open()
    {
        if (!_playerInfo.PlayerData.premium)
            goPremium = true;
        else if (_playerInfo.PlayerData.friendsInvited < 2)
            inviteFriends = true;
        else
            completeInfo = true;

        tournamentReward = true;
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "continue":
            {
                switch (completeLevel)
                {
                    case 0:
                    {
                        if (CheckInput("nickname"))
                        {
                            _playerInfo.PlayerData.nickname = nicknameInput.text;
                            _controller.UpdateDataFromLocal();
                            completeLevel += 1;
                        }
                    }
                        break;
                    
                    case 1:
                    {
                        if (CheckInput("phone number"))
                        {
                            _playerInfo.PlayerData.phone = phoneNumberInput.text;
                            _controller.UpdateDataFromLocal();
                            completeLevel += 1;
                        }
                    }
                        break;
                    
                    case 2:
                    {
                        if (CheckInput("address"))
                        {
                            _playerInfo.PlayerData.address = addressInput.text;
                            _controller.UpdateDataFromLocal();

                            nameTextUI.text = "Your name: " + _playerInfo.PlayerData.nickname;
                            phoneTextUI.text = "Your phone number: " + _playerInfo.PlayerData.phone;
                            addressTextUI.text = "Your address: " + _playerInfo.PlayerData.address;
                            editInfo = true;
                        }
                    }
                        break;
                }
                
                UpdateCompleteLevels();
                _playerInfo.PlayerData.completeLevel = completeLevel;
                _playerInfo.SaveGame();
            }
                break;

            case "verify":
            {
                if (completeLevel == 3)
                    completeLevel += 1;
                
                UpdateCompleteLevels();
            }
                break;
            
            case "back":
            {
                Close();
            }
                break;

            case "premium":
            {
                Close();
                _controller.ButtonClick("premium");
            }
                break;
            
            case "continue invite friends":
            {
                inviteFriends = false;
                completeInfo = true;
            }
                break;
            
            case "award":
            {
                award = true;
            }
                break;
            
            case "edit info":
            {
                completeLevel = 0;
                _playerInfo.PlayerData.completeLevel = 0;
                editInfo = false;
                _playerInfo.SaveGame();
                UpdateCompleteLevels();
            }
                break;

            case "confirm":
            {
                if (editInfo)
                {
                    completeLevel += 1;
                    editInfo = false;
                    UpdateCompleteLevels();
                }
            }
                break;
        }
    }

    void Close()
    {
        _playerInfo.PlayerData.completeLevel = completeLevel;
        _playerInfo.SaveGame();
        
        tournamentReward = false;
        goPremium = false;
        inviteFriends = false;
        completeInfo = false;
        award = false;
        updated = false;
    }

    void UpdateCompleteLevels()
    {
        for (int i = 0; i < completeLevels.Count; i++)
        {
            if (i < completeLevel)
            {
                completeLevels[i].level.color = doneColor;
                completeLevels[i].vertex.color = doneColor;
                completeLevels[i].line.color = doneColor;
                completeLevels[i].tick.SetActive(true);
            }
            else
            {
                completeLevels[i].level.color = normalColor;
                completeLevels[i].vertex.color = normalColor;
                completeLevels[i].line.color = normalColor;
                completeLevels[i].tick.SetActive(false);
            }
        }
    }
    
    bool CheckInput(string type)
    {
        bool ok = true;
        
        switch (type)
        {
            case "nickname":
            {
                if (nicknameInput.text.Length == 0)
                {
                    string message = _loadExcel.itemDatabase_book[0].nicknameEmptyError;
                    nicknameErrorTextUI.text = message;
                    ok = false;
                }
                else
                if (nicknameInput.text.Length > 11)
                {
                    ok = false;
                    string message = _loadExcel.itemDatabase_book[0].nicknameCharError;
                    nicknameErrorTextUI.text = message;
                }

                if (!_controller.IsEnglishAlphabetic(nicknameInput.text))
                {
                    ok = false;
                    string message = _loadExcel.itemDatabase_book[0].nicknameEnglishError;
                    nicknameErrorTextUI.text = message;
                }
            }
                break;
            
            case "phone number":
            {
                if (phoneNumberInput.text.Length == 0)
                {
                    string message = "Phone number cannot be empty";
                    phoneNumberErrorTextUI.text = message;
                    ok = false;
                }
                else
                if (phoneNumberInput.text.Length > 20)
                {
                    ok = false;
                    string message = "The phone number cannot exceed 20 digits";
                    phoneNumberErrorTextUI.text = message;
                }

                if (!_controller.IsNumeric(phoneNumberInput.text))
                {
                    ok = false;
                    string message = "The phone number must contain only digits";
                    phoneNumberErrorTextUI.text = message;
                }
            }
                break;
            
            case "address":
            {
                if (addressInput.text.Length == 0)
                {
                    string message = "Address cannot be empty";
                    addressErrorTextUI.text = message;
                    ok = false;
                }
            }
                break;
        }

        return ok;
    }
    
    public void InputChanged()
    {
        nicknameErrorTextUI.text = "";
        phoneNumberErrorTextUI.text = "";
        addressErrorTextUI.text = "";
    }
}
