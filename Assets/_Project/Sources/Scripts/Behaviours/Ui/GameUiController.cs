using UnityEngine;
using DoozyUI;


public class GameUiController : CustomMonoBehaviour {
    [SerializeField] private UIButton restartButton;

    [SerializeField] private KeyCode debugKeyRestart;


    protected override void OnEnable() {
        base.OnEnable();

        restartButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.Results;
        });
    }

    protected override void OnDisable() {
        base.OnDisable();
    }

    private void Update() {
        if(Input.GetKeyDown(debugKeyRestart)) {
            restartButton.ExecuteClick();
        } 
    }
}
