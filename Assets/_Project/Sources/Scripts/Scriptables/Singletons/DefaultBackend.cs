using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using WEngine;


/// <summary>
/// <para>This module handles connection to a backend service for saving and loading of cloud data.</para>
/// <para>It also handles saving and loading of some local data such as AppData.</para>
/// </summary>
[CreateAssetMenu(menuName = "Scriptables/Singleton/DefaultBackend")]
public class DefaultBackend : ABackendScriptable {
    public override UserInfo UserInfo { get { return userInfo; } }
    public override Dictionary<string, LeaderboardEntry[]> TopLeaderboardEntries { get { return topLeaderboardEntries; } }
    public override Dictionary<string, LeaderboardEntry> PlayerLeaderboardEntry { get { return playerLeaderboardEntry; } }
    public override string StatusInfo { get { return statusInfo; } }
    public override bool IsAvailable { get { return isAvailable; } }
    public override bool IsAuthenticated { get { return isAuthenticated; } }

    [Title("Strings")]
    public string StatusSignedIn = "SIGNED IN! WELCOME!";
    public string StatusSignedOut = "SIGNED OUT! SEE YOU AGAIN!";
    public string StatusNotValidEmailAddress = "EMAIL ADDRESS NOT VALID!";
    public string StatusNotValidDisplayName = "DISPLAY NAME NOT VALID!";
    public string StatusUserNameTaken = "USERNAME IS TAKEN!";
    public string StatusUserNotRecognized = "USER NOT RECOGNIZED!";
    public string StatusUserLocked = "USER TEMPORARILY LOCKED!";
    public string StatusRegistering = "REGISTERING...";
    public string StatusRegistrationSuccess = "REGISTRATION SUCCESS!";
    public string StatusRegistrationFailed = "REGISTRATION FAILED!";
    public string StatusAuthenticating = "SIGNING YOU IN...";
    public string StatusAuthenticationSuccess = "SIGN IN SUCCESS, ALL GOOD!";
    public string StatusAuthenticationFailed = "SIGN IN FAILED";

    [Title("Status")]
#pragma warning disable 0414
    [MultiLineProperty, SerializeField] protected string statusInfo;
    [SerializeField] protected bool isAvailable = true;
    [SerializeField] protected bool isAuthenticated;
#pragma warning restore 0414

    [Title("Debug")]
#pragma warning disable 0414
    [SerializeField] protected UserInfo userInfo = new UserInfo();
    [SerializeField] protected Dictionary<string, LeaderboardEntry[]> topLeaderboardEntries = new Dictionary<string, LeaderboardEntry[]>();
    [SerializeField] protected Dictionary<string, LeaderboardEntry> playerLeaderboardEntry = new Dictionary<string, LeaderboardEntry>();
    [SerializeField] protected Dictionary<string, string> savedData = new Dictionary<string, string>();
#pragma warning restore 0414

    protected string userInfoDataKey = "UserInfoData";


    public override void Initialize() {
        // Load previous user info here so that we can still have 
        // the user details even without internet
        LoadData(userInfoDataKey, typeof(UserInfo), BackendDataType.Local, 
            (loadedData) => {
                userInfo = ((UserInfo)loadedData).Copy();
            }, null);
    }

    public override void RegisterOrAuthenticate(string userName, string password, string displayName, Action onSuccess, Action onFailed) {
        // Check email
        if(!Utilities.IsEmailValid(userName)) {
            statusInfo = StatusNotValidEmailAddress;
            if(onFailed != null) onFailed.Invoke();
            return;
        }

        // Check display name
        if(displayName.Length <= 0) {
            statusInfo = StatusNotValidDisplayName;
            if(onFailed != null) onFailed.Invoke();
            return;
        }

        // Save user info if authentication is successful
        onSuccess += () => {
            SaveData(userInfoDataKey, UserInfo, BackendDataType.Local, null, null);
        };

        // Set user info
        userInfo.UserName = userName;
        userInfo.DisplayName = displayName;
        userInfo.IsSignedInLastTime = true;

        if(onSuccess != null) onSuccess.Invoke();
        _OnRegistered();
        _OnAuthenticated();
        isAuthenticated = true;

        statusInfo = StatusRegistrationSuccess;
        statusInfo = StatusAuthenticationSuccess;
    }

    public override void GetTopLeaderboardEntries(string leaderboardShortCode, int entryCount, Action<Dictionary<string, LeaderboardEntry[]>> onSuccess, Action onFail) {
        entryCount = 2;
        var entries = new LeaderboardEntry[2] {
            new LeaderboardEntry() {
                Rank = 1,
                Score = 10,
                DisplayName = "Player",
                City = "City",
                Country = "XX"
            },
            new LeaderboardEntry() {
                Rank = 2,
                Score = 5,
                DisplayName = "Player2",
                City = "City2",
                Country = "YY"
            }
        };

        topLeaderboardEntries[leaderboardShortCode] = entries;
        if(onSuccess != null) onSuccess.Invoke(topLeaderboardEntries);
    }

    public override void GetPlayerLeaderboardEntry(string[] leaderboardShortCode, Action<Dictionary<string, LeaderboardEntry>> onSuccess, Action onFail) {
        var entry = new LeaderboardEntry() {
            Rank = 1,
            Score = 10,
            DisplayName = "Player",
            City = "City",
            Country = "XX"
        };

        playerLeaderboardEntry[leaderboardShortCode[0]] = entry;
        if(onSuccess != null) onSuccess.Invoke(playerLeaderboardEntry);
    }

    public override void SubmitScoreToLeaderboard(string leaderboardEventCode, long score, Action onSuccess, Action onFail) {
        if(onSuccess != null) onSuccess.Invoke();
    }

    public override void LoadData(string key, Type type, BackendDataType dataType, Action<object> onSuccess, Action onFail) {
        switch(dataType) {
            default:
                // Load from a text file
                string localDataPath = "";

                localDataPath += Path.Combine(Application.persistentDataPath, string.Format("{0}.json", key));

                if(File.Exists(localDataPath)) {
                    savedData[key] = File.ReadAllText(localDataPath);

                    Debugger.Log(DebuggerType.Log, "File loaded: {0}", localDataPath);

                    // Construct type from JSON
                    string jsonData = "";
                    if(savedData.TryGetValue(key, out jsonData)) {
                        var obj = JsonConvert.DeserializeObject(jsonData, type);
                        if(obj != null) {
                            if(onSuccess != null) onSuccess(obj);
                            Debugger.Log(DebuggerType.Log, "Local data successfully loaded: {0}", jsonData);

                            
                        } else {
                            if(onFail != null) onFail();
                        }
                    } else {
                        if(onFail != null) onFail();
                    }
                }
                break;
        }
    }

    public override void SaveData(string key, object data, BackendDataType dataType, Action onSuccess, Action onFail) {
        switch(dataType) {
            default:
                // Serialize data to JSON
                savedData[key] = JsonConvert.SerializeObject(data, Formatting.Indented);

                Debugger.Log(DebuggerType.Log, "Local data successfully saved: {0}{1}", Environment.NewLine, savedData[key]);

                // Save to a text file
                string localDataPath = "";

                localDataPath += Path.Combine(Application.persistentDataPath, string.Format("{0}.json", key));

                File.WriteAllText(localDataPath, savedData[key]);
                Debugger.Log(DebuggerType.Log, "File saved: {0}", localDataPath);

                if(onSuccess != null) onSuccess();
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
    }

    protected override void Reset() {
        userInfo = new UserInfo();
        statusInfo = "";
        isAvailable = true;
        isAuthenticated = false;
        topLeaderboardEntries = new Dictionary<string, LeaderboardEntry[]>();
        playerLeaderboardEntry = new Dictionary<string, LeaderboardEntry>();
        savedData = new Dictionary<string, string>();
    }
}


public abstract class ABackendScriptable : CustomScriptableObject {
    public event Action OnAvailable = delegate { };
    public event Action OnAuthenticated = delegate { };
    public event Action OnRegistered = delegate { };
    public event Action OnSignOut = delegate { };
    public event Action OnReset = delegate { };

    protected void _OnAvailable() { if(OnAvailable != null) OnAvailable(); }
    protected void _OnAuthenticated() { if(OnAuthenticated != null) OnAuthenticated(); }
    protected void _OnRegistered() { if(OnRegistered != null) OnRegistered(); }
    protected void _OnSignOut() { if(OnSignOut != null) OnSignOut(); }
    protected void _OnReset() { if(OnReset != null) OnReset(); }

    abstract public UserInfo UserInfo { get; }
    abstract public Dictionary<string, LeaderboardEntry[]> TopLeaderboardEntries { get; }
    abstract public Dictionary<string, LeaderboardEntry> PlayerLeaderboardEntry { get; }
    abstract public string StatusInfo { get; }
    abstract public bool IsAvailable { get; }
    abstract public bool IsAuthenticated { get; }


    abstract public void Initialize();
    abstract public void RegisterOrAuthenticate(string userName, string password, string displayName, Action onSuccess, Action onFailed);
    abstract public void GetTopLeaderboardEntries(string leaderboardShortCode, int entryCount, Action<Dictionary<string, LeaderboardEntry[]>> onSuccess, Action onFail);
    abstract public void GetPlayerLeaderboardEntry(string[] leaderboardShortCode, Action<Dictionary<string, LeaderboardEntry>> onSuccess, Action onFail);
    abstract public void SubmitScoreToLeaderboard(string leaderboardEventCode, long score, Action onSuccess, Action onFail);
    abstract public void LoadData(string key, Type type, BackendDataType dataType, Action<object> onSuccess, Action onFail);
    abstract public void SaveData(string key, object data, BackendDataType dataType, Action onSuccess, Action onFail);
    abstract public void SignOut();
}


[Serializable]
public class UserInfo {
    public string UserName = "LocalUser";
    public string UserId = "LocalTest";
    public string Password = "LocalPassword";
    public string DisplayName = "LocalPlayer";
    public bool IsSignedInLastTime = false;
}


[Serializable]
public class LeaderboardEntry {
    public long Rank = 0;
    public long Score = 0;
    public string DisplayName = "Player";
    public string City = "City";
    public string Country = "XX";
}


public enum BackendDataType {
    Automatic,
    Local,
    Cloud,
    LocalAndCloud
}