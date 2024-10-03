using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TournamentController : MonoBehaviour
{
    [Header("General")]
    public bool active;
    public bool canActive;
    public bool toActive;
    public bool timeUpdated;
    public bool toUpdateData;
    public bool dataUpdated;
    private bool rewardTimeUpdating;
    private bool rewardTimeUpdated;
    private bool editInfo;
    
    [Header("Tournament UI")]
    public bool tournament;
    public CanvasGroup tournamentUI;

    [Header("Options")]
    public int gameNumber;
    private string startTournament;
    private bool toStart;
    private string id;
    
    [Header("Time")]
    public DateTime lastTime;
    private DateTime startTime;
    private TimeSpan differentTimingToStart;
    private bool lessDay_startTime;

    [Header("Access")]
    public CanvasGroup tournomantButtonUI;
    public CanvasGroup playOnlineButtonUI;
    public CanvasGroup playWithFriendsButtonUI;
    public GameObject tournamentTimeBox;
    public GameObject playWithFriendsExtraIcon;
    public Text timeTextUI;
    public Transform matches;
    public Text infoTextUI;
    public Text[] searchInfoTextUI;
    public GameObject startButton;
    public GameObject[] claimPrizeButton;
    public CanvasGroup loadingUI;
    public GameObject[] giftIcon;
    public GameObject[] coinIcon;
    // Finish
    private bool finish;
    public GameObject finishUI;
    public Text finishTitleTextUI;
    public Text earnedPointsTextUI;
    public GameObject finishCoinIcon;
    public GameObject reservedIcon;
    public Text finishInfoTextUI;
    public CanvasGroup finishLoadingUI;
    public GameObject exitButton;
    public GameObject nextButton;
    public CanvasGroup rewardTimeLoadingUI;
    public Text finishExitButtonTextUI;

    [Header("Prefab")]
    public GameObject matchPrefab;
    public GameObject prizePrefab;

    [Header("Match")]
    public Image prize;
    public List<MatchClass> match = new List<MatchClass>();
    [Serializable]
    public class MatchClass
    {
        public Status status;
        public Image image;

        public enum Status
        {
            none,
            win,
            lose
        }
    }

    [Header("Color")]
    public Color winColor;
    public Color loseColor;
    
    private LoadExcel _loadExcel;
    private PopupController _popupController;
    private Controller _controller;
    private PlayerInfo _playerInfo;
    private SearchSystem _searchSystem;
    private OnlineGameServer _onlineGameServer;
    private TournamentReward _tournamentReward;
    private MenuController _menuController;

    public class TournamnetClass
    {
        public string id;
        public string start_time;
        public string coin_award;
        public string other_award;
        public int wins_to_champion;
    }

    private void Start()
    {
        _loadExcel = FindObjectOfType<LoadExcel>();
        _popupController = FindObjectOfType<PopupController>();
        _controller = FindObjectOfType<Controller>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _tournamentReward = FindObjectOfType<TournamentReward>();
        _menuController = FindObjectOfType<MenuController>();

        tournament = false;
        tournamentUI.alpha = 0;
        tournamentTimeBox.SetActive(false);
        StartCoroutine(GetTime("lastTime"));
    }

    private void Update()
    {
        if (!toUpdateData && _loadExcel.updated && timeUpdated)
        {
            StartCoroutine(GetData());
            _controller.CheckPremiumTime();
            toUpdateData = true;
        }
        
        _controller.SetCanvas(loadingUI, !dataUpdated && _menuController.menu, 0, 1);
        SetButtonUI(tournomantButtonUI, (active || canActive) && dataUpdated);
        SetButtonUI(playOnlineButtonUI, !active && dataUpdated);
        SetButtonUI(playWithFriendsButtonUI, !active && dataUpdated);
        _controller.SetCanvas(tournamentUI, tournament, 0, 1);
        _controller.SetCanvas(finishLoadingUI, !dataUpdated && finishUI.activeSelf, 0, 1);
        searchInfoTextUI[0].gameObject.SetActive(_searchSystem.searching || _searchSystem.canceling || _searchSystem.opponentFound);
        searchInfoTextUI[1].gameObject.SetActive(_searchSystem.searching || _searchSystem.canceling || _searchSystem.opponentFound);
        _controller.SetCanvas(rewardTimeLoadingUI, rewardTimeUpdating || (rewardTimeUpdated && _tournamentReward.differentTime.Hours == 0), 0, 1);
        startButton.gameObject.SetActive(!searchInfoTextUI[0].gameObject.activeSelf && !_playerInfo.PlayerData.tournamentLose && !claimPrizeButton[0].activeSelf);
        exitButton.SetActive(!_searchSystem.searching);
        nextButton.SetActive(startButton.activeSelf && !claimPrizeButton[0].activeSelf);
        claimPrizeButton[0].SetActive(_playerInfo.PlayerData.tournamentMatchWins >= gameNumber && dataUpdated);
        claimPrizeButton[1].SetActive(claimPrizeButton[0].activeSelf);
        playWithFriendsExtraIcon.SetActive(dataUpdated);

        if (_playerInfo.PlayerData.tournamentMatchWins >= gameNumber && dataUpdated)
            finishExitButtonTextUI.text = "Main Menu";
        else
            finishExitButtonTextUI.text = "Exit Tournament";
        
        SetTimeText();
        SetSearchInfo();
    }

    void SetSearchInfo()
    {
        if (_onlineGameServer.connected || _searchSystem.opponentFound)
        {
            if (_searchSystem.canceling)
            {
                searchInfoTextUI[0].text = "<color=red>Canceling...</color>";
                searchInfoTextUI[1].text = "<color=red>Canceling...</color>";
            }
            else
            if (_searchSystem.opponentFound)
            {
                searchInfoTextUI[0].text = "<color=yellow>Opponent found, starting the game...</color>";
                searchInfoTextUI[1].text = "<color=yellow>Opponent found, starting the game...</color>";
            }
            else
            if (_searchSystem.searching)
            {
                searchInfoTextUI[0].text = "Waiting for opponent...";
                searchInfoTextUI[1].text = "Waiting for opponent...";
            }
            else
            {
                searchInfoTextUI[0].text = "";
                searchInfoTextUI[1].text = "";
            }
        }
        else
        {
            searchInfoTextUI[0].text = "Connecting...";
            searchInfoTextUI[1].text = "Connecting...";
        }
    }
    
    public void SetUI(string type)
    {
        switch (type)
        {
            case "finish":
            {
                finishUI.SetActive(true);
                finish = true;

                if (_playerInfo.PlayerData.tournamentLose)
                {
                    finishTitleTextUI.text = "You lost";
                    earnedPointsTextUI.text = "<color=red>-" + _playerInfo.PlayerData.tournamentDoublePoint.ToString() + "</color>";

                    if (_playerInfo.PlayerData.premium || _playerInfo.PlayerData.reserved < _playerInfo.PlayerData.tournamentDoublePoint)
                    {
                        _playerInfo.PlayerData.coin -= _playerInfo.PlayerData.tournamentDoublePoint;
                        finishCoinIcon.SetActive(true);
                        reservedIcon.SetActive(false);
                    }
                    else
                    {
                        _playerInfo.PlayerData.reserved -= _playerInfo.PlayerData.tournamentDoublePoint;
                        finishCoinIcon.SetActive(false);
                        reservedIcon.SetActive(true);
                    }
                }
                else
                {
                    finishTitleTextUI.text = "You won";
                    earnedPointsTextUI.text = "<color=green>+" + _playerInfo.PlayerData.tournamentDoublePoint.ToString() + "</color>";
                    
                    if (_playerInfo.PlayerData.premium)
                    {
                        _playerInfo.PlayerData.coin += _playerInfo.PlayerData.tournamentDoublePoint;
                        finishCoinIcon.SetActive(true);
                        reservedIcon.SetActive(false);
                    }
                    else
                    {
                        _playerInfo.PlayerData.reserved += _playerInfo.PlayerData.tournamentDoublePoint;
                        finishCoinIcon.SetActive(false);
                        reservedIcon.SetActive(true);
                    }
                }

                _controller.UpdateDataFromLocal();
            }
                break;

            case "normal":
            {
                finishUI.SetActive(false);
            }
                break;
        }
    }

    void SetTimeText()
    {
        if (tournamentTimeBox.activeSelf)
        {
            if (!canActive)
            {
                if (lessDay_startTime)
                {
                    if (differentTimingToStart.TotalHours > 1)
                    {
                        timeTextUI.text = differentTimingToStart.Hours.ToString("F0") + "h";
                        timeTextUI.fontSize = 10;
                    }
                    else if (differentTimingToStart.Minutes > 2)
                    {
                        timeTextUI.text = differentTimingToStart.Minutes.ToString("F0") + "m";
                        timeTextUI.fontSize = 10;
                    }
                    else
                    {
                        timeTextUI.text = differentTimingToStart.Seconds.ToString("F0") + "s";
                        timeTextUI.fontSize = 10;
                    }
                }
                else
                {
                    timeTextUI.text = differentTimingToStart.Days.ToString("F0") + "d";
                    timeTextUI.fontSize = 10;
                }
            }
        }
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "tournament":
            {
                if (toActive && canActive && !active && dataUpdated)
                {
                    _popupController.OpenPopUp(true, true, false,
                        startTournament, StartNo, StartYes,
                        null);
                }
                else if (active && dataUpdated)
                {
                    SetUI("normal");
                    tournament = true;
                }
            }
                break;

            case "back":
            {
                if (_playerInfo.PlayerData.tournamentMatchWins < gameNumber)
                {
                    _searchSystem.ButtonClick("back");
                }
                else
                {
                    StartCoroutine(GetTime("rewardTime"));
                    _playerInfo.PlayerData.tournament = false;
                    _controller.UpdateDataFromLocal();
                }
                
                tournament = false;
                finish = false;
                timeUpdated = false;
                StartCoroutine(GetTime("lastTime"));
            }
                break;
            
            case "start":
            {
                _searchSystem.Search("tournament", false);
            }
                break;
            
            case "claim":
            {
                StartCoroutine(GetTime("rewardTime"));
                _tournamentReward.Open();
                _playerInfo.PlayerData.tournament = false;
                _controller.UpdateDataFromLocal();
                tournament = false;
            }
                break;
        }
    }

    public void StartYes()
    {
        if (toActive && canActive && !active && dataUpdated)
        {
            _playerInfo.PlayerData.tournament = true;
            _playerInfo.PlayerData.tournamentId = id;
            _playerInfo.PlayerData.tournamentMatchWins = 0;
            _playerInfo.PlayerData.tournamentLose = false;
            _controller.UpdateDataFromLocal();
            active = true;
            _popupController.show = false;
            SetUI("normal");
            tournament = true;
        }
    }
    
    public void StartNo()
    {
        _popupController.show = false;
    }

    public IEnumerator GetTime(string type)
    {
        //string url = "https://worldtimeapi.org/api/timezone/Etc/UTC";
        string url = "http://202.133.89.237:3000/getTime";

        UnityWebRequest request = UnityWebRequest.Get(url);

        if (type == "rewardTime")
        {
            rewardTimeUpdating = true;
            print("<color=pink>Get Reward Time ...</color>");
        }
        else if (type == "lastTime")
        {
            print("<color=pink>Get Last Time ...</color>");
        }
        else if (type == "premiumTime")
        {
            _controller.premiumTimeUpdated = false;
            print("<color=pink>Get Premium Time ...</color>");
        }
        
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch time: " + request.error);
            ShowInternetProblemPopUp();
        }
        else
        {
            string json = request.downloadHandler.text;
            //print(json);
            //DateTime time = DateTime.ParseExact(json, "M/d/yyyy, h:mm:ss tt", CultureInfo.InvariantCulture);
            //print(time);
            //DateTime time = ParseTimeFromJson(json);
            DateTime time = ConvertTimeToDateTime(json);

            if (type == "lastTime")
            {
                lastTime = time;
                timeUpdated = true;
                Debug.Log("<color=pink>Current time from server: </color>" + time);
            }else if (type == "rewardTime")
            {
                _playerInfo.PlayerData.rewardTime = DateTimeToString(time.AddHours(48));
                _playerInfo.SaveGame();
                rewardTimeUpdating = false;
                rewardTimeUpdated = true;
                _tournamentReward.CheckRewardTime();
                Debug.Log("<color=pink>Reward time set: </color>" + _playerInfo.PlayerData.rewardTime);
            }
            else if (type == "premiumTime")
            {
                _controller.premiumTime = time;
                _controller.premiumTimeUpdated = true;
                Debug.Log("<color=pink>Premium time set: </color>" + time);
            }
        }
    }

    DateTime ParseTimeFromJson(string json)
    {
        int dateTimeIndex = json.IndexOf("\"datetime\":\"") + 12;
        string dateTimeString = json.Substring(dateTimeIndex, 19); // "2023-08-14T12:34:56"
        return DateTime.Parse(dateTimeString).ToUniversalTime();
    }
    
    public IEnumerator GetData()
    {
        yield return new WaitForSeconds(0.5f);
        
        WWWForm form = new WWWForm();
        form.AddField("playerId", _playerInfo.PlayerData.token);
        string url = "https://api.backgammononline24.com/tournaments?player_id=131231";
        UnityWebRequest req = UnityWebRequest.Get(url);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        Debug.Log("<color=yellow> Get tournamnet data... </color>");
        yield return req.Send();

        if (req.responseCode == 200)
        {
            Debug.Log("<color=yellow> Get tournamnet data: </color> Successful");
            string json = req.downloadHandler.text;
            //Debug.Log(json);
            var _Response = JsonConvert.DeserializeObject<List<TournamnetClass>>(json);
            toActive = _Response.Count > 0;

            bool debug = false;
            
            #region Debug
            debug = true;


            if (debug)
            {
                if (!_tournamentReward.isActive)
                    UpdateData(DateTimeToString(lastTime.AddSeconds(-50)), 3, "HAMID");
                else
                {
                    Debug.Log("<color=red>Your tournament reward is active, so you can not start other tournament</color>");
                    dataUpdated = true;
                }
            }
            #endregion
            
            // Set Icons
            coinIcon[0].SetActive(_Response[0].other_award == "" || _Response[0].other_award == "null");
            giftIcon[0].SetActive(!coinIcon[0].activeSelf);
                
            coinIcon[1].SetActive(coinIcon[0].activeSelf);
            giftIcon[1].SetActive(giftIcon[0].activeSelf);
                
            coinIcon[2].SetActive(coinIcon[0].activeSelf);
            giftIcon[2].SetActive(giftIcon[0].activeSelf);

            if (toActive && !debug)
            {
                if (!_tournamentReward.isActive)
                {
                    // Debug Information
                    for (int i = 0; i < _Response.Count; i++)
                        Debug.Log("<color=orange> Tournament(" + i + "): </color>" + "(<color=white>id: " +
                                  _Response[i].id + ")</color>" + "(<color=white>startTime: " +
                                  ConvertTimeToDateTime(_Response[i].start_time) + ")</color>");

                    int targetResponseId = -1;
                    int nextResponseId = -1;
                    int previousResponseId = -1;

                    if (_playerInfo.PlayerData.tournament)
                    {
                        bool founded = false;

                        for (int i = 0; i < _Response.Count; i++)
                        {
                            if (_Response[i].id == _playerInfo.PlayerData.tournamentId)
                            {
                                string _startTime = _Response[i].start_time;
                                int _gameNumber = _Response[i].wins_to_champion;
                                string id = _Response[i].id;

                                Debug.Log("<color=yellow> Selected tournament ID (Info): </color>" + i);
                                founded = true;
                                UpdateData(_startTime, _gameNumber, id);
                            }
                        }

                        if (!founded)
                        {
                            Debug.Log("<color=red> Your previous tournament is end </color>");
                            _playerInfo.PlayerData.tournament = false;
                            _playerInfo.PlayerData.tournamentId = "";
                            _controller.UpdateDataFromLocal();
                            StartCoroutine(GetData());
                        }
                    }
                    else
                    {
                        var futureResponses = _Response.Select((_Response, index) => new { _Response, index })
                            .Where(r => ConvertTimeToDateTime(r._Response.start_time) > lastTime)
                            .ToList();

                        var pastResponses = _Response.Select((_Response, index) => new { _Response, index })
                            .Where(r => ConvertTimeToDateTime(r._Response.start_time) < lastTime)
                            .ToList();

                        if (pastResponses.Any())
                        {
                            var closestResponse = pastResponses
                                .OrderBy(
                                    r => (lastTime - ConvertTimeToDateTime(r._Response.start_time)).Ticks)
                                .First();

                            previousResponseId = closestResponse.index;
                        }
                        else
                        {
                            Console.WriteLine(
                                "<color=red> No _Response has a (start_time) previous (lastTime) </color>");
                        }

                        if (futureResponses.Any())
                        {
                            var closestResponse = futureResponses
                                .OrderBy(
                                    r => (ConvertTimeToDateTime(r._Response.start_time) - lastTime).Ticks)
                                .First();

                            nextResponseId = closestResponse.index;
                        }
                        else
                        {
                            Console.WriteLine("<color=red> No _Response has a (start_time) after (lastTime) </color>");
                        }

                        if (previousResponseId != -1)
                        {
                            TimeSpan differente =
                                lastTime - ConvertTimeToDateTime(_Response[previousResponseId].start_time);
                            if (differente.TotalSeconds <= 60)
                                targetResponseId = previousResponseId;
                            else if (nextResponseId != -1)
                                targetResponseId = nextResponseId;
                            else
                                toActive = false;
                        }
                        else if (nextResponseId != -1)
                            targetResponseId = nextResponseId;
                        else
                            toActive = false;
                    }

                    if (toActive)
                    {
                        string _startTime = _Response[targetResponseId].start_time;
                        int _gameNumber = _Response[targetResponseId].wins_to_champion;
                        string id = _Response[targetResponseId].id;

                        Debug.Log("<color=yellow> Selected tournament ID (Time): </color>" + targetResponseId);
                        UpdateData(_startTime, _gameNumber, id);
                    }
                    else
                    {
                        Debug.Log("<color=red>There are currently no tournaments</color>");
                        dataUpdated = true;
                    }
                }
                else
                {
                    Debug.Log("<color=red>Your tournament reward is active, so you can not start other tournament</color>");
                    dataUpdated = true;
                }
            }
        }
        else
        {
            Debug.Log("<color=red> Get tournamnet data: Error: </color>" + req.responseCode);
            toActive = false;
            dataUpdated = true;
        }
    }

    public DateTime ConvertTimeToDateTime(string dateString)
    {
        return DateTime.Parse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
    
    public string ConvertTimeToString(DateTime dateTime)
    {
        return dateTime.ToString(null, System.Globalization.DateTimeFormatInfo.InvariantInfo);
    }
    
    public static DateTime ConvertTo24HourFormat(DateTime dateTime12Hour)
    {
        int hour24 = dateTime12Hour.Hour;
        int minute = dateTime12Hour.Minute;

        DateTime dateTime24Hour = new DateTime(dateTime12Hour.Year, dateTime12Hour.Month, dateTime12Hour.Day, hour24, minute, 0);

        return dateTime24Hour;
    }
    
    public static string ConvertTimeFormat(string inputTime)
    {
        DateTime dateTime = DateTime.ParseExact(inputTime, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        string formattedTime = dateTime.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        return formattedTime;
    }

    public DateTime StringToDateTime(string time)
    {
        string dateString = time;
        DateTime targetDateTime;
        bool isValidDate = DateTime.TryParse(dateString, out targetDateTime);

        if (isValidDate)
            return targetDateTime;
        else
            return new DateTime();
    }

    string DateTimeToString(DateTime time)
    {
        return time.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void UpdateData(string _startTime, int _gameNumber, string _id)
    {
        if (!toActive) { Debug.Log("<color=red> Tournament is off </color>"); dataUpdated = true; return; }
        
        // Set StartTime
        startTime = ConvertTimeToDateTime(_startTime);
        
        gameNumber = _gameNumber;
        
        // Check time
        if (startTime > lastTime)
        {
            differentTimingToStart = startTime - lastTime;

            if (differentTimingToStart <= TimeSpan.Zero)
                IsStart(_id);
            else
                ToStart();
        }
        else
            IsStart(_id);
        
        id = _id;
        startTournament = _loadExcel.itemDatabase_book[0].startTournament;
        
        tournamentTimeBox.SetActive(toStart);

        SetupItems();
        
        if (!_playerInfo.PlayerData.tournamentLose)
        {
            if (_playerInfo.PlayerData.tournamentMatchWins < gameNumber)
            {
                infoTextUI.text = "Win " + (gameNumber - _playerInfo.PlayerData.tournamentMatchWins).ToString() + " matches to earn the prize";
                finishInfoTextUI.text = (gameNumber - _playerInfo.PlayerData.tournamentMatchWins).ToString() + " more matches to win the tournament";
            }
            else
            {
                infoTextUI.text = "You win the tournament";
                finishInfoTextUI.text = "You win the tournament";
                _playerInfo.PlayerData.tournamentWin = true;
                _playerInfo.SaveGame();
            }
        }else
        {
            infoTextUI.text = "You lose the tournament";
            //finishInfoTextUI.text = "You lose the tournament";
            finishInfoTextUI.text = "";
        }
        
        dataUpdated = true;
    }

    void IsStart(string _id)
    {
        canActive = true;
        toStart = false;
        active = _playerInfo.PlayerData.tournament;
        
        if (active && _playerInfo.PlayerData.tournamentId != _id)
        {
            if (_playerInfo.PlayerData.tournamentWin && _playerInfo.PlayerData.completeLevel < 4)
            {
                active = false;
                toActive = false;
                canActive = false;
                Debug.Log("<color=red> Your previous tournament is win (The information is not completed)</color>");
                return;
            }
            else
            {
                Debug.Log("<color=red> Your previous tournament is end </color>");
                _playerInfo.PlayerData.tournament = false;
                _playerInfo.PlayerData.tournamentId = "";
                _controller.UpdateDataFromLocal();
                StartCoroutine(GetData());
                return;
            }
        }
        
        StopCoroutine(CountdownCoroutine_ToStart());
        
        if (!active)
        {
            ResetTournament();
            Debug.Log("<color=red> Tournament is start </color>");
        }
        else if (!finish)
        {
            if (_playerInfo.PlayerData.tournamentLose)
            {
                if (_playerInfo.PlayerData.tournamentId != _id)
                {
                    ResetTournament();
                }
                else
                {
                    active = false;
                    toActive = false;
                    canActive = false;
                }
                
                _popupController.OpenPopUp(false, false, true, "Your previous tournament is lose", null, null, LoseConfirmButton);
                Debug.Log("<color=red> Your previous tournament is lose </color>");
            }
            else
            {
                if (_playerInfo.PlayerData.tournamentMatchWins < gameNumber)
                {
                    Debug.Log("<color=red> You are in tournament </color>");
                }
                else
                {
                    active = false;
                    toActive = false;
                    canActive = false;

                    if (_playerInfo.PlayerData.completeLevel < 4)
                    {
                        Debug.Log("<color=red> Your tournament is win (The information is not completed) </color>");
                    }
                    else
                    {
                        ResetTournament();
                        Debug.Log("<color=red> Your tournament is win (The information is completed) </color>");
                    }
                }
            }
        }
    }

    public void LoseConfirmButton()
    {
        _popupController.show = false;
    }

    void ToStart()
    {
        canActive = false;
        active = false;
        toStart = true;
        StartCoroutine(CountdownCoroutine_ToStart());
        lessDay_startTime = Math.Abs(differentTimingToStart.TotalDays) < 1;
        Debug.Log("<color=red> Tournament to start </color>");
    }

    void ResetTournament()
    {
        _playerInfo.PlayerData.tournamentLose = false;
        _playerInfo.PlayerData.tournamentWin = false;
        _playerInfo.PlayerData.tournamentMatchWins = 0;
        _playerInfo.PlayerData.tournamentDoublePoint = 0;
    }

    void ResetItems()
    {
        if (match.Count > 0)
        {
            foreach (MatchClass _match in match)
            {
                Destroy(_match.image.gameObject);
            }
        }
        match.Clear();
        if (prize != null)
            Destroy(prize.gameObject);
    }

    void SetupItems()
    {
        ResetItems();
            
        for (int i = 0; i < gameNumber; i++)
        {
            Image matchImage = Instantiate(matchPrefab, transform.position, Quaternion.identity, matches).GetComponent<Image>();
            matchImage.transform.GetChild(0).GetComponent<Text>().text = "Match " + (i + 1).ToString();
            MatchClass matchClass = new MatchClass();
            matchClass.image = matchImage;
            matchClass.status = MatchClass.Status.none;
            match.Add(matchClass);
        }
        
        Image prizeImage = Instantiate(prizePrefab, transform.position, Quaternion.identity, matches).GetComponent<Image>();
        prize = prizeImage;

        UpdateItems();
    }

    void UpdateItems()
    {
        try
        {
            int last = 0;
            if (_playerInfo.PlayerData.tournamentMatchWins > 0)
            {
                for (int i = 0; i < _playerInfo.PlayerData.tournamentMatchWins; i++)
                {
                    match[i].status = MatchClass.Status.win;
                    last = i;
                }
            }
            else
                last = -1;

            if (_playerInfo.PlayerData.tournamentLose)
                match[last+1].status = MatchClass.Status.lose;

            foreach (MatchClass _match in match)
            {
                switch (_match.status)
                {
                    case MatchClass.Status.none:
                    {
                        _match.image.color = Color.white;
                    }
                        break;

                    case MatchClass.Status.win:
                    {
                        _match.image.color = winColor;
                        _match.image.transform.GetChild(1).gameObject.SetActive(true);
                    }
                        break;

                    case MatchClass.Status.lose:
                    {
                        _match.image.color = loseColor;
                        _match.image.transform.GetChild(2).gameObject.SetActive(true);
                    }
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    IEnumerator CountdownCoroutine_ToStart()
    {
        while (differentTimingToStart.TotalSeconds > 0)
        {
            differentTimingToStart = differentTimingToStart.Subtract(TimeSpan.FromSeconds(1));
            yield return new WaitForSeconds(1);
        }

        timeUpdated = false;
        StartCoroutine(GetTime("lastTime"));
    }

    void SetButtonUI(CanvasGroup buttonCanvas, bool active)
    {
        buttonCanvas.interactable = active;
        
        if (active)
            buttonCanvas.alpha = 1;
        else
            buttonCanvas.alpha = 0.5f;
    }

    void ShowInternetProblemPopUp()
    {
        _popupController.OpenPopUp(false, false, true,
            "There was a problem receiving the information, please check your internet connection", null, null,
            ConfirmButtonClick);
    }
    
    public void ConfirmButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
