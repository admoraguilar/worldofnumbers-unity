using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Scriptables/BoosterData")]
public class BoosterData : ScriptableObject {
    [Title("Config")]
    public bool InstantApply;
    public bool GainedByWatchingAd;
    public int Price;

    [Title("Boosts")]
    public float Timer;
    public int Score;
    public int LifeCount;
    public int Accumulated;
}

