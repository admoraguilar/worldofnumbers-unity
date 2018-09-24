using UnityEngine;


public class GameController : CustomMonoBehaviour {
    private GameModeScriptable lastGameMode;

    
    private void OnGameModeSet(GameModeScriptable gameMode) {
        if(lastGameMode != null) lastGameMode.DeInit();
        lastGameMode = gameMode;

        gameMode.Init();
        gameMode.Start();
    }

    protected override void OnEnable() {
        base.OnEnable();
        Game.GameMode.OnValueSet += OnGameModeSet;
    }

    protected override void OnDisable() {
        base.OnDisable();
        Game.GameMode.OnValueSet -= OnGameModeSet;
    }

    private void Start() {
        OnGameModeSet(Game.GameMode.Value);
    }

    private void Update() {
        if(Game.GameMode.Value != null) Game.GameMode.Value.Update();
    }
}
