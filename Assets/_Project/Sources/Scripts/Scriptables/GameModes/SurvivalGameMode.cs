using UnityEngine;
using WEngine;
using DarkTonic.MasterAudio;


[CreateAssetMenu(menuName = "Scriptables/GameMode/Survival")]
public class SurvivalGameMode : ClassicGameMode {
    protected override void OnTargetNumberSet(int target) {
        if(target < 0) {
            Game.LifeCount--;

            if(Game.LifeCount <= 0) {
                Game.GameState.Value = GameStateType.Results;
                MasterAudio.PlaySoundAndForget("GameOver");
            } else {
                // A hacky way to delay reset play area to avoid race conditions
                ActionTemplates.RunActionNextFrame("ClassicGameMode: ResetPlay", ResetPlay);
                MasterAudio.PlaySoundAndForget("SetSuccess");
            }
        } else if(target == 0) {
            Game.Score += ScoreEachSet;
            Game.AccumulatedScore += ScoreEachSet;
            Game.TimerValue = PlayTimeLength;

            if(Game.Score > HiScore) {
                HiScore = Game.Score;

                // We play an sfx the first time we beat the hi-score in our
                // current game
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
}
