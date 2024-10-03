using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoubleController : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    [Header("General")]
    public int Multiple;
    public GameSetup.sides Side;
    public bool canDouble;
    public GameObject playerDoubleUI;
    public GameObject opponentDoubleUI;
    public Text doubleTextUI;
    public GameObject DoubleButton;
    private bool updated;

    [Header("Request")]
    public bool myRequest;
    public bool Requesting;
    public bool RequestedMe;
    public CanvasGroup DoubleRequestUI;
    public float RequestFullTime;
    public float RequestTime;
    public Image FillImage;
    public Text infoRequestTextUI;

    [Header("Response")]
    public CanvasGroup DoubleResponseUI;
    public GameObject WaitBarUI;
    public GameObject AcceptUI;
    public GameObject ResignUI;
    public Text infoResponseTextUI;
    public Image BarUI;
    float FillBar;
    bool resigned;

    GameSetup GameSetup;
    Text DoubleTextUI;
    RollButton RollButton;
    Animator anim;
    PlayerInfo playerInfo;
    PauseController pauseController;
    FinishController FinishCon;
    private OnlineGameServer _onlineGameServer;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        RollButton = FindObjectOfType<RollButton>();
        anim = GetComponent<Animator>();
        playerInfo = FindObjectOfType<PlayerInfo>();
        pauseController = FindObjectOfType<PauseController>();
        FinishCon = FindObjectOfType<FinishController>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();

        socketManager = SocketManager.Instance;
        Side = GameSetup.sides.none;
    }

    void Update()
    {
        if (!updated && GameSetup.Updated)
        {
            Multiple = 5;
            //Multiple = _onlineGameServer.gameInformation.startDoubleCount;
            updated = true;
        }
        
        if (GameSetup.Updated)
        {
            canDouble = Multiple < 64 && RollButton.canRoll && (Side == GameSetup.sides.none || Side == GameSetup.GeneralOptions.playerSide) && !Requesting;
            DoubleButton.SetActive(canDouble);
            playerDoubleUI.SetActive(Side == GameSetup.GeneralOptions.playerSide || Side == GameSetup.sides.none);
            opponentDoubleUI.SetActive(Side == GameSetup.GeneralOptions.OpponentSide || Side == GameSetup.sides.none);
            doubleTextUI.text = Multiple.ToString();
        }
        
        float speed = 10 * Time.deltaTime;
        if (RequestedMe)
        {
            DoubleRequestUI.gameObject.SetActive(true);

            if (DoubleRequestUI.alpha < 1)
                DoubleRequestUI.alpha += speed;

            if (DoubleRequestUI.alpha >= 1)
                DoubleRequestUI.alpha = 1;

            RequestTime -= Time.deltaTime;
            FillImage.fillAmount = GameSetup.LineRange(RequestFullTime, RequestTime);

            if (RequestTime <= 0 && !resigned)
            {
                StartCoroutine(EndRequest(false, Multiple,GameSetup.GeneralOptions.OpponentSide));
                resigned = true;
            }
        }
        else
        {
            if (DoubleRequestUI.alpha > 0)
                DoubleRequestUI.alpha -= speed;

            if (DoubleRequestUI.alpha <= 0)
            {
                DoubleRequestUI.alpha = 0;
                DoubleRequestUI.gameObject.SetActive(false);
            }

            RequestTime = RequestFullTime;
        }

        setAlphaMyRequest();
    }

    void setAlphaMyRequest()
    {
        float speed = 10 * Time.deltaTime;

        if (Requesting && myRequest)
        {
            BarUI.fillAmount = GameSetup.LineRange(RequestFullTime, FillBar);
            FillBar -= Time.deltaTime;

            DoubleResponseUI.gameObject.SetActive(true);

            if (DoubleResponseUI.alpha < 1)
                DoubleResponseUI.alpha += speed;

            if (DoubleResponseUI.alpha >= 1)
                DoubleResponseUI.alpha = 1;

            // Time Out (Double Response)
            if (FillBar <= 0)
            {
                FillBar = 0;

                if (!resigned)
                {
                    // Local
                    if (GameSetup.GeneralOptions.AI)
                    {
                        resigned = true;
                    }
                    // Online
                    if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
                    {
                        /*var commandData = new
                        {
                            name = "DENY_DOUBLE",
                            playerColor = _onlineGameServer.gameInformation.color
                        };
                        _onlineGameServer.socket.EmitAsync("command", commandData);
                        Debug.Log("<color=yellow> DENY_DOUBLE </color> <color=white> Processing... </color>");
                        resigned = true;*/
                    }
                }
            }

        }else
        {
            if (DoubleResponseUI.alpha > 0)
                DoubleResponseUI.alpha -= speed;

            if (DoubleResponseUI.alpha <= 0)
            {
                DoubleResponseUI.alpha = 0;
                DoubleResponseUI.gameObject.SetActive(false);
            }
        }
    }

    public void DoubleButtonClick()
    {
        if (canDouble && ((GameSetup.GeneralOptions.Online && !pauseController.Pause && _onlineGameServer.socket.Connected) || GameSetup.GeneralOptions.AI))
        {
            RequestDouble(GameSetup.GeneralOptions.OpponentSide);
            canDouble = false;
        }
    }

    public void RequestDouble(GameSetup.sides side)
    {
        Requesting = true;
        GameSetup.Audio.DoubleRequest.Play();

        if (side == GameSetup.GeneralOptions.playerSide)
        {
            myRequest = false;
            RequestedMe = true;
        }
        else
        {
            myRequest = true;
            FillBar = RequestFullTime;
            resigned = false;

            // Local
            if (GameSetup.GeneralOptions.AI)
            {
                bool response = false;
                if (GameSetup.finishCount.playerCount < GameSetup.finishCount.opponentCount)
                    response = true;
                else
                {
                    int homeCountPlayer = 0;
                    int homeCountOpponent = 0;
                    foreach (CheckerController checker in GameSetup.Checkers)
                    {
                        if (checker.ID >= 15 && checker.columnStart.ID < 6)
                            homeCountPlayer += 1;

                        if (checker.ID < 15 && checker.columnStart.ID >= 18)
                            homeCountOpponent += 1;
                    }
                    
                    if ((homeCountOpponent >= homeCountPlayer) || (homeCountOpponent+homeCountPlayer < 30))
                        response = true;
                }

                StartCoroutine(EndRequest(response, Multiple, GameSetup.GeneralOptions.OpponentSide));
            }
            else
            // Online
            if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
            {
                var commandData = new
                {
                    name = "REQUEST_DOUBLE",
                    playerColor = _onlineGameServer.gameInformation.color
                };
                
                _onlineGameServer.socket.EmitAsync("command", commandData);

                WaitBarUI.SetActive(true);
                AcceptUI.SetActive(false);
                ResignUI.SetActive(false);
                infoResponseTextUI.gameObject.SetActive(true);

                Debug.Log("<color=yellow> REQUEST_DOUBLE </color> <color=white>  Processing... </color>");
            }
        }
    }
    public void Response(bool accept)
    {
        GameSetup.Audio.Click.Play();

        if (accept)
        {
            // Local
            if (GameSetup.GeneralOptions.AI)
            {
                StartCoroutine(EndRequest(true, Multiple,GameSetup.GeneralOptions.OpponentSide));
            }
            else
            // Online
            if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
            {
                var commandData = new
                {
                    name = "ACCEPT_DOUBLE",
                    playerColor = _onlineGameServer.gameInformation.color
                };
                _onlineGameServer.socket.EmitAsync("command", commandData);
                Debug.Log("<color=yellow> AcceptDouble </color> <color=white>  Processing... </color>");
            }
        }
        else
        {
            StartCoroutine(EndRequest(false, Multiple,GameSetup.GeneralOptions.playerSide));
        }
    }

    public IEnumerator EndRequest(bool accept, int LastDouble, GameSetup.sides side)
    {
        WaitBarUI.SetActive(false);
        infoResponseTextUI.gameObject.SetActive(false);
        AcceptUI.SetActive(accept);
        ResignUI.SetActive(!accept);
        
        if (RequestedMe)
            yield return new WaitForSeconds(Time.deltaTime);
        else
            yield return new WaitForSeconds(100 * Time.deltaTime);

        if (accept)
        {
            // Local
            if (GameSetup.GeneralOptions.AI)
                Multiple *= 2;
            else
            // Online
            if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
            {
                //Multiple = LastDouble;
                Multiple *= 2;
            }

            if (RequestedMe)
            {
                Side = GameSetup.GeneralOptions.playerSide;

                // Local
                if (GameSetup.GeneralOptions.AI)
                {
                    float delay2 = 1;
                    if (GameSetup.StartingRollDice) delay2 = 30;
                    GameSetup.StartingRollDice = false;

                    StartCoroutine(GameSetup.GeneralOptions.DiceAnimConOpponent.RollDice(delay2,false));
                }
            }
            else
            {
                Side = GameSetup.GeneralOptions.OpponentSide;
            }
        }else
        {
            // Online
            if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
            {
                var commandData = new
                {
                    name = "DENY_DOUBLE",
                    playerColor = _onlineGameServer.gameInformation.color
                };
                _onlineGameServer.socket.EmitAsync("command", commandData);
                Debug.Log("<color=yellow> DENY_DOUBLE </color> <color=white> Processing... </color>");
            }
            else
            // Local
            {
                FinishCon.playerWin = (side == GameSetup.GeneralOptions.OpponentSide);
                FinishCon.Finished = true;
            }
        }

        myRequest = false;
        RequestedMe = false;
        Requesting = false;
    }
}
