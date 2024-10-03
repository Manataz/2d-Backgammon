using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Unity.Services.Analytics;
using Unity.Services.Core;

public class Controller : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    private Authentication _authentication;
    private MenuController _menuController;
    private ProfileController _profileController;
    private SignController _signController;
    private LoadingUpdate _loadingUpdate;
    private LoadExcel _loadExcel;
    private PlayWithFriends _playWithFriends;
    private RankController _rankController;
    private TournamentController _tournamentController;
    private PopupController _popupController;
    private TournamentReward _tournamentReward;

    public AudioSource clickAudio;
    
    [Header("Loading")]
    public bool loading;
    public CanvasGroup loadingUI;

    [Header("Nickname")] 
    public CanvasGroup nicknameUI;
    public bool nickname;
    public InputField nicknameInputUI;
    public Text nicknameErrorTextUI;
    public GameObject usernameErrorIcon;

    [Header("Avatar")]
    public bool avatar;
    public CanvasGroup avatarUI;
    public int avatarId;
    public Image[] avatarImage;
    public Image[] avatarFrameImage;
    public Image[] avatarVertexImage;
    public Color[] avatarVertexColor;

    [Header("Region")]
    public string regionData;
    public string stateData;
    private bool regionLoaded;
    public bool region;
    public bool regionScroll;
    public bool stateScroll;
    public Image regionIcon;
    public Sprite globalIcon;
    public CanvasGroup regionUI;
    public CanvasGroup regionScrollUI;
    public CanvasGroup stateScrollUI;
    public Image regionScrollButton;
    public Sprite[] regionButtonSprite;
    public GameObject regionItemPrefab;
    public GameObject stateItemPrefab;
    public Transform regionContent;
    public Transform stateContent;
    public ScrollRect regionScrollRect;
    public ScrollRect stateScrollRect;
    public InputField[] searchInputUI;
    public GameObject[] clearButton;
    public Text regionTextUI;
    public Text stateTextUI;
    public CanvasGroup stateButtonCanvas;
    [System.Serializable]
    public class Region
    {
        public string name;
        public Sprite flag;
    }
    public List<Region> regions = new List<Region>();
    private List<GameObject> regionItems = new List<GameObject>();
    private List<GameObject> stateItems = new List<GameObject>();

    [Serializable]
    public class State
    {
        public string region;
        public string[] name;
    }
    public List<State> states;

    [Header("Birthdate")]
    public int[] birthdateData = new []{-1,-1,-1};
    public bool birthdate;
    public bool monthScroll;
    public bool dayScroll;
    public bool yearScroll;
    public CanvasGroup birthdateUI;
    public CanvasGroup monthScrollUI;
    public CanvasGroup dayScrollUI;
    public CanvasGroup yearScrollUI;
    public Text[] birthdateTextUI;
    public ScrollRect monthScrollRect;
    public ScrollRect dayScrollRect;
    public GameObject numberItemPrefab;
    public Transform dayContent;
    public ScrollRect yearScrollRect;
    public Transform yearContent;
    
    [Header("Board")] 
    public int boardId;
    public bool board;
    public CanvasGroup boardUI;
    public Image[] boardFrameImage;
    public Image[] boardVertexImage;
    public Color[] boardVertexColor;
    
    [Header("Checker")] 
    public int checkerId;
    public bool checker;
    public CanvasGroup checkerUI;
    public Image[] checkerFrameImage;
    public Image[] checkerVertexImage;
    public Color[] checkerVertexColor;

    [Header("Text UI")] // Load Data
    public Text avatarTitleTextUI;
    public Text signUpErrorTextUI;
    public Text optionsTitleTextUI;
    public Text profileTitleTextUI;
    public Text nicknameTitleTextUI;
    public Text nicknameHolderTextUI;
    public Text regionTitleTextUI;
    public Text birthdateTitleTextUI;
    public Text boardTitleTextUI;
    public Text checkerTitleTextUI;
    public Text premiumPriceTextUI;
    public Text premiumInfo1TextUI;
    public Text premiumInfo2TextUI;
    public Text premiumTitleTextUI;
    public Text premiumReward1TextUI;
    public Text premiumReward2TextUI;
    public Text premiumReward3TextUI;

    [Header("Premium")]
    public bool premium;
    public CanvasGroup premiumUI;
    public bool premiumTimeUpdated;
    public DateTime premiumTime;
    public CanvasGroup premiumLoadingUI;
    private bool buyingPremium;
    private bool premiumNotif;
    public CanvasGroup premiumNotifUI;

    [Header("Coin UI")]
    public bool coin;
    public CanvasGroup coinUI;
    public Text[] yourPointsTextUI;
    public Text nextClassPointsTextUI;
    public Text[] doublePointsTextUI;
    public Text premiumPointsTextUI;
    public GameObject nextClassPoints;
    public GameObject doublePoints;
    public GameObject premiumPoints;
    public GameObject goPremiumButton;
    public Text classTextUI;
    public Image regionIconUI;
    public Image coinAvatarUI;
    
    [Header("Class")]
    public int classC;
    public int classB;
    public int classA;

    private void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _authentication = FindObjectOfType<Authentication>();
        _menuController = FindObjectOfType<MenuController>();
        _profileController = FindObjectOfType<ProfileController>();
        _signController = FindObjectOfType<SignController>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _playWithFriends = FindObjectOfType<PlayWithFriends>();
        _rankController = FindObjectOfType<RankController>();
        _tournamentController = FindObjectOfType<TournamentController>();
        _popupController = FindObjectOfType<PopupController>();
        _tournamentReward = FindObjectOfType<TournamentReward>();

        UnityServices.InitializeAsync();

        nicknameErrorTextUI.text = "";
        avatarUI.alpha = 0;
        nicknameUI.alpha = 0;
        loadingUI.alpha = 0;
        regionUI.alpha = 0;
        regionScrollUI.alpha = 0;
        birthdateUI.alpha = 0;
        monthScrollUI.alpha = 0;
        boardUI.alpha = 0;
        checkerUI.alpha = 0;
        coinUI.alpha = 0;
        
        SetupDayList();
        SetupYearList();
        searchInputUI[0].onValueChanged.AddListener(FilterList);
        searchInputUI[1].onValueChanged.AddListener(FilterList);
        RegionScrollToTop();
        StateScrollToTop();
        SetStateButton(false);
        
        avatarImage[0].sprite = _playerInfo.avatars[0];
        avatarImage[1].sprite = _playerInfo.avatars[1];
    }

    private void Update()
    {
        loading = (_signController.signing && !_loadingUpdate.loading) || _signController.loading;
        
        SetCanvas(avatarUI, avatar, 0, 1);
        SetCanvas(nicknameUI, nickname, 0, 1);
        SetCanvas(loadingUI, loading, 0, 1);
        SetCanvas(regionUI, region, 0, 1);
        SetCanvas(regionScrollUI, regionScroll, 0, 1);
        SetCanvas(stateScrollUI, stateScroll, 0, 1);
        SetCanvas(birthdateUI, birthdate, 0, 1);
        SetCanvas(monthScrollUI, monthScroll, 0, 1);
        SetCanvas(dayScrollUI, dayScroll, 0, 1);
        SetCanvas(yearScrollUI, yearScroll, 0, 1);
        SetCanvas(yearScrollUI, yearScroll, 0, 1);
        SetCanvas(boardUI, board, 0, 1);
        SetCanvas(checkerUI, checker, 0, 1);
        SetCanvas(premiumUI, premium, 0, 1);
        SetCanvas(coinUI, coin, 0, 1);
        SetCanvas(premiumLoadingUI, buyingPremium, 0, 1);
        SetCanvas(premiumNotifUI, premiumNotif, 0, 1);
        
        SetupAvatar();
        BoardSetup();
        CheckerSetup();
        SetPremium();
        
        usernameErrorIcon.SetActive(nicknameErrorTextUI.text.Length > 0);

        clearButton[0].SetActive(searchInputUI[0].text.Length > 0);
        clearButton[1].SetActive(searchInputUI[1].text.Length > 0);

        if (regionScroll)
            regionScrollButton.sprite = regionButtonSprite[1];
        else
            regionScrollButton.sprite = regionButtonSprite[0];
    }

    void SetStateButton(bool active)
    {
        if (active)
        {
            stateButtonCanvas.alpha = 1;
            stateButtonCanvas.interactable = true;
        }
        else
        {
            stateButtonCanvas.alpha = 0.5f;
            stateButtonCanvas.interactable = false;
        }
    }

    public void ButtonClick(string type)
    {
        clickAudio.Play();
        
        switch (type)
        {
            // Avatar
            
            case "avatar back":
            {
                avatar = false;
            }
                break;
            
            case "avatar confirm":
            {
                _playerInfo.PlayerData.avatarId = avatarId;
                UpdateDataFromLocal();
                avatar = false;
            }
                break;
            
            // Nickname
            
            case "nickname back":
            {
                nickname = false;
            }
                break;
            
            case "nickname confirm":
            {
                if (CheckInput("nickname"))
                {
                    _playerInfo.PlayerData.nickname = nicknameInputUI.text;
                    UpdateDataFromLocal();
                    nickname = false;
                }
            }
                break;
            
            // Region
            
            case "clear":
            {
                searchInputUI[0].text = "";
                searchInputUI[1].text = "";
            }
                break;
            
            case "region":
            {
                UpdateUI();
                RegionScrollToTop();
                StateScrollToTop();
                regionData = _playerInfo.PlayerData.region;
                SetupStateList();
                region = true;
            }
                break;
            
            case "region scroll":
            {
                regionScroll = true;
            }
                break;
            
            case "region back":
            {
                region = false;
            }
                break;
            
            case "region scroll back":
            {
                regionScroll = false;
            }
                break;
            
            case "region confirm":
            {
                _playerInfo.PlayerData.region = regionData;
                _playerInfo.PlayerData.state = stateData;
                UpdateDataFromLocal();
                SetupStateList();
                region = false;
            }
                break;
            
            case "state scroll":
            {
                stateScroll = true;
            }
                break;
            
            case "state scroll back":
            {
                stateScroll = false;
            }
                break;
            
            // Birthdate
            
            case "month scroll":
            {
                MonthScrollToTop();
                monthScroll = true;
            }
                break;
            
            case "month scroll back":
            {
                monthScroll = false;
            }
                break;
            
            case "day scroll":
            {
                DayScrollToTop();
                dayScroll = true;
            }
                break;
            
            case "day scroll back":
            {
                dayScroll = false;
            }
                break;
            
            case "year scroll":
            {
                YearScrollToTop();
                yearScroll = true;
            }
                break;
            
            case "year scroll back":
            {
                yearScroll = false;
            }
                break;
            
            case "birthdate":
            {
                UpdateUI();
                birthdate = true;
            }
                break;
            
            case "birthdate back":
            {
                birthdate = false;
            }
                break;
            
            case "birthdate confirm":
            {
                _playerInfo.PlayerData.birthdate = birthdateData;
                UpdateDataFromLocal();
                birthdate = false;
            }
                break;
            
            // Board
            
            case "board":
            {
                board = true;
                UpdateUI();
            }
                break;
            
            case "board back":
            {
                board = false;
            }
                break;
            
            case "board confirm":
            {
                switch (boardId)
                {
                    case 0:
                    {
                        _playerInfo.PlayerData.boardType = GameSetup.ArtClass.ArtType.type1;
                    }
                        break;
            
                    case 1:
                    {
                        _playerInfo.PlayerData.boardType = GameSetup.ArtClass.ArtType.type2;
                    }
                        break;
            
                    case 2:
                    {
                        _playerInfo.PlayerData.boardType = GameSetup.ArtClass.ArtType.type3;
                    }
                        break;
                }
                
                _playerInfo.SaveGame();
                board = false;
            }
                break;
            
            // Checker
            
            case "checker":
            {
                checker = true;
                UpdateUI();
            }
                break;
            
            case "checker back":
            {
                checker = false;
            }
                break;
            
            case "checker confirm":
            {
                switch (checkerId)
                {
                    case 0:
                    {
                        _playerInfo.PlayerData.checkerSide = GameSetup.sides.White;
                    }
                        break;
            
                    case 1:
                    {
                        _playerInfo.PlayerData.checkerSide = GameSetup.sides.Black;
                    }
                        break;
                }
                
                _playerInfo.SaveGame();
                checker = false;
            }
                break;
            
            // Premium
            
            case "premium":
            {
                premium = true;
            }
                break;
            
            case "premium back":
            {
                premium = false;
            }
                break;
            
            // Coin
            
            case "coin back":
            {
                coin = false;
            }
                break;
            
            case "premium notif back":
            {
                premiumNotif = false;
            }
                break;
        }
    }

    public void BuyPremiumButtonClick()
    {
        Debug.Log("<color=yellow>Buying premium...</color>");
        buyingPremium = true;
        StartCoroutine(_tournamentController.GetTime("premiumTime"));
    }

    void SetPremium()
    {
        if (buyingPremium && premiumTimeUpdated && !_playerInfo.PlayerData.premium)
        {
            _playerInfo.PlayerData.premiumTime = _tournamentController.ConvertTimeToString(premiumTime);
            _playerInfo.PlayerData.premium = true;
            buyingPremium = false;
            UpdateDataFromLocal();
            premium = false;
            premiumNotif = true;
            _tournamentReward.updated = false;
            Debug.Log("<color=yellow>Premium set active</color>");
        }
    }

    public void CheckPremiumTime()
    {
        if (_playerInfo.PlayerData.premium && _playerInfo.PlayerData.premiumTime != "")
        {
            Debug.Log("<color=yellow>You </color><color=green>ARE</color> <color=yellow>premium member</color>");
            
            TimeSpan differentTiming = _tournamentController.ConvertTimeToDateTime(_playerInfo.PlayerData.premiumTime) - _tournamentController.lastTime;

            if (differentTiming.TotalDays > 30)
            {
                _playerInfo.PlayerData.premium = false;
                _playerInfo.PlayerData.premiumTime = "";
                _popupController.OpenPopUp(false, false, true, "Your subscription has expired", null, null, PopUpHide);
                premiumNotif = false;
                UpdateDataFromLocal();
                Debug.Log("<color=yellow>Your premium is </color><color=red>END</color>");
            }
        }
        else
        {
            Debug.Log("<color=yellow>You are </color><color=red>NOT</color><color=yellow> premium member</color>");
        }
    }

    public void PopUpHide()
    {
        _popupController.show = false;
    }

    public void SetLoadDataUI(string type)
    {
        if (type == "book")
        {
            avatarTitleTextUI.text = _loadExcel.itemDatabase_book[0].avatarTitle;
            optionsTitleTextUI.text = _loadExcel.itemDatabase_book[0].optionsTitle;
            profileTitleTextUI.text = _loadExcel.itemDatabase_book[0].profileTitle;
            nicknameTitleTextUI.text = _loadExcel.itemDatabase_book[0].nicknameTitle;
            nicknameHolderTextUI.text = _loadExcel.itemDatabase_book[0].nicknameHolder;
            regionTitleTextUI.text = _loadExcel.itemDatabase_book[0].regionTitle;
            birthdateTitleTextUI.text = _loadExcel.itemDatabase_book[0].birthdateTitle;
            boardTitleTextUI.text = _loadExcel.itemDatabase_book[0].boardTitle;
            checkerTitleTextUI.text = _loadExcel.itemDatabase_book[0].checkerTitle;
            premiumPriceTextUI.text = _loadExcel.itemDatabase_book[0].premiumPrice;
            premiumInfo1TextUI.text = _loadExcel.itemDatabase_book[0].premiumInfo1;
            premiumInfo2TextUI.text = _loadExcel.itemDatabase_book[0].premiumInfo2;
            premiumTitleTextUI.text = _loadExcel.itemDatabase_book[0].premiumTitle;
            premiumReward1TextUI.text = _loadExcel.itemDatabase_book[0].premiumReward1;
            premiumReward2TextUI.text = _loadExcel.itemDatabase_book[0].premiumReward2;
            premiumReward3TextUI.text = _loadExcel.itemDatabase_book[0].premiumReward3;
            regionTextUI.text = _loadExcel.itemDatabase_book[0].regionButton;
            stateTextUI.text = _loadExcel.itemDatabase_book[0].stateButton;
        }
    }
    
    void BoardSetup()
    {
        float speed = 3 * Time.deltaTime;
        if (boardId == 0)
        {
            boardFrameImage[0].transform.localScale = Vector2.Lerp(boardFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            boardFrameImage[1].transform.localScale = Vector2.Lerp( boardFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardFrameImage[2].transform.localScale = Vector2.Lerp( boardFrameImage[2].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardVertexImage[0].color = boardVertexColor[1];
            boardVertexImage[1].color = boardVertexColor[0];
            boardVertexImage[2].color = boardVertexColor[0];
        }
        else if (boardId == 1)
        {
            boardFrameImage[1].transform.localScale = Vector2.Lerp(boardFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            boardFrameImage[0].transform.localScale = Vector2.Lerp( boardFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardFrameImage[2].transform.localScale = Vector2.Lerp( boardFrameImage[2].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardVertexImage[1].color = boardVertexColor[1];
            boardVertexImage[0].color = boardVertexColor[0];
            boardVertexImage[2].color = boardVertexColor[0];
        }
        else if (boardId == 2)
        {
            boardFrameImage[2].transform.localScale = Vector2.Lerp(boardFrameImage[2].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            boardFrameImage[1].transform.localScale = Vector2.Lerp( boardFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardFrameImage[0].transform.localScale = Vector2.Lerp( boardFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardVertexImage[2].color = boardVertexColor[1];
            boardVertexImage[1].color = boardVertexColor[0];
            boardVertexImage[0].color = boardVertexColor[0];
        }

        boardFrameImage[0].enabled = boardId == 0;
        boardFrameImage[1].enabled = boardId == 1;
        boardFrameImage[2].enabled = boardId == 2;
    }
    
    void CheckerSetup()
    {
        float speed = 3 * Time.deltaTime;
        if (checkerId == 0)
        {
            checkerFrameImage[0].transform.localScale = Vector2.Lerp(checkerFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            checkerFrameImage[1].transform.localScale = Vector2.Lerp( checkerFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerVertexImage[0].color = checkerVertexColor[1];
            checkerVertexImage[1].color = checkerVertexColor[0];
        }
        else if (checkerId == 1)
        {
            checkerFrameImage[1].transform.localScale = Vector2.Lerp(checkerFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            checkerFrameImage[0].transform.localScale = Vector2.Lerp( checkerFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerVertexImage[1].color = checkerVertexColor[1];
            checkerVertexImage[0].color = checkerVertexColor[0];
        }

        checkerFrameImage[0].enabled = checkerId == 0;
        checkerFrameImage[1].enabled = checkerId == 1;
    }
    
    public void CheckerClick(int id)
    {
        checkerId = id;
    }
    
    public void BoardClick(int id)
    {
        boardId = id;
    }

    public void FlagClick(string name)
    {
        regionData = name;
        UpdateRegionIcon();
        SetupStateList();
        regionScroll = false;
    }
    
    public void StateClick(string name)
    {
        stateData = name;
        UpdateStateButton();
        stateScroll = false;
    }
    
    public void MonthClick(int id)
    {
        birthdateData[0] = id;
        UpdateBirthdateUI();
        monthScroll = false;
    }
    
    public void DayClick(int id)
    {
        birthdateData[1] = id;
        UpdateBirthdateUI();
        dayScroll = false;
    }
    
    public void YearClick(int id)
    {
        birthdateData[2] = id;
        UpdateBirthdateUI();
        yearScroll = false;
    }

    void UpdateBirthdateUI()
    {
        for (int i = 0; i < birthdateData.Length; i++)
        {
            if (birthdateData[i] != -1)
            {
                switch (i)
                {
                    case 0:
                    {
                        switch (birthdateData[i])
                        {
                            case 1: birthdateTextUI[i].text = "January";
                                break;
                            case 2: birthdateTextUI[i].text = "February";
                                break;
                            case 3: birthdateTextUI[i].text = "March";
                                break;
                            case 4: birthdateTextUI[i].text = "April";
                                break;
                            case 5: birthdateTextUI[i].text = "May";
                                break;
                            case 6: birthdateTextUI[i].text = "June";
                                break;
                            case 7: birthdateTextUI[i].text = "July";
                                break;
                            case 8: birthdateTextUI[i].text = "August";
                                break;
                            case 9: birthdateTextUI[i].text = "September";
                                break;
                            case 10: birthdateTextUI[i].text = "October";
                                break;
                            case 11: birthdateTextUI[i].text = "November";
                                break;
                            case 12: birthdateTextUI[i].text = "December";
                                break;
                        }
                    }
                        break;
                    
                    case 1:
                        birthdateTextUI[i].text = birthdateData[i].ToString();
                        break;
                    
                    case 2:
                        birthdateTextUI[i].text = birthdateData[i].ToString();
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0: birthdateTextUI[i].text = "Month";
                        break;
                    
                    case 1: birthdateTextUI[i].text = "Day";
                        break;
                    case 2: birthdateTextUI[i].text = "Year";
                        break;
                }
            }
        }
    }

    void RegionScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        regionScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void StateScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        stateScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void MonthScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        monthScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void DayScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        dayScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void YearScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        yearScrollRect.verticalNormalizedPosition = 1f;
    }
    
    public void SetupRegionList()
    {
        for (int i = 0; i < regions.Count; i++)
        {
            string fieldName = $"r{i + 1}";
            FieldInfo field = _loadExcel.itemDatabase_regions[0].GetType().GetField(fieldName);
            
            if (field != null)
            {
                string regionName = field.GetValue(_loadExcel.itemDatabase_regions[0]) as string;

                if (regionName != "null" && regionName != "")
                {
                    GameObject newItem = Instantiate(regionItemPrefab, regionContent);
                    newItem.transform.Find("flag").GetComponent<Image>().sprite = regions[i].flag;
                    newItem.transform.Find("name").GetComponent<Text>().text = regionName;
                    newItem.GetComponent<Button>().onClick.AddListener(() => FlagClick(regionName));
                    regionItems.Add(newItem);
                }
            }
        }

        RegionScrollToTop();
    }

    public void SetupStateList()
    {
        bool founded = false;
        State state = new State();
        foreach (State _state in states)
        {
            if (_state.region == regionData)
            {
                state = _state;
                founded = true;
                break;
            }
        }
        
        SetStateButton(founded);

        if (founded)
        {
            if (stateItems.Count > 0)
            {
                foreach (GameObject stateItem in stateItems)
                {
                    Destroy(stateItem);
                }
                
                stateItems.Clear();
            }
            
            for (int i = 0; i < state.name.Length ; i++)
            {
                GameObject newItem = Instantiate(stateItemPrefab, stateContent);
                string name = state.name[i];
                newItem.transform.Find("name").GetComponent<Text>().text = name;
                newItem.GetComponent<Button>().onClick.AddListener(() => StateClick(name));
                stateItems.Add(newItem);
            }
        }
        else
        {
            stateTextUI.text = _loadExcel.itemDatabase_book[0].stateButton;
            Debug.Log("<color=red>No state found for this region: </color>" + regionData);
        }
        
        StateScrollToTop();
        UpdateStateButton();
    }

    void SetupDayList()
    {
        for (int i=1; i<=31;i++)
        {
            GameObject newItem = Instantiate(numberItemPrefab, dayContent);
            newItem.transform.Find("name").GetComponent<Text>().text = i.ToString();
            int day = i;
            newItem.GetComponent<Button>().onClick.AddListener(() => DayClick(day));
        }

        DayScrollToTop();
    }
    
    void SetupYearList()
    {
        int currentYear = System.DateTime.Now.Year;
        int startYear = currentYear - 10;
        int endYear = currentYear - 70;

        for (int year = startYear; year >= endYear; year--)
        {
            GameObject newItem = Instantiate(numberItemPrefab, yearContent);
            newItem.transform.Find("name").GetComponent<Text>().text = year.ToString();
            int y = year;
            newItem.GetComponent<Button>().onClick.AddListener(() => YearClick(y));
        }

        YearScrollToTop();
    }
    
    void FilterList(string searchText)
    {
        List<GameObject> filteredItems = new List<GameObject>();

        foreach (var item in regionItems)
        {
            var countryName = item.transform.Find("name").GetComponent<Text>().text;
            bool isMatch = countryName.ToLower().Contains(searchText.ToLower());
            item.SetActive(isMatch);
            
            if (isMatch)
                filteredItems.Add(item);
        }

        for (int i = 0; i < filteredItems.Count; i++)
            filteredItems[i].transform.SetSiblingIndex(i);

        RegionScrollToTop();
    }
    
    void FilterListState(string searchText)
    {
        List<GameObject> filteredItems = new List<GameObject>();

        foreach (var item in stateItems)
        {
            var stateName = item.transform.Find("name").GetComponent<Text>().text;
            bool isMatch = stateName.ToLower().Contains(searchText.ToLower());
            item.SetActive(isMatch);
            
            if (isMatch)
                filteredItems.Add(item);
        }

        for (int i = 0; i < filteredItems.Count; i++)
            filteredItems[i].transform.SetSiblingIndex(i);

        StateScrollToTop();
    }

    public async Task UpdateDataFromLocal()
    {
        await _authentication.SaveDataAsync(_playerInfo.PlayerData.token, _playerInfo.PlayerData.coin, _playerInfo.PlayerData.region, _playerInfo.PlayerData.state, _playerInfo.PlayerData.refrralCode, _playerInfo.PlayerData.birthdate, _playerInfo.PlayerData.nickname, _playerInfo.PlayerData.reserved, (_playerInfo.PlayerData.premium) ? 1 : 0, _playerInfo.PlayerData.avatarId, (_playerInfo.PlayerData.tournament) ? 1 : 0, _playerInfo.PlayerData.tournamentId, _playerInfo.PlayerData.phone, _playerInfo.PlayerData.address, _playerInfo.PlayerData.winCount, _playerInfo.PlayerData.loseCount, _playerInfo.PlayerData.premiumTime);
        _authentication.UpdateLeaderboard(_playerInfo.PlayerData.nickname, _playerInfo.PlayerData.region);
        
        Debug.Log("<color=orange> Updated Data From Local </color>");
        _playerInfo.SaveGame();
        
        UpdateUI();
    }

    public void InputChanged()
    {
        nicknameErrorTextUI.text = "";
    }
    
    bool CheckInput(string type)
    {
        bool ok = true;
        
        switch (type)
        {
            case "nickname":
            {
                if (nicknameInputUI.text.Length == 0)
                {
                    string message = _loadExcel.itemDatabase_book[0].nicknameEmptyError;
                    nicknameErrorTextUI.text = message;
                    ok = false;
                }
                else
                if (nicknameInputUI.text.Length > 11)
                {
                    ok = false;
                    string message = _loadExcel.itemDatabase_book[0].nicknameCharError;
                    nicknameErrorTextUI.text = message;
                }

                if (!IsEnglishAlphabetic(nicknameInputUI.text))
                {
                    ok = false;
                    string message = _loadExcel.itemDatabase_book[0].nicknameEnglishError;
                    nicknameErrorTextUI.text = message;
                }
            }
                break;
        }

        return ok;
    }
    
    public bool IsEnglishAlphabetic(string input)
    {
        foreach (char c in input)
        {
            if (!((c >= 'A' && c <= 'Z') || 
                  (c >= 'a' && c <= 'z') || 
                  (c >= '0' && c <= '9') || 
                  c == ' '))
            {
                return false;
            }
        }

        return true;
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

    void UpdateRegionIcon()
    {
        if (regionData != "")
        {
            foreach (Region region in regions)
            {
                if (region.name == regionData)
                {
                    regionIcon.sprite = region.flag;
                    break;
                }
            }
        }
        else
            regionIcon.sprite = globalIcon;
    }

    public Sprite GetRegionIcon(string name)
    {
        if (regionData != "")
        {
            foreach (Region region in regions)
            {
                if (region.name == name)
                {
                    return region.flag;
                    break;
                }
            }
            
            return globalIcon;
        }
        else
            return globalIcon;
    }
    
    void UpdateStateButton()
    {
        stateTextUI.text = stateData;
        
        if (stateData == "")
            stateTextUI.text = _loadExcel.itemDatabase_book[0].stateButton;
    }
    
    public void UpdateUI()
    {
        _menuController.usernameTextUI.text = _playerInfo.PlayerData.nickname;
        _menuController.coinTextUI.text = _playerInfo.PlayerData.coin.ToString();
        _profileController.usernameTextUI.text = _playerInfo.PlayerData.nickname;
        avatarId = _playerInfo.PlayerData.avatarId;
        nicknameInputUI.text = _playerInfo.PlayerData.nickname;
        regionData = _playerInfo.PlayerData.region;
        birthdateData = new []{_playerInfo.PlayerData.birthdate[0], _playerInfo.PlayerData.birthdate[1], _playerInfo.PlayerData.birthdate[2]};
        _profileController.UpdateStatus();
        _menuController.friendCoinTextUI.text = "+" + _playWithFriends.coin.ToString();

        #region Avatar
            _menuController.avatarUI.sprite = _playerInfo.avatars[_playerInfo.PlayerData.avatarId];
            _profileController.avatarUI.sprite = _playerInfo.avatars[_playerInfo.PlayerData.avatarId];
            _rankController.avatarUI.sprite = _playerInfo.avatars[_playerInfo.PlayerData.avatarId];
            coinAvatarUI.sprite = _playerInfo.avatars[_playerInfo.PlayerData.avatarId];
        #endregion

        switch (_playerInfo.PlayerData.boardType)
        {
            case GameSetup.ArtClass.ArtType.type1:
            {
                boardId = 0;
            }
                break;
            
            case GameSetup.ArtClass.ArtType.type2:
            {
                boardId = 1;
            }
                break;
            
            case GameSetup.ArtClass.ArtType.type3:
            {
                boardId = 2;
            }
                break;
        }
        
        switch (_playerInfo.PlayerData.checkerSide)
        {
            case GameSetup.sides.White:
            {
                checkerId = 0;
            }
                break;
            
            case GameSetup.sides.Black:
            {
                checkerId = 1;
            }
                break;
        }
        
        _menuController.premiumButton.SetActive(!_playerInfo.PlayerData.premium);
        _menuController.premiumMember.SetActive(_playerInfo.PlayerData.premium);

        _menuController.classTextUI.text = _playerInfo.PlayerData.playerClass;
        UpdateRegionIcon();
        UpdateBirthdateUI();

        if (_playerInfo.PlayerData.region != "")
        {
            _profileController.regionIconUI.sprite = regionIcon.sprite;
            _menuController.regionIconUI.sprite = regionIcon.sprite;
            regionIconUI.sprite = regionIcon.sprite;
            _rankController.regionIconUI.sprite = regionIcon.sprite;
        }
        else
        {
            _profileController.regionIconUI.sprite = globalIcon;
            _menuController.regionIconUI.sprite = globalIcon;
            regionIconUI.sprite = globalIcon;
            _rankController.regionIconUI.sprite = globalIcon;
        }

        Vector2 pos = _profileController.usernameTextUI.transform.position;
        if (_playerInfo.PlayerData.birthdate[2] != -1)
        {
            int currentYear = System.DateTime.Now.Year; _profileController.ageTextUI.text = "<color=grey> Age: </color>" + "<color=black>" + (currentYear - _playerInfo.PlayerData.birthdate[2]) + "</color>";
            
            pos.y = _profileController.usernameTextPosY_default;
        }
        else
        {
            _profileController.ageTextUI.text = "";
            pos.y = _profileController.usernameTextPosY_default - 20;
        }
        _profileController.usernameTextUI.transform.position = pos;
            

        if (_playerInfo.PlayerData.email == "")
            _profileController.emailTextUI.text = "<color=red>  " + "(Unregistered)" + "</color>";
        else
            _profileController.emailTextUI.text = _playerInfo.PlayerData.email;
        
        if (_playerInfo.PlayerData.phone == "")
            _profileController.phoneTextUI.text = "<color=red>   " + "(Unregistered)" + "</color>";
        else
            _profileController.phoneTextUI.text = _playerInfo.PlayerData.phone;

        #region Coin UI
            yourPointsTextUI[0].text = _playerInfo.PlayerData.coin.ToString();
            yourPointsTextUI[1].text = _playerInfo.PlayerData.coin.ToString();
            
            if (_playerInfo.PlayerData.coin <= classC)
                nextClassPointsTextUI.text = (classB - _playerInfo.PlayerData.coin).ToString();
            else
            if (_playerInfo.PlayerData.coin <= classA)
                nextClassPointsTextUI.text = ((classA+1) - _playerInfo.PlayerData.coin).ToString();
            
            nextClassPoints.SetActive(_playerInfo.PlayerData.coin <= classA);
            doublePoints.SetActive(!_playerInfo.PlayerData.premium);
            premiumPoints.SetActive(!_playerInfo.PlayerData.premium);
            goPremiumButton.SetActive(!_playerInfo.PlayerData.premium);
            
            if (!_playerInfo.PlayerData.premium)
            {
                doublePointsTextUI[0].text = _playerInfo.PlayerData.reserved.ToString();
                doublePointsTextUI[1].text = _playerInfo.PlayerData.reserved.ToString();
                premiumPointsTextUI.text = (_playerInfo.PlayerData.coin + _playerInfo.PlayerData.reserved).ToString();
            }
            
            classTextUI.text = _playerInfo.PlayerData.playerClass + " Class";
        #endregion
    }

    void SetupAvatar()
    {
        float speed = 3 * Time.deltaTime;
        if (avatarId == 0)
        {
            avatarFrameImage[0].transform.localScale = Vector2.Lerp(avatarFrameImage[0].transform.localScale, new Vector2(1f, 1f), speed);
            avatarFrameImage[1].transform.localScale = Vector2.Lerp( avatarFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            avatarVertexImage[0].color = avatarVertexColor[1];
            avatarVertexImage[1].color = avatarVertexColor[0];
        }
        else if (avatarId == 1)
        {
            avatarFrameImage[1].transform.localScale = Vector2.Lerp(avatarFrameImage[1].transform.localScale, new Vector2(1f, 1f), speed);
            avatarFrameImage[0].transform.localScale = Vector2.Lerp(avatarFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
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

    public void SetCanvas(CanvasGroup canvas, bool active, float min, float max)
    {
        float speed = 10 * Time.deltaTime;
        
        if (active)
        {
            canvas.gameObject.SetActive(true);

            if (canvas.alpha < max)
                canvas.alpha += speed;
        }else
        {
            if (canvas.alpha > min)
                canvas.alpha -= speed;
            
            if (canvas.alpha <= 0 && min == 0)
                canvas.gameObject.SetActive(false);
        }
    }
    
    public int[] ConvertStringToIntArray(string input)
    {
        input = input.Trim('[', ']');
        string[] stringArray = input.Split(',');
        int[] intArray = new int[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
        {
            intArray[i] = int.Parse(stringArray[i]);
        }

        return intArray;
    }
}
