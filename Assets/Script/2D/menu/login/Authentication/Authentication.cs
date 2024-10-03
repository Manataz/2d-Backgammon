using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

public class Authentication : MonoBehaviour
{
    public string nickname;
    public int avatarId;
    public string state;
    public string refrralCode;
    public int[] birthdate = new []{-1,-1,-1};
    public int coin;
    
    private PlayerInfo _playerInfo;
    private SignController _signController;
    private Controller _controller;
    private SearchSystem _searchSystem;
    private TournamentController _tournamentController;

    void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _signController = FindObjectOfType<SignController>();
        _controller = FindObjectOfType<Controller>();
        _searchSystem = FindObjectOfType<SearchSystem>();
        _tournamentController = FindObjectOfType<TournamentController>();
    }

    public async void TrySignIn()
    {
        try
        {
            await UnityServices.InitializeAsync();
            
            if (_playerInfo.PlayerData.isGuest)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            SetupEvents();
            Debug.Log("<color=green> Unity Services Initialized </color>");
            
            if (_playerInfo.PlayerData.token != "")
            {
                await LoadDataAsync(_playerInfo.PlayerData.token);
                _signController.LoggedIn();
            }
            else
            {
                SignOut();
                _signController.signing = false;
                _signController.signUp = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during Unity Services initialization: " + ex);
        }
    }

    void SetupEvents()
    {
        var authService = AuthenticationService.Instance;
        authService.SignedIn += async () =>
        {
            Debug.Log("<color=orange> Signed in as: " + authService.PlayerId + "</color>");
            _playerInfo.PlayerData.token = authService.PlayerId;
            _playerInfo.SaveGame();
            await LoadDataAsync(authService.PlayerId);
        };
        authService.SignInFailed += error =>
        {
            _signController.signing = false;
            Debug.LogError("Sign-in failed: " + error);
            _signController.signUpError.SetActive(true);
        };
        authService.SignedOut += () => Debug.Log("<color=red> Signed out </color>");
    }

    public void SignOut()
    {
        try
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log("<color=red> Signed out successfully </color>");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    
    public async void DeleteAccount()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
            Debug.Log("<color=red> Account deleted successfully </color>");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    
    private async Task HandleAuthentication(Func<Task> authTask, string successMessage)
    {
        try
        {
            await authTask();
            Debug.Log(successMessage);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    #region Save && Load Data
    public async Task SaveDataAsync(string userId,int coin, string region, string state, string enteredReferral, int[] birthdate, string nickname, int reserved, int premium, int avatarId, int tournament, string tournamentId, string phone, string address, int winCount, int loseCount, string premiumTime)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("User is not signed in.");
                return;
            }

            var data = new Dictionary<string, object>
            {
                { "userId", userId },
                { "coin", coin },
                { "region", region },
                { "state", state },
                { "enteredReferral", enteredReferral },
                { "birthdate", birthdate },
                { "nickname", nickname },
                { "reserved", reserved },
                { "premium", premium },
                { "avatarId", avatarId },
                { "tournament", tournament },
                { "tournamentId", tournamentId },
                { "phone", phone },
                { "address", address },
                { "winCount", winCount },
                { "loseCount", loseCount },
                { "premiumTime", premiumTime }
            };
            
            UpdateLeaderboard(nickname, region);
            
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("<color=green> Data saved successfully </color>");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    
    public async void UpdateLeaderboard(string nickname, string region)
    {
        var metadata = new Dictionary<string, string>
        {
            { "nickname", nickname },
            { "flag", region }
        };
        
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("general", 2000, new AddPlayerScoreOptions { Metadata = metadata });
        //Debug.Log(JsonConvert.SerializeObject(playerEntry));
        
        Debug.Log("<color=yellow>Leaderboard Updated: </color>" + "general");
    }
    
    public async Task LoadDataAsync(string userId)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("Sign-in failed. Unable to load data.");
                SignOut();
                _signController.signing = false;
                _signController.signUp = true;
                return;
            }
            
            var data = await CloudSaveService.Instance.Data.LoadAllAsync();

            if (data.TryGetValue("userId", out var savedUserId) && savedUserId.ToString() == userId)
            {
                if (data.TryGetValue("userId", out var _userId))
                {
                    _playerInfo.PlayerData.token = _userId;
                    Debug.Log("<color=orange> Loaded (userId): </color> <color=white>" + _userId + "</color>");
                }else
                    Debug.LogError("No data found for (userId)");
                
                
                if (data.TryGetValue("coin", out var coin))
                {
                    _playerInfo.PlayerData.coin = int.Parse(coin);
                    Debug.Log("<color=orange> Loaded (coin): </color> <color=white>" + coin + "</color>");
                }else
                    Debug.LogError("No data found for (coin)");

                if (data.TryGetValue("region", out var region))
                {
                    _playerInfo.PlayerData.region = region;
                    Debug.Log("<color=orange> Loaded (region): </color> <color=white>" + region + "</color>");
                }else
                    Debug.LogError("No data found for (region)");
                
                if (data.TryGetValue("state", out var state))
                {
                    _playerInfo.PlayerData.state = state;
                    Debug.Log("<color=orange> Loaded (state): </color> <color=white>" + state + "</color>");
                }else
                    Debug.LogError("No data found for (state)");

                if (data.TryGetValue("enteredReferral", out var enteredReferral))
                {
                    _playerInfo.PlayerData.refrralCode = enteredReferral;
                    Debug.Log("<color=orange> Loaded (enteredReferral): </color> <color=white>" + enteredReferral + "</color>");
                }else
                    Debug.LogError("No data found for (enteredReferral)");

                if (data.TryGetValue("nickname", out var nickname))
                {
                    _playerInfo.PlayerData.nickname = nickname;
                    Debug.Log("<color=orange> Loaded (nickname): </color> <color=white>" + nickname + "</color>");
                }else
                    Debug.LogError("No data found for (nickname)");
                
                if (data.TryGetValue("birthdate", out var birthdate))
                {
                    _playerInfo.PlayerData.birthdate = _controller.ConvertStringToIntArray(birthdate);
                    Debug.Log("<color=orange> Loaded (birthdate): </color> <color=white>" + birthdate + "</color>");
                }else
                    Debug.LogError("No data found for (birthdate)");
                
                if (data.TryGetValue("reserved", out var reserved))
                {
                    _playerInfo.PlayerData.reserved = int.Parse(reserved);
                    Debug.Log("<color=orange> Loaded (reserved): </color> <color=white>" + reserved + "</color>");
                }else
                    Debug.LogError("No data found for (reserved)");
                
                if (data.TryGetValue("premium", out var premium))
                {
                    _playerInfo.PlayerData.premium = (int.Parse(premium) == 1) ? true : false;
                    Debug.Log("<color=orange> Loaded (premium): </color> <color=white>" + premium + "</color>");
                }else
                    Debug.LogError("No data found for (premium)");
                
                if (data.TryGetValue("avatarId", out var avatarId))
                {
                    _playerInfo.PlayerData.avatarId = int.Parse(avatarId);
                    Debug.Log("<color=orange> Loaded (avatarId): </color> <color=white>" + avatarId + "</color>");
                }else
                    Debug.LogError("No data found for (avatarId)");
                
                if (data.TryGetValue("tournament", out var tournament))
                {
                    _playerInfo.PlayerData.tournament = (int.Parse(tournament) == 1) ? true : false;
                    Debug.Log("<color=orange> Loaded (tournament): </color> <color=white>" + tournament + "</color>");
                }else
                    Debug.LogError("No data found for (tournament)");
                
                if (data.TryGetValue("tournamentId", out var tournamentId))
                {
                    _playerInfo.PlayerData.tournamentId = tournamentId;
                    Debug.Log("<color=orange> Loaded (tournamentId): </color> <color=white>" + tournamentId + "</color>");
                }else
                    Debug.LogError("No data found for (tournamentId)");
                
                if (data.TryGetValue("phone", out var phone))
                {
                    _playerInfo.PlayerData.phone = phone;
                    Debug.Log("<color=orange> Loaded (phone): </color> <color=white>" + phone + "</color>");
                }else
                    Debug.LogError("No data found for (phone)");
                
                if (data.TryGetValue("address", out var address))
                {
                    _playerInfo.PlayerData.address = address;
                    Debug.Log("<color=orange> Loaded (address): </color> <color=white>" + address + "</color>");
                }else
                    Debug.LogError("No data found for (address)");
                
                if (data.TryGetValue("winCount", out var winCount))
                {
                    _playerInfo.PlayerData.winCount = int.Parse(winCount);
                    Debug.Log("<color=orange> Loaded (winCount): </color> <color=white>" + winCount + "</color>");
                }else
                    Debug.LogError("No data found for (winCount)");
                
                if (data.TryGetValue("loseCount", out var loseCount))
                {
                    _playerInfo.PlayerData.loseCount = int.Parse(loseCount);
                    Debug.Log("<color=orange> Loaded (loseCount): </color> <color=white>" + loseCount + "</color>");
                }else
                    Debug.LogError("No data found for (loseCount)");
                
                if (data.TryGetValue("premiumTime", out var premiumTime))
                {
                    _playerInfo.PlayerData.premiumTime = premiumTime;
                    Debug.Log("<color=orange> Loaded (premiumTime): </color> <color=white>" + premiumTime + "</color>");
                }else
                    Debug.LogError("No data found for (premiumTime)");
            }
            else
            {
                Debug.LogError("No data found for this user");
                _playerInfo.SetDefault();
                _controller.UpdateUI();
            }

            #region Debug
            //_playerInfo.PlayerData.tournamentMatchWins = 2;
            //_playerInfo.PlayerData.tournamentLose = true;
            //_playerInfo.PlayerData.coin = 1000;
            //_playerInfo.PlayerData.premium = true;
            #endregion
            
            _signController.signing = false;
            _signController.signUp = false;
            
            _playerInfo.SaveGame();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    #endregion

    #region Sign As Guest
    public async void ClickSignInAsGuest()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            _playerInfo.PlayerData.nickname = nickname;
            _playerInfo.PlayerData.avatarId = avatarId;
            _playerInfo.PlayerData.state = state;
            _playerInfo.PlayerData.refrralCode = refrralCode;
            _playerInfo.PlayerData.birthdate = new []{birthdate[0],birthdate[1],birthdate[2]};
            _playerInfo.PlayerData.coin = coin;
            
            _signController.signing = false;
            _signController.signUp = false;
            _signController.LoggedIn();
        
            string playerId = AuthenticationService.Instance.PlayerId;
            _playerInfo.PlayerData.token = playerId;
            _playerInfo.PlayerData.isGuest = true;
            _playerInfo.SaveGame();
            print(playerId);
            await SaveDataAsync(playerId, coin, "", state, refrralCode, birthdate, nickname, 0, 0, avatarId, 0, "", "", "", 0, 0, "");
            
            Debug.Log("Signed in as guest");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    #endregion

    #region Sign With Google
    public async void ClickSignInWithGoogle()
    {
        try
        {
            var options = new SignInOptions { CreateAccount = true };
            await AuthenticationService.Instance.SignInWithGoogleAsync("google_id_token", options);
            Debug.Log("Signed in with Google");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    async Task SignInWithGoogle()
    {
        try
        {
            var options = new SignInOptions { CreateAccount = true };
            await AuthenticationService.Instance.SignInWithGoogleAsync("google_id_token", options);
            Debug.Log("Signed in with Google");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    #endregion
}
