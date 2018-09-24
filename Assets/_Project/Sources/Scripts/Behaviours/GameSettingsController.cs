using UnityEngine;
using DarkTonic.MasterAudio;


public class GameSettingsController : CustomMonoBehaviour {
    private void OnIsAudioSet(bool value) {
        if(value) MasterAudio.UnmuteEverything();
        else MasterAudio.MuteEverything();
    }

    protected override void OnEnable() {
        base.OnEnable();
        Settings.IsAudioOn.OnValueSet += OnIsAudioSet;
    }

    protected override void OnDisable() {
        base.OnDisable();
        Settings.IsAudioOn.OnValueSet -= OnIsAudioSet;
    }

    private void Start() {
        Application.targetFrameRate = Settings.TargetFramerate;

        // Start on tutorial if a new player
        if(Settings.IsNewPlayer) {
            Game.GameState.Value = GameStateType.Tutorial;
            Settings.IsNewPlayer = false;
        }

        OnIsAudioSet(Settings.IsAudioOn.Value);
    }
}