using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PimDeWitte.UnityMainThreadDispatcher;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SearchSystem : MonoBehaviour
{
    [Header("General")]
    public bool playOnline;
    public int betCount;
    private int betCount_old;
    private bool updated;
    public bool searching;
    public bool opponentFound;
    public bool canceling;
    private float time;
    public bool AI;
    
    [Header("Canvas")]
    public CanvasGroup playOnlineUI;
    public CanvasGroup betItemsUI;

    [Header("Access")]
    public Text infoTextUI;
    public GameObject backButton;
    public List<BetItem> BetItems;
    public AudioSource opponentFindAudio;
    
    private PlayerInfo _playerInfo;
    private OnlineGameServer _onlineGameServer;
    private LevelManager _levelManager;
    private PlayWithFriends _playWithFriends;
    private Controller _controller;

    public List<BetClass> betClassC;
    public List<BetClass> betClassB;
    public List<BetClass> betClassA;

    [System.Serializable]
    public class BetClass
    {
        public int count;
        public bool forClass;
        public bool forPremium;
    }

    public class GameJoinedClass
    {
        public string roomName;
        public string player1;
        public string player2;
        public string player1Color;
        public string player2Color;
        public playerDataClass player1Data;
        public playerDataClass player2Data;
        
        public class playerDataClass
        {
            public string name;
            public int coin;
            public string region;
            public int avatarId;
        }
    }
    
    void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _levelManager = FindObjectOfType<LevelManager>();
        _playWithFriends = FindObjectOfType<PlayWithFriends>();
        _controller = FindObjectOfType<Controller>();

        UpdateBetItems();
    }
    
    void Update()
    {
        if (_onlineGameServer == null) { return; }
        
        if (!updated && _playerInfo.Updated)
        {
            SetupBetItems(_playerInfo.PlayerData.playerClass);
            updated = true;
        }

        if (!updated) { return; }
        
        if (time > 0 && searching && AI && playOnline)
            time -= Time.deltaTime;

        if (time <= 0 && !opponentFound && searching && playOnline)
        {
            _onlineGameServer.gameInformation.online = false;
            SceneManager.LoadScene("Game");
        }

        if (playOnline && _onlineGameServer.socket.Connected && betCount_old != betCount)
        {
            StopCoroutine(SearchDelay());
            StartCoroutine(SearchDelay());
            betCount_old = betCount;
        }

        if (searching || opponentFound)
            betItemsUI.alpha = 0.5f;
        else
            betItemsUI.alpha = 1f;

        betItemsUI.interactable = !searching;
        backButton.SetActive(!opponentFound);
        
        if (_onlineGameServer.connected || opponentFound)
        {
            if (canceling)
                infoTextUI.text = "<color=red>Canceling...</color>";
            else
            if (_playerInfo.PlayerData.coin < betCount)
                infoTextUI.text = "<color=red>You dont have enough coin</color>";
            else
            if (opponentFound)
                infoTextUI.text = "<color=yellow>Opponent found, starting the game...</color>";
            else
            if (searching)
                infoTextUI.text = "Waiting for opponent...";
            else
                infoTextUI.text = "Select the amount of coins";
        }
        else
            infoTextUI.text = "Connecting...";

        SetCanvas(playOnlineUI, playOnline, 0, 1);
    }
    
    void SetCanvas(CanvasGroup canvas, bool active, float min, float max)
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

    public void SocketOn()
    {
        _onlineGameServer.socket.On("GAME_JOINED", response =>
        {
            try
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    //Debug.Log(response);
                    List<GameJoinedClass> _Response = JsonConvert.DeserializeObject<List<GameJoinedClass>>(response.ToString());
                    
                    opponentFindAudio.Play();
                    
                    string opponentToken = "";
                    string color = "";
                    GameJoinedClass.playerDataClass data = null;
                    if (_Response[0].player1 == _playerInfo.PlayerData.token)
                    {
                        opponentToken = _Response[0].player2;
                        color = _Response[0].player1Color;
                        data = _Response[0].player2Data;
                    }
                    else
                    if (_Response[0].player2 == _playerInfo.PlayerData.token)
                    {
                        opponentToken = _Response[0].player1;
                        color = _Response[0].player2Color;
                        data = _Response[0].player1Data;
                    }
                    _onlineGameServer.gameInformation.online = true;
                    _onlineGameServer.gameInformation.roomName = _Response[0].roomName;
                    _onlineGameServer.gameInformation.color = color;
                    _onlineGameServer.gameInformation.opponentName = data.name;
                    _onlineGameServer.gameInformation.opponentRegion = data.region;
                    _onlineGameServer.gameInformation.opponentCoin = data.coin;
                    _onlineGameServer.gameInformation.opponentAvatarID = data.avatarId;
                    _onlineGameServer.gameInformation.opponentToken = opponentToken;
                    Debug.Log("<color=yellow> (Match Found) </color> Game Information: (roomName: " + _Response[0].roomName + "), (color: " + color + ")");
                    opponentFound = true;
                    searching = false;
                    StartCoroutine(StartGame(300));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
        
        _onlineGameServer.socket.On("CancelMatchmaking", response =>
        {
            if (!opponentFound)
            {
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.messageClass>>(response.ToString());

                if (result[0].message == "Matchmaking canceled successfully.")
                {
                    playOnline = false;
                    searching = false;
                    canceling = false;
                }
            }
        });
    }

    IEnumerator StartGame(float delay)
    {
        Debug.Log("<color=yellow> Starting the game... </color>");
        yield return new WaitForSeconds(delay * Time.deltaTime);
        Debug.Log("<color=yellow> Game Started </color>");

        if (_levelManager != null)
            _levelManager.LoadScene(0);
        else
            SceneManager.LoadScene("Game");
    }

    IEnumerator SearchDelay()
    {
        yield return new WaitForSeconds(400 * Time.deltaTime);
        if (_playerInfo.PlayerData.coin >= betCount)
        {
            Search("casual", true);
        }
    }

    public void Search(string type, bool _ai)
    {
        SocketOn();
        
        time = 30;
        AI = _ai;
        Debug.Log("<color=yellow> Searching for opponent... </color>");
        opponentFound = false;

        var playerData = new
        {
            name = _playerInfo.PlayerData.nickname,
            coin = _playerInfo.PlayerData.coin,
            region = _playerInfo.PlayerData.region,
            avatarId = _playerInfo.PlayerData.avatarId
        };

        int refferalCode = 0;

        if (_playWithFriends != null)
            refferalCode = _playWithFriends.friendRefferalCode;

        var obj = new
        {
            playerClass = _playerInfo.PlayerData.playerClass,
            number = betCount,
            mode = type,
            playerToken = _playerInfo.PlayerData.token,
            tournamentID = _playerInfo.PlayerData.tournamentId,
            refferalCode = refferalCode,
            rematchKey = _onlineGameServer.gameInformation.rematchKey,
            playerData
        };

        if (_playWithFriends != null && type == _playWithFriends.searchType)
        {
            _onlineGameServer.gameInformation.playWithFriend = true;
            _onlineGameServer.gameInformation.coin = _playWithFriends.coin;
        }
        else
        {
            _onlineGameServer.gameInformation.playWithFriend = false;
            _onlineGameServer.gameInformation.coin = betCount;
        }
    
        print(_playerInfo.PlayerData.token);
        
        _onlineGameServer.socket.EmitAsync("matchmaking", obj);
        
        Debug.Log("<color=yellow> (Emit) </color> matchmaking");
        searching = true;
    }

    public void UpdateBetItems()
    {
        foreach (BetItem item in BetItems)
        {
            item.choose = item.count == betCount;
        }
    }

    public void ButtonClick(string type)
    {
        if (_controller != null)
            _controller.clickAudio.Play();
        
        switch (type)
        {
            case "back":
            {
                if (!opponentFound && searching)
                {
                    canceling = true;
                    _onlineGameServer.socket.EmitAsync("cancelMatchmaking");
                    Debug.Log("<color=yellow> (Emit) </color> cancelMatchmaking");
                    time = 10;
                }
                else
                if (!searching)
                    playOnline = false;
            }
                break;
        }
    }

    public void SetupBetItems(string _class)
    {
        foreach (BetItem item in BetItems)
            item.gameObject.SetActive(false);

        switch (_class)
        {
            case "C":
            {
                for (int i = 0; i < betClassC.Count; i++)
                {
                    BetItems[i].gameObject.SetActive(true);
                    BetItems[i].count = betClassC[i].count;
                    BetItems[i].forClass = betClassC[i].forClass;
                    
                    if (!_playerInfo.PlayerData.premium)
                        BetItems[i].forPremium = betClassC[i].forPremium;
                    else
                        BetItems[i].forPremium = false;
                }
            }
                break;
            
            case "B":
            {
                for (int i = 0; i < betClassB.Count; i++)
                {
                    BetItems[i].gameObject.SetActive(true);
                    BetItems[i].count = betClassB[i].count;
                    BetItems[i].forClass = betClassB[i].forClass;
                    
                    if (!_playerInfo.PlayerData.premium)
                        BetItems[i].forPremium = betClassB[i].forPremium;
                    else
                        BetItems[i].forPremium = false;
                }
            }
                break;
            
            case "A":
            {
                for (int i = 0; i < betClassA.Count; i++)
                {
                    BetItems[i].gameObject.SetActive(true);
                    BetItems[i].count = betClassA[i].count;
                    BetItems[i].forClass = betClassA[i].forClass;
                    
                    if (!_playerInfo.PlayerData.premium)
                        BetItems[i].forPremium = betClassA[i].forPremium;
                    else
                        BetItems[i].forPremium = false;
                }
            }
                break;
        }

        bool resetBetCount = true;
        foreach (BetItem item in BetItems)
        {
            if (betCount == item.count)
            {
                resetBetCount = false;
                break;
            }
        }
        
        if (resetBetCount)
            betCount = BetItems[0].count;

        UpdateBetItems();
    }
}
