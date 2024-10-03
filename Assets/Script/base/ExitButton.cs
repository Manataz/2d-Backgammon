using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    PopupController _popupController;
    GameSetup GameSetup;
    FinishController FinishCon;
    CanvasGroup CanvasGroup;
    private PlayerInfo _playerInfo;
    private OnlineGameServer _onlineGameServer;

    public string[] message;

    public bool surrenderProccessing;

    private string info;
    private bool updated;

    private void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        _popupController = FindObjectOfType<PopupController>();
        FinishCon = FindObjectOfType<FinishController>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _onlineGameServer = FindObjectOfType<OnlineGameServer>();
        CanvasGroup = GetComponent<CanvasGroup>();

        socketManager = SocketManager.Instance;
        CanvasGroup.alpha = 1;
    }

    private void Update()
    {
        if (GameSetup.Updated && !updated)
        {
            info = _playerInfo.PlayerData.resignText;
            updated = true;
        }

        CanvasGroup.interactable = GameSetup.AllReady;
    }

    public void ButtonClick()
    {
        if (GameSetup.AllReady)
        {
            string Me = message[0];
            _popupController.OpenPopUp(true, true, false, info, No, Yes, null);
        }
    }

    public void Yes()
    {
        if (GameSetup.AllReady)
        {
            // Local
            if (GameSetup.GeneralOptions.AI)
            {
                FinishCon.Finished = true;
            }
            // Online
            else if (GameSetup.GeneralOptions.Online && _onlineGameServer.socket.Connected)
            {
                var commandData = new
                {
                    name = "RESIGN",
                    playerColor = _onlineGameServer.gameInformation.color
                };
            
                _onlineGameServer.socket.EmitAsync("command", commandData);
                Debug.Log("<color=yellow> RESIGN: </color> <color=white> Processing... </color>");
            }

            _popupController.show = false;
        }
    }

    public void No()
    {
        if (GameSetup.AllReady)
        {
            _popupController.show = false;
        }
    }
}
