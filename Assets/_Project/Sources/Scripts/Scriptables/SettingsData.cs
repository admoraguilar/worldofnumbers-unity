using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Scriptables/SettingsData")]
public class SettingsData : CustomScriptableObject {
    [Title("Config")]
    public int TargetFramerate = 60;
    public int LoseCountToEnableAds = 5;
    public int AdsCurrencyCountPerEnabled = 2;
    public int MaxBoostersPerGame = 5;
    public float BackgroundSwapInterval = 2f;
    public float SplashScreenShowLength = 2f;
    public Sprite[] BackgroundSprites = new Sprite[0];
    public GameModeScriptable[] GameModes = new GameModeScriptable[0];
    public GameSkinScriptable[] GameSkins = new GameSkinScriptable[0];
    public string[] CreditsNames;
    public string[] TipsText;


    [Title("Runtime Data")]
    public int RuntimeLoseCountToEnableAds = 0;
    public int RuntimeAdsCurrencyCount = 0;
    public bool IsNewPlayer = true;
    public bool IsSignedInLastTime = false;
    public MonitoredVariable<bool> IsAudioOn = new MonitoredVariable<bool>(true);


    protected override void Reset() {
        RuntimeLoseCountToEnableAds = LoseCountToEnableAds;
        RuntimeAdsCurrencyCount = AdsCurrencyCountPerEnabled;
        IsNewPlayer = true;
        IsSignedInLastTime = false;
        IsAudioOn = new MonitoredVariable<bool>(true);
    }
}
