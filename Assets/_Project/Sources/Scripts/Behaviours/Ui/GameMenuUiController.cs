using UnityEngine;
using DoozyUI;


public class GameMenuUiController : CustomMonoBehaviour {
    [SerializeField] private UIButton howToPlayButton;
    [SerializeField] private UIButton skinsButton;
    [SerializeField] private UIButton settingsButton;
    [SerializeField] private UIButton leaderboardsButton;
    [SerializeField] private UIButton shareButton;


    protected override void OnEnable() {
        base.OnEnable();

        // Game Menu
        howToPlayButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.Tutorial;
        });

        skinsButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.SkinsMenu;
        });

        settingsButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.MainMenu;
        });

        leaderboardsButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.Leaderboards;
        });

        shareButton.OnClick.AddListener(() => {
            var hiScore = Game.GameMode.Value.HiScore;
            var gameMode = Game.GameMode.Value.name;
            var message = "";

            message = string.Format("I got {0} on {1}! Think you can beat my score?", hiScore, gameMode);
            message += System.Environment.NewLine;
            message += string.Format("{0}Play Store: https://play.google.com/store/apps/details?id=com.ProjectJW.WorldOfNumber", System.Environment.NewLine);
            message += string.Format("{0}App Store: https://itunes.apple.com/us/app/world-of-numbers/id1407198093", System.Environment.NewLine);

            NativeShare.ShareScreenshotWithText(message);
        });
    }
}
