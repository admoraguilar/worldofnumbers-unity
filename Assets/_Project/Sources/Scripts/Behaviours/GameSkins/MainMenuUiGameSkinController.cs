using UnityEngine;
using UnityEngine.UI;


public class MainMenuUiGameSkinController : CustomMonoBehaviour {
    [SerializeField] private Image[] buttonBackgrounds;
    [SerializeField] private Image backToGameIcon;
    [SerializeField] private Image audioIcon;


    private void OnGameSkinSet(GameSkinScriptable gameSkin) {
        foreach(var buttonBackground in buttonBackgrounds) {
            buttonBackground.sprite = gameSkin.RectangleButtonSprite;
        }

        backToGameIcon.sprite = 
            gameSkin.BackToGameSprite;
    }

    private void OnIsAudioSet(bool value) {
        audioIcon.sprite = value ? Game.GameSkin.Value.AudioOnSprite : Game.GameSkin.Value.AudioOffSprite;
    }

    protected override void OnEnable() {
        base.OnEnable();

        Game.GameSkin.OnValueSet += OnGameSkinSet;
        Settings.IsAudioOn.OnValueSet += OnIsAudioSet;
    }

    protected override void OnDisable() {
        base.OnDisable();

        Game.GameSkin.OnValueSet -= OnGameSkinSet;
        Settings.IsAudioOn.OnValueSet -= OnIsAudioSet;
    }

    private void Start() {
        OnGameSkinSet(Game.GameSkin.Value);
        OnIsAudioSet(Settings.IsAudioOn.Value);
    }
}
