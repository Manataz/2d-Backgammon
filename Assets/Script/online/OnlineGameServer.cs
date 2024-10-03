using System;
using System.Collections;
using System.Threading.Tasks;
using SocketIOClient;
using UnityEngine;
using UnityEngine.UI;

public class OnlineGameServer : MonoBehaviour
{
    public static OnlineGameServer Instance { get; private set; }
    
    public SocketIO socket;
    public bool connected;
    public bool serverUrlUpdated;
    private string serverUrl = "";

    private SearchSystem _searchSystem;

    public GameInformationClass gameInformation;
    
    [System.Serializable]
    public class GameInformationClass
    {
        public bool online;
        public bool live;
        public string roomName;
        public bool playWithFriend;
        public int coin;
        public int startDoubleCount;
        public int rematchKey;
        public string color;
        public string opponentName;
        public int opponentCoin;
        public string opponentRegion;
        public int opponentAvatarID;
        public string opponentToken;
    }

    public FirstDiceClass firstDice;
    
    [System.Serializable]
    public class FirstDiceClass
    {
        public int playerDice;
        public int opponentDice;
    }

    public class messageClass
    {
        public string message;
    }
    
    public class GameOverClass
    {
        public string message;
        public int rematchKey;
    }

    public class firstDiceClass
    {
        public int dice1;
        public int dice2;
        public string starter;
    }
    
    public class RollDiceClass
    {
        public int dice1;
        public int dice2;
        public string color;
        public bool isBlocked;
    } 

    public class MoveClass
    {
        public bool success;
        public string message;
        public string from;
        public int to;
        public string playerColor;
    }

    public class EndTurnClass
    {
        public string message;
        public string completedPlayer;
    }

    public class AcceptDoubleClass
    {
        public string playerColor;
        public int doubleAmount;
    }

    private void Start()
    {
        _searchSystem = FindObjectOfType<SearchSystem>();
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetServerUrl(string url)
    {
        serverUrl = url;
        serverUrlUpdated = true;
        SocketConnect();
    }
    
    public async void SocketConnect()
    {
        InitializeSocket();
        RegisterSocketEvents();
        await ConnectToServer();
    }

    public async Task MatchMaking(int playerLevel) => await SendCommand("matchmaking", playerLevel);
    public async Task Move(int fromPosition, int toPosition, bool isBlack) => await SendMoveCommand(fromPosition, toPosition, isBlack);
    public async Task GetGameStats() => await SendCommand("GET_BOARD_STATUS");
    public async Task RequestFirstDiceRoll() => await SendCommand("FIRST_DICE");
    public async Task RollDice() => await SendCommand("REQUEST_ROLL_DICE");
    public async Task RequestDouble() => await SendCommand("REQUEST_DOUBLE");
    public async Task AcceptDouble() => await SendCommand("ACCEPT_DOUBLE");
    public async Task DenyDouble() => await SendCommand("DENY_DOUBLE");
    public async Task SetBoard() => await SendCommand("SET_BOARD");

    private void InitializeSocket()
    {
        socket = new SocketIOUnity(serverUrl, new SocketIOOptions());
    }

    private void RegisterSocketEvents()
    {
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("<color=blue>Socket: Connected</color>");
            connected = true;
            _searchSystem.SocketOn();
        };

        socket.On("REQUEST_ROLL_DICE", response => HandleResponse("REQUEST_ROLL_DICE", response));
        ///socket.On("GAME_JOINED", response => HandleResponse("GAME_JOINED", response));
        socket.On("GET_BOARD_STATUS", response => HandleResponse("GET_BOARD_STATUS", response));
        socket.On("MOVE", response => HandleResponse("MOVE", response));
        socket.On("FIRST_DICE", response => HandleFirstDiceResponse(response));
        socket.On("REQUEST_DOUBLE", response => HandleResponse("REQUEST_DOUBLE", response));
        socket.On("ACCEPT_DOUBLE", response => HandleResponse("ACCEPT_DOUBLE", response));
        socket.On("DENY_DOUBLE", response => HandleResponse("DENY_DOUBLE", response));
        
        if (_searchSystem != null)
            _searchSystem.SocketOn();
    }

    private async Task ConnectToServer()
    {
        Debug.Log("<color=blue>Socket: Connecting...</color>");
        try
        {
            await socket.ConnectAsync();
            Debug.Log("<color=blue>Socket: Connection successful</color>");
        }
        catch (Exception ex)
        {
            Debug.LogError($"<color=red>Socket: Connection failed - {ex.Message}</color>");
        }
    }

    private async Task SendMoveCommand(int fromPosition, int toPosition, bool isBlack)
    {
        try
        {
            string color = isBlack ? "BLACK" : "WHITE";

            var commandData = new
            {
                name = "MOVE",
                playerColor = color,
                from = fromPosition,
                to = toPosition
            };

            await socket.EmitAsync("command", commandData);
            Debug.Log("<color=blue>Socket: Move sent successfully</color>");
        }
        catch (Exception ex)
        {
            Debug.LogError($"<color=red>Socket: Command sending failed - {ex.Message}</color>");
        }
    }

    private async Task SendCommand(string commandName, object commandValue = null)
    {
        try
        {
            var commandData = new
            {
                name = commandName,
                value = commandValue
            };

            await socket.EmitAsync("command", commandData);
            string response = $"Socket: {commandName} sent successfully";
            Debug.Log(response);
        }
        catch (Exception ex)
        {
            Debug.LogError($"<color=red>Socket: {commandName} sending failed - {ex.Message}</color>");
        }
    }

    private void HandleResponse(string eventName, SocketIOResponse response)
    {
        string _response = $"<color=blue>{eventName} Response: {response} </color>";
        Debug.Log(_response);
    }

    private void HandleFirstDiceResponse(SocketIOResponse response)
    {
        // Handle the response from the server for the first dice roll
        Debug.Log($"<color=blue>First Dice Response: {response}</color>");
    }
}
