using UnityEngine;
using TMPro;


public class SplashScreenUiController : CustomMonoBehaviour {
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI tipText;


    private void OnGameStateSet(GameStateType state) {
        switch(state) {
            case GameStateType.SplashScreen:
                // Randomize tips text
                int index = Random.Range(0, Settings.TipsText.Length);
                tipText.SetText(Settings.TipsText[index]);
                break;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();

        Game.GameState.OnValueSet += OnGameStateSet;
    }

    protected override void OnDisable() {
        base.OnDisable();

        Game.GameState.OnValueSet -= OnGameStateSet;
    }
}
