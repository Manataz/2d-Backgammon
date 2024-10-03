using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUpdate : MonoBehaviour
{
    // Options
    private string apiUrl = "https://api.github.com/repos/l3ardia/backgammon-assets/contents/Localization/";
    private string personalAccessToken;
    
    private string localFilePath_book;
    private string localFilePath_regions;
    
    private string fileName_book = "book.csv";
    private string fileName_regions = "regions.csv";

    [Header("General")]
    public bool loaded;
    public bool loading;
    public CanvasGroup loadingUI;
    public Text versionText;
    public Image loadingBar;
    public float loadPercent;
    private bool updated;

    private Controller _controller;
    private LoadExcel _loadExcel;
    private PopupController _popupController;
    private PlayerInfo _playerInfo;
    private TournamentController _tournamentController;
    private SignController _signController;
    private SearchSystem _searchSystem;
    private OnlineGameServer _onlineGameServer;
    private RewardAds _rewardAds;
    private Authentication _authentication;

    public GameDataClass gameData;
    
    [Serializable]
    public class GameDataClass
    {
        public string contact_us;
        public string terms_of_use;
        public string privacy_policy;
        public string android_app_name;
        public string ios_app_id;
    }

    void Start()
    {
        _controller = FindObjectOfType<Controller>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _popupController = FindObjectOfType<PopupController>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _tournamentController = FindObjectOfType<TournamentController>();
        _signController = FindObjectOfType<SignController>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _rewardAds = FindObjectOfType<RewardAds>();
        _authentication = FindObjectOfType<Authentication>();

        loaded = false;
        loading = true;
        loadingBar.fillAmount = 0;
        versionText.text = "Version: " + GetProjectVersion();
        
        localFilePath_book = Path.Combine(Application.persistentDataPath, "Resources", fileName_book);
        localFilePath_regions = Path.Combine(Application.persistentDataPath, "Resources", fileName_regions);
        
        string directoryPath_book = Path.GetDirectoryName(localFilePath_book);
        if (!Directory.Exists(directoryPath_book))
        {
            Directory.CreateDirectory(directoryPath_book);
        }
        
        string directoryPath_regions = Path.GetDirectoryName(localFilePath_regions);
        if (!Directory.Exists(directoryPath_regions))
        {
            Directory.CreateDirectory(directoryPath_regions);
        }
        
        loadPercent = 60;
        StartCoroutine(GetGameData());
        StartCoroutine(GetStateData());
    }

    private void Update()
    {
        if (_playerInfo.Updated && !updated)
        {
            switch (_playerInfo.PlayerData.menuStatus)
            {
                case "to premium":
                {
                    loadingBar.fillAmount = 1;
                    _signController.LoggedIn();
                    _controller.premium = true;
                }
                    break;
                
                case "after game":
                {
                    loadingBar.fillAmount = 1;
                    _signController.LoggedIn();
                    
                    if (!_playerInfo.PlayerData.premium)
                        _rewardAds.ClickRewardAdsButton();
                }
                    break;

                case "tournament":
                {
                    if (_playerInfo.PlayerData.tournament)
                    {
                        loadingBar.fillAmount = 1;
                        _signController.LoggedIn();
                        _tournamentController.SetUI("finish");
                        _tournamentController.tournament = true;
                    }
                }
                    break;
            }
            
            _playerInfo.PlayerData.menuStatus = "";
            _playerInfo.SaveGame();

            updated = true;
        }
        
        _controller.SetCanvas(loadingUI, loading, 0, 1);

        if (loadingBar.fillAmount < loadPercent/100)
            loadingBar.fillAmount += Time.deltaTime * (loadPercent/100) * 0.3f;

        if (loadingBar.fillAmount >= 0.995f && !loaded)
        {
            loading = false;
            loaded = true;
        }
    }

    public void StartDownload(string token)
    {
        personalAccessToken = token;
        StartCoroutine(DownloadCSV_book(apiUrl + fileName_book, localFilePath_book));
        StartCoroutine(DownloadCSV_regions(apiUrl + fileName_regions, localFilePath_regions));
    }

    public IEnumerator GetGameData()
    {
        yield return new WaitForSeconds(0.5f);

        WWWForm form = new WWWForm();
        string url = "https://api.backgammononline24.com/init";
        UnityWebRequest req = UnityWebRequest.Get(url);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        Debug.Log("<color=yellow> Get Game data... </color>");
        yield return req.Send();

        if (req.responseCode == 200)
        {
            Debug.Log("<color=yellow> Get Game data: </color> Successful");
            string json = req.downloadHandler.text;
            //Debug.Log(json);
            var _Response = JsonConvert.DeserializeObject<GameDataClass>(json);
            gameData.contact_us = _Response.contact_us;
            gameData.terms_of_use = _Response.terms_of_use;
            gameData.privacy_policy = _Response.privacy_policy;
            gameData.android_app_name = _Response.android_app_name;
            gameData.ios_app_id = _Response.ios_app_id;
            loadPercent = 100;
        }
        else
        {
            Debug.Log("<color=red> Get Game data: Error: </color>" + req.responseCode);
        }
    }
    
    public IEnumerator GetStateData()
    {
        yield return new WaitForSeconds(0.5f);

        WWWForm form = new WWWForm();
        string url = "https://api.backgammononline24.com/country/states?country=Australia";
        UnityWebRequest req = UnityWebRequest.Get(url);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        Debug.Log("<color=yellow> Get State data... </color>");
        yield return req.Send();

        if (req.responseCode == 200)
        {
            Debug.Log("<color=yellow> Get State data: </color> Successful");
            string json = req.downloadHandler.text;
            //Debug.Log(json);

            string trimmedInput = json.Trim('[', ']');
            
            string[] states = trimmedInput.Split(new string[] { "\",\"" }, StringSplitOptions.None)
                .Select(s => s.Trim('\"')) // حذف کردن " از ابتدا و انتهای هر عنصر
                .ToArray();

            Controller.State state = new Controller.State();
            state.region = "Australia";
            state.name = states;
            _controller.states.Add(state);
        }
        else
        {
            Debug.Log("<color=red> Get State data: Error: </color>" + req.responseCode);
        }
    }

    private IEnumerator DownloadCSV_book(string url, string localFilePath)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "token " + personalAccessToken);
            webRequest.SetRequestHeader("User-Agent", "Unity");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                _popupController.OpenPopUp(false, false, true,
                    "There was a problem receiving the information, please check your internet connection", null, null,
                    ConfirmButtonClick);
            }
            else
            {
                string json = webRequest.downloadHandler.text;
                var jsonData = JsonUtility.FromJson<GitHubFile>(json);
                byte[] data = System.Convert.FromBase64String(jsonData.content);
                File.WriteAllBytes(localFilePath, data);
                Debug.Log("<color=yellow> (book) CSV file downloaded and saved at: </color>" + localFilePath);

                _loadExcel.itemDatabase_book.Clear();
                _loadExcel.LoadItemData("book");
            }
        }
    }
    
    private IEnumerator DownloadCSV_regions(string url, string localFilePath)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "token " + personalAccessToken);
            webRequest.SetRequestHeader("User-Agent", "Unity");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                _popupController.OpenPopUp(false, false, true,
                    "There was a problem receiving the information, please check your internet connection", null, null,
                    ConfirmButtonClick);
            }
            else
            {
                string json = webRequest.downloadHandler.text;
                var jsonData = JsonUtility.FromJson<GitHubFile>(json);
                byte[] data = System.Convert.FromBase64String(jsonData.content);
                File.WriteAllBytes(localFilePath, data);
                Debug.Log("<color=yellow> (regions) CSV file downloaded and saved at: </color>" + localFilePath);

                _loadExcel.itemDatabase_regions.Clear();
                _loadExcel.LoadItemData("regions");
            }
        }
    }

    public void ConfirmButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [System.Serializable]
    public class GitHubFile
    {
        public string content;
    }
    
    public static string GetProjectVersion()
    {
        string gameVersion = Application.version;
        if (!string.IsNullOrEmpty(gameVersion))
        {
            return gameVersion.Trim();
        }
        return "Unknown Version";
    }
}
