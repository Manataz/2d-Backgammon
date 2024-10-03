using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class RankController : MonoBehaviour
{
    [Header("General")]
    public bool rank;
    private bool loading;

    [Header("Canvas")]
    public CanvasGroup rankUI;
    public CanvasGroup loadingUI;
    
    [Header("Access")]
    public Image regionIconUI;
    public Image avatarUI;
    
    [Serializable]
    public class Top10Class
    {
        public GameObject obj;
        public Text nicknameTextUI;
        public Image regionImageUI;
        public Text coinTextUI;
    }
    
    public class PlayerListClass
    {
        public string nickname;
        public string region;
        public int coin;
    }

    public List<Top10Class> top10 = new List<Top10Class>();

    private Controller _controller;

    private void Start()
    {
        _controller = FindObjectOfType<Controller>();

        DisableItems();
    }

    private void Update()
    {
        _controller.SetCanvas(rankUI, rank, 0, 1);
        _controller.SetCanvas(loadingUI, loading, 0, 1);
    }

    public void UpdateData()
    {
        loading = true;
        GetTopCoinPlayers();
    }

    void SetTop10(List<PlayerListClass> list)
    {
        for (int i = 0; i < top10.Count; i++)
        {
            if (i < list.Count)
            {
                top10[i].obj.SetActive(true);
                top10[i].nicknameTextUI.text = list[i].nickname;
                top10[i].regionImageUI.sprite = _controller.GetRegionIcon(list[i].region);
                top10[i].coinTextUI.text = list[i].coin.ToString();
            }
            else
            {
                top10[i].obj.SetActive(false);
            }
        }

        loading = false;
    }

    public void ButtonClick(string type)
    {
        _controller.clickAudio.Play();
        
        switch (type)
        {
            case "back":
            {
                rank = false;

                DisableItems();
            }
                break;
        }
    }

    void DisableItems()
    {
        for (int i = 0; i < top10.Count; i++)
            top10[i].obj.SetActive(false);
    }

    async void GetTopCoinPlayers()
    {
        var response = await LeaderboardsService.Instance.GetScoresAsync("general", new GetScoresOptions { Limit = 10 , IncludeMetadata = true });
        List<PlayerListClass> playerList = new List<PlayerListClass>();
        
        foreach (var entry in response.Results)
        {
            if (!string.IsNullOrEmpty(entry.Metadata))
            {
                var metadata = JObject.Parse(entry.Metadata);
                
                string nickname = metadata["nickname"]?.ToString() ?? "Unknown";
                string region = metadata["flag"]?.ToString() ?? "Unknown";

                PlayerListClass player= new PlayerListClass();
                player.coin = int.Parse(entry.Score.ToString());
                player.nickname = nickname;
                player.region = region;
                playerList.Add(player);
                
                //Debug.Log("playerToken: " + entry.PlayerId + ", coin: " + entry.Score + ", nickname: " + nickname + ", region: " + region);
            }
            else
            {
                //Debug.Log("playerToken: " + entry.PlayerId + ", coin: " + entry.Score + ", No metadata found.");
            }
        }

        if (playerList.Count > 0)
        {
            SetTop10(playerList);
        }
        
        Debug.Log("<color=yellow>Get Leaderboard: </color><color=white>" + "general" + "</color><color=red> (players: " + playerList.Count +")</color>");
    }
}
