using UnityEngine;
using Sirenix.OdinInspector;
using WEngine;
using DarkTonic.MasterAudio;


[CreateAssetMenu(menuName = "Scriptables/GameMode/Classic")]
public class ClassicGameMode : GameModeScriptable {
    [Title("Classic Play Mode: Play Area")]
    public int TargetNumberRangeMin = 5;
    public int TargetNumberRangeMax = 20;
    public int InputNumbersDivideCount = 4;
    public int InputNumbersMaxNumber = 8;
    public float InputNumbersPercentRangeMin = 1f;
    public float InputNumbersPercentRangeMax = 25f;


    protected void FullReset() {
        ResetPlayStats();
        ResetPlay();
        ApplyBoosters();
    }

    protected void ResetPlayStats() {
        Game.Score = 0;
        Game.LifeCount = 1;
        Game.IsBeatHiScoreInCurrentGame = false;
        Game.IsRunningOutOfTime = false;
        Game.TimerValue = PlayTimeLength;
    }

    protected void ResetPlay() {
        ResetPlay(Random.Range(TargetNumberRangeMin, TargetNumberRangeMax),
                  InputNumbersDivideCount,
                  InputNumbersMaxNumber,
                  InputNumbersPercentRangeMin,
                  InputNumbersPercentRangeMax);
    }

    protected virtual void OnGameStateSet(GameStateType state) {
        switch(state) {
            case GameStateType.SplashScreen:
                var action = new ActionQueue("ClassicGameMode: SplashScreen");
                //action.AddAction(() => Game.GameState.Value = GameStateType.SplashScreen);
                action.AddAction(new WaitForSeconds(Settings.SplashScreenShowLength));
                action.AddAction(() => Game.GameState.Value = GameStateType.GameMenu);
                action.Start();
                break;
            case GameStateType.Game:
                MasterAudio.PlaySoundAndForget("PlayButtonClick");
                break;
            case GameStateType.GameMenu:
                FullReset();
                break;
            case GameStateType.Results:
                MasterAudio.PlaySoundAndForget("GameOver");
                break;
        }
    }

    protected virtual void OnTargetNumberSet(int target) {
        if(target < 0) {
            Game.LifeCount--;

            if(Game.LifeCount <= 0) {
                Game.GameState.Value = GameStateType.Results;
                MasterAudio.PlaySoundAndForget("GameOver");
            } 
            else {
                // A hacky way to delay reset play area to avoid race conditions
                ActionTemplates.RunActionNextFrame("ClassicGameMode: ResetPlay", ResetPlay);
                MasterAudio.PlaySoundAndForget("SetSuccess");
            }
        } else if(target == 0) {
            Game.Score += ScoreEachSet;
            Game.AccumulatedScore += ScoreEachSet;

            if(Game.Score > HiScore) {
                HiScore = Game.Score;

                if(!Game.IsBeatHiScoreInCurrentGame) {
                    MasterAudio.PlaySoundAndForget("BeatHiScore");
                    Game.IsBeatHiScoreInCurrentGame = true;
                }
            }

            // A hacky way to delay reset play area to avoid race conditions
            ActionTemplates.RunActionNextFrame("ClassicGameMode: ResetPlay", ResetPlay);
            MasterAudio.PlaySoundAndForget("SetSuccess");
        }
    }

    public override void Init() {
        base.Init();

        Game.GameState.OnValueSet += OnGameStateSet;
        Game.TargetNumber.OnValueSet += OnTargetNumberSet;
    }

    public override void DeInit() {
        Game.GameState.OnValueSet -= OnGameStateSet;
        Game.TargetNumber.OnValueSet -= OnTargetNumberSet;
    }

    public override void Start() {
        FullReset();
    }

    public override void Update() {
        switch(Game.GameState.Value) {
            case GameStateType.Game:
                if(Game.TimerValue > 0f) {
                    Game.TimerValue -= Time.deltaTime;
                }

                // For effects like ticking bomb
                if(Game.TimerValue <= TimeBombStartTime && !Game.IsRunningOutOfTime) {
                    Game.IsRunningOutOfTime = true;
                }

                // Game over
                if(Game.TimerValue < 0f) {
                    Game.GameState.Value = GameStateType.Results;
                }
                break;
        }
    }
}
