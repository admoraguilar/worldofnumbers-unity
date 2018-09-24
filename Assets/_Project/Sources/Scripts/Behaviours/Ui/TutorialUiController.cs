using UnityEngine;
using UnityEngine.UI;
using DoozyUI;


public class TutorialUiController : CustomMonoBehaviour {
    [SerializeField] private UIButton backToGameButton;


    protected override void OnEnable() {
        base.OnEnable();

        backToGameButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.GameMenu;
        });
    }
}
