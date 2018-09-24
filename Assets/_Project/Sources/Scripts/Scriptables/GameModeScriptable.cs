using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public abstract class GameModeScriptable : CustomScriptableObject {
    public int BoostersInEffectCount { get { return boostersInEffect.Count; } }

    [Title("Game Mode: Config")]
    public int ScoreEachSet = 2;
    public float PlayTimeLength = 60f;
    public float TimeBombStartTime = 10f;
    public string LeaderboardCode = "LEADERBOARD_";
    public string LeaderboardEventCode = "SUBMITSCORELEADERBOARD_";
    public BoosterData[] AvailableBoosters;

    [Title("Game Mode: Runtime Data")]
    public int HiScore;

    [Title("Game Mode: Debug")]
    [ShowInInspector] protected List<BoosterData> boostersInEffect = new List<BoosterData>();


    public void AddBoosters(BoosterData booster) {
        boostersInEffect.Add(booster);
    }

    protected void ResetPlay(int target, int divideCount, int maxNumbers, float minPercent, float maxPercent) {
        Game.TargetNumber.Value = target;
        Game.InputNumbers.Value = Utilities.GenerateRandomNumbers(
            Game.TargetNumber.Value,
            divideCount,
            maxNumbers,
            minPercent,
            maxPercent
        );

        // Shuffle numbers
        Game.InputNumbers.Value.Shuffle();
    }

    protected void ApplyBoosters() {
        foreach(var booster in boostersInEffect) {
            Game.TimerValue += booster.Timer;
            Game.Score += booster.Score;
            Game.LifeCount += booster.LifeCount;
        }

        boostersInEffect.Clear();
    }

    public virtual void Init() { }
    public virtual void DeInit() { }
    public virtual void Start() { }
    public virtual void Update() { }

    protected override void Reset() {
        HiScore = 0;
    }
}
