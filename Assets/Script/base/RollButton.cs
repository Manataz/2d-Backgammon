using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    GameSetup GameSetup;
    DoubleController DoubleController;
    PlayerInfo playerInfo;
    Image image;
    PauseController pauseController;
    private OnlineGameServer _onlineGameServer;

    public bool canRoll;
    public bool can;

    public GameObject diceIcon;
    
    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        DoubleController = FindObjectOfType<DoubleController>();
        playerInfo = FindObjectOfType<PlayerInfo>();
        image = transform.GetComponent<Image>();
        pauseController = FindObjectOfType<PauseController>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();

        socketManager = SocketManager.Instance;

        canRoll = false;
    }

    public void ButtonClick()
    {
        if (can)
        {
            GameSetup.Audio.Click.Play();
            StartCoroutine(PlayerRoll());
            canRoll = false;
        }
    }

    public IEnumerator PlayerRoll()
    {
        bool Rolled = false;
        bool FirstMove = false;

        int e1 = 0;
        int e2 = 0;

        bool isBlocked = false;

        // Local Roll
        if (GameSetup.GeneralOptions.AI)
        {
            if (!GameSetup.StartingRollDice)
            {
                GameSetup.GeneralOptions.DiceAnimConPlayer.EndShow();
                GameSetup.GeneralOptions.DiceAnimConOpponent.EndShow();

            }else
            {
                e1 = GameSetup.Roll.playerStartRoll;
                e2 = GameSetup.Roll.opponentStartRoll;

                GameSetup.Roll.Rolls.Add(e1);
                GameSetup.Roll.Rolls.Add(e2);

                if (e1 == e2)
                {
                    GameSetup.Roll.Rolls.Add(e1);
                    GameSetup.Roll.Rolls.Add(e2);
                }
            }

            Rolled = true;
        }
        else
        // Online Roll
        if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
        {
            GameSetup.Roll.Rolls.Clear();

            GameSetup.GeneralOptions.DiceAnimConPlayer.EndShow();
            GameSetup.GeneralOptions.DiceAnimConOpponent.EndShow();

            _onlineGameServer.socket.On("REQUEST_ROLL_DICE", (response) =>
            {
                if (GameSetup.DeveloperMode)
                    Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> RollDice: (Player) </color> <color=white>" + response.ToString() + "</color>");

                var result = JsonConvert.DeserializeObject<List<OnlineGameServer.RollDiceClass>>(response.ToString());

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {

                    if (result[0].color == _onlineGameServer.gameInformation.color)
                    {
                        e1 = result[0].dice1;
                        e2 = result[0].dice2;

                        GameSetup.Roll.Rolls.Add(e1);
                        GameSetup.Roll.Rolls.Add(e2);

                        if (e1 == e2)
                        {
                            GameSetup.Roll.Rolls.Add(e1);
                            GameSetup.Roll.Rolls.Add(e2);
                        }

                        isBlocked = result[0].isBlocked;

                        Debug.Log("<color=green> RollDice: (Player) </color> <color=white> Done </color>");
                        Rolled = true;
                    }
                });
            });
            
            /*e1 = UnityEngine.Random.Range(1,7);
            e2 = UnityEngine.Random.Range(1,7);*/
            
            var commandData = new
            {
                name = "REQUEST_ROLL_DICE",
                playerColor = _onlineGameServer.gameInformation.color
            };
            _onlineGameServer.socket.EmitAsync("command", commandData);
            Debug.Log("<color=yellow> RollDice (Player) </color> <color=white>  Processing... </color>");
        }
        
        if ((!FirstMove && GameSetup.GeneralOptions.Online) || (!GameSetup.StartingRollDice && GameSetup.GeneralOptions.AI))
        {
            while (!Rolled) { yield return new WaitForSeconds(Time.deltaTime); }
            float delay = 0.1f;
            if (GameSetup.StartingRollDice) delay = 30;
            GameSetup.StartingRollDice = false;

            print(isBlocked);
            StartCoroutine(GameSetup.GeneralOptions.DiceAnimConPlayer.RollDice(delay,(!GameSetup.GeneralOptions.AI && GameSetup.GeneralOptions.Online && isBlocked)));

        }else
        {
            StartCoroutine(GameSetup.GeneralOptions.DiceAnimConPlayer.StartTurn(50,false));
            GameSetup.StartingRollDice = true;
        }
    }

    void Update()
    {
        image.enabled = canRoll && !DoubleController.Requesting && !GameSetup.Roll.AutoRoll;
        diceIcon.SetActive(canRoll);
        can = canRoll && !DoubleController.Requesting && !socketManager.GameData.FirstMove && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI);

        if (GameSetup.Roll.AutoRoll && can)
            ButtonClick();
    }

    public int GetRandomNumber(int[] numbers)
    {
        int n = numbers.Length;
        for (int i = 0; i < n; i++)
        {
            int rnd = UnityEngine.Random.Range(i, n);
            int temp = numbers[i];
            numbers[i] = numbers[rnd];
            numbers[rnd] = temp;
        }
        return numbers[0];
    }
}
