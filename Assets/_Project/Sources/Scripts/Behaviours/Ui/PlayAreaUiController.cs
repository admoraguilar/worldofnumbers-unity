using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using DoozyUI;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;



public class PlayAreaUiController : CustomMonoBehaviour {
    [SerializeField] private List<PlayAreaUiRectangle> inputRectangles = new List<PlayAreaUiRectangle>();
    [SerializeField] private PlayAreaUiRectangle aimRectangle;
    [SerializeField] private Image radialTimerImage;
    [SerializeField] private ModeChangerButton modeChangerButton;
    [SerializeField] private PlayAreaTexts playAreaTexts;

    [SerializeField] private KeyCode[] debugKeyInput;


    private void OnGameStateSet(GameStateType state) {
        // Enable/Disable functionality based on conditions
        foreach(var rectangle in inputRectangles) {
            rectangle.Button.Interactable = state == GameStateType.Game || state == GameStateType.GameMenu;
        }

        modeChangerButton.Button.Interactable = state == GameStateType.GameMenu;
    }

    private void OnTargetNumberSet(int target) {
        aimRectangle.NumberValueText.SetText(target.ToString());

        // Play a simple animation
        if(target == 0) {
            aimRectangle.Button.ExecuteClick();
        }
    }

    private void OnInputNumbersSet(int[] inputNumbers) {
        // Shuffle rectangle list
        inputNumbers.Shuffle();

        int inputRectangleIndex = 0;
        foreach(var inputNumber in inputNumbers) {
            var inputRectangle = inputRectangles[inputRectangleIndex];
            inputRectangle.RectangleUi.Show(false);

            // Setup ui
            inputRectangle.NumberValueText.SetText(inputNumber.ToString());

            inputRectangle.Button.OnClick.RemoveAllListeners();
            inputRectangle.Button.OnClick.AddListener(() => {
                // We start Game if any rectangle is clicked
                if(Game.GameState.Value != GameStateType.Game)
                    Game.GameState.Value = GameStateType.Game;

                inputRectangle.RectangleUi.Hide(false);
                Game.TargetNumber.Value -= inputNumber;

                // Prevent double tap
                inputRectangle.Button.OnClick.RemoveAllListeners();

                MasterAudio.PlaySoundAndForget("PlayRectangleClick");
            });

            inputRectangleIndex++;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();

        Game.GameState.OnValueSet += OnGameStateSet;
        Game.TargetNumber.OnValueSet += OnTargetNumberSet;
        Game.InputNumbers.OnValueSet += OnInputNumbersSet;

        modeChangerButton.Button.OnClick.AddListener(() => {
            int index = Array.IndexOf(Settings.GameModes, Game.GameMode.Value);
            index = index >= Settings.GameModes.Length - 1 ? 0 : ++index;
            Game.GameMode.Value = Settings.GameModes[index];
        });
    }

    protected override void OnDisable() {
        base.OnDisable();

        Game.GameState.OnValueSet -= OnGameStateSet;
        Game.TargetNumber.OnValueSet -= OnTargetNumberSet;
        Game.InputNumbers.OnValueSet -= OnInputNumbersSet;
    }

    private void Update() {
        playAreaTexts.Score.SetText(Game.Score.ToString());
        playAreaTexts.HiScore.SetText(Game.GameMode.Value.HiScore.ToString());
        playAreaTexts.AccumulatedScore.SetText(Game.AccumulatedScore.ToString());
        playAreaTexts.LifeCount.SetText(Game.LifeCount.ToString());
        modeChangerButton.Text.SetText(Game.GameMode.Value.name);

        // Timer
        TimeSpan timeSpan = TimeSpan.FromSeconds(Game.TimerValue);
        playAreaTexts.TimerValue.SetText(string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds));

        // Radial timer
        radialTimerImage.fillAmount = Mathf.InverseLerp(0f, Game.GameMode.Value.PlayTimeLength, Game.TimerValue);

        // Debug keyboard input
        for(int i = 0; i < debugKeyInput.Length; ++i) {
            var key = debugKeyInput[i];

            if(Input.GetKeyDown(key)) {
                inputRectangles[i].Button.ExecuteClick();
            }
        }
    }


    [Serializable]
    public class PlayAreaUiRectangle {
        public UIElement RectangleUi;
        public UIButton Button;
        public TextMeshProUGUI NumberValueText;
    }

    [Serializable]
    public class ModeChangerButton {
        public UIButton Button;
        public TextMeshProUGUI Text;
    }

    [Serializable]
    public class PlayAreaTexts {
        public TextMeshProUGUI Score;
        public TextMeshProUGUI HiScore;
        public TextMeshProUGUI AccumulatedScore;
        public TextMeshProUGUI LifeCount;
        public TextMeshProUGUI TimerValue;
    }
}
