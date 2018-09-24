using UnityEngine;
using UnityEngine.UI;
using System;


public class GameMenuUiGameSkinController : CustomMonoBehaviour {
    [SerializeField] private Image[] buttonBackgrounds;
    [SerializeField] private Image howToPlayIcon;
    [SerializeField] private Image skinsIcon;
    [SerializeField] private Image settingsIcon;
    [SerializeField] private Image leaderboardsIcon;
    [SerializeField] private Image shareIcon;


    private void OnGameSkinSet(GameSkinScriptable gameSkin) {
        foreach(var buttonBackground in buttonBackgrounds) {
            buttonBackground.sprite = gameSkin.RectangleButtonSprite;
        }

        howToPlayIcon.sprite = gameSkin.HowToPlaySprite;
        skinsIcon.sprite = gameSkin.SkinsSprite;
        settingsIcon.sprite = gameSkin.SettingsSprite;
        leaderboardsIcon.sprite = gameSkin.LeaderboardsSprite;
        shareIcon.sprite = gameSkin.ShareSprite;
    }

    protected override void OnEnable() {
        base.OnEnable();

        Game.GameSkin.OnValueSet += OnGameSkinSet;
    }

    protected override void OnDisable() {
        base.OnDisable();

        Game.GameSkin.OnValueSet -= OnGameSkinSet;
    }

    private void Start() {
        OnGameSkinSet(Game.GameSkin.Value);
    }
}
