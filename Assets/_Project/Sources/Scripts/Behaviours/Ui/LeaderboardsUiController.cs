using UnityEngine;
using TMPro;
using System;
using System.Collections;
using DoozyUI;
using WEngine;


public class LeaderboardsUiController : CustomMonoBehaviour {
    [SerializeField] private UIButton backToGameButton;
    [SerializeField] private TextMeshProUGUI leaderboardsModeText;
    [SerializeField] private GameObject entriesPanel;
    [SerializeField] private GameObject requiresSignInPanel;
    [SerializeField] private GameObject requiresInternetPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private LeaderboardEntryText[] topLeaderboardEntryTexts;
    [SerializeField] private LeaderboardEntryText playerLeaderboardEntryText;

    private ActionQueue loadEntriesAction = new ActionQueue("LeaderboardsUiController: Load Entries()");


    private void DisplayPanel(PanelToDisplay panel) {
        requiresSignInPanel.SetActive(panel == PanelToDisplay.RequiresSignIn);
        requiresInternetPanel.SetActive(panel == PanelToDisplay.RequiresInternet);
        loadingPanel.SetActive(panel == PanelToDisplay.Loading);
        entriesPanel.SetActive(panel == PanelToDisplay.PlayerEntries);
    }

    private IEnumerator LoadEntries(string leaderboardShortCode) {
        DisplayPanel(PanelToDisplay.Loading);

        bool isLoadedTopEntries = false;
        bool isLoadedPlayerEntry = false;

        // Get top entries
        Backend.GetTopLeaderboardEntries(leaderboardShortCode, 20,
            (System.Collections.Generic.Dictionary<string, LeaderboardEntry[]> entries) => {
                Debugger.Log(DebuggerType.Log, "Success top leaderboard load LeaderboardsUiController");

                int topEntryTextIndex = 0;
                foreach(var entry in entries[leaderboardShortCode]) {
                    if(topEntryTextIndex >= topLeaderboardEntryTexts.Length) continue;

                    // Display entry text
                    var entryText = topLeaderboardEntryTexts[topEntryTextIndex];

                    entryText.EntryTextObject.SetActive(true);
                    entryText.RankText.SetText(entry.Rank.ToString());
                    entryText.NameText.SetText(entry.DisplayName);
                    entryText.ScoreText.SetText(entry.Score.ToString());

                    topEntryTextIndex++;
                }

                for(int i = topEntryTextIndex; i < topLeaderboardEntryTexts.Length; ++i) {
                    var textEntry = topLeaderboardEntryTexts[i];
                    textEntry.EntryTextObject.SetActive(false);
                }

                isLoadedTopEntries = true;
            }, 
            () => {
                foreach(var entryText in topLeaderboardEntryTexts) {
                    entryText.EntryTextObject.SetActive(false);
                }
            }
        );

        // Get player entry
        Backend.GetPlayerLeaderboardEntry(new string[] { leaderboardShortCode },
            (System.Collections.Generic.Dictionary<string, LeaderboardEntry> leaderboards) => {
                Debugger.Log(DebuggerType.Log, "Success player leaderboard load LeaderboardsUiController");

                var entry = leaderboards[leaderboardShortCode];
                playerLeaderboardEntryText.EntryTextObject.SetActive(true);
                playerLeaderboardEntryText.RankText.SetText(entry.Rank.ToString());
                playerLeaderboardEntryText.NameText.SetText(entry.DisplayName);
                playerLeaderboardEntryText.ScoreText.SetText(entry.Score.ToString());

                isLoadedPlayerEntry = true;
            }, 
            () => {
                playerLeaderboardEntryText.EntryTextObject.SetActive(false);
            }
        );

        Debugger.Log(DebuggerType.Log, "Done: {0} | Done: {1}", isLoadedPlayerEntry.ToString(), isLoadedTopEntries.ToString());

        yield return new WaitUntil(() => isLoadedTopEntries && isLoadedPlayerEntry);
        //yield return new WaitForSeconds(2f);

        DisplayPanel(PanelToDisplay.PlayerEntries);
        Debugger.Log(DebuggerType.Log, "Displaying player entries.");
    }

    private void OnGameStateSet(GameStateType state) {
        switch(state) {
            case GameStateType.Leaderboards:
                // Replace some texts
                leaderboardsModeText.SetText(string.Format("Global: {0}", Game.GameMode.Value.name));

                // Display panel
                if(!Backend.IsAvailable && Backend.IsAuthenticated) {
                    DisplayPanel(PanelToDisplay.RequiresInternet);
                } else if(Backend.IsAvailable && !Backend.IsAuthenticated) {
                    DisplayPanel(PanelToDisplay.RequiresSignIn);
                } else if(Backend.IsAvailable && Backend.IsAuthenticated) {
                    loadEntriesAction.Stop();
                    loadEntriesAction.AddAction(LoadEntries(Game.GameMode.Value.LeaderboardCode));
                    loadEntriesAction.Start();
                }
                break;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();

        Game.GameState.OnValueSet += OnGameStateSet;

        backToGameButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.GameMenu;
        });
    }

    protected override void OnDisable() {
        base.OnDisable();

        Game.GameState.OnValueSet -= OnGameStateSet;
    }


    public enum PanelToDisplay {
        RequiresSignIn,
        RequiresInternet,
        Loading,
        PlayerEntries
    }

    [Serializable]
    public class LeaderboardEntryText {
        public GameObject EntryTextObject;
        public TextMeshProUGUI RankText;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI ScoreText;
    }
}
