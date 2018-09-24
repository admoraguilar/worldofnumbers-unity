using UnityEngine;
using Sirenix.OdinInspector;


public enum GameStateType {
    GameMenu,
    Game,
    Results,
    MainMenu,
    Leaderboards,
    SkinsMenu,
    Tutorial,
    SplashScreen,
    Credits
}


[CreateAssetMenu(menuName = "Scriptables/GameData")]
public class GameData : CustomScriptableObject {
    [Title("Runtime Data")]
    public int Score = 0;
    public int AccumulatedScore = 0;
    public int LifeCount = 0;
    public float TimerValue = 0f;
    public bool IsBeatHiScoreInCurrentGame = false;
    public bool IsRunningOutOfTime = false;
    public MonitoredVariable<GameStateType> GameState = new MonitoredVariable<GameStateType>(GameStateType.GameMenu);
    public MonitoredVariable<GameModeScriptable> GameMode = new MonitoredVariable<GameModeScriptable>();
    public MonitoredVariable<GameSkinScriptable> GameSkin = new MonitoredVariable<GameSkinScriptable>();
    public MonitoredVariable<int> TargetNumber = new MonitoredVariable<int>();
    public MonitoredVariable<int[]> InputNumbers = new MonitoredVariable<int[]>(new int[0]);


    protected override void Reset() {
        Score = 0;
        AccumulatedScore = 0;
        LifeCount = 0;
        TargetNumber = new MonitoredVariable<int>();
        InputNumbers = new MonitoredVariable<int[]>(new int[0]);
        TimerValue = 0;
        IsBeatHiScoreInCurrentGame = false;
        IsRunningOutOfTime = false;
        GameState = new MonitoredVariable<GameStateType>(GameStateType.GameMenu);
        GameMode = new MonitoredVariable<GameModeScriptable>(Settings.GameModes[0]);
        GameSkin = new MonitoredVariable<GameSkinScriptable>(Settings.GameSkins[0]); 
    }
}
