using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoneButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    GameSetup GameSetup;
    PauseController pauseController;
    private OnlineGameServer _onlineGameServer;
    
    public Image image;
    public GameObject processingObj;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        image = transform.GetComponent<Image>();
        pauseController = FindObjectOfType<PauseController>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();

        socketManager = SocketManager.Instance;
    }

    public void ButtonClick()
    {
        if (GameSetup.canDone && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI))
        {
            GameSetup.Audio.Click.Play();
            StartCoroutine(playerDone(true));
        }
    }

    public IEnumerator playerDone(bool verifyMove)
    {
        // Local
        if (GameSetup.GeneralOptions.AI)
        {
            GameSetup.EndTurn("player", GameSetup.GeneralOptions.delayTurn);
        }
        else
        // Online
        if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
        {
            GameSetup.doneProcessing_new = true;
            GameSetup.doneProcessing = true;

            if (GameSetup.History.Count > 0 && verifyMove)
            {
                foreach (GameSetup.HistoryClass history in GameSetup.History)
                {
                    if (history.verify)
                    {
                        yield return new WaitForSeconds(1);
                        GameSetup.VerifyMove(history);
                    }
                }
            }
            else
            {
                GameSetup.DoneOnline();
            }
        }

        GameSetup.canDone = false;
    }
}
