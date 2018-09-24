using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class BackgroundUiGameSkinController : CustomMonoBehaviour {
    [SerializeField] private Image background1;
    [SerializeField] private Image background2;


    private void OnGameSkinSet(GameSkinScriptable gameSkin) {
        gameSkin.DoBackgroundsBehaviour(background1, background2);
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
