 using System;
 using System.Collections.Generic;
 using System.Net.Http;
 using System.Threading.Tasks;
 using Newtonsoft.Json.Linq;
 using Unity.Services.Authentication;
 using Unity.Services.Core;
 using Unity.Services.RemoteConfig;
 using UnityEngine;
 using Unity.RemoteConfig;
 using Unity.Services.RemoteConfig;
 using Unity.Services.Core;
 using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;
    private LoadExcel _loadExcel;
    private LoadingUpdate _loadingUpdate;
    private Controller _controller;
    private SignController _signController;
    private PlayWithFriends _playWithFriends;
    private OnlineGameServer _onlineGameServer;
    private PopupController _popupController;
    
    public string BuildCode;
    public string ipRegion;

    [System.Serializable]
    public class Data
    {
        //General
        public string token;
        public bool premium;
        public string premiumTime;
        public string nickname;
        public int avatarId;
        public string region;
        public string state;
        public string email;
        public string phone;
        public string address;
        public string refrralCode;
        public int coin;
        public int reserved;
        public bool sound;
        public bool music;
        public int[] birthdate = new[] {-1, -1, -1};
        public string playerClass;
        public string playerClass_old;
        public bool isGuest;
        public int friendsInvited;
        public int winCount;
        public int loseCount;
        //Other
        public GameSetup.ArtClass.ArtType boardType;
        public GameSetup.sides checkerSide;
        public int turnTime;
        public int doubleTime;
        public string doubleRequest;
        public string doubleResponse;
        public string resignText;
        public string menuStatus;
        public int completeLevel;
        //Tournament
        public bool tournament;
        public string tournamentId;
        public int tournamentMatchWins;
        public bool tournamentLose;
        public bool tournamentWin;
        public int tournamentDoublePoint;
        public string rewardTime;
    }

    public Data PlayerData;
    public bool Updated;
    public List<Sprite> avatars = new List<Sprite>();

    private void Start()
    {
        _loadExcel = FindObjectOfType<LoadExcel>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _controller = FindObjectOfType<Controller>();
        _signController = FindObjectOfType<SignController>();
        _playWithFriends = FindObjectOfType<PlayWithFriends>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _popupController = FindObjectOfType<PopupController>();
        
        socketManager = SocketManager.Instance;
        Application.targetFrameRate = 60;
    }

    AudioSource[] Audios;

    private void Update()
    {
        if (!Updated)
        {
            string json = PlayerPrefs.GetString("PlayerData");

            if (PlayerPrefs.HasKey("PlayerData"))
                PlayerData = JsonUtility.FromJson<Data>(json);

            #region Default
            if (!PlayerPrefs.HasKey("PlayerData"))
            {
                Debug.Log("PlayerData: Default");
                PlayerData.token = "";
                PlayerData.nickname = "";
                PlayerData.avatarId = 0;
                PlayerData.region = "";
                PlayerData.state = "";
                PlayerData.refrralCode = "";
                PlayerData.coin = 0;
                PlayerData.reserved = 0;
                PlayerData.sound = true;
                PlayerData.music = true;
                PlayerData.email = "";
                PlayerData.phone = "";
                PlayerData.birthdate = new[] {-1, -1, -1};
                PlayerData.playerClass = "";
                PlayerData.boardType = GameSetup.ArtClass.ArtType.type1;
                PlayerData.checkerSide = GameSetup.sides.White;
                PlayerData.turnTime = 25;
                PlayerData.doubleTime = 15;
                PlayerData.doubleRequest = "Your apponent has requested the double.";
                PlayerData.doubleResponse = "Waiting for your apponent to accept the double.";
                PlayerData.resignText = "Are you sure about the resign?";
                PlayerData.premium = false;
                PlayerData.menuStatus = "";
                PlayerData.tournament = false;
                PlayerData.tournamentId = "";
                PlayerData.tournamentMatchWins = 0;
                PlayerData.tournamentLose = false;
                PlayerData.tournamentDoublePoint = 0;
                PlayerData.isGuest = true;
                PlayerData.friendsInvited = 0;
                PlayerData.completeLevel = 0;
                PlayerData.address = "";
                PlayerData.tournamentWin = false;
                PlayerData.winCount = 0;
                PlayerData.loseCount = 0;
                PlayerData.playerClass_old = "";
                PlayerData.premiumTime = "";
            }
            #endregion
            
            #region Debug
            //PlayerData.boardType = GameSetup.ArtClass.ArtType.type3;
            //PlayerData.checkerSide = GameSetup.sides.White;
            //PlayerData.coin = 6000;
            #endregion
            
            SetAudio();
            SaveGame();

            Updated = true;
        }
    }

    #if UNITY_EDITOR
    class SetPlayerPrefs
    {
        [UnityEditor.MenuItem("Tools/Reset PlayerPrefs")]
        private static void Reset()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=red> PlayerPrefs Deleted </color>");
        }
    }
    #endif

    public void SaveGame()
    {
        Debug.Log("<color=green> PlayerData: Save </color>");
        string json = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        Debug.Log("<color=red> Application: Quit </color>");
    }

    public void SetAudio()
    {
        Audios = Resources.FindObjectsOfTypeAll<AudioSource>();

        foreach (AudioSource audio in Audios)
        {
            if (audio.gameObject.layer == LayerMask.NameToLayer("Sound"))
                audio.mute = !PlayerData.sound;
            else
            if (audio.gameObject.layer == LayerMask.NameToLayer("Music"))
                audio.mute = !PlayerData.music;
        }
    }
    
    public void SetDefault()
    {
        PlayerData.nickname = "Guest";
        PlayerData.avatarId = 0;
        PlayerData.state = "";
        PlayerData.birthdate = new []{-1, -1, -1};
        PlayerData.coin = 100;
        PlayerData.reserved = 0;
        PlayerData.region = ipRegion;
        PlayerData.boardType = GameSetup.ArtClass.ArtType.type1;
        PlayerData.checkerSide = GameSetup.sides.White;
        PlayerData.turnTime = 25;
        PlayerData.doubleTime = 25;
        PlayerData.doubleRequest = "Your apponent has requested the double.";
        PlayerData.doubleResponse = "Waiting for your apponent to accept the double.";
        PlayerData.resignText = "Are you sure about the resign?";
        PlayerData.premium = false;
        PlayerData.menuStatus = "";
        PlayerData.tournament = false;
        PlayerData.tournamentId = "";
        PlayerData.tournamentMatchWins = 0;
        PlayerData.tournamentLose = false;
        PlayerData.tournamentDoublePoint = 0;
        PlayerData.isGuest = true;
        PlayerData.friendsInvited = 0;
        PlayerData.completeLevel = 0;
        PlayerData.phone = "";
        PlayerData.address = "";
        PlayerData.tournamentWin = false;
        PlayerData.winCount = 0;
        PlayerData.loseCount = 0;
        PlayerData.playerClass = "";
        PlayerData.playerClass_old = "";
        PlayerData.premiumTime = "";
    }

    public void UpdateClass()
    {
        int classC = int.Parse(_loadExcel.itemDatabase_book[0].classC);
        int classB = int.Parse(_loadExcel.itemDatabase_book[0].classB);
        int classA = int.Parse(_loadExcel.itemDatabase_book[0].classA);
        
        if (PlayerData.coin <= classC)
            PlayerData.playerClass = "C";
        else if (PlayerData.coin <= classB)
            PlayerData.playerClass = "B";
        else if (PlayerData.coin > classA)
            PlayerData.playerClass = "A";

        if (_controller != null)
        {
            _controller.classC = classC;
            _controller.classB = classB;
            _controller.classA = classA;
        }

        if (PlayerData.playerClass_old != "" && PlayerData.playerClass_old != PlayerData.playerClass && PlayerData.playerClass != "C")
        {
            _popupController.OpenPopUp(false, false, true, "Congratulations!\nYou are now in " + PlayerData.playerClass + " class.", null, null, ConfirmButtonClick);
        }

        PlayerData.playerClass_old = PlayerData.playerClass;
    }

    public void ConfirmButtonClick()
    {
        _popupController.show = false;
    }

    #region Cloud Config
    struct userAttributes { }
    struct appAttributes { }
    
    public async void GetRemoteKeys()
    {
        try
        {
            /*await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();*/
            var res = await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            GetCountry(RemoteConfigService.Instance.appConfig.GetString("IPAPI_ACCESS_KEY"));
            _loadingUpdate.StartDownload(RemoteConfigService.Instance.appConfig.GetString("GITHUB_ACCESS_KEY"));
            
            /*print(RemoteConfigService.Instance.appConfig.GetString("PLAYER_CLASS"));
            print(RemoteConfigService.Instance.appConfig.GetString("REWARD_ADS_RETRY_TIME"));
            print(RemoteConfigService.Instance.appConfig.GetString("REWARD_ADS_MAX_VALUE"));*/

            if (_signController != null && _controller != null && _signController.newAccount)
            {
                PlayerData.coin = int.Parse(RemoteConfigService.Instance.appConfig.GetString("INITIAL_SETUP_COINS"));
                Debug.Log("<color=red> Default coin set: </color>" + PlayerData.coin);
                _controller.UpdateDataFromLocal();
            }

            if (PlayerData.coin < 0 && _controller != null)
            {
                PlayerData.coin = 0;
                Debug.Log("<color=red> coin set: </color> ZERO");
                _controller.UpdateDataFromLocal();
            }

            if (_playWithFriends != null)
                _playWithFriends.coin = int.Parse(RemoteConfigService.Instance.appConfig.GetString("PLAY_WITH_FRIENDS_COINS"));

            if (_onlineGameServer != null)
                _onlineGameServer.gameInformation.startDoubleCount = int.Parse(RemoteConfigService.Instance.appConfig.GetString("INITIAL_DOUBLE_COINS"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Failed to fetch configs, is your project linked and configuration deployed?");
        }
    }
    #endregion

    #region Get Player Region
    private static readonly HttpClient client = new HttpClient();
    private string apiUrl;

    public async void GetCountry(string apiKey)
    {
        apiUrl = "https://ipapi.co/json/?key=" + apiKey;
        string countryName = await GetCountryNameAsync();
        if (!string.IsNullOrEmpty(countryName))
        {
            Debug.Log($"<color=yellow> Player is located in: </color> {countryName}");
            ipRegion = countryName;

            if (PlayerData.region == "")
            {
                PlayerData.region = ipRegion;
                _controller.UpdateDataFromLocal();
                _controller.UpdateUI();
            }
        }
    }

    public async Task<string> GetCountryCodeAsync()
    {
        try
        {
            string responseString = await client.GetStringAsync(apiUrl);
            JObject responseJson = JObject.Parse(responseString);
            string countryCode = responseJson["country"].ToString();
            return countryCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get country code: {e.Message}");
            return null;
        }
    }

    public async Task<string> GetCountryNameAsync()
    {
        try
        {
            string responseString = await client.GetStringAsync(apiUrl);
            JObject responseJson = JObject.Parse(responseString);
            string countryName = responseJson["country_name"].ToString();
            return countryName;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get country name: {e.Message}");
            return null;
        }
    }
    #endregion

    public void SetTimeFromData()
    {
        switch (PlayerData.playerClass)
        {
            case "C":
                PlayerData.turnTime = int.Parse(_loadExcel.itemDatabase_book[0].turnTimeC);
                break;
                
            case "B":
                PlayerData.turnTime = int.Parse(_loadExcel.itemDatabase_book[0].turnTimeB);
                break;
                
            case "A":
                PlayerData.turnTime = int.Parse(_loadExcel.itemDatabase_book[0].turnTimeA);
                break;
        }

        PlayerData.doubleTime = int.Parse(_loadExcel.itemDatabase_book[0].doubleTime);
        
        SaveGame();
    }

    public void SetInGameTextFromData()
    {
        PlayerData.doubleRequest = _loadExcel.itemDatabase_book[0].doubleRequest;
        PlayerData.doubleResponse = _loadExcel.itemDatabase_book[0].doubleResponse;
        PlayerData.resignText = _loadExcel.itemDatabase_book[0].resignText;
        SaveGame();
    }
}
