using UnityEngine;
using TMPro;
using System;
using DoozyUI;
using WEngine;
using DarkTonic.MasterAudio;


public class ResultsUiController : CustomMonoBehaviour {
    [SerializeField] private UIButton backToGameButton;
    [SerializeField] private TextMeshProUGUI appliedBoostersCountText;
    [SerializeField] private TextMeshProUGUI maxBoostersText;
    [SerializeField] private BoosterUi[] boosterUis;

    [SerializeField] private KeyCode debugKeyBackToGame;


    private void ResetBoostersUi(BoosterData[] boosters) {
        int boosterUiIndex = 0;
        foreach(var booster in boosters) {
            var boosterUi = boosterUis[boosterUiIndex];
            boosterUi.BoosterUiContainer.SetActive(true);

            // Setup ui
            boosterUi.NameText.SetText(booster.name);
            boosterUi.PriceText.SetText(booster.Price.ToString());

            boosterUi.BuyButton.OnClick.RemoveAllListeners();
            boosterUi.BuyButton.OnClick.AddListener(() => {
                // Apply boosters if have enough accumulated
                if(Game.AccumulatedScore > booster.Price) {
                    Game.GameMode.Value.AddBoosters(booster);
                    Game.AccumulatedScore -= booster.Price;

                    MasterAudio.PlaySoundAndForget("BuySuccess");
                } else {
                    MasterAudio.PlaySoundAndForget("BuyFailed");
                }
            });

            boosterUi.WatchButton.OnClick.RemoveAllListeners();
            boosterUi.WatchButton.OnClick.AddListener(() => {
                // Shows an ads and applies the booster when the ad is finished
                Ads.Show((result) => {
                    switch(result) {
                        case UnityEngine.Advertisements.ShowResult.Finished:
                            // Apply booster
                            Game.GameMode.Value.AddBoosters(booster);

                            if(Settings.RuntimeLoseCountToEnableAds <= 0) {
                                Settings.RuntimeAdsCurrencyCount--;

                                // Reset ad counter
                                if(Settings.RuntimeAdsCurrencyCount <= 0) {
                                    Settings.RuntimeAdsCurrencyCount = Settings.AdsCurrencyCountPerEnabled;
                                    Settings.RuntimeLoseCountToEnableAds = Settings.LoseCountToEnableAds;
                                }
                            }
                            
                            MasterAudio.PlaySoundAndForget("BuySuccess");
                            break;
                    }
                });
            });

            boosterUiIndex++;
        }

        // Disable excess boosterui
        for(int i = boosterUiIndex; i < boosterUis.Length; ++i) {
            var boosterUi = boosterUis[i];
            boosterUi.BoosterUiContainer.SetActive(false);
        }
    }

    private void OnGameStateSet(GameStateType gameState) {
        switch(gameState) {
            case GameStateType.Results:
                // Decrease ad counter
                if(Settings.RuntimeLoseCountToEnableAds > 0) Settings.RuntimeLoseCountToEnableAds--;
                break;
        }
    }

    private void OnGameModeSet(GameModeScriptable gameMode) {
        ResetBoostersUi(gameMode.AvailableBoosters);
    }

    protected override void OnEnable() {
        base.OnEnable();
        Game.GameState.OnValueSet += OnGameStateSet;
        Game.GameMode.OnValueSet += OnGameModeSet;

        backToGameButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.SplashScreen;
        });
    }

    protected override void OnDisable() {
        base.OnDisable();
        Game.GameState.OnValueSet -= OnGameStateSet;
        Game.GameMode.OnValueSet -= OnGameModeSet;
    }

    private void Start() {
        ResetBoostersUi(Game.GameMode.Value.AvailableBoosters);
    }

    private void Update() {
        appliedBoostersCountText.SetText(Game.GameMode.Value.BoostersInEffectCount.ToString());
        maxBoostersText.SetText(Settings.MaxBoostersPerGame.ToString());

        // Only show ad buttons when lose count counter is 0
        for(int i = 0; i < Game.GameMode.Value.AvailableBoosters.Length; ++i) {
            boosterUis[i].BoosterUiContainer.gameObject.SetActive(!(Game.GameMode.Value.BoostersInEffectCount >= Settings.MaxBoostersPerGame));
            boosterUis[i].WatchButton.gameObject.SetActive(Settings.RuntimeLoseCountToEnableAds <= 0);
        }

        // Debug back to game key
        if(Input.GetKeyDown(debugKeyBackToGame)) {
            backToGameButton.ExecuteClick();
        }
    }

    [Serializable]
    public class BoosterUi {
        public GameObject BoosterUiContainer;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI PriceText;
        public UIButton BuyButton;
        public UIButton WatchButton;
    }
}
