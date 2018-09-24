using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using GameSparks.Core;
using WEngine;


[CreateAssetMenu(menuName = "Scriptables/Singleton/GamesparksBackend")]
public class GamesparksBackend : DefaultBackend {
    public override bool IsAvailable { get { return isAvailable = GS.Available; } }
    public override bool IsAuthenticated { get { return isAuthenticated = GS.Authenticated; } }

    [Title("GamesparksBackend: Config")]
    [SerializeField] private bool isAutoLogin;


    public override void RegisterOrAuthenticate(string userName, string password, string displayName, Action onSuccess, Action onFail) {
        statusInfo = StatusAuthenticating;

        // Check email
        if(!Utilities.IsEmailValid(userName)) {
            statusInfo = StatusNotValidEmailAddress;
            if(onFail != null) onFail.Invoke();
            return;
        }

        // Save user info if authentication is successful
        onSuccess += () => {
            SaveData("UserInfoData", userInfo, BackendDataType.Local, null, null);
        };

        Action Register = () => {
            // Check display name
            if(displayName.Length <= 0) {
                statusInfo = StatusNotValidDisplayName;
                if(onFail != null) onFail.Invoke();
                return;
            }

            new GameSparks.Api.Requests.RegistrationRequest()
            .SetUserName(userName)
            .SetPassword(password)
            .SetDisplayName(displayName)
            .Send(
                // Success response
                (GameSparks.Api.Responses.RegistrationResponse response) => {
                    Debugger.Log(DebuggerType.Log, "Player successfully registered: {0} | {1}", response.UserId, response.DisplayName);

                    // Set user info
                    userInfo.UserName = userName;
                    userInfo.UserId = response.UserId;
                    userInfo.DisplayName = response.DisplayName;
                    userInfo.IsSignedInLastTime = true;

                    // Change status
                    isAuthenticated = true;
                    statusInfo = StatusRegistrationSuccess;
                    statusInfo = StatusAuthenticationSuccess;
                    _OnRegistered();
                    if(onSuccess != null) onSuccess();
                },

                // Error response
                (GameSparks.Api.Responses.RegistrationResponse response) => {
                    Debugger.Log(DebuggerType.Warning, "Error registering player: {0} | Error code: {1}", response.UserId, response.Errors.JSON);
                    switch(response.Errors.GetString("USERNAME")) {
                        case "TAKEN":
                            statusInfo = StatusUserNameTaken;
                            break;
                    }

                    isAuthenticated = false;
                    statusInfo = StatusRegistrationFailed;
                    statusInfo = StatusAuthenticationFailed;
                    if(onFail != null) onFail();
                }
            );
        };

        Action Authenticate = () => {
            new GameSparks.Api.Requests.AuthenticationRequest()
            .SetUserName(userName)
            .SetPassword(password)
            .Send(
                // Success response
                (GameSparks.Api.Responses.AuthenticationResponse response) => {
                    Debugger.Log(DebuggerType.Log, "Player successfully authenticated: {0} | {1}", response.UserId, response.DisplayName);

                    // Set user info
                    userInfo.UserName = userName;
                    userInfo.UserId = response.UserId;
                    userInfo.DisplayName = response.DisplayName;
                    userInfo.IsSignedInLastTime = true;

                    // Change status
                    isAuthenticated = true;
                    statusInfo = StatusAuthenticationSuccess;
                    _OnAuthenticated();
                    if(onSuccess != null) onSuccess();
                },

                // Error response
                (GameSparks.Api.Responses.AuthenticationResponse response) => {
                    Debugger.Log(DebuggerType.Warning, "Error authenticating player: {0} | Error code: {1}", response.UserId, response.Errors.JSON);
                    switch(response.Errors.GetString("DETAILS")) {
                        case "UNRECOGNISED":
                            Debugger.Log(DebuggerType.Warning, "Account is unrecognized! Registering instead...");
                            statusInfo = StatusUserNotRecognized;

                            // Send registration request
                            Register();
                            break;
                        case "LOCKED":
                            statusInfo = StatusUserLocked;
                            break;
                    }

                    isAuthenticated = false;
                    statusInfo = StatusAuthenticationFailed;
                    if(onFail != null) onFail();
                }
            );
        };

        // Send authentication request
        Authenticate();
    }

    public override void GetTopLeaderboardEntries(string leaderboardShortCode, int entryCount, Action<Dictionary<string, LeaderboardEntry[]>> onSuccess, Action onFail) {
        new GameSparks.Api.Requests.LeaderboardDataRequest()
            .SetDontErrorOnNotSocial(false)
            .SetEntryCount(entryCount)
            .SetLeaderboardShortCode(leaderboardShortCode)
            .Send(
            (GameSparks.Api.Responses.LeaderboardDataResponse response) => {
                Debugger.Log(DebuggerType.Log, "Successfully retrieved {0} leaderboard entries.", response.LeaderboardShortCode);

                var leaderboardEntries = new List<LeaderboardEntry>();
                foreach(var entry in response.Data) {
                    leaderboardEntries.Add(new LeaderboardEntry() {
                        Rank = entry.Rank.GetValueOrDefault(),
                        Score = entry.GetNumberValue("SCORE").GetValueOrDefault(),
                        DisplayName = entry.UserName,
                        City = entry.City,
                        Country = entry.Country
                    });
                }

                topLeaderboardEntries[response.LeaderboardShortCode] = leaderboardEntries.ToArray();
                if(onSuccess != null) onSuccess(topLeaderboardEntries);
            },
            (GameSparks.Api.Responses.LeaderboardDataResponse response) => {
                Debugger.Log(DebuggerType.Log, "Error retrieving {0} leaderboard entry.", response.LeaderboardShortCode);
                if(onFail != null) onFail();
            });
    }

    public override void GetPlayerLeaderboardEntry(string[] leaderboardShortCodes, Action<Dictionary<string, LeaderboardEntry>> onSuccess, Action onFail) {
        var leaderboards = new List<string>(leaderboardShortCodes);

        new GameSparks.Api.Requests.GetLeaderboardEntriesRequest()
            .SetLeaderboards(leaderboards)
            .Send(
            (GameSparks.Api.Responses.GetLeaderboardEntriesResponse response) => {
                Debugger.Log(DebuggerType.Log, "Successfully retrieved player leaderboard entry. {0}", response.JSONData);

                foreach(var leaderboard in leaderboards) {
                    object data = null;
                    if(response.JSONData.TryGetValue(leaderboard, out data)) {
                        GSData entry = (GSData)data;

                        var leaderboardEntry = new LeaderboardEntry() {
                            Rank = entry.GetInt("rank").GetValueOrDefault(),
                            Score = entry.GetNumber("SCORE").GetValueOrDefault(),
                            DisplayName = entry.GetString("userName"),
                            City = entry.GetString("city"),
                            Country = entry.GetString("country")
                        };

                        playerLeaderboardEntry[leaderboard] = leaderboardEntry;
                    } else {
                        Debugger.Log(DebuggerType.Log, "Player leaderboard: NULL");
                    } 
                }

                if(onSuccess != null) onSuccess(playerLeaderboardEntry);
            },
            (GameSparks.Api.Responses.GetLeaderboardEntriesResponse response) => {
                Debugger.Log(DebuggerType.Log, "Error retrieving player leaderboard entry.");
                if(onFail != null) onFail();
            });
    }

    public override void SubmitScoreToLeaderboard(string leaderboardEventCode, long score, Action onSuccess, Action onFail) {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey(leaderboardEventCode)
            .SetEventAttribute("SCORE", score)
            .Send(
            (GameSparks.Api.Responses.LogEventResponse response) => {
                Debugger.Log(DebuggerType.Log, "Score submit to {0} successful!", leaderboardEventCode);
                if(onSuccess != null) onSuccess();
            },
            (GameSparks.Api.Responses.LogEventResponse response) => {
                Debugger.Log(DebuggerType.Log, "Failed to submit to {0} successful!", leaderboardEventCode);
                if(onFail != null) onFail();
            });
    }

    public override void LoadData(string key, Type type, BackendDataType dataType, Action<object> onSuccess, Action onFail) {
        if(dataType == BackendDataType.Automatic) {
            dataType = isAuthenticated ? BackendDataType.Cloud : BackendDataType.Local;
        }

        switch(dataType) {
            case BackendDataType.Cloud:
                new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("LOADJSONDATA_PLAYER")
                    .SetEventAttribute("KEY", key)
                    .Send(
                        // Success response
                        (GameSparks.Api.Responses.LogEventResponse response) => {
                            if(response.ScriptData.GetGSData(key) == null) {
                                Debugger.Log(DebuggerType.Log, "Cloud data successfully loaded but NULL.");
                                if(onFail != null) onFail();
                            } else {
                                string jsonData = response.ScriptData.GetGSData(key).GetGSData(key).JSON;
                                Debugger.Log(DebuggerType.Log, "Cloud data successfully loaded: {0}", jsonData);
                                if(onSuccess != null) onSuccess(JsonConvert.DeserializeObject(jsonData, type));
                            }
                        },

                        // Error response
                        (GameSparks.Api.Responses.LogEventResponse response) => {
                            Debugger.Log(DebuggerType.Warning, "Error loading cloud data: {response.Errors.JSON}", response.Errors.JSON);
                            if(onFail != null) onFail();
                        }
                    );
                break;
            case BackendDataType.Local:
                base.LoadData(key, type, dataType, onSuccess, onFail);
                break;
        }
    }

    public override void SaveData(string key, object data, BackendDataType dataType, Action onSuccess, Action onFail) {
        if(dataType == BackendDataType.Automatic) {
            dataType = isAuthenticated ? BackendDataType.Cloud : BackendDataType.Local;
        }

        Action CloudSave = () => {
            new GameSparks.Api.Requests.LogEventRequest()
                    .SetEventKey("SAVEJSONDATA_PLAYER")
                    .SetEventAttribute("KEY", key)
                    .SetEventAttribute("DATA", new GSRequestData().AddJSONStringAsObject(key, JsonConvert.SerializeObject(data)))
                    .Send(
                        // Success response
                        (GameSparks.Api.Responses.LogEventResponse response) => {
                            Debugger.Log(DebuggerType.Log, "Cloud data successfully saved: {0}", JsonConvert.SerializeObject(data, Formatting.Indented));
                            if(onSuccess != null) onSuccess();
                        },

                        // Error response
                        (GameSparks.Api.Responses.LogEventResponse response) => {
                            Debugger.Log(DebuggerType.Warning, "Error saving cloud data: {0}", response.Errors.JSON);
                            if(onFail != null) onFail();
                        }
                    );
        };

        switch(dataType) {
            case BackendDataType.Cloud:
                CloudSave();
                break;
            case BackendDataType.Local:
                base.SaveData(key, data, dataType, onSuccess, onFail);
                break;
            case BackendDataType.LocalAndCloud:
                base.SaveData(key, data, dataType, onSuccess, onFail);
                if(isAuthenticated) CloudSave();
                break;
        }
    }

    public override void SignOut() {
        _OnSignOut();
        isAuthenticated = false;
        statusInfo = StatusSignedOut;

        // Empty the user info data
        userInfo = new UserInfo();
        SaveData(userInfoDataKey, userInfo, BackendDataType.Local, null, null);

        // Reset gamesparks
        ActionTemplates.RunActionAfterSeconds("GamesparksBackend: GSReset", 1f, 
            () => {
                GS.Reset();
                _OnReset();
            }
        );
    }

    private void ShutdownGamesparks() {
        GS.Disconnect();
        GS.ShutDown();
    }

    protected override void OnEnable() {
        base.OnEnable();

        GS.GameSparksAvailable += (status) => {
            isAvailable = status;
        };

        // Logging out on game load prevents gamesparks
        // from auto-signing in, if you want auto-login
        // then comment this line
        if(!isAutoLogin) SignOut();
        Reset();
    }

    protected override void OnDisable() {
        base.OnDisable();

        ShutdownGamesparks();
        Reset();
    }

    protected override void Reset() {
        userInfo = new UserInfo();
        statusInfo = "";
        isAvailable = false;
        isAuthenticated = false;
        topLeaderboardEntries = new Dictionary<string, LeaderboardEntry[]>();
        playerLeaderboardEntry = new Dictionary<string, LeaderboardEntry>();
        savedData = new Dictionary<string, string>();
    }
}
