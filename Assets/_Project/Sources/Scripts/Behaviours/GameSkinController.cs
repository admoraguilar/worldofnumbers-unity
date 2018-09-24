using UnityEngine;


public class GameSkinController : CustomMonoBehaviour {
    private GameSkinScriptable lastGameSkin;


    private void OnGameSkinSet(GameSkinScriptable gameSkin) {
        if(lastGameSkin != null) lastGameSkin.DeInit();
        lastGameSkin = gameSkin;

        gameSkin.Init();
        gameSkin.Start();
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

    private void Update() {
        if(Game.GameSkin.Value != null) Game.GameSkin.Value.Update();
    }
}
