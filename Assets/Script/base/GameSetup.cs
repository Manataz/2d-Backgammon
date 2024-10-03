using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameSetup : MonoBehaviour
{
    public SocketManager socketManager = SocketManager.Instance;

    #region General
    // ----------------------------------------------------------- General
    [Header("General")]
    public bool Updated;
    public bool DeveloperMode;
    public bool showLog;
    public bool socketConnected;
    public bool AllReady;
    public bool canDone;
    public bool doneProcessing;
    public bool doneProcessing_new;
    private bool doneProcessing_set;
    public bool timeOut;
    
    public Camera mainCamera;
    public Canvas canvas;
    public GameObject Checker;
    public BoxCollider boardBoxCollider;
    public PlayerInfo playerInfo;
    PauseController pauseController;
    public CheckerController touchNearChecker;
    FinishController finishController;
    public FinishCount finishCount;
    private OnlineGameServer _onlineGameServer;
    private ExitButton _exitButton;
    private SearchSystem _searchSystem;
    #endregion

    #region MyRegion
    [System.Serializable]
    public class Region
    {
        public string name;
        public Sprite flag;
    }
    public List<Region> regions = new List<Region>();
    #endregion

    #region UI
    // ----------------------------------------------------------- UI
    [System.Serializable]
    public class UIClass
    {
        [Header("General")]
        public CanvasGroup Blocked;
        public Text BlockedText;
        public List<Sprite> diceImages = new List<Sprite>();
        public List<Sprite> Checker2D = new List<Sprite>();
        public CanvasGroup InternetProblemUI;
        public GameObject WaitUI;
        public UnityEngine.UI.Text WaitTextUI;
        public Text coinCount;
        public Transform WaitLoadingUI;
        public Text gameTimeTextUI;
        public Sprite[] checkerUI;

        [Header("Player")]
        public UnityEngine.UI.Text playerNickname;
        public UnityEngine.UI.Image playerTurnTiming;
        public UnityEngine.UI.Image playerFullTiming;
        public List<int> playerRollNumbers = new List<int>();
        public UnityEngine.UI.Image playerCheckerUI;
        public Image playerAvatarImage;
        public Text playerTurnTimeText;
        public Text playerFullTimeText;
        public GameObject playerWarn;
        public Text playerCoinTextUI;
        public Image playerRegionIcon;

        [Header("Opponent")]
        public UnityEngine.UI.Text opponentNickname;
        public UnityEngine.UI.Image opponentTurnTiming;
        public UnityEngine.UI.Image opponentFullTiming;
        public List<int> opponentRollNumbers = new List<int>();
        public UnityEngine.UI.Image opponentCheckerUI;
        public Image opponentAvatarImage;
        public Text opponentTurnTimeText;
        public Text opponentFullTimeText;
        public GameObject opponentWarn;
        public Text opponentCoinTextUI;
        public Image opponentRegionIcon;
        public GameObject opponentRegionObject;
    }
    public UIClass UI;
    #endregion

    #region Audio
    [System.Serializable]
    public class AudioClass
    {
        public AudioSource myTurn;
        public AudioSource opponentTurn;
        public AudioSource Click;
        public AudioSource Blocked;
        public AudioSource DoubleRequest;
    }
    public AudioClass Audio;
    #endregion

    #region Art
    [Serializable]
    public class ArtClass
    {
        public SpriteRenderer flatRight;
        public SpriteRenderer flatLeft;
        public SpriteRenderer[] place;

        public Sprite[] flatRightSprite;
        public Sprite[] flatLeftSprite;
        public List<SpriteClass> placeHighlightSprite;
        public List<SpriteClass> placeSprite;
        
        [Serializable]
        public class SpriteClass
        {
            public Sprite[] sprite;
        }

        public enum ArtType
        {
            type1,
            type2,
            type3
        }
    }
    public ArtClass art;
    #endregion

    #region VFX
    [System.Serializable]
    public class VfxClass
    {
        public GameObject Kick;
    }
    public VfxClass Vfx;
    #endregion

    #region Double
    [Header("Double")]
    DoubleController DoubleController;
    #endregion
    
    #region CheckersPoint
    [Header("Checkers Point")]
    public int playerCheckersPoint;
    public int opponentCheckersPoint;
    #endregion

    #region Finish Action
    public bool FinishAction;
    #endregion

    #region Opponent Information
    [Header("Opponent Information")]
    public string opponentNickname;
    public int opponentAvatarID;
    #endregion

    #region Kick
    bool kickblock;
    CheckerController kickedChecker;
    #endregion

    #region Type Options
    // ----------------------------------------------------------- Type Options

    [System.Serializable]
    public class OptionsClass
    {
        public Transform startSetup;
        public ColumnOptionsClass ColumnOption;
        public PlaceOptionsClass PlaceOption;
        public outColumnOptionsClass outColumnOption;
        public finishColumnOptionClass finishColumnOption;
    }
    public OptionsClass Options;

    #endregion

    #region General Options
    // ----------------------------------------------------------- General Options

    [System.Serializable]
    public class GeneralOptionsClass
    {
        public type Type;
        [Range(0, 10)]
        public float fixSpeed;
        public Location PlayerLocation;
        public sides playerSide;
        public Color HighLightColor;
        public GameObject BoardMiddleColl;

        [Header("Timing")]
        public float fullTime;
        public float turnTime;
        public float notReadyTime;
        public float EmitTryTime;

        [Header("Auto-fill with the 'Playerlocation'")]
        public Location OpponentLocation;
        public sides OpponentSide;

        [Header("Opponent Type")]
        public bool AI;
        public bool Online;

        [Header("Delay")]
        [Range(1, 500)]
        public float delayTurn;
        [Range(1, 500)]
        public float delayAIMove;
        [Range(1, 500)]
        public float delayUpdateNaxtMove;

        [Header("Column")]
        public GameObject HighLightPrefab;
        public GameObject HighLightFinishPrefab;
        public Vector3 rangeSizeColumn;
        public bool showEditorColumn;

        [Header("Place")]
        public bool showEditorPlace;

        [Header("FinishColumn")]
        public Vector3 rangeSizeFinishColumn;
        public Vector3 rangeDistance;
        public SpriteRenderer[] finishHighlight;

        [Header("RollDice")]
        public DiceAnimController DiceAnimConPlayer;
        public DiceAnimController DiceAnimConOpponent;

        [Range(-3, 3)]
        public float textDistanceX;
        [Range(0, 3)]
        public float textDistanceY;
        [Range(0, 3)]
        public float textDistanceZ;

        [Range(0, 40)]
        public float textSize;

        [Header("Dice Material")]
        public Material Material1;
        public Material Material2;
        public Material Material3;

        public void SetOpponent()
        {
            Location location = Location.Up;
            if (PlayerLocation == Location.Up) location = Location.Down;
            OpponentLocation = location;

            sides side = sides.White;
            if (playerSide == sides.White) side = sides.Black;
            OpponentSide = side;
        }
    }

    public GeneralOptionsClass GeneralOptions;

    public enum Location
    {
        Down,
        Up
    }

    public enum type
    {
        type1,
        type2,
        type3
    }

    public enum direction
    {
        Up,
        Down
    }

    public enum sides
    {
        none,
        White,
        Black
    }

    #endregion

    #region Timing
    // ----------------------------------------------------------- Timing

    [System.Serializable]
    public class TimeClass
    {
        public float gameTimeSec;
        public float gameTimeMin;
        public bool timePassing;

        [Header("Player")]
        public float playerFullTime;
        public float playerTurnTime;
        public float playerNotReadyTime;

        [Header("Opponent")]
        public float opponentFullTime;
        public float opponentTurnTime;
        public float opponentNotReadyTime;
    }
    public TimeClass Timing;

    #endregion

    #region Column options
    // ----------------------------------------------------------- Column Options

    [System.Serializable]
    public class ColumnOptionsClass
    {
        [Header("Setup at start")]
        [Range(0, 2)]
        public float distanceX;
        [Range(0, 20)]
        public float distanceZ;
        [Range(0, 5)]
        public float middleDistance;
    }

    //public ColumnOptionsClass ColumnOption;

    #endregion

    #region Place options
    // ----------------------------------------------------------- Place Options

    [System.Serializable]
    public class PlaceOptionsClass
    {
        [Header("Setup at start")]
        [Range(0, 2)]
        public float distanceZ;
        [Range(0, 2)]
        public float distanceY;

    }

    public PlaceOptionsClass PlaceOptions;
    #endregion

    #region OutColumn Options

    // ----------------------------------------------------------- OutColumn Options

    [System.Serializable]
    public class outColumnOptionsClass
    {
        [Range(0, 10)]
        public float distanceX;
        [Range(0, 5)]
        public float distanceY;
        [Range(0, 20)]
        public float distanceZ;
        [Range(0, 20)]
        public float distanceBetween;
    }

    //public outColumnOptionsClass outColumnOption;

    #endregion

    #region FinishColumn Options
    // ----------------------------------------------------------- finishColumn Options

    [System.Serializable]
    public class finishColumnOptionClass
    {
        [Range(0, 10)]
        public float distanceX;
        [Range(-5, 5)]
        public float distanceY;
        [Range(0, 20)]
        public float distanceZ;
        [Range(0, 20)]
        public float distanceBetween;
        [Range(0, 1)]
        public float RowDistance;
    }

    //public finishColumnOptionClass finishColumnOption;
    #endregion

    #region Column Class
    // ----------------------------------------------------------- Column Class

    [System.Serializable]
    public class ColumnClass
    {
        public int ID;
        public sides Side;
        public int FullCount;
        public Vector3 Pos;
        public int maxCount;
        public direction Direction;
        public bool HighLight;
        public GameObject HighLightObject;
        public List<PlaceClass> Place = new List<PlaceClass>();
        public int BlockedRoll;
        public float placeDistanceY;
        public bool isFinish;
        public SpriteRenderer highlightSprite;
        public bool highlightUp;

        public void Update()
        {
            FullCount = 0;
            foreach (var place in Place)
            {
                if (place.Full)
                    FullCount++;
            }
        }
    }

    public List<ColumnClass> Column = new List<ColumnClass>();
    public List<ColumnClass> outColumn = new List<ColumnClass>();
    public List<ColumnClass> finishColumn = new List<ColumnClass>();

    #endregion

    #region place Class
    // ----------------------------------------------------------- Place Class

    [System.Serializable]
    public class PlaceClass
    {
        public bool Full;
        public Vector3 Pos;
        public CheckerController Checker;
    }
    #endregion

    #region Roll
    // ----------------------------------------------------------- Roll

    [System.Serializable]
    public class RollClass
    {
        //player
        public bool playerTurn;
        public bool playerAction;
        public bool playerHome;
        public bool playerDone;
        public bool playerProcessing;
        public bool AutoRoll;
        //opponent
        public bool opTurn;
        public bool opponentAction;
        public bool opponentHome;
        public bool opponentProcessing;
        //other
        public RollButton rollButton;
        public DoneButton doneButton;
        public List<int> Rolls = new List<int>();
        public int playerStartRoll;
        public int opponentStartRoll;
        public int moveNumber;
        public int moveVerifyNumber;

        //public List<int> Blocked = new List<int>();
    }

    [Header("Roll")]
    public RollClass Roll;
    #endregion

    #region HighLight
    // ----------------------------------------------------------- HighLight

    [Header("HighLight Options")]
    public Material highLightMaterial;
    bool change;
    Color myColor;
    public bool disableCheckersHighLight;

    public CheckerController[] Checkers;

    #endregion

    #region History
    // ----------------------------------------------------------- History
    [Header("History")]
    public UndoButton UndoButton;

    [System.Serializable]
    public class HistoryClass
    {
        public CheckerController checker;
        public ColumnClass startColumn;
        public ColumnClass targetColumn;
        public List<int> roll = new List<int>();
        public bool haveKick;
        public CheckerController checkerKick;
        public ColumnClass startColumnKick;
        public ColumnClass targetColumnKick;
        public bool verify;
    }

    public List<HistoryClass> History = new List<HistoryClass>();
    #endregion

    #region Starting
    // ----------------------------------------------------------- Starting
    [Header("Starting")]
    public bool Starting;
    public bool StartingRollDice;

    [System.Serializable]
    public class SpawnLocationClass
    {
        public Transform Right;
        public Transform Left;
    }
    public SpawnLocationClass SpawnLocation;
    #endregion

    #region Void Start
    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- void Start

    void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        UI.BlockedText = UI.Blocked.transform.GetChild(1).GetComponent<Text>();
        pauseController = FindObjectOfType<PauseController>();
        finishController = FindObjectOfType<FinishController>();
        finishCount = FindObjectOfType<FinishCount>();
        DoubleController = FindObjectOfType<DoubleController>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        _exitButton = FindObjectOfType<ExitButton>();
        _searchSystem = FindObjectOfType<SearchSystem>();

        socketManager = SocketManager.Instance;
        canDone = false;
        
        UI.Blocked.alpha = 0;
        UI.InternetProblemUI.alpha = 0;
        UI.WaitUI.gameObject.SetActive(false);
        
        if (UI.playerFullTimeText != null)
            UI.playerFullTimeText.text = "0:" + Timing.playerFullTime.ToString("F0");

        UI.gameTimeTextUI.text = "0:0";
        Timing.gameTimeSec = 0;
        Timing.gameTimeMin = 0;
    }

    void SetScreen()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    #endregion

    #region Void Update
    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- void Update
    void Update()
    {
        if (!Updated && playerInfo.Updated)
        {
            SetScreen();
            myColor = GeneralOptions.HighLightColor;
            GeneralOptions.SetOpponent();
            SetupColumns();
            SetupOutColumn();
            SetupFinishColumn();
            SetupPlaces();
            Starting = false;
            SetArtType();

            DoubleController.infoRequestTextUI.text = playerInfo.PlayerData.doubleRequest;
            DoubleController.infoResponseTextUI.text = playerInfo.PlayerData.doubleResponse;
            
            UpdateRegionIcon(playerInfo.PlayerData.region, UI.playerRegionIcon);

            switch (GeneralOptions.playerSide)
            {
                case sides.White:
                {
                    UI.playerCheckerUI.sprite = UI.checkerUI[0];
                    UI.opponentCheckerUI.sprite = UI.checkerUI[1];
                }
                    break;
                
                case sides.Black:
                {
                    UI.playerCheckerUI.sprite = UI.checkerUI[1];
                    UI.opponentCheckerUI.sprite = UI.checkerUI[0];
                }
                    break;
            }
            
            // Set Coins
            UI.playerCoinTextUI.text = playerInfo.PlayerData.coin.ToString();

            // Set Online
            if (_onlineGameServer != null && _onlineGameServer.gameInformation.online)
            {
                GeneralOptions.AI = false;
                GeneralOptions.Online = true;

                opponentNickname = _onlineGameServer.gameInformation.opponentName;
                opponentAvatarID = _onlineGameServer.gameInformation.opponentAvatarID;
                UI.opponentCoinTextUI.text = _onlineGameServer.gameInformation.opponentCoin.ToString();
                
                UpdateRegionIcon(_onlineGameServer.gameInformation.opponentRegion, UI.opponentRegionIcon);
                UI.opponentRegionObject.SetActive(true);

                Debug.Log("<color=blue> ( Online ) </color>");

                SocketOn();
                pauseController.PauseOn();
            }
            else
            // Set Local
            if (GeneralOptions.AI)
            {
                opponentAvatarID = UnityEngine.Random.Range(0, playerInfo.avatars.Count);
                string[] names = { "Alice", "Bob", "Charlie", "David", "Eva", "Frank", "Grace", "Henry", "Isabel", "Jack" };
                int randomIndex = UnityEngine.Random.Range(0, names.Length);
                opponentNickname = names[randomIndex] + " <color=grey>AI</color>";
                UI.opponentCoinTextUI.text = UnityEngine.Random.Range(10,1000).ToString();
                UI.opponentRegionObject.SetActive(false);

                AllReady = true;
                Debug.Log("<color=blue> ( AI ) </color>");
            }

            GeneralOptions.turnTime = playerInfo.PlayerData.turnTime;
            GeneralOptions.fullTime = 0;
            DoubleController.RequestFullTime = playerInfo.PlayerData.doubleTime;
            
            BeginSetup();

            Updated = true;
        }

        if (Updated)
        {
            Roll.doneButton.gameObject.SetActive(canDone);
            SetInternetProblemUI();
            UndoButton.canUndo = Roll.playerTurn && History.Count > 0 && !doneProcessing;
            UndoButton.textCount.text = "x" + History.Count.ToString();
            SetHighlight();
            
            Roll.doneButton.image.enabled = !doneProcessing;
            Roll.doneButton.processingObj.SetActive(doneProcessing);

            foreach (ColumnClass column in Column)
            {
                column.Update();
                column.HighLightObject.SetActive(column.HighLight);

                if (column.FullCount > 0)
                {
                    if (column.Place[0].Checker.Side == CheckerController.side.White)
                        column.Side = sides.White;
                    else
                    if (column.Place[0].Checker.Side == CheckerController.side.Black)
                        column.Side = sides.Black;
                    else
                        column.Side = sides.none;
                }
            }

            foreach (ColumnClass column in outColumn)
                column.Update();

            foreach (ColumnClass column in finishColumn)
            {
                column.Update();
                //column.HighLightObject.SetActive(column.HighLight);
            }

            SetHomeCheckers();

            setTime();
            //player
            UI.playerTurnTiming.fillAmount = LineRange(GeneralOptions.turnTime, Timing.playerTurnTime);
            if (UI.playerFullTiming != null) UI.playerFullTiming.fillAmount = LineRange(GeneralOptions.fullTime, Timing.playerFullTime);
            UI.playerNickname.text = playerInfo.PlayerData.nickname;
            UI.playerAvatarImage.sprite = playerInfo.avatars[playerInfo.PlayerData.avatarId];
            UI.playerAvatarImage.enabled = !Roll.playerTurn;
            UI.playerTurnTimeText.text = Timing.playerTurnTime.ToString("F0");
            if (Roll.playerTurn && UI.playerFullTimeText != null) UI.playerFullTimeText.text = Timing.playerFullTime.ToString("F0");
            //opponent
            UI.opponentTurnTiming.fillAmount = LineRange(GeneralOptions.turnTime, Timing.opponentTurnTime);
            if (UI.opponentFullTiming != null) UI.opponentFullTiming.fillAmount = LineRange(GeneralOptions.fullTime, Timing.opponentFullTime);
            UI.opponentNickname.text = opponentNickname;
            UI.opponentAvatarImage.sprite = playerInfo.avatars[opponentAvatarID];
            UI.opponentAvatarImage.enabled = !Roll.opTurn;
            UI.opponentTurnTimeText.text = Timing.opponentTurnTime.ToString("F0");
            if (Roll.opTurn && UI.opponentFullTimeText != null) UI.opponentFullTimeText.text = Timing.opponentFullTime.ToString("F0");
            
            // Local
            if (GeneralOptions.AI)
            {
                Roll.playerProcessing = Roll.playerTurn && Roll.Rolls.Count == 0 && !Roll.rollButton.canRoll && History.Count == 0;
                Roll.opponentProcessing = Roll.opTurn && Roll.Rolls.Count == 0 && !Roll.rollButton.canRoll && History.Count == 0;
            }
            else
            // Online
            if (GeneralOptions.Online)
            {
                Roll.playerProcessing = pauseController.Pause || !_onlineGameServer.socket.Connected || DoubleController.Requesting;
                Roll.opponentProcessing = pauseController.Pause || !_onlineGameServer.socket.Connected || DoubleController.Requesting;

                if (doneProcessing != doneProcessing_new && !doneProcessing_set)
                {
                    if (doneProcessing_new)
                        doneProcessing = true;
                    else
                    {
                        StartCoroutine(SetDoneProcessing());
                        doneProcessing_set = true;
                    }
                }
            }

            if (GeneralOptions.AI) CheckPlayerBlocked();

            if (socketManager.Connected && !socketConnected)
            {
                Timing.playerNotReadyTime = GeneralOptions.notReadyTime;
                socketConnected = true;
            }
        }
    }
    #endregion

    IEnumerator SetDoneProcessing()
    {
        yield return new WaitForSeconds(1);
        doneProcessing = doneProcessing_new;
        doneProcessing_set = false;
    }

    void SetArtType()
    {
        switch (playerInfo.PlayerData.boardType)
        {
            case ArtClass.ArtType.type1:
            {
                SetArt(0);
            }
                break;
            
            case ArtClass.ArtType.type2:
            {
                SetArt(1);
            }
                break;
            
            case ArtClass.ArtType.type3:
            {
                SetArt(2);
            }
                break;
        }
    }

    void SetArt(int type)
    {
        art.flatLeft.sprite = art.flatLeftSprite[type];
        art.flatRight.sprite = art.flatRightSprite[type];
        
        int placeType = 0;
        for (int i = 0; i < art.place.Length; i++)
        {
            art.place[i].sprite = art.placeSprite[type].sprite[placeType];
            art.place[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = art.placeHighlightSprite[type].sprite[placeType];

            if (placeType == 0)
                placeType = 1;
            else
                placeType = 0;
        }
    }

    public void ToggleAutoRoll()
    {
        Roll.AutoRoll = !Roll.AutoRoll;
    }

    #region Set Roll UI
    public IEnumerator SetRollUI(sides side, List<int> rolls)
    {
        if (rolls.Count > 1)
        {
            List<int> RollNumbers = new List<int>();
            DiceAnimController diceAnimCon = null;

            bool player = false;
            bool opponent = false;

            yield return new WaitForSeconds(10 * Time.deltaTime);

            if (side == GeneralOptions.playerSide)
            {
                RollNumbers = UI.playerRollNumbers;
                player = true;
                diceAnimCon = GeneralOptions.DiceAnimConPlayer;
            }
            else if (side == GeneralOptions.OpponentSide)
            {
                RollNumbers = UI.opponentRollNumbers;
                opponent = true;
                diceAnimCon = GeneralOptions.DiceAnimConOpponent;
            }

            if (RollNumbers.Count > 1)
            {
                RollNumbers[0] = rolls[0];
                RollNumbers[1] = rolls[1];
            }

            bool haveFour = false;
            bool isDouble = false;

            if (rolls[0] == rolls[1])
                isDouble = true;

            if (rolls.Count > 2)
            {
                RollNumbers[2] = rolls[2];
                RollNumbers[3] = rolls[3];
                haveFour = true;
            }

            float alphaHigh = 1f;
            float alphaLow = 0.3f;
            while ((Roll.playerTurn && player) || (Roll.opTurn && opponent))
            {
                yield return new WaitForSeconds(Time.deltaTime);
                if (!isDouble)
                {
                    if (haveFour)
                    {
                        diceAnimCon.dice[2].used = !Roll.Rolls.Contains(RollNumbers[2]);
                        diceAnimCon.dice[3].used = !Roll.Rolls.Contains(RollNumbers[3]);
                    }

                    diceAnimCon.dice[0].used = !Roll.Rolls.Contains(RollNumbers[0]);
                    diceAnimCon.dice[1].used = !Roll.Rolls.Contains(RollNumbers[1]);
                }
                else
                {
                    if (Roll.Rolls.Count > 3)
                        diceAnimCon.dice[3].used = false;
                    else
                        diceAnimCon.dice[3].used = true;

                    if (Roll.Rolls.Count > 2)
                        diceAnimCon.dice[2].used = false;
                    else
                        diceAnimCon.dice[2].used = true;

                    if (Roll.Rolls.Count > 1)
                        diceAnimCon.dice[1].used = false;
                    else
                        diceAnimCon.dice[1].used = true;

                    if (Roll.Rolls.Count > 0)
                        diceAnimCon.dice[0].used = false;
                    else
                        diceAnimCon.dice[0].used = true;
                }
            }
        }
    }
    #endregion

    #region Set Blocked UI
    IEnumerator SetBlockedUI(string english, string persian, string arabic)
    {
        UI.BlockedText.text = english;
        
        Audio.Blocked.Play();
        float speed = 10;

        while (UI.Blocked.alpha < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            UI.Blocked.alpha += speed * Time.deltaTime;
        }

        yield return new WaitForSeconds(100 * Time.deltaTime);

        while (UI.Blocked.alpha > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            UI.Blocked.alpha -= speed * Time.deltaTime;
        }
    }
    #endregion

    public float LineRange(float max, float current)
    {
        return current / max;
    }

    #region GetTouchNearChecker
    public CheckerController GetTouchNearChecker()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        CheckerController targetChecker = null;

        if (touchNearChecker == null)
        {
            for (int i = 0; i < Checkers.Length; i++)
            {
                float touchDistanceTarget = float.MaxValue;
                float touchDistance = Vector3.Distance(mainCamera.WorldToScreenPoint(Checkers[i].transform.position), mousePosScreen);

                if (targetChecker != null)
                {
                    touchDistanceTarget = Vector3.Distance(mainCamera.WorldToScreenPoint(targetChecker.transform.position), mousePosScreen);
                    if (touchDistance < touchDistanceTarget) targetChecker = Checkers[i];

                }
                else if (touchDistance < 120)
                {
                    targetChecker = Checkers[i];
                }
            }
        }

        return targetChecker;
    }
    #endregion

    #region Set Time
    void setTime()
    {
        // Set Game Time
        Timing.gameTimeSec += Time.deltaTime;

        if (Timing.gameTimeSec > 59)
        {
            Timing.gameTimeMin += 1;
            Timing.gameTimeSec = 0;
        }

        string sec = "0" + Timing.gameTimeSec.ToString("F0");
        string min = "0" + Timing.gameTimeMin.ToString("F0");

        if (Timing.gameTimeSec >= 10)
            sec = Timing.gameTimeSec.ToString("F0");
        if (Timing.gameTimeMin >= 10)
            sec = Timing.gameTimeMin.ToString("F0");
        
        UI.gameTimeTextUI.text = min + ":" + sec;
        
        // Set Warn
        UI.playerWarn.SetActive(Timing.playerTurnTime < 10);
        UI.opponentWarn.SetActive(Timing.opponentTurnTime < 10);

        if ((Roll.playerTurn || Roll.opTurn) && !Roll.playerProcessing && !Roll.opponentProcessing)
        {
            if (!Timing.timePassing)
            {
                if (Roll.playerTurn)
                    Timing.playerTurnTime = GeneralOptions.turnTime;
                else
                if (Roll.opTurn)
                    Timing.opponentTurnTime = GeneralOptions.turnTime;

                Timing.timePassing = true;

            } else
            {
                if (Roll.playerTurn)
                {
                    Timing.playerTurnTime -= Time.deltaTime;

                    if (Timing.playerTurnTime <= 0)
                    {
                        Timing.playerTurnTime = 0;
                        Timing.playerFullTime -= Time.deltaTime;

                        if (Timing.playerFullTime <= 0)
                        {
                            Timing.playerFullTime = 0;

                            if (GeneralOptions.AI)
                            {
                                finishController.playerWin = false;
                                finishController.Finished = true;
                            }
                            else if (GeneralOptions.Online && !timeOut && !finishController.Finished)
                            {
                                _exitButton.Yes();
                                timeOut = true;
                            }
                        }
                    }
                }
                else
                if (Roll.opTurn)
                {
                    Timing.opponentTurnTime -= Time.deltaTime;

                    if (Timing.opponentTurnTime <= 0)
                    {
                        Timing.opponentTurnTime = 0;
                        Timing.opponentFullTime -= Time.deltaTime;

                        if (Timing.opponentFullTime <= 0)
                        {
                            Timing.opponentFullTime = 0;

                            if (GeneralOptions.AI)
                            {
                                finishController.playerWin = true;
                                finishController.Finished = true;
                            }

                            //Debug.Log("Opponent Time Out");
                        }

                    }
                }
            }
        }

        if (Roll.playerTurn && !Roll.playerAction && Timing.playerTurnTime < GeneralOptions.turnTime / 2.5f) // force to roll dice
            Roll.rollButton.ButtonClick();

        if (!Roll.playerTurn)
            Timing.playerTurnTime = GeneralOptions.turnTime;

        if (!Roll.opTurn)
            Timing.opponentTurnTime = GeneralOptions.turnTime;
    }
    #endregion

    #region MyRegion
    public void UpdateRegionIcon(string region, Image icon)
    {
        if (region != "")
        {
            foreach (Region _region in regions)
            {
                if (_region.name == region)
                {
                    icon.sprite = _region.flag;
                    break;
                }
            }
        }
    }
    #endregion

    #region Set Internet Problem UI
    void SetInternetProblemUI()
    {
        float speed = 10 * Time.deltaTime;

        if (GeneralOptions.Online && !_onlineGameServer.socket.Connected && !finishController.Finished)
        {
            UI.InternetProblemUI.gameObject.SetActive(true);

            if (UI.InternetProblemUI.alpha < 1)
                UI.InternetProblemUI.alpha += speed;

            if (UI.InternetProblemUI.alpha <= 1)
                UI.InternetProblemUI.alpha = 1;
        }
        else
        {
            if (UI.InternetProblemUI.alpha > 0)
                UI.InternetProblemUI.alpha -= speed;

            if (UI.InternetProblemUI.alpha <= 0)
            {
                UI.InternetProblemUI.alpha = 0;
                UI.InternetProblemUI.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Verify Move
    public void VerifyMove(HistoryClass history)
    {
        if (GeneralOptions.Online && _onlineGameServer.socket.Connected)
        {
            int fromID = 0;
            int toID = 0;

            switch (_onlineGameServer.gameInformation.color)
            {
                case "WHITE":
                {
                    // From
                    if (history.startColumn.ID == 24) // Out
                    {
                        fromID = -1;
                    }
                    else
                    {
                        print(history.startColumn.ID);
                        fromID = history.startColumn.ID + 1;
                        print(fromID);
                    }
                    
                    // To
                    if (history.targetColumn.ID == -1) // Finish
                    {
                        toID = 0;
                    }
                    else
                    {
                        toID = history.targetColumn.ID + 1;
                    }
                }
                    break;
                
                case "BLACK":
                {
                    // From
                    if (history.startColumn.ID == 24) // Out
                    {
                        fromID = -1;
                    }
                    else
                    {
                        print(history.startColumn.ID);
                        fromID = (23 - history.startColumn.ID) + 1;
                        print(fromID);
                    }
                    
                    // To
                    if (history.targetColumn.ID == -1) // Finish
                    {
                        toID = 25;
                    }
                    else
                    {
                        toID = (23 - history.targetColumn.ID) + 1;
                    }
                }
                    break;
            }
            
            
            var commandData = new
            {
                name = "MOVE",
                playerColor = _onlineGameServer.gameInformation.color,
                from = fromID,
                to = toID
            };

            var commandData2 = new
            {
                name = "MOVE",
                playerColor = _onlineGameServer.gameInformation.color,
                from = "BAR",
                to = toID
            };

            if (DeveloperMode)
                Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> Move: (Sanded) </color> <color=white> (from: " + commandData.from + ") | (to: " + commandData.to + "</color>)");
            
            if (commandData.from != -1)
                _onlineGameServer.socket.EmitAsync("command", commandData);
            else
                _onlineGameServer.socket.EmitAsync("command", commandData2);

            Debug.Log("<color=yellow> Move </color> <color=white>  Verifying... </color>");
        }
    }
    #endregion

    #region Socket On
    public void SocketOn()
    {
        print("<color=red> Listening To Server </color>");
        
        _onlineGameServer.socket.On("playerReady", response => {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (DeveloperMode)
                    Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> playerReady: </color> <color=white>" + response.ToString() + "</color>");
                
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.messageClass>>(response.ToString());

                if (result[0].message == "Both players are ready. The game has started.")
                {
                    UI.WaitUI.gameObject.SetActive(false);
                    AllReady = true;
                }
            });
        });

        _onlineGameServer.socket.On("FIRST_DICE", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.firstDiceClass>>(response.ToString());
                
                int playerDice = 0;
                int opponentDice = 0;

                if (_onlineGameServer.gameInformation.color == "WHITE")
                {
                    playerDice = result[0].dice1;
                    opponentDice = result[0].dice2;
                }
                else
                {
                    playerDice = result[0].dice2;
                    opponentDice = result[0].dice1;
                }

                _onlineGameServer.firstDice.playerDice = playerDice;
                _onlineGameServer.firstDice.opponentDice = opponentDice;

                GeneralOptions.DiceAnimConPlayer.StartRollDice();
                GeneralOptions.DiceAnimConOpponent.StartRollDice();
            });
        });
        
        _onlineGameServer.socket.On("MOVE", response =>
        {
            try
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (DeveloperMode)
                        Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> MOVE:  </color> <color=white>" +
                                  response.ToString() + "</color>");

                    var result = JsonConvert.DeserializeObject<List<OnlineGameServer.MoveClass>>(response.ToString());

                    if (result[0].playerColor == _onlineGameServer.gameInformation.color)
                    {
                        if (result[0].success)
                        {
                            Roll.moveVerifyNumber += 1;

                            if (Roll.moveVerifyNumber == Roll.moveNumber) // Done
                            {
                                DoneOnline();
                            }
                        }
                    }
                    else
                    {
                        ColumnClass columnStart = null;
                        ColumnClass columnTarget = null;

                        int fromColumn = 0;
                        int toColumn = 0;
                        bool isOut = false;
      
                        switch (_onlineGameServer.gameInformation.color)
                        {
                            case "WHITE":
                            {
                                // From
                                if (result[0].from == "BAR") // Out
                                {
                                    fromColumn = 1;
                                    columnStart = outColumn[fromColumn];
                                }
                                else
                                {
                                    fromColumn = int.Parse(result[0].from) - 1;
                                    columnStart = Column[fromColumn];
                                }
                                
                                if (result[0].to == 0 || result[0].to == 25) // Finish
                                {
                                    toColumn = 0;
                                    columnTarget = finishColumn[toColumn];
                                }
                                else
                                {
                                    toColumn = result[0].to - 1;
                                    columnTarget = Column[toColumn];
                                }
                            }
                                break;
                
                            case "BLACK":
                            {
                                // From
                                if (result[0].from == "BAR") // Out
                                {
                                    fromColumn = 1;
                                    columnStart = outColumn[fromColumn];
                                }
                                else
                                {
                                    fromColumn = 24 - int.Parse(result[0].from);
                                    columnStart = Column[fromColumn];
                                }

                                if (result[0].to == 25 || result[0].to == 0) // Finish
                                {
                                    toColumn = 0;
                                    columnTarget = finishColumn[toColumn];
                                }
                                else
                                {
                                    toColumn = 24 - result[0].to;
                                    columnTarget = Column[toColumn];
                                }
                            }
                                break;
                        }
                        
                        CheckerController checkerTarget = GetLastChecker(columnStart, GeneralOptions.OpponentSide);
                        checkerTarget.CheckFinish(columnTarget);
                        checkerTarget.UpdateRolls(columnTarget, true, false);
                        PlaceChecker(checkerTarget, columnTarget, true, columnStart, false);
                        Debug.Log("<color=yellow> Move: (" + name + "):  </color> <color=white> Done </color>");
                    }
                });
            }catch(Exception e) { Debug.LogException(e); }
        });
        
        _onlineGameServer.socket.On("turnEnded", response =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (DeveloperMode)
                    Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> turnEnded:  </color> <color=white>" + response.ToString() + "</color>");
                
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.EndTurnClass>>(response.ToString());

                if (result[0].message != "It is not your turn!")
                {
                    if (result[0].completedPlayer == _onlineGameServer.gameInformation.color)
                    {
                        EndTurn("player", GeneralOptions.delayTurn);
                        Debug.Log("<color=green> ChangeTurn: (Opponent) </color> <color=white> Done </color>");
                    }
                    else
                    {
                        EndTurn("opponent", GeneralOptions.delayTurn);
                        Debug.Log("<color=green> ChangeTurn: (Player) </color> <color=white> Done </color>");
                    }
                }
            }); 
        });

        _onlineGameServer.socket.On("REQUEST_DOUBLE", response => { 
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (DeveloperMode)
                    Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> REQUEST_DOUBLE:  </color> <color=white>" + response.ToString() + "</color>");
                
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.messageClass>>(response.ToString());

                string color = "";
                
                switch (result[0].message)
                {
                    case "WHITE has requested to double the stakes!": color = "WHITE";
                        break;

                    case "BLACK has requested to double the stakes!": color = "BLACK";
                        break;
                }

                if (color == _onlineGameServer.gameInformation.color)
                {
                    Debug.Log("<color=green> REQUEST_DOUBLE (to Opponent) </color> <color=white> Done </color>");
                }
                else
                {
                    DoubleController.Requesting = true;
                    DoubleController.RequestedMe = true;
                    DoubleController.myRequest = false;

                    Debug.Log("<color=green> REQUEST_DOUBLE (to Player) </color> <color=white> Done </color>");
                }
            }); 
        });
        
        _onlineGameServer.socket.On("ACCEPT_DOUBLE", response => { 
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (DeveloperMode)
                    Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> ACCEPT_DOUBLE:  </color> <color=white>" + response.ToString() + "</color>");
                
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.AcceptDoubleClass>>(response.ToString());
                
                StartCoroutine(DoubleController.EndRequest(true, result[0].doubleAmount,GeneralOptions.OpponentSide));
                Debug.Log("<color=green> AcceptDouble </color> <color=white> Done </color>");
            }); 
        });
        
        _onlineGameServer.socket.On("GAME_OVER", response => { 
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (DeveloperMode)
                    Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=pink> GAME_OVER:  </color> <color=white>" + response.ToString() + "</color>");
                
                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.GameOverClass>>(response.ToString());

                _onlineGameServer.gameInformation.rematchKey = result[0].rematchKey;
                
                string strResponse = result[0].message.ToString();
                strResponse = strResponse.Trim('[', ']');
                strResponse = strResponse.Replace("\"", "");

                string type = "";
                string winColor = "";
                
                switch (strResponse)
                {
                    case "WHITE denied the double and loses!":
                    {
                        type = "denied the double";
                        winColor = "BLACK";
                    }
                        break;

                    case "BLACK denied the double and loses!":
                    {
                        type = "denied the double";
                        winColor = "WHITE";
                    }
                        break;

                    case "BLACK has borne off all checkers and wins the game!":
                    {
                        type = "all checker";
                        winColor = "BLACK";
                    }
                        break;
                    
                    case "WHITE has borne off all checkers and wins the game!":
                    {
                        type = "all checker";
                        winColor = "WHITE";
                    }
                        break;
                        
                    case "BLACK has resigned. WHITE wins!":
                    {
                        type = "resigned";
                        winColor = "WHITE";
                    }
                        break;
                    
                    case "WHITE has resigned. BLACK wins!":
                    {
                        type = "resigned";
                        winColor = "BLACK";
                    }
                        break;
                }

                finishController.FinishOnline(winColor, type, _onlineGameServer.gameInformation.coin, DoubleController.Multiple);
                
                DoubleController.myRequest = false;
                DoubleController.RequestedMe = false;
                DoubleController.Requesting = false;
                
                _searchSystem.SocketOn();
            }); 
        });
    }

    #region RollDice Opponent
    void RollDiceOpponent()
    {
        _onlineGameServer.socket.On("REQUEST_ROLL_DICE", (response) =>
        {
            if (DeveloperMode)
                Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> RollDice: (Opponent) </color> <color=white>" + response.ToString() + "</color>");

            var result = JsonConvert.DeserializeObject<List<OnlineGameServer.RollDiceClass>>(response.ToString());
            
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (result[0].color != _onlineGameServer.gameInformation.color) // opponent RollDice
                {
                    GeneralOptions.DiceAnimConPlayer.EndShow();
                    GeneralOptions.DiceAnimConOpponent.EndShow();

                    Roll.Rolls.Clear();

                    int e1 = result[0].dice1;
                    int e2 = result[0].dice2;

                    Roll.Rolls.Add(e1);
                    Roll.Rolls.Add(e2);

                    if (e1 == e2)
                    {
                        Roll.Rolls.Add(e1);
                        Roll.Rolls.Add(e2);
                    }

                    Roll.opponentAction = true;

                    float delay = 1;
                    if (StartingRollDice) delay = 30;

                    StartCoroutine(GeneralOptions.DiceAnimConOpponent.RollDice(delay, false/*result[0].isBlocked*/));

                    Debug.Log("<color=green> RollDice: (Opponent) </color> <color=white> Done </color>");

                    StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls)); 
                }
            });
        });
    }
    #endregion

    #endregion
    
    public void DoneOnline()
    {
        var commandData = new
        {
            name = "SET_PLAYER_DONE",
            playerColor = _onlineGameServer.gameInformation.color
        };
            
        _onlineGameServer.socket.EmitAsync("command", commandData);
        Debug.Log("<color=yellow> Submit </color> <color=white>  Processing... </color>");
        doneProcessing_new = false;
    }

    #region Set Game
    void SetGame(SocketManager.OpponentFoundInGameData.CheckerData[] holeSetup, SocketManager.OpponentFoundInGameData.multipleClass multiple,string currentPlayer, int[] roll, bool isBlocked)
    {
        if (DeveloperMode)
            Debug.Log("<color=red> (DEVELOPER MODE) </color> <color=yellow> SetGame: </color> holeSetup: " + holeSetup.ToString() + " | multiple: " + multiple.ToString());

        ClearFullPlaces();

        // Set Checkers Columns
        for (int i = 0; i < holeSetup.Length; i++)
        {
            PlaceClass placeTarget = null;
            int columnTargetID = holeSetup[i].column;
            int checkerTargetID = 29 - holeSetup[i].id;
            ColumnClass columnTarget = null;


            if (columnTargetID == 0)
            {
                if (checkerTargetID >= 15)
                    placeTarget = GetEmpityPlace(outColumn[1]); // Player   
                else
                {
                    columnTarget = finishColumn[1];
                    placeTarget = GetEmpityPlace(finishColumn[1]); // Opponent
                }
                    
            }
            else
            if (columnTargetID == 25)
            {
                if (checkerTargetID >= 15)
                {
                    placeTarget = GetEmpityPlace(finishColumn[0]); // Player
                    columnTarget = finishColumn[0];
                }
                else
                    placeTarget = GetEmpityPlace(outColumn[0]); // Opponent
            }
            else
                placeTarget = GetEmpityPlace(Column[columnTargetID-1]);

            print(i);
            CheckerController checkerTarget = Checkers[checkerTargetID];
            placeTarget.Checker = checkerTarget;
            checkerTarget.FixTarget = placeTarget.Pos;
            placeTarget.Full = true;
            checkerTarget.canFix = true;

            if (columnTarget != null)
                checkerTarget.CheckFinish(columnTarget);
        }

        // Set Double owner
        if (multiple.owner == playerInfo.PlayerData.token)
            DoubleController.Side = GeneralOptions.playerSide;
        else if (multiple.owner != "")
            DoubleController.Side = GeneralOptions.OpponentSide;
        else
            DoubleController.Side = sides.none;

        // Set Double Multiple
        /*DoubleController.Multiple = multiple.ratio;
        DoubleController.SetMultipleRot();*/

        ResetCheckers();

        if (roll[0] != 0)
        {
            Roll.Rolls.Clear();

            if (socketManager.GameData.FirstMove)
            {
                socketManager.GameData.FirstRollDice.currentPlayer = currentPlayer;
                socketManager.GameData.FirstRollDice.rolledDice = roll;

                Roll.Rolls.Add(roll[0]);
                Roll.Rolls.Add(roll[1]);

                if (roll[0] == roll[1])
                {
                    Roll.Rolls.Add(roll[0]);
                    Roll.Rolls.Add(roll[1]);
                }

                StartCoroutine(StartRollDice(50));

                socketManager.GameData.FirstMove = false;

                SetRollUI(GeneralOptions.playerSide, Roll.Rolls);

            }else
            {
                GeneralOptions.DiceAnimConPlayer.EndShow();
                GeneralOptions.DiceAnimConOpponent.EndShow();

                Roll.Rolls.Add(roll[0]);
                Roll.Rolls.Add(roll[1]);

                if (roll[0] == roll[1])
                {
                    Roll.Rolls.Add(roll[0]);
                    Roll.Rolls.Add(roll[1]);
                }

                if (currentPlayer == playerInfo.PlayerData.token)
                {
                    StartCoroutine(GeneralOptions.DiceAnimConPlayer.RollDice(1, isBlocked));
                    StartCoroutine(SetRollUI(GeneralOptions.playerSide, Roll.Rolls));

                    Roll.opTurn = false;
                    Roll.opponentAction = false;
                    Roll.playerTurn = true;
                    Roll.playerAction = true;
                }
                else
                {
                    StartCoroutine(GeneralOptions.DiceAnimConOpponent.RollDice(1, isBlocked));
                    StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls));

                    Roll.playerTurn = false;
                    Roll.playerAction = false;
                    Roll.opTurn = true;
                    Roll.opponentAction = true;
                }
            }
        }else
        {
            if (currentPlayer == playerInfo.PlayerData.token)
            {
                Roll.playerTurn = true;
                Roll.playerAction = true;
                Roll.opTurn = false;
                Roll.opponentAction = false;

                StartCoroutine(SetTurn("player", GeneralOptions.delayTurn));
            }
            else
            {
                Roll.playerTurn = false;
                Roll.playerAction = false;
                Roll.opTurn = true;
                Roll.opponentAction = true;

                StartCoroutine(SetTurn("opponent", GeneralOptions.delayTurn));
            }
        }

        Debug.Log("<color=green> SetGame </color> <color=white> Updated </color>");
    }
    #endregion

    #region get Empity Place
    PlaceClass GetEmpityPlace(ColumnClass column)
    {
        for (int i = 0; i < column.Place.Count; i++)
            if (!column.Place[i].Full) return column.Place[i];

        return null;
    }
    #endregion

    #region Clear Full Places
    void ClearFullPlaces()
    {
        // Column
        foreach(ColumnClass column in Column)
        {
            foreach(PlaceClass place in column.Place)
            {
                place.Full = false;
            }
        }

        // Finish Column
        foreach (ColumnClass column in finishColumn)
        {
            foreach (PlaceClass place in column.Place)
            {
                place.Full = false;
            }
        }

        // OutColumn
        foreach (ColumnClass column in outColumn)
        {
            foreach (PlaceClass place in column.Place)
            {
                place.Full = false;
            }
        }
    }
    #endregion

    #region Set Home Checkers
    void SetHomeCheckers()
    {
        List<CheckerController> playerCheckers = new List<CheckerController>();
        List<CheckerController> opponentCheckers = new List<CheckerController>();

        foreach (CheckerController checker in Checkers)
        {
            if (((checker.Side == CheckerController.side.White && GeneralOptions.playerSide == sides.White) || (checker.Side == CheckerController.side.Black && GeneralOptions.playerSide == sides.Black)) && !playerCheckers.Contains(checker))
                playerCheckers.Add(checker);

            if (((checker.Side == CheckerController.side.White && GeneralOptions.OpponentSide == sides.White) || (checker.Side == CheckerController.side.Black && GeneralOptions.OpponentSide == sides.Black)) && !opponentCheckers.Contains(checker))
                opponentCheckers.Add(checker);
        }

        int playerHomeCount = 0;
        int opponentHomeCount = 0;

        for (int i = 0; i < playerCheckers.Count; i++)
        {
            //player
            if (GeneralOptions.PlayerLocation == Location.Down && (playerCheckers[i].columnStart.ID <= 5 && (playerCheckers[i].columnStart.ID >= 0 && playerCheckers[i].columnStart.ID <= 23)) || playerCheckers[i].finished) playerHomeCount += 1;
            else
            if (GeneralOptions.PlayerLocation == Location.Up && (playerCheckers[i].columnStart.ID >= 18 && (playerCheckers[i].columnStart.ID >= 0 && playerCheckers[i].columnStart.ID <= 23)) || playerCheckers[i].finished) playerHomeCount += 1;

            //opponent
            if (GeneralOptions.OpponentLocation == Location.Down && (opponentCheckers[i].columnStart.ID <= 5 && (opponentCheckers[i].columnStart.ID >= 0 && opponentCheckers[i].columnStart.ID <= 23)) || opponentCheckers[i].finished) opponentHomeCount += 1;
            else
            if (GeneralOptions.OpponentLocation == Location.Up && (opponentCheckers[i].columnStart.ID >= 18 && (opponentCheckers[i].columnStart.ID >= 0 && opponentCheckers[i].columnStart.ID <= 23)) || opponentCheckers[i].finished) opponentHomeCount += 1;
        }

        Roll.playerHome = playerHomeCount == 15;
        Roll.opponentHome = opponentHomeCount == 15;
    }
    #endregion

    #region Setup Finish Column
    void SetupFinishColumn()
    {
        finishColumnOptionClass finishColumnOption = Options.finishColumnOption;

        for (int i = 0; i < 2; i++)
            finishColumn.Add(new ColumnClass());

        finishColumn[0].Pos = Column[0].Pos;
        finishColumn[0].Pos.x += finishColumnOption.distanceX;
        finishColumn[0].Pos.y += finishColumnOption.distanceZ;

        finishColumn[1].Pos = finishColumn[0].Pos;
        finishColumn[1].Pos.y -= finishColumnOption.distanceBetween;

        finishColumn[0].ID = 24;
        finishColumn[1].ID = -1;
        
        finishColumn[0].isFinish = true;
        finishColumn[1].isFinish = true;

        finishColumn[0].Direction = direction.Up;
        finishColumn[1].Direction = direction.Up;

        if ((GeneralOptions.playerSide == sides.White && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.Black) && GeneralOptions.PlayerLocation == Location.Up)
        {
            finishColumn[0].Side = sides.Black;
            finishColumn[1].Side = sides.White;

        }
        else
        if ((GeneralOptions.playerSide == sides.Black && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.White) && GeneralOptions.PlayerLocation == Location.Up)
        {
            finishColumn[0].Side = sides.White;
            finishColumn[1].Side = sides.Black;
        }
        //
        // float dis = -0.3f;
        // for (int i = 0; i < 2; i++)
        //     finishColumn[i].HighLightObject = Instantiate(GeneralOptions.HighLightFinishPrefab, new Vector3(finishColumn[i].Pos.x, finishColumn[i].Pos.y, finishColumn[i].Pos.z + dis), GeneralOptions.HighLightFinishPrefab.transform.rotation);

        finishColumn[0].highlightSprite = GeneralOptions.finishHighlight[0];
        finishColumn[1].highlightSprite = GeneralOptions.finishHighlight[1];
    }
    #endregion

    #region Setup Out Column
    void SetupOutColumn()
    {
        outColumnOptionsClass outColumnOption = Options.outColumnOption;;

        for (int i = 0; i < 2; i++)
            outColumn.Add(new ColumnClass());

        outColumn[0].Pos = Column[0].Pos;
        outColumn[0].Pos.x -= outColumnOption.distanceX;
        outColumn[0].Pos.y += outColumnOption.distanceZ;

        outColumn[1].Pos = outColumn[0].Pos;
        outColumn[1].Pos.y -= outColumnOption.distanceBetween;

        outColumn[0].ID = 24;
        outColumn[1].ID = -1;
        
        outColumn[0].isFinish = false;
        outColumn[1].isFinish = false;

        outColumn[0].Direction = direction.Down;
        outColumn[1].Direction = direction.Up;

        if ((GeneralOptions.playerSide == sides.White && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.Black) && GeneralOptions.PlayerLocation == Location.Up)
        {
            outColumn[0].Side = sides.White;
            outColumn[1].Side = sides.Black;

        } else
        if ((GeneralOptions.playerSide == sides.Black && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.White) && GeneralOptions.PlayerLocation == Location.Up)
        {
            outColumn[0].Side = sides.Black;
            outColumn[1].Side = sides.White;
        }
    }
    #endregion

    #region CheckBlock
    bool CheckBlock()
    {
        foreach (CheckerController checker in Checkers)
        {
            if (checker.canDrag)
                return false;
        }
        return true;
    }
    #endregion

    #region Starting To End
    IEnumerator StartingToEnd(float delay)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        try
        {
            Starting = false;

            if (GeneralOptions.AI)
                StartCoroutine(StartRollDice(50));
            else
            if (GeneralOptions.Online && _onlineGameServer.socket.Connected)
            {
                if (!_onlineGameServer.gameInformation.live)
                {
                    UI.WaitUI.gameObject.SetActive(true);
                    RollDiceOpponent();
                    var commandData = new
                    {
                        name = "SET_PLAYER_READY",
                        playerColor = _onlineGameServer.gameInformation.color
                    };
                    _onlineGameServer.socket.EmitAsync("command", commandData);
                    Debug.Log("<color=green> READY </color>");
                }
                else
                {
                    socketManager.socket.Emit("Resume");
                    Debug.Log("<color=green> Resume </color>");
                    socketManager.GameData.gameLive = false;
                    AllReady = true;
                }
            }
        }catch(Exception e) { Debug.LogException(e); }
    }
    #endregion

    #region Start RollDice
    IEnumerator StartRollDice(float delay)
    {
        if (Roll.playerStartRoll == 0 || Roll.playerStartRoll == Roll.opponentStartRoll)
        {
            StartingRollDice = true;
            GeneralOptions.DiceAnimConPlayer.EndShow();
            GeneralOptions.DiceAnimConOpponent.EndShow();
            yield return new WaitForSeconds(delay * Time.deltaTime);

            GeneralOptions.DiceAnimConPlayer.StartRollDice();
            GeneralOptions.DiceAnimConOpponent.StartRollDice();

            Debug.Log("Start Roll: " + "Player: " + Roll.playerStartRoll + " | " + "Opponent: " + Roll.opponentStartRoll);
        }
    }
    #endregion

    #region Check Start RollDice
    public IEnumerator checkStartRollDice(float delay)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        
        GeneralOptions.DiceAnimConPlayer.dice[0].StopDice();
        GeneralOptions.DiceAnimConOpponent.dice[0].StopDice();
        
        StartCoroutine(GeneralOptions.DiceAnimConPlayer.playSound());

        if (Roll.playerStartRoll == Roll.opponentStartRoll)
        {
            yield return new WaitForSeconds(100 * Time.deltaTime);
            StartCoroutine(StartRollDice(50));
        }
        else
        if (Roll.playerStartRoll > Roll.opponentStartRoll)
        {
            StartingRollDice = false;

            yield return new WaitForSeconds(100 * Time.deltaTime);
            
            StartCoroutine(SetTurn("player", GeneralOptions.delayTurn));
            StartCoroutine(SetBlockedUI("YOU START", "شما شروع کنید" ,"أنت تبدأ"));
            
            GeneralOptions.DiceAnimConPlayer.EndShow();
            GeneralOptions.DiceAnimConOpponent.EndShow();
        }
        else
        {
            StartingRollDice = false;

            yield return new WaitForSeconds(100 * Time.deltaTime);
            
            StartCoroutine(SetTurn("opponent", GeneralOptions.delayTurn));
            StartCoroutine(SetBlockedUI("OPPONENT START", "حریف شروع می کند", "الخصم يبدأ"));

            GeneralOptions.DiceAnimConPlayer.EndShow();
            GeneralOptions.DiceAnimConOpponent.EndShow();
        }
    }
    #endregion

    #region SetTurn
    IEnumerator SetTurn(string side, float delay)
    {
        foreach (CheckerController checker in Checkers)
        {
            checker.canDrag = false;
            checker.HighLight = false;
            checker.columnTarget.Clear();
        }

        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (side == "player")
        {
            Roll.playerTurn = true;
            Audio.myTurn.Play();
            
            Roll.rollButton.canRoll = true;
        }
        else
        if (side == "opponent")
        {
            Roll.opTurn = true;
            Audio.opponentTurn.Play();

            // Local
            if (GeneralOptions.AI)
            {
                //int DoubleReq = UnityEngine.Random.Range(1, 20);
                bool goodDouble = (finishCount.playerCount < finishCount.opponentCount);
                if (goodDouble && DoubleController.Multiple < 64 && (DoubleController.Side == GeneralOptions.OpponentSide || DoubleController.Side == sides.none) && !StartingRollDice)
                    DoubleController.RequestDouble(GeneralOptions.playerSide);
                else
                {
                    GeneralOptions.DiceAnimConPlayer.EndShow();
                    GeneralOptions.DiceAnimConOpponent.EndShow();

                    float delay2 = 1;
                    if (StartingRollDice) delay2 = 30;

                    StartCoroutine(GeneralOptions.DiceAnimConOpponent.RollDice(delay2,false));
                }
            }else
            // Online
            if (GeneralOptions.Online)
            {
                if (socketManager.GameData.FirstMove)
                {
                    Roll.Rolls.Clear();

                    int e1 = socketManager.GameData.FirstRollDice.rolledDice[0];
                    int e2 = socketManager.GameData.FirstRollDice.rolledDice[1];

                    Roll.Rolls.Add(e1);
                    Roll.Rolls.Add(e2);

                    if (e1 == e2)
                    {
                        Roll.Rolls.Add(e1);
                        Roll.Rolls.Add(e2);
                    }

                    Roll.opponentAction = true;

                    StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls));

                    socketManager.GameData.FirstMove = false;
                }
            }
        }
        Debug.Log(side + " Turn");
    }
    #endregion

    #region  Blocked Online
    public void BlockedOnline(string side)
    {
        Roll.Rolls.Clear();

        if (side == "player")
        {
            StartCoroutine(SetBlockedUI("BLOCKED", "مسدود شد", "تم الحظر"));
            DoneOnline();
            Debug.Log("<color=yellow> Blocked </color> <color=white> Player </color>");
        }
        else if (side == "opponent")
        {
            StartCoroutine(SetBlockedUI("OPPONENT BLOCKED", "حریف مسدود شد", "تم حظر الخصم"));
            Debug.Log("<color=yellow> Blocked </color> <color=white> Opponent </color>");
        }
    }
    #endregion

    #region AI Act
    public IEnumerator AIAct(float delay, string type)
    {
        if (Roll.Rolls.Count > 0)
            UsableChecker(GeneralOptions.OpponentLocation, GeneralOptions.OpponentSide, Roll.Rolls);
        yield return new WaitForSeconds(delay * Time.deltaTime);

        List<CheckerController> avableCheckers = new List<CheckerController>();

        for (int i = 0; i < Checkers.Length; i++)
        {
            if (Checkers[i].columnTarget.Count > 0 && !avableCheckers.Contains(Checkers[i]))
                avableCheckers.Add(Checkers[i]);
        }

        if (avableCheckers.Count > 0)
        {
            CheckerController checkerTarget = null;
            ColumnClass columnTarget = null;
            if (type == "random")
            {
                int checkerTargetID = -1;
                checkerTargetID = UnityEngine.Random.Range(0, avableCheckers.Count);
                checkerTarget = avableCheckers[checkerTargetID];
                
                int columnTargetID = -1;
                columnTargetID = UnityEngine.Random.Range(0, checkerTarget.columnTarget.Count);
                columnTarget = checkerTarget.columnTarget[columnTargetID];

            }else if (type == "intelligent")
            {
                // Check For Kick Checker (1)
                #region Check For Kick Checker

                bool canKick = false;
                bool haveTarget = false;
                foreach (CheckerController checker in avableCheckers)
                {
                    if (checker.columnTarget.Count > 0)
                    {
                        haveTarget = true;
                        break;
                    }
                }

                if (haveTarget)
                {
                    foreach (CheckerController checker in avableCheckers)
                    {
                        foreach (ColumnClass column in checker.columnTarget)
                        {
                            if (column.Side == GeneralOptions.playerSide && column.FullCount == 1)
                            {
                                canKick = true;
                                columnTarget = column;
                                checkerTarget = checker;
                                break;
                            }
                        }
                    }
                }
                #endregion
                if (!canKick || !haveTarget)
                {
                    // Check For Out Home Checker (2)
                    #region Check For Out Home Checker

                    List<CheckerController> outHomeCheckers = new List<CheckerController>();
                    
                    foreach (CheckerController checker in avableCheckers)
                    {
                        if (checker.columnStart.ID < 18)
                            outHomeCheckers.Add(checker);
                    }

                    bool haveAllyColumn = false;
                    if (outHomeCheckers.Count > 0)
                    {
                        // Check For Have Ally Column (3)
                        #region Check For Have Ally Column
                        
                        haveTarget = false;
                        foreach (CheckerController checker in outHomeCheckers)
                        {
                            if (checker.columnTarget.Count > 0)
                            {
                                haveTarget = true;
                                break;
                            }
                        }

                        if (haveTarget)
                        {
                            foreach (CheckerController checker in outHomeCheckers)
                            {
                                foreach (ColumnClass column in checker.columnTarget)
                                {
                                    if (column.FullCount > 0 && column.Side == GeneralOptions.OpponentSide)
                                    {
                                        haveAllyColumn = true;
                                        columnTarget = column;
                                        checkerTarget = checker;
                                        break;
                                    }
                                }
                            }
                        }
                        
                        if (!haveAllyColumn || !haveTarget)
                        {
                            int targetCheckerID = UnityEngine.Random.Range(0, outHomeCheckers.Count);
                            int targetColumnID = UnityEngine.Random.Range(0,outHomeCheckers[targetCheckerID].columnTarget.Count);
                            columnTarget = outHomeCheckers[targetCheckerID].columnTarget[targetColumnID];
                            checkerTarget = outHomeCheckers[targetCheckerID];
                        }
                        #endregion
                    }
                    #endregion
                    else
                    {
                        // Check For Can Finish Checker (4)
                        #region Check For Can Finish Checker
                        
                        haveTarget = false;
                        foreach (CheckerController checker in avableCheckers)
                        {
                            if (checker.columnTarget.Count > 0)
                            {
                                haveTarget = true;
                                break;
                            }
                        }
                        
                        bool canFinishChecker = false;
                        if (haveTarget)
                        {
                            
                            foreach (CheckerController checker in avableCheckers)
                            {
                                foreach (ColumnClass column in checker.columnTarget)
                                {
                                    if (column.ID > 23)
                                    {
                                        canFinishChecker = true;
                                        columnTarget = column;
                                        checkerTarget = checker;
                                    }
                                }
                            }
                        }

                        if (!canFinishChecker || !haveTarget)
                        {
                            int targetCheckerID = UnityEngine.Random.Range(0, avableCheckers.Count);
                            int targetColumnID = UnityEngine.Random.Range(0,avableCheckers[targetCheckerID].columnTarget.Count);
                            columnTarget = avableCheckers[targetCheckerID].columnTarget[targetColumnID];
                            checkerTarget = avableCheckers[targetCheckerID];
                        }
                        #endregion
                    }
                }
            }

            if (checkerTarget != null && columnTarget != null)
            {
                checkerTarget.CheckFinish(columnTarget);
                ResetCheckers();
                checkerTarget.UpdateRolls(columnTarget, false, false);
                PlaceChecker(checkerTarget, columnTarget, true, checkerTarget.columnStart, false);

                if (Roll.Rolls.Count > 0)
                    StartCoroutine(checkerTarget.UpdateNaxtMove(GeneralOptions.delayUpdateNaxtMove, "opponent"));
                else
                    EndTurn("opponent", GeneralOptions.delayTurn);
            }else
                EndTurn("opponent", GeneralOptions.delayTurn);
        }
        else
        {
            EndTurn("opponent", GeneralOptions.delayTurn);
            Roll.Rolls.Clear();
            Debug.Log("Opponent Blocked !"); 
            StartCoroutine(SetBlockedUI("OPPONENT BLOCKED", "حریف مسدود شد", "تم حظر الخصم"));
        }
    }
    #endregion

    #region Get Touch Position
    public Vector3 GetTouchPosition()
    {
        Vector3 touchPosition = Vector3.zero;
        
        Vector3 mousePosition = Input.mousePosition;
        
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

        touchPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        return touchPosition;
    }
    #endregion

    #region Clear ColumnHighLight
    void ClearColumnHighLight()
    {
        foreach (ColumnClass column in Column)
            column.HighLight = false;

        foreach (ColumnClass column in finishColumn)
            column.HighLight = false;
    }
    #endregion

    #region EndTurn
    public void EndTurn(string side, float changeTurnDelay)
    {
        History.Clear();
        ClearColumnHighLight();

        GeneralOptions.DiceAnimConOpponent.EndShow();
        GeneralOptions.DiceAnimConPlayer.EndShow();

        if (side == "player")
        {
            Roll.playerTurn = false;
            Roll.playerAction = false;

            if (GeneralOptions.Online)
                RollDiceOpponent();

            ResetCheckers();
            //GeneralOptions.DiceControllerPlayer.Show = false;
            StartCoroutine(SetTurn("opponent", changeTurnDelay));
        }
        else
        if (side == "opponent")
        {
            Roll.opTurn = false;
            Roll.opponentAction = false;

            ResetCheckers();
            //GeneralOptions.DiceControllerOpponent.Show = false;
            StartCoroutine(SetTurn("player", changeTurnDelay));
        }

        Roll.moveNumber = 0;
        Roll.moveVerifyNumber = 0;
        Debug.Log("End " + side + " Turn");
    }
    #endregion

    #region Begin Setup
    public void BeginSetup()
    {
        Timing.playerFullTime = GeneralOptions.fullTime;
        Timing.opponentFullTime = GeneralOptions.fullTime;
        Starting = true;
        StartCoroutine(StartingToEnd(100));

        GameObject CheckersObj = new GameObject("Checkers");

        for (int i = 0; i < 30; i++)
        {
            GameObject checker = Instantiate(Checker, new Vector3(1000,1000,1000), Checker.transform.rotation);
            CheckerController CheckerCon = checker.GetComponent<CheckerController>();
            checker.transform.SetParent(CheckersObj.transform);
            CheckerCon.ID = i;

            if (i < 15)
            {
                if (GeneralOptions.PlayerLocation == Location.Down)
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.Black;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.White;
                } else
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.White;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.Black;
                }
            }
            else
            {
                if (GeneralOptions.PlayerLocation == Location.Down)
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.White;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.Black;
                } else
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.Black;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.White;
                }
            }

            switch (i)
            {
                case <= 1: PlaceChecker(CheckerCon,Column[0],false,Column[0], false); break;
                case <= 6: PlaceChecker(CheckerCon,Column[11],false,Column[0], false); break;
                case <= 9: PlaceChecker(CheckerCon,Column[16],false,Column[0], false); break;
                case <= 14: PlaceChecker(CheckerCon,Column[18],false,Column[0], false); break;
                case <= 16: PlaceChecker(CheckerCon,Column[23],false,Column[0], false); break;
                case <= 21: PlaceChecker(CheckerCon,Column[12],false,Column[0], false); break;
                case <= 24: PlaceChecker(CheckerCon,Column[7],false,Column[0], false); break;
                case <= 29: PlaceChecker(CheckerCon,Column[5],false,Column[0], false); break;
            }

            /*if (i < 15)
            {
                PlaceChecker(CheckerCon, Column[23], false, Column[0], false);
            }
            else
                PlaceChecker(CheckerCon, Column[0], false, Column[0], false);*/
                
            /*if (i < 15)
            {
                if (i < 12)
                    PlaceChecker(CheckerCon, Column[18], false, Column[0], false);
                else
                    PlaceChecker(CheckerCon, Column[22], false, Column[0], false);
            }
            else
            {
                if (i < 19)
                    PlaceChecker(CheckerCon, Column[1], false, Column[0], false);
                else
                if (i < 26)
                    PlaceChecker(CheckerCon, Column[5], false, Column[0], false);
                else
                if (i < 27)
                    PlaceChecker(CheckerCon, Column[11], false, Column[0], false);
                else
                if (i < 28)
                    PlaceChecker(CheckerCon, Column[13], false, Column[0], false);
                else
                    PlaceChecker(CheckerCon, Column[19], false, Column[0], false);
            }*/

            CheckerCon.canDrag = false;
            CheckerCon.Stand = false;
            Checkers = FindObjectsOfType<CheckerController>();
            StartCoroutine(startingCheckers(0.01f));
        }
    }
    #endregion

    #region place Checker
    public void PlaceChecker(CheckerController checker, ColumnClass columnTarget, bool haveStart, ColumnClass columnStart, bool isHistory)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                if (haveStart)
                {
                    checker.FirstAudio = false;
                    checker.played = false;
                }

                sides side = sides.White;
                bool kick = false;
                if (haveStart)
                {
                    for (int i = 0; i < columnStart.Place.Count; i++)
                    {
                        if (columnStart.Place[i].Checker == checker)
                        {
                            if (columnStart.Side != side) side = sides.Black;
                            columnStart.Place[i].Checker = null;
                            columnStart.Place[i].Full = false;
                            if (!columnStart.isFinish) UpdateColumnPlacesHeight(columnStart, false);
                            break;
                        }
                    }
                }

                for (int i = 0; i < columnTarget.Place.Count; i++)
                {
                    if (!columnTarget.Place[i].Full)
                    {
                        if (columnTarget.Place[i] != columnTarget.Place[0] && side != columnTarget.Side && columnTarget.Side != sides.none)
                        {
                            KickChecker(columnTarget.Place[i - 1], columnTarget);

                            checker.kickplayed = false;
                            kick = true;
                            i -= 1;
                        }

                        if (!isHistory)
                            checker.multipy = 1.2f;
                        else
                            checker.multipy = 4;

                        columnTarget.Place[i].Checker = checker;
                        checker.columnStart = columnTarget;
                        checker.FixTarget = columnTarget.Place[i].Pos;
                        checker.isDragging = false;
                        checker.canDrag = false;
                        checker.Fixing = true;
                        columnTarget.Place[i].Full = true;
                        checker.SetLayerOrder("place");
                        if (!columnTarget.isFinish) UpdateColumnPlacesHeight(columnTarget, true);

                        CheckCheckersPoint();

                        if (kick)
                        {
                            StartCoroutine(setCheckerCanFix(checker));
                            kick = false;
                        }

                        return;
                    }

                    if (i == columnTarget.Place.Count - 1) Debug.Log(columnTarget + ": is Full !");
                }

            }
            catch (Exception error) { Debug.Log(error); }
        });

    }
    #endregion

    void UpdateColumnPlacesHeight(ColumnClass column, bool add)
    {
        float newPosY = column.placeDistanceY;
        float compressValue = 0.05f;

        if (add)
            newPosY -= compressValue;
        else
            newPosY += compressValue;

        Vector2 pos = column.Place[0].Pos;
        
        for (int i = 1; i < 15; i++)
        {
            if (column.ID < 12)
                pos.y += newPosY;
            else
                pos.y -= newPosY;

            column.Place[i].Pos = pos;
            if (column.Place[i].Checker != null) column.Place[i].Checker.FixTarget = column.Place[i].Pos;
        }

        column.placeDistanceY = newPosY;
    }

    void CheckCheckersPoint()
    {
        int playerPoint = 0;
        for (int i = 0; i < 15; i++)
        {
            playerPoint += Checkers[i].columnStart.ID + 1;
        }
        
        int opponentPoint = 0;
        for (int i = 15; i < 30; i++)
        {
            opponentPoint += Mathf.Abs(Checkers[i].columnStart.ID - 24);
        }

        playerCheckersPoint = playerPoint;
        opponentCheckersPoint = opponentPoint;
    }

    #region Starting checkers
    public IEnumerator startingCheckers(float delay)
    {
        int i = 29;
        while (true)
        {
            yield return new WaitForSeconds(delay * Time.deltaTime);

            if (i >= 0)
            {
                Checkers[i].multipy = 2;
                Checkers[i].canFix = true;
                Checkers[i].transform.position = Checkers[i].FixTarget;
            }
            else
                break;

            i -= 1;
        }
    }
    #endregion

    #region Set checker CanFix
    IEnumerator setCheckerCanFix(CheckerController checker)
    {
        yield return new WaitForSeconds(25 * Time.deltaTime);
        while (checker.distanceToFix > 1.1f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        kickedChecker.multipy = 2.2f;
        kickedChecker.canFix = true;

        GameObject kickVFX = Instantiate(Vfx.Kick, Camera.main.WorldToScreenPoint(checker.transform.position), Quaternion.identity, canvas.transform);
        yield return new WaitForSeconds(50 * Time.deltaTime);
        Destroy(kickVFX);
    }
    #endregion

    #region Kick Checker
    void KickChecker(PlaceClass place,ColumnClass startColumn)
    {
        ColumnClass column = outColumn[0];
        if ((place.Checker.Side == CheckerController.side.Black && GeneralOptions.playerSide == sides.Black) || (place.Checker.Side == CheckerController.side.White && GeneralOptions.playerSide == sides.White))
        {
            if (GeneralOptions.PlayerLocation == Location.Down)
                column = outColumn[0];
            else
                column = outColumn[1];
        }
        else
        {
            if (GeneralOptions.PlayerLocation == Location.Down)
                column = outColumn[1];
            else
                column = outColumn[0];
        }

        for (int j = 0; j < column.Place.Count; j++)
        {
            if (!column.Place[j].Full)
            {
                column.Place[j].Checker = place.Checker;
                //column.Place[j].Checker.SetLayerTime("OnTop", "Default", 50); // itsnew
                place.Checker = null;
                place.Full = false;
                column.Place[j].Checker.columnStart = column;
                column.Place[j].Checker.FixTarget = column.Place[j].Pos;
                column.Place[j].Checker.isDragging = false;
                column.Place[j].Checker.canDrag = false;
                column.Place[j].Checker.canFix = false;
                column.Place[j].Checker.Fixing = true;
                column.Place[j].Full = true;
                kickedChecker = column.Place[j].Checker;

                //Add Kick information to History
                if (History.Count > 0)
                {
                    HistoryClass history = History[History.Count - 1];
                    history.haveKick = true;
                    history.checkerKick = column.Place[j].Checker;
                    history.startColumnKick = startColumn;
                    history.targetColumnKick = column;
                }

                return;
            }
        }
    }
    #endregion

    #region Reset Checkers
    public void ResetCheckers()
    {
        foreach (CheckerController checker in Checkers)
        {
            checker.columnTarget.Clear();
            checker.canDrag = false;
        }
    }
    #endregion

    #region Check Player Blocked
    void CheckPlayerBlocked()
    {
        if (CheckBlock() && Roll.playerTurn && !Roll.playerProcessing && Roll.playerAction && History.Count == 0)
        {
            StartCoroutine(SetBlockedUI("BLOCKED", "مسدود شد", "تم الحظر"));
            Roll.doneButton.playerDone(false);
            Debug.Log("Blocked !");
        }
    }
    #endregion

    #region Check Have Kick
    bool CheckHaveKick(ColumnClass outcolumn, sides side)
    {
        for (int i = 0; i < outcolumn.Place.Count; i++)
        {
            if (outcolumn.Place[i].Full)
                return true;
        }

        return false;
    }
    #endregion

    #region Reset Column HighLight
    public void ResetColumnHighLight()
    {
        foreach (ColumnClass column in Column)
            column.HighLight = false;

        foreach (ColumnClass column in finishColumn)
            column.HighLight = false;
    }
    #endregion

    #region Get Last Checker
    public CheckerController GetLastChecker(ColumnClass column, sides side)
    {
        if (column.FullCount > 0 && column.Side == side)
        {
            for (int i = 14; i >= 0; i--)
            {
                if (column.Place[i].Full)
                {
                    return column.Place[i].Checker;
                }
            }
        }

        return null;
    }
    #endregion

    #region Usable Checker
    public IEnumerator UsableCheckerDelay(float delay, Location location, sides side, List<int> roll)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        UsableChecker(location, side, roll);
    }

    void ResetBlockedRoll()
    {
        foreach(ColumnClass column in Column)
        {
            column.BlockedRoll = 0;
        }

        foreach (ColumnClass column in outColumn)
        {
            column.BlockedRoll = 0;
        }

        foreach (ColumnClass column in finishColumn)
        {
            column.BlockedRoll = 0;
        }
    }

    public void UsableChecker(Location location, sides side, List<int> roll)
    {
        ResetColumnHighLight();
        ResetCheckers();
        ResetBlockedRoll();

        // Find OutColumn
        ColumnClass outcolumn = outColumn[0];
        if (GeneralOptions.playerSide == side)
        {
            if (GeneralOptions.PlayerLocation == Location.Up)
                outcolumn = outColumn[1];
        } else
        {
            if (GeneralOptions.PlayerLocation == Location.Down)
                outcolumn = outColumn[1];
        }

        List<CheckerController> LastCheckers = new List<CheckerController>();

        if (!CheckHaveKick(outcolumn, side)) // NotHave Kick Checker
        {
            foreach(ColumnClass column in Column)
            {
                if (GetLastChecker(column, side) != null && !LastCheckers.Contains(GetLastChecker(column, side)))
                    LastCheckers.Add(GetLastChecker(column, side));
            }
        }
        else // Have Kick Checker
        {
            for (int i = 14; i >= 0; i--)
            {
                if (outcolumn.Place[i].Full && !LastCheckers.Contains(outcolumn.Place[i].Checker))
                    LastCheckers.Add(outcolumn.Place[i].Checker);
            }
        }

        ResetCheckers();
        CheckFinishColumn(side, roll);

        foreach (CheckerController checker in LastCheckers)
        {
            CheckMovableCloumns(checker, roll, side, location);
            if (GeneralOptions.playerSide == side && checker.columnTarget.Count > 0)
                checker.canDrag = true;
        }

        Roll.playerAction = GeneralOptions.playerSide == side;
        Roll.opponentAction = GeneralOptions.OpponentSide == side;

        // Fix
        bool haveUsable = false;
        for (int i = 0; i < Checkers.Length; i++)
            if (Checkers[i].canDrag) { haveUsable = true; break; }
        if (!haveUsable)
            if (Roll.playerTurn) playerDone(true);
    }
    #endregion

    #region Player Done
    public void playerDone(bool done)
    {
        canDone = done;

        if (done)
            ClearColumnHighLight();
    }
    #endregion

    #region Check Movable Columns
    void CheckMovableCloumns(CheckerController checker, List<int> roll, sides side, Location location)
    {
        kickblock = false;

        int targetID;
        for (int n1 = 0; n1 < Roll.Rolls.Count; n1++)
        {
            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1];
            else
            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1];
            else break;

            bool columnCheck = ColumnCheck(targetID, side);
            if (!columnCheck)
            {
                if (targetID >= 0 && targetID <= 23)
                {
                    checker.columnStart.BlockedRoll = roll[n1];
                }
            }

            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]))
            {
                kickblock = CheckKickBlock(targetID, side);
                if (kickblock)
                {
                    if (targetID >= 0 && targetID <= 23)
                    {
                        checker.columnStart.BlockedRoll = roll[n1];
                    }
                }

                checker.columnTarget.Add(Column[targetID]);
                if (Roll.Rolls.Count > 1)
                {
                    for (int n2 = 0; n2 < Roll.Rolls.Count; n2++)
                    {
                        if (n2 != n1)
                        {
                            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1] - roll[n2];
                            else
                            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1] + roll[n2];
                            else break;

                            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]) && !kickblock)
                            {
                                kickblock = CheckKickBlock(targetID, side);
                                checker.columnTarget.Add(Column[targetID]);

                                if (Roll.Rolls.Count > 2)
                                {
                                    for (int n3 = 0; n3 < Roll.Rolls.Count; n3++)
                                    {
                                        if (n3 != n1 && n3 != n2)
                                        {
                                            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1] - roll[n2] - roll[n3];
                                            else
                                            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1] + roll[n2] + roll[n3];
                                            else break;

                                            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]) && !kickblock)
                                            {
                                                kickblock = CheckKickBlock(targetID, side);
                                                checker.columnTarget.Add(Column[targetID]);

                                                if (Roll.Rolls.Count > 3)
                                                {
                                                    for (int n4 = 0; n4 < Roll.Rolls.Count; n4++)
                                                    {
                                                        if (n4 != n1 && n4 != n2 && n4 != n3)
                                                        {
                                                            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1] - roll[n2] - roll[n3] - roll[n4];
                                                            else
                                                            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1] + roll[n2] + roll[n3] + roll[n4];
                                                            else break;

                                                            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]) && !kickblock)
                                                            {
                                                                checker.columnTarget.Add(Column[targetID]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    bool CheckKickBlock(int targetID, sides side)
    {
        if (Column[targetID].Side != side && Column[targetID].Side != sides.none)
            return true;
        else
            return false;
    }

    bool ColumnCheck(int targetID, sides side)
    {
        bool maxCheck = targetID >= 0 && targetID < Column.Count;
        if (maxCheck)
        {
            if (Column[targetID].Side == side || Column[targetID].FullCount <= 1)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    #endregion

    #region Check Finish Column
    void CheckFinishColumn(sides side, List<int> roll)
    {
        // Player Home
        bool playerHome = GeneralOptions.playerSide == side && Roll.playerHome;
        bool playerDownHome = false;
        bool playerUpHome = false;

        if (playerHome)
        {
            playerDownHome = GeneralOptions.PlayerLocation == Location.Down;
            playerUpHome = GeneralOptions.PlayerLocation == Location.Up;
        }

        bool playerCanHome = playerDownHome || playerUpHome;

        // Opponent Home
        bool opponentHome = GeneralOptions.OpponentSide == side && Roll.opponentHome;
        bool opponentDownHome = false;
        bool opponentUpHome = false;

        if (opponentHome)
        {
            opponentDownHome = GeneralOptions.OpponentLocation == Location.Down;
            opponentUpHome = GeneralOptions.OpponentLocation == Location.Up;
        }

        bool Down = playerDownHome || opponentDownHome;
        bool Up = playerUpHome || opponentUpHome;
        bool opponentCanHome = opponentDownHome || opponentUpHome;
        bool Home = playerCanHome || opponentCanHome;

        ColumnClass finishColumnDown = finishColumn[1];
        ColumnClass finishColumnUp = finishColumn[0];

        if (Home)
        {
            List<ColumnClass> homeColumn = new List<ColumnClass>();
            // Home Columns
            if (Down)
            {
                for (int i = 1; i <= 6; i++)
                    homeColumn.Add(Column[i - 1]);
            }
            else
            if (Up)
            {
                for (int i = 1; i <= 6; i++)
                    homeColumn.Add(Column[24 - i]);
            }

            // Last Checkers of Home Columns
            List<CheckerController> homeLastCheckers = new List<CheckerController>();
            CheckerController lastChecker = null;

            for (int j = 0; j < homeColumn.Count; j++)
            {
                lastChecker = GetLastchecker(homeColumn[j], side);

                if (lastChecker != null && !homeLastCheckers.Contains(lastChecker))
                {
                    homeLastCheckers.Add(lastChecker);
                    lastChecker = null;
                }
            }

            // check for Add FinishColumn to Checker
            if (homeLastCheckers.Count > 0)
            {
                foreach (CheckerController checker in homeLastCheckers)
                {
                    int targetID = checker.columnStart.ID;
                    if (Up) targetID = Mathf.Abs(checker.columnStart.ID - 23);

                    for (int n1 = 0; n1 < roll.Count; n1++)
                    {
                        
                        if (targetID == (roll[n1] - 1))
                        {
                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                checker.columnTarget.Add(finishColumnDown);
                            else
                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                checker.columnTarget.Add(finishColumnUp);
                        }
                        else
                        {
                            if (Roll.Rolls.Count > 1)
                            {
                                for (int n2 = 0; n2 < roll.Count; n2++)
                                {
                                    if (n1 != n2)
                                    {
                                        if (targetID == (roll[n1] + roll[n2]) - 1)
                                        {
                                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                                checker.columnTarget.Add(finishColumnDown);
                                            else
                                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                                checker.columnTarget.Add(finishColumnUp);
                                        }
                                        else
                                        {
                                            if (Roll.Rolls.Count > 2)
                                            {
                                                for (int n3 = 0; n3 < roll.Count; n3++)
                                                {
                                                    if (n3 != n1 && n3 != n2)
                                                    {
                                                        if (targetID == (roll[n1] + roll[n2] + roll[n3]) - 1)
                                                        {
                                                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                                                checker.columnTarget.Add(finishColumnDown);
                                                            else
                                                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                                                checker.columnTarget.Add(finishColumnUp);
                                                        }
                                                        else
                                                        {
                                                            if (Roll.Rolls.Count > 3)
                                                            {
                                                                for (int n4 = 0; n4 < roll.Count; n4++)
                                                                {
                                                                    if (n4 != n1 && n4 != n2 && n4 != n3)
                                                                    {
                                                                        if (targetID == (roll[n1] + roll[n2] + roll[n3] + roll[n4]) - 1)
                                                                        {
                                                                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                                                                checker.columnTarget.Add(finishColumnDown);
                                                                            else
                                                                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                                                                checker.columnTarget.Add(finishColumnUp);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int maxRoll = FindMaxRoll(roll);
            bool empity = true;

            for (int k = 0; k < homeColumn.Count; k++)
            {
                if ((homeColumn[k].ID >= maxRoll - 1 && Down) || (Mathf.Abs(homeColumn[k].ID - 23) >= maxRoll - 1 && Up))
                {
                    if (homeColumn[k].FullCount > 0 && homeColumn[k].Side == side)
                        empity = false;
                }
            }

            if (empity)
            {
                for (int l = 2; l <= 6; l++)
                {
                    if (maxRoll - l >= 0)
                    {
                        if (homeColumn[maxRoll - l].FullCount > 0 && homeColumn[maxRoll - l].Side == side)
                        {
                            CheckerController checker = GetLastchecker(homeColumn[maxRoll - l], side);

                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                checker.columnTarget.Add(finishColumnDown);
                            else
                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                checker.columnTarget.Add(finishColumnUp);

                            break;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Find Max Roll
    public int FindMaxRoll(List<int> roll)
    {
        int MaxRoll = 0;

        MaxRoll = roll[0];

        if (roll.Count > 1)
        {
            if (roll[1] > MaxRoll)
                MaxRoll = roll[1];

            if (roll.Count > 2)
            {
                if (roll[2] > MaxRoll)
                    MaxRoll = roll[2];

                if (roll.Count > 3)
                {
                    if (roll[3] > MaxRoll)
                        MaxRoll = roll[3];
                }
            }
        }

        return MaxRoll;
    }
    #endregion

    #region Get Last Checker
    CheckerController GetLastchecker(ColumnClass column, sides side)
    {
        CheckerController lastChecker = null;
        if (column.FullCount > 0 && column.Side == side)
        {
            for (int i = 14; i >= 0; i--)
            {
                if (column.Place[i].Full && lastChecker == null)
                    lastChecker = column.Place[i].Checker;
            }
        }

        return lastChecker;
    }
    #endregion

    #region Draw Gizmos
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (GeneralOptions.showEditorPlace || GeneralOptions.showEditorColumn)
        {
            foreach(ColumnClass column in Column)
            {   
                if (GeneralOptions.showEditorPlace)
                {
                    foreach(PlaceClass palce in column.Place)
                    {
                        if (!palce.Full) Gizmos.color = new Color(1,0,0,0.8f); else Gizmos.color = new Color(0,1,0,0.8f);
                        Gizmos.DrawSphere(palce.Pos, 0.15f);
                    } 
                }

                if (GeneralOptions.showEditorColumn)
                {
                    Gizmos.color = new Color(0,0,1,0.3f);
                    Gizmos.DrawCube(column.Pos, GeneralOptions.rangeSizeColumn);
                }
            }
        }

        if (GeneralOptions.showEditorPlace)
        {
            foreach(ColumnClass column in outColumn)
            {
                foreach(PlaceClass palce in column.Place)
                {
                    if (!palce.Full) Gizmos.color = new Color(1,0,0,0.8f); else Gizmos.color = new Color(0,1,0,0.8f);
                    Gizmos.DrawSphere(palce.Pos, 0.15f);
                } 
            }

            foreach (ColumnClass column in finishColumn)
            {
                foreach (PlaceClass palce in column.Place)
                {
                    if (!palce.Full) Gizmos.color = new Color(1, 0, 0, 0.8f); else Gizmos.color = new Color(0, 1, 0, 0.8f);
                    Gizmos.DrawSphere(palce.Pos, 0.15f);
                }

                if (GeneralOptions.showEditorColumn)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.3f);
                    Gizmos.DrawCube(column.Pos + GeneralOptions.rangeDistance, GeneralOptions.rangeSizeFinishColumn);
                }
            }
        }
        
    }
#endif
    #endregion

    void SetHighlight()
    {
        float speed = 1.2f;
        
        foreach (ColumnClass column in Column)
        {
            Color color = column.highlightSprite.color;
            
            if (column.HighLight)
            {
                if (!column.highlightUp)
                    color.a += speed * Time.deltaTime;
                
                if (column.highlightUp)
                    color.a -= speed * Time.deltaTime;

                if (color.a <= 0.3f)
                    column.highlightUp = false;
                
                if (color.a >= 0.9f)
                    column.highlightUp = true;
            }
            else
            {
                color.a = 0;
            }
            
            column.highlightSprite.color = color;
        }
        
        foreach (ColumnClass column in finishColumn)
        {
            Color color = column.highlightSprite.color;
            
            if (column.HighLight)
            {
                if (!column.highlightUp)
                    color.a += speed * 0.4f * Time.deltaTime;
                
                if (column.highlightUp)
                    color.a -= speed * 0.4f * Time.deltaTime;

                if (color.a <= 0f)
                    column.highlightUp = false;
                
                if (color.a >= 0.3f)
                    column.highlightUp = true;
            }
            else
            {
                color.a = 0;
            }
            
            column.highlightSprite.color = color;
        }
    }

    #region Setup Columns
    void SetupColumns()
    {
        GameObject ColumnObjs = new GameObject("Column Objects");
        GameObject ColumnHighLights = new GameObject("Column HighLights");
        ColumnHighLights.transform.SetParent(ColumnObjs.transform);

        ColumnOptionsClass ColumnOption = Options.ColumnOption;

        for (int i=0;i<24;i++)
        {
            Column.Add(new ColumnClass());
            Column[i].ID = i;
            Column[i].Side = sides.none;
            Column[i].isFinish = false;
            Column[i].highlightSprite = art.place[i].transform.GetChild(0).GetComponent<SpriteRenderer>();

            // Set Direction
            if (i < 12)
                Column[i].Direction = direction.Up;
            else
                Column[i].Direction = direction.Down;

            Transform startSetup = Options.startSetup;

            // Set Position
            if (i == 0)
                Column[i].Pos = startSetup.position;
            else
            {
                Column[i].Pos = Column[i-1].Pos;

                if (i < 12 && i != 6)
                    Column[i].Pos.x -= ColumnOption.distanceX;
                else
                if (i == 6)
                    Column[i].Pos.x -= ColumnOption.middleDistance;
                else
                if (i == 12)
                    Column[i].Pos.y += ColumnOption.distanceZ;
                else
                if (i > 12 && i != 18)
                    Column[i].Pos.x += ColumnOption.distanceX;
                else
                if (i == 18)
                    Column[i].Pos.x += ColumnOption.middleDistance;
            }

            float dis = -0.3f;
            UnityEngine.Quaternion rot = GeneralOptions.HighLightPrefab.transform.rotation;
            if (i >= 12)
            {
                rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 180);
                dis = Mathf.Abs(dis);
            }

            GameObject highLight = Column[i].HighLightObject = Instantiate(GeneralOptions.HighLightPrefab,new Vector3(Column[i].Pos.x,Column[i].Pos.y,Column[i].Pos.z+dis),rot);
            highLight.transform.SetParent(ColumnHighLights.transform);
        }
    }
    #endregion

    #region Setup Places
    void SetupPlaces()
    {
        foreach(ColumnClass column in Column)
            SetupPlacesMain(column);

        foreach(ColumnClass column in outColumn)
            SetupPlacesMain(column);

        foreach (ColumnClass column in finishColumn)
            SetupPlacesFinish(column);
    }

    void SetupPlacesMain(ColumnClass column)
    {
        PlaceOptionsClass PlaceOptions = Options.PlaceOption;
        Vector2 pos = column.Pos;
        column.placeDistanceY = PlaceOptions.distanceY;

        for (int i = 0; i < 15; i++)
        {
            column.Place.Add(new PlaceClass());
            column.Place[i].Full = false;

            if (i == 0)
                column.Place[i].Pos = pos;
            else if (column.ID < 12)
                pos.y += PlaceOptions.distanceY;
            else
                pos.y -= PlaceOptions.distanceY;

            column.Place[i].Pos = pos;
        }
    }

    void SetupPlacesFinish(ColumnClass column)
    {
        for (int i = 0; i < 15; i++)
        {
            column.Place.Add(new PlaceClass());
            column.Place[i].Full = false;

            if (i == 0)
                column.Place[i].Pos = column.Pos;
            else
                column.Pos.y += Options.finishColumnOption.RowDistance;

            column.Place[i].Pos = column.Pos;
        }
    }

    void SetupNewRow(ColumnClass column,int startedPlaceID, PlaceOptionsClass PlaceOptions)
    {
        float RowDistance = Vector3.Distance(column.Place[startedPlaceID].Pos,column.Place[startedPlaceID+1].Pos)/2;
        column.Pos = column.Place[startedPlaceID].Pos;

        switch(column.Direction)
        {
            case direction.Up: column.Pos.y += RowDistance;  break;
            case direction.Down: column.Pos.y -= RowDistance;  break;
        }

        column.Pos.z -= PlaceOptions.distanceY;
    }
    #endregion

    #region show LOG
    /*void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (textLog != null)
        {
            textLog.text += "\n" + logString;

            if (textLog.text.Length > 480)
                textLog.text = "<color=red> ---------------- CONSOLE: ----------------</color>";
        }
    }*/
    #endregion
}