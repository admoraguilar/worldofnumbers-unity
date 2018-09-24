using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using WEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class GameSerializationController : CustomMonoBehaviour {
    private GameData gameData = new GameData();
    private SettingsData settingsData = new SettingsData();
    private bool IsSavedBeforeQuitting = false;
    private string gameDataKey = "GameData";
    private string settingsDataKey = "SettingsData";


    private void LoadData() {
        Backend.LoadData(settingsDataKey, typeof(SettingsData), BackendDataType.Local,
                         (loadedData) => {
                             settingsData = (SettingsData)loadedData;

                             Settings.IsAudioOn.Value = settingsData.IsAudioOn.GetValueOrDefault();

                             Debugger.Log(DebuggerType.Log, "GameSerializationController: Setting loaded successfully.");
                         },
                         () => {
                             Settings.IsAudioOn.Value = true;

                             Debugger.Log(DebuggerType.Log, "GameSerializationController: No settings to load. Defaults loaded");
                         });

        Backend.LoadData(gameDataKey, typeof(GameData), BackendDataType.Automatic,
                         (loadedData) => {
                             gameData = (GameData)loadedData;

                             // Assign the data
                             foreach(var gameMode in Settings.GameModes) {
                                 if(gameData.GameModes.ContainsKey(gameMode.name)) {
                                     var gameModeData = gameData.GameModes[gameMode.name];
                                     gameMode.HiScore = gameModeData.HiScore.GetValueOrDefault();
                                 }
                             }

                             Game.AccumulatedScore = gameData.AccumulatedScore.GetValueOrDefault();
                             Settings.IsNewPlayer = gameData.IsNewPlayer.GetValueOrDefault();
                             Settings.RuntimeLoseCountToEnableAds = gameData.RuntimeLoseCount.GetValueOrDefault();

                             Debugger.Log(DebuggerType.Log, "GameSerializationController: Game data loaded successfully.");
                         },
                         () => {
                             // Assign the default data
                             foreach(var gameMode in Settings.GameModes) {
                                 gameMode.HiScore = 0;
                             }

                             Game.AccumulatedScore = 0;
                             Settings.IsNewPlayer = false;
                             Settings.IsSignedInLastTime = false;
                             Settings.RuntimeLoseCountToEnableAds = Settings.LoseCountToEnableAds;

                             Debugger.Log(DebuggerType.Log, "GameSerializationController: No game data to load. Defaults loaded.");
                         });
    }

    private void SaveData() {
        // Save game data
        Backend.SaveData(gameDataKey, gameData, BackendDataType.LocalAndCloud,
                         () => { Debugger.Log(DebuggerType.Log, "GameSerializationController: Local game data saved successfully."); },
                         () => { Debugger.Log(DebuggerType.Log, "GameSerializationController: Failed to save local game data."); });

        // Save settings data
        Backend.SaveData(settingsDataKey, settingsData, BackendDataType.Local,
                         () => { Debugger.Log(DebuggerType.Log, "GameSerializationController: Settings saved successfully."); },
                         () => { Debugger.Log(DebuggerType.Log, "GameSerializationController: Failed to save settings."); });
    }

    private void UpdateData() {
        // Update settings data
        settingsData.IsAudioOn = Settings.IsAudioOn.Value;

        // Update game data
        foreach(var gameMode in Settings.GameModes) {
            if(!gameData.GameModes.ContainsKey(gameMode.name)) {
                gameData.GameModes[gameMode.name] = new GameModeData();
            }
            gameData.GameModes[gameMode.name].HiScore = gameMode.HiScore;
        }

        gameData.DataTimestamp = DateTime.Now.Ticks;
        gameData.AccumulatedScore = Game.AccumulatedScore;
        gameData.IsNewPlayer = Settings.IsNewPlayer;
        gameData.RuntimeLoseCount = Settings.RuntimeLoseCountToEnableAds;
    }

    private void OnBackendRegistered() {
        // Save game data to a freshly created account
        SaveData();

        // Send hi-score because maybe this player has beaten it 
        // and it didn't go through the leaderboards for
        // he/she was offline
        Backend.SubmitScoreToLeaderboard(Game.GameMode.Value.LeaderboardEventCode, Game.GameMode.Value.HiScore, null, null);
    }

    private void OnBackendAuthenticated() {
        // Sync the data of this signed account
        LoadData();

        // Send hi-score because maybe this player has beaten it 
        // and it didn't go through the leaderboards for
        // he/she was offline
        Backend.SubmitScoreToLeaderboard(Game.GameMode.Value.LeaderboardEventCode, Game.GameMode.Value.HiScore, null, null);
    }

    private void OnBackendReset() {
        // Reset game data on backend reset
        foreach(var gameMode in Settings.GameModes) {
            if(gameData.GameModes.ContainsKey(gameMode.name)) {
                var gameModeData = gameData.GameModes[gameMode.name];
                gameMode.HiScore = 0;
            }
        }

        Game.AccumulatedScore = 0;
        Settings.IsNewPlayer = false;
        Settings.RuntimeLoseCountToEnableAds = 0;

        Backend.SaveData(gameDataKey, gameData, BackendDataType.Local,
                         () => { Debugger.Log(DebuggerType.Log, "GameSerializationController: Local game data saved successfully."); },
                         () => { Debugger.Log(DebuggerType.Log, "GameSerializationController: Failed to save local game data."); });
    }

    private void OnGameStateSet(GameStateType state) {
        switch(state) {
            case GameStateType.Results:
                // Send hi-score if player has currently beaten it
                if(Game.IsBeatHiScoreInCurrentGame) {
                    Backend.SubmitScoreToLeaderboard(Game.GameMode.Value.LeaderboardEventCode, Game.GameMode.Value.HiScore, null, null);
                }
                break;
        }
    }

    private void OnBackendSignedOut() {
        // Save game data before signing out
        SaveData();
    }

    protected override void OnEnable() {
        base.OnEnable();

        Backend.OnRegistered += OnBackendRegistered;
        Backend.OnAuthenticated += OnBackendAuthenticated;
        Backend.OnSignOut += OnBackendSignedOut;
        Backend.OnReset += OnBackendReset;
        Game.GameState.OnValueSet += OnGameStateSet;

        Application.wantsToQuit += () => {
            if(IsSavedBeforeQuitting) return true;

            // Delay quit to save before quitting
            var saveBeforeQuitAction = new ActionQueue("ClassicGameMode: SplashScreen");
            saveBeforeQuitAction.AddAction(() => {
                Debugger.Log(DebuggerType.Log, "Trying to save while quitting");
                UpdateData();
                SaveData();
                IsSavedBeforeQuitting = true;
            });
            saveBeforeQuitAction.AddAction(new WaitForEndOfFrame());
            saveBeforeQuitAction.AddAction(() => Application.Quit());
            saveBeforeQuitAction.Start();

            return false;
        };
    }

    protected override void OnDisable() {
        base.OnDisable();

        Backend.OnRegistered -= OnBackendRegistered;
        Backend.OnAuthenticated -= OnBackendAuthenticated;
        Backend.OnSignOut -= OnBackendSignedOut;
        Backend.OnReset -= OnBackendReset;
        Game.GameState.OnValueSet -= OnGameStateSet;
    }

    private void Start() {
        Backend.Initialize();
        LoadData();
    }

    private void Update() {
        UpdateData();
    }

    private void OnApplicationPause(bool pause) {
        Debugger.Log(DebuggerType.Log, string.Format("Pause: {0}", pause.ToString()));

        if(pause) {
            SaveData();
        }
    }

    private void OnApplicationFocus(bool focus) {
        Debugger.Log(DebuggerType.Log, string.Format("Focus: {0}", focus.ToString()));

#if UNITY_EDITOR
        if(!focus) {
            SaveData();
        }
#endif
    }

    /// <summary>
    /// Wrap the data we wanna save.
    /// </summary>
    [Serializable]
    public class GameData {
        public Dictionary<string, GameModeData> GameModes = new Dictionary<string, GameModeData>();
        public int? AccumulatedScore = 0;
        public int? RuntimeLoseCount = 0;
        public long? DataTimestamp = 0; 
        public bool? IsNewPlayer = true;
    }


    [Serializable]
    public class GameModeData {
        public int? HiScore;
    }

    [Serializable]
    public class SettingsData {
        public bool? IsAudioOn = true;
    }
}


#if UNITY_EDITOR

public class _PersistentDataHandler {
    /// <summary>
    /// We use the SEO here because this is initialization code.
    /// This is the new rule for using the Unity init codes
    /// Awake: References
    /// Start: Attaching delegates/listeners
    /// 
    /// GameInitSystem/Start: Call default values so that attached delegates on Start will be invoked
    ///                       properly
    /// </summary>
    [InitializeOnLoadMethod]
    private static void RunOnEditorLoad() {
        UtilitiesEditor.SetExecutionOrder(typeof(GameSerializationController), -10);
    }
}

#endif