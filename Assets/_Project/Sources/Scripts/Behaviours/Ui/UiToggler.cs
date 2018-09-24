using UnityEngine;
using DoozyUI;


public class UiToggler : CustomMonoBehaviour {
    [SerializeField] private UIElement uiElement;
    [SerializeField] private GameStateType toggleOnState;
    [SerializeField] private ToggleType toggleType = ToggleType.UIElementActive;
    [SerializeField] private bool isInstantAction = false;


    private void OnGameStateSet(GameStateType state) {
        if(toggleOnState == state) {
            switch(toggleType) {
                case ToggleType.UIElementActive: if(!uiElement.isVisible) uiElement.Show(isInstantAction); break;
                //case ToggleType.UIElementActive: Ui.SetUiHierarchy(uiElement.elementName, uiElement.elementCategory); break;
                case ToggleType.CanvasGroupInteractable: uiElement.CanvasGroup.interactable = true; break;
            } 
        } else {
            switch(toggleType) {
                case ToggleType.UIElementActive: if(uiElement.isVisible) uiElement.Hide(isInstantAction); break;
                //case ToggleType.UIElementActive: Ui.PopUiHierarchy(uiElement.elementName, uiElement.elementCategory); break;
                case ToggleType.CanvasGroupInteractable: uiElement.CanvasGroup.interactable = false; break;
            } 
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

    private void Start() {
        OnGameStateSet(Game.GameState.Value);
    }


    public enum ToggleType {
        UIElementActive,
        CanvasGroupInteractable,
    }
}
