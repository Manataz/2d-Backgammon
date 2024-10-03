using System.Collections;
using System.Collections.Generic;
using TmDice25D;
using UnityEngine;
using UnityEngine.UI;
using static CheckerController;

public class DiceAnimController : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    public enum own
    {
        player,
        opponent
    }

    public DiceRoll3DTex[] dice;

    public own Own;

    bool lightOn;

    public bool Roll;
    public bool Show;

    int roll1;
    int roll2;

    private GameSetup GameSetup;
    private OnlineGameServer _onlineGameServer;

    public bool Right;

    public bool Updated;

    public List<AudioSource> Sound;

    public AudioSource DoubleDiceAudio1;
    public AudioSource DoubleDiceAudio2;

    public Sprite[] dices;
    public Color diceNormalColor;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();

        socketManager = SocketManager.Instance;
        EndShow();
        
        dice[0].EndShow();
        dice[1].EndShow();
        dice[2].EndShow();
        dice[3].EndShow();
    }

    void Update()
    {
        if (!Updated && GameSetup.Updated)
        {
            Right = (Own == own.player && GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down) || (Own == own.opponent && GameSetup.GeneralOptions.OpponentLocation == GameSetup.Location.Down);
            Updated = true;
        }
    }

    public IEnumerator RollDice(float delay, bool isBlocked)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        
        // Local Roll
        if (GameSetup.GeneralOptions.AI)
        {
            GameSetup.Roll.Rolls.Clear();

            int e1 = 0;
            int e2 = 0;

            if (!GameSetup.StartingRollDice)
            {
                int[] roll = { 1, 2, 3, 4, 5, 6 };
                e1 = GetRandomNumber(roll);
                e2 = Random.Range(1, 7);
            }

            GameSetup.Roll.Rolls.Add(e1);
            GameSetup.Roll.Rolls.Add(e2);

            if (e1 == e2)
            {
                GameSetup.Roll.Rolls.Add(e1);
                GameSetup.Roll.Rolls.Add(e2);
            }

            Debug.Log("Local Roll: " + "e: (" + e1 + "," + e2 + ")");
        }

        roll1 = GameSetup.Roll.Rolls[0];
        roll2 = GameSetup.Roll.Rolls[1];
        
        dice[0].RollDice(GameSetup.Roll.Rolls[0]);
        dice[1].RollDice(GameSetup.Roll.Rolls[1]);

        StartCoroutine(StartTurn(100, isBlocked));

        Show = true;
        Roll = true;

        StartCoroutine(playSound());
    }

    public void StartRollDice()
    {
        if (!GameSetup.Roll.playerTurn && !GameSetup.Roll.opTurn)
        {
            int e = 0;

            // Local Roll
            if (GameSetup.GeneralOptions.AI)
            {
                GameSetup.Roll.Rolls.Clear();

                int[] roll = { 1, 2, 3, 4, 5, 6 };
                e = GetRandomNumber(roll);
            }
            else
            // Online Roll
            if (GameSetup.GeneralOptions.Online)
            {
                if (Own == own.player)
                    e = _onlineGameServer.firstDice.playerDice;
                else
                if (Own == own.opponent)
                    e = _onlineGameServer.firstDice.opponentDice;
            }

            if (Own == own.player)
                GameSetup.Roll.playerStartRoll = e;
            else
            if (Own == own.opponent)
                GameSetup.Roll.opponentStartRoll = e;

            roll1 = e;
            
            dice[0].RollDice(e);

            if (Own == own.player)
                StartCoroutine(GameSetup.checkStartRollDice(100));

            Show = true;
            Roll = true;

            StartCoroutine(playSound());
        }
    }

    public IEnumerator playSound()
    {
        yield return new WaitForSeconds(1 * Time.deltaTime);
        int play = Random.Range(0, Sound.Count);
        Sound[play].Play();
        yield return new WaitForSeconds(2 * Time.deltaTime);
        if (play != 0) Sound[play-1].Play(); else Sound[Sound.Count-1].Play();
    }

    public int GetRandomNumber(int[] numbers)
    {
        int n = numbers.Length;
        for (int i = 0; i < n; i++)
        {
            int rnd = Random.Range(i, n);
            int temp = numbers[i];
            numbers[i] = numbers[rnd];
            numbers[rnd] = temp;
        }
        return numbers[0];
    }

    public void EndShow()
    {
        Show = false;
        Roll = false;
        
        dice[0].EndShow();
        dice[1].EndShow();
        dice[2].EndShow();
        dice[3].EndShow();
    }

    public IEnumerator StartTurn(float delay, bool isBlocked)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        
        dice[0].StopDice();
        dice[1].StopDice();
        
        if (dice[0].roll == dice[1].roll)
        {
            dice[2].SetRoll(dice[0].roll);
            dice[3].SetRoll(dice[0].roll);
        }
        
        StartCoroutine(playSound());

        if (Own == own.opponent)
        {
            StartCoroutine(GameSetup.SetRollUI(GameSetup.GeneralOptions.OpponentSide, GameSetup.Roll.Rolls));

            if (GameSetup.GeneralOptions.AI)
                StartCoroutine(GameSetup.AIAct(GameSetup.GeneralOptions.delayAIMove, "intelligent"));

            if (GameSetup.GeneralOptions.Online && _onlineGameServer.connected)
            {
                if (isBlocked)
                    GameSetup.BlockedOnline("opponent");
            }
        }
        else
        {
            GameSetup.UsableChecker(GameSetup.GeneralOptions.PlayerLocation, GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls);
            StartCoroutine(GameSetup.SetRollUI(GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls));

            if (GameSetup.GeneralOptions.Online && _onlineGameServer.connected)
            {
                if (isBlocked)
                    GameSetup.BlockedOnline("player");
            }
        }

        GameSetup.StartingRollDice = false;
    }
}
