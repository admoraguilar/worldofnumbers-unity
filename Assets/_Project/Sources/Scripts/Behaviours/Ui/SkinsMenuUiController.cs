using UnityEngine;
using TMPro;
using System;
using DoozyUI;


public class SkinsMenuUiController : CustomMonoBehaviour {
    [SerializeField] private UIButton backToGameButton;
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private UIButton prevSkinButton;
    [SerializeField] private UIButton nextSkinButton;
     

    private void OnGameSkinSet(GameSkinScriptable gameSkin) {
        skinNameText.SetText(gameSkin.name);
    }

    protected override void OnEnable() {
        base.OnEnable();
        Game.GameSkin.OnValueSet += OnGameSkinSet;

        backToGameButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.GameMenu;
        });

        prevSkinButton.OnClick.AddListener(() => {
            int index = Array.IndexOf(Settings.GameSkins, Game.GameSkin.Value);
            index = index <= 0 ? Settings.GameSkins.Length - 1 : --index;
            Game.GameSkin.Value = Settings.GameSkins[index];
        });

        nextSkinButton.OnClick.AddListener(() => {
            int index = Array.IndexOf(Settings.GameSkins, Game.GameSkin.Value);
            index = index >= Settings.GameSkins.Length - 1 ? 0 : ++index;
            Game.GameSkin.Value = Settings.GameSkins[index];
        });
    }

    protected override void OnDisable() {
        base.OnDisable();
        Game.GameSkin.OnValueSet -= OnGameSkinSet;
    }

    private void Start() {
        OnGameSkinSet(Game.GameSkin.Value);
    }
}
