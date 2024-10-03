using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FinishController : MonoBehaviour
{
    [Header("General")]
    public bool Finished;
    public bool LastFinish;
    public bool showFinish;
    public int opponentAvatarID;
    float timeToExit;
    public string mainMenuScene;
    public bool Updated;
    public float timeToExitDefault;
    bool delayFinish;
    public bool playerWin; // Local
    public bool lastFinish_Opponent; // Local
    private bool rematch;
    private bool toFixShowFinish;

    [Header("Access")]
    public CanvasGroup FinishUI;
    public Image BlackScreenUI;
    public CanvasGroup mainCanvasUI;
    public ExitButton ExitButton;
    public Text titleTextUI;
    public Text infoTextUI;
    public GameObject[] winTemplate;
    public GameObject[] loseTemplate;
    public GameObject reservedBox;
    public Text coinTextUI;
    public Text reservedTextUI;
    public Image playerAvatarBorder;
    public Image opponentAvatarBorder;
    public Image playerAvatar;
    public Image opponentAvatar;
    public Image playerRegionIcon;
    public Image opponentRegionIcon;
    public GameObject newGameButton;
    public GameObject rematchButton;
    public Text rematchTextUI;
    public GameObject[] window;
    public GameObject opponentRegion;
    public ParticleSystem finishVfx;

    public UnityEngine.UI.Text playerNickname;
    public UnityEngine.UI.Text opponentNickname;
    public UnityEngine.UI.Text LastPointCount;
    public UnityEngine.UI.Text doublePointCount;

    public Camera mainCamera;
    GameSetup GameSetup;
    PlayerInfo playerInfo;
    DoubleController DoubleController;
    private OnlineGameServer _onlineGameServer;
    private SearchSystem _searchSystem;
    private Authentication _authentication;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        playerInfo = FindObjectOfType<PlayerInfo>();
        DoubleController = FindObjectOfType<DoubleController>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _authentication = FindObjectOfType<Authentication>();
        
        timeToExit = timeToExitDefault;
    }

    void Update()
    {
        if (!Updated && playerInfo.Updated && GameSetup.Updated)
        {
            playerNickname.text = playerInfo.PlayerData.nickname;
            opponentNickname.text = GameSetup.opponentNickname;
            opponentAvatarID = GameSetup.opponentAvatarID;

            playerAvatar.sprite = playerInfo.avatars[playerInfo.PlayerData.avatarId];
            
            GameSetup.UpdateRegionIcon(playerInfo.PlayerData.region, playerRegionIcon);

            if (GameSetup.GeneralOptions.Online)
            {
                opponentAvatar.sprite = playerInfo.avatars[_onlineGameServer.gameInformation.opponentAvatarID];
                GameSetup.UpdateRegionIcon(_onlineGameServer.gameInformation.opponentRegion, opponentRegionIcon);
            }

            Updated = true;
        }

        if (Updated)
        {
            if (delayFinish && !showFinish)
            {
                BlackScreenUI.enabled = true;

                Color color = BlackScreenUI.color;
                if (color.a < 0.8f)
                    color.a += Time.deltaTime;
                BlackScreenUI.color = color;
                
                if (mainCamera.orthographicSize > 6.2)
                    mainCamera.orthographicSize -= 0.5f * Time.deltaTime;
                else
                    showFinish = true;
            }

            if (showFinish)
            {
                Color color = BlackScreenUI.color;
                if (color.a > 0)
                    color.a -= 2 * Time.deltaTime;
                BlackScreenUI.color = color;
            }
            
            LastFinish = GameSetup.finishColumn[1].FullCount == 15;
            lastFinish_Opponent = GameSetup.finishColumn[0].FullCount == 15;
  
            if (GameSetup.GeneralOptions.AI) CheckFinishLocal();
            SetAlpha();
            
            newGameButton.SetActive(!rematch);
            rematchButton.SetActive(!rematch);
            SetRematchText();

            if (Finished && !toFixShowFinish)
            {
                StartCoroutine(ToShowFinish());
                toFixShowFinish = true;
            }
        }
    }

    IEnumerator ToShowFinish()
    {
        yield return new WaitForSeconds(10);
        if (!showFinish)
        {
            showFinish = true;
            Debug.Log("<color=red>Show Finish Fixed</color>");
        }
    }

    void SetRematchText()
    {
        if (rematch)
        {
            if (_searchSystem.searching)
                rematchTextUI.text = "Waiting for opponent to rematch...";
            else
            if (_searchSystem.opponentFound)
                rematchTextUI.text = "Rematch accepted, starting the game...";
            else
            if (!_onlineGameServer.connected)
                rematchTextUI.text = "connecting...";
            else
                rematchTextUI.text = "";
        }else
            rematchTextUI.text = "";
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            case "new game":
            {
                if (GameSetup.GeneralOptions.Online)
                    _searchSystem.playOnline = true;
                else
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
                break;
            
            case "main menu":
            {
                playerInfo.PlayerData.menuStatus = "after game";
                playerInfo.SaveGame();
                _searchSystem.ButtonClick("back");
                SceneManager.LoadScene("menu");
            }
                break;

            case "premium":
            {
                playerInfo.PlayerData.menuStatus = "to premium";
                playerInfo.SaveGame();
                SceneManager.LoadScene("menu");
            }
                break;

            case "rematch":
            {
                rematch = true;
                _searchSystem.Search("rematch", false);
            }
                break;
        }
    }

    public void FinishOnline(string winColor, string type, int coinPoint, int _doublePoint)
    {
        if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
        {
            Finished = true;
            
            winTemplate[0].SetActive(false);
            winTemplate[1].SetActive(false);
            loseTemplate[0].SetActive(false);
            loseTemplate[1].SetActive(false);
            
            reservedBox.SetActive(!playerInfo.PlayerData.premium);

            if (winColor == _onlineGameServer.gameInformation.color)
            {
                finishVfx.Play();
                titleTextUI.text = "You won";
                
                playerAvatarBorder.color = Color.green;
                opponentAvatarBorder.color = Color.red;
                
                opponentRegion.SetActive(true);

                bool haveInfo = false;

                switch (type)
                {
                    case "denied the double":
                        infoTextUI.text = "Opponent resigned the game.";
                        haveInfo = true;
                        break;
                    
                    case "resigned":
                        infoTextUI.text = "Opponent resigned the game.";
                        haveInfo = true;
                        break;
                    
                    default:
                        infoTextUI.text = "";
                        break;
                }
                
                if (GameSetup.timeOut)
                {
                    infoTextUI.text = "The opponent has timed out.";
                    haveInfo = true;
                }
                
                window[0].SetActive(haveInfo);
                window[1].SetActive(!haveInfo);
                
                winTemplate[0].SetActive(!playerInfo.PlayerData.premium);
                winTemplate[1].SetActive(playerInfo.PlayerData.premium);

                LastPointCount.text = "<color=green>+";
                LastPointCount.text += coinPoint.ToString() + "</color>";
                
                doublePointCount.text = "<color=green>+";
                doublePointCount.text += _doublePoint.ToString() + "</color>";
                
                if (!playerInfo.PlayerData.premium)
                {
                    coinTextUI.text = (playerInfo.PlayerData.coin + coinPoint).ToString();
                    reservedTextUI.text = "Reserved: " + (playerInfo.PlayerData.reserved + _doublePoint).ToString();
                }
                else
                    coinTextUI.text = (playerInfo.PlayerData.coin + coinPoint + _doublePoint).ToString();
                
                if (GameSetup.finishColumn[1].FullCount != 15) delayFinish = true;

                CheckTournamentFinish(_doublePoint, true);
                Debug.Log("<color=green> Finish </color> <color=white> Win </color>");
            }
            else if (!delayFinish)
            {
                titleTextUI.text = "You lost";
                
                playerAvatarBorder.color = Color.red;
                opponentAvatarBorder.color = Color.green;

                bool haveInfo = false;
                
                switch (type)
                {
                    case "denied the double":
                    {
                        infoTextUI.text = "You resigned the game.";
                        haveInfo = true;
                    }
                        break;
                    
                    case "resigned":
                        infoTextUI.text = "You resigned the game.";
                        haveInfo = true;
                        break;
                    
                    default:
                        infoTextUI.text = "";
                        break;
                }
                
                if (GameSetup.timeOut)
                {
                    infoTextUI.text = "timed out.";
                    haveInfo = true;
                }
                
                window[0].SetActive(haveInfo);
                window[1].SetActive(!haveInfo);
                
                loseTemplate[0].SetActive(!playerInfo.PlayerData.premium && playerInfo.PlayerData.reserved > 0);
                loseTemplate[1].SetActive(playerInfo.PlayerData.premium || playerInfo.PlayerData.reserved == 0);

                LastPointCount.text = "<color=red>-";
                LastPointCount.text += coinPoint.ToString() + "</color>";
                
                doublePointCount.text = "<color=red>-";
                doublePointCount.text += _doublePoint.ToString() + "</color>";
                
                if (!playerInfo.PlayerData.premium)
                {
                    if (playerInfo.PlayerData.reserved >= _doublePoint)
                    {
                        if (playerInfo.PlayerData.coin >= coinPoint)
                            coinTextUI.text = (playerInfo.PlayerData.coin - coinPoint).ToString();
                        else
                            coinTextUI.text = "0";
                        
                        reservedTextUI.text = "Reserved: " + (playerInfo.PlayerData.reserved - _doublePoint).ToString();
                    }
                    else
                    {
                        if (playerInfo.PlayerData.coin >= (coinPoint + _doublePoint))
                            coinTextUI.text = (playerInfo.PlayerData.coin - coinPoint - _doublePoint).ToString();
                        else
                            coinTextUI.text = "0";
                        
                        reservedTextUI.text = "Reserved: 0";
                    }
                }
                else
                    coinTextUI.text = (playerInfo.PlayerData.coin - coinPoint - _doublePoint).ToString();

                CheckTournamentFinish(_doublePoint, false);
                Debug.Log("<color=green> Finish </color> <color=white> Lose </color>");
            }
            
            delayFinish = true;
        }
    }

    void CheckTournamentFinish(int _doublePoint, bool win)
    {
        if (playerInfo.PlayerData.tournament)
        {
            playerInfo.PlayerData.menuStatus = "tournament";
            playerInfo.PlayerData.tournamentDoublePoint = _doublePoint;

            if (win)
                playerInfo.PlayerData.tournamentMatchWins += 1;
            else
                playerInfo.PlayerData.tournamentLose = true;
            
            playerInfo.SaveGame();
            Debug.Log("<color=red> Tournament finish called </color>" + "status: " + win);
            SceneManager.LoadScene("menu");
        }
    }

    void CheckFinishLocal()
    {
        if (Finished && !LastFinish && !delayFinish)
        {
            winTemplate[0].SetActive(false);
            winTemplate[1].SetActive(false);
            loseTemplate[0].SetActive(false);
            loseTemplate[1].SetActive(false);
            
            if (playerWin)
            {
                finishVfx.Play();
                titleTextUI.text = "You won";
                LastPointCount.text = "<color=green>+0</color>";
                doublePointCount.text = "<color=green>+";
                doublePointCount.text += "0" + "</color>";
                
                playerAvatarBorder.color = Color.green;
                opponentAvatarBorder.color = Color.red;
                
                winTemplate[0].SetActive(false);
                winTemplate[1].SetActive(true);
                
                window[0].SetActive(false);
                window[1].SetActive(true);
            }
            else
            {
                titleTextUI.text = "You lost";
                LastPointCount.text = "<color=red>-0</color>";
                doublePointCount.text = "<color=red>-";
                doublePointCount.text += "0" + "</color>";
                
                playerAvatarBorder.color = Color.red;
                opponentAvatarBorder.color = Color.green;
                
                loseTemplate[0].SetActive(false);
                loseTemplate[1].SetActive(true);
                
                window[0].SetActive(false);
                window[1].SetActive(true);
            }
            
            infoTextUI.text = "";
            coinTextUI.text = playerInfo.PlayerData.coin.ToString();
            reservedTextUI.text = "Reserved: 0";
            opponentRegion.SetActive(false);
            
            delayFinish = true;

            if (playerWin)
                Debug.Log("Finish: Win");
            else
                Debug.Log("Finish: Lose");
        }

        if (LastFinish || lastFinish_Opponent)
        {
            Finished = true;
            
            winTemplate[0].SetActive(false);
            winTemplate[1].SetActive(false);
            loseTemplate[0].SetActive(false);
            loseTemplate[1].SetActive(false);

            if (!delayFinish)
            {
                if ((GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down &&
                     GameSetup.finishColumn[1].FullCount == 15) ||
                    (GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Up &&
                     GameSetup.finishColumn[0].FullCount == 15))
                {
                    finishVfx.Play();
                    titleTextUI.text = "You won";
                    infoTextUI.text = "";
                    LastPointCount.text = "<color=green>+0</color>";
                    doublePointCount.text = "<color=green>+";
                    doublePointCount.text += "0" + "</color>";
                    winTemplate[0].SetActive(false);
                    winTemplate[1].SetActive(true);
                    window[0].SetActive(false);
                    window[1].SetActive(true);
                    playerAvatarBorder.color = Color.green;
                    opponentAvatarBorder.color = Color.red;
                    //playerInfo.PlayerData.winCount += 1;
                    //UpdateDataFromLocal();
                    Debug.Log("Finish: Win");
                }
                else
                {

                    titleTextUI.text = "You lost";
                    infoTextUI.text = "";
                    LastPointCount.text = "<color=red>-0</color>";
                    doublePointCount.text = "<color=red>-";
                    doublePointCount.text += "0" + "</color>";
                    delayFinish = true;
                    loseTemplate[0].SetActive(false);
                    loseTemplate[1].SetActive(true);
                    window[0].SetActive(false);
                    window[1].SetActive(true);
                    playerAvatarBorder.color = Color.red;
                    opponentAvatarBorder.color = Color.green;
                    //playerInfo.PlayerData.loseCount += 1;
                    //UpdateDataFromLocal();
                    Debug.Log("Finish: Lose");
                }
                
                infoTextUI.text = "";
                coinTextUI.text = playerInfo.PlayerData.coin.ToString();
                reservedTextUI.text = "Reserved: 0";
                opponentRegion.SetActive(false);
                
                delayFinish = true;
            }
        }
    }

    void SetAlpha()
    {
        float speed = 5 * Time.deltaTime;

        if (showFinish)
        {
            FinishUI.gameObject.SetActive(true);

            if (FinishUI.alpha < 1)
                FinishUI.alpha += speed;

            if (FinishUI.alpha >= 1)
                FinishUI.alpha = 1;

        }else
        {
            if (FinishUI.alpha > 0)
                FinishUI.alpha -= speed;

            if (FinishUI.alpha <= 0)
            {
                FinishUI.alpha = 0;
                FinishUI.gameObject.SetActive(false);
            }
        }
        
        if (Finished)
        {
            if (mainCanvasUI.alpha > 0)
                mainCanvasUI.alpha -= 10 * Time.deltaTime;

            if (mainCanvasUI.alpha <= 0)
                mainCanvasUI.alpha = 0;
        }else
        {
            mainCanvasUI.alpha = 1;
        }
    }
}
