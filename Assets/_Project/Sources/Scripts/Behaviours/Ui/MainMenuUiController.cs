using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DoozyUI;
using Sirenix.OdinInspector;
using WEngine;


public class MainMenuUiController : CustomMonoBehaviour {
    [Title("Main Menu")]
    [SerializeField] private UIButton backToGameButton;
    [SerializeField] private UIButton audioButton;
    [SerializeField] private UIButton manageAccountButton;
    [SerializeField] private TextMeshProUGUI manageAccountButtonText;

    [Title("Credits")]
    [SerializeField] private UIButton prevCreditsButton;
    [SerializeField] private UIButton nextCreditsButton;
    [SerializeField] private TextMeshProUGUI[] nameTexts;

    private int namesMaxPages;
    private int currentNamesPage;

    [Title("Sign In")]
    [SerializeField] private UIElement signInUi;
    [SerializeField] private UIButton signInBackButton;
    [SerializeField] private TMP_InputField displayNameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private UIButton signInButton;
    [SerializeField] private TextMeshProUGUI signInStatusText;

    [Title("Sign Out")]
    [SerializeField] private UIElement signOutUi;
    [SerializeField] private UIButton signOutBackButton;
    [SerializeField] private UIButton signOutButton;
    [SerializeField] private TextMeshProUGUI signOutNameText;
    [SerializeField] private TextMeshProUGUI signOutStatusText;


    #region Credits Methods

    private void ShowNames(string[] names, int maxPerPage, int page) {
        var namesArr = names.Skip(maxPerPage * page).Take(maxPerPage).ToArray();

        for(int i = 0; i < nameTexts.Length; ++i) {
            if(i < namesArr.Length) nameTexts[i].SetText(namesArr[i]);
            else nameTexts[i].SetText("");
        }
    }

    #endregion

    protected override void OnEnable() {
        base.OnEnable();

        #region Main Menu

        backToGameButton.OnClick.AddListener(() => {
            Game.GameState.Value = GameStateType.GameMenu;
        });

        audioButton.OnClick.AddListener(() => {
            Settings.IsAudioOn.Value = !Settings.IsAudioOn.Value;
        });
        manageAccountButton.OnClick.AddListener(() => {
            if(Backend.IsAuthenticated || Backend.UserInfo.IsSignedInLastTime) signOutUi.Show(false);
            if(!Backend.IsAuthenticated && !Backend.UserInfo.IsSignedInLastTime) signInUi.Show(false);
        });

        #endregion

        #region Credits

        prevCreditsButton.OnClick.AddListener(() => {
            currentNamesPage = currentNamesPage <= 0 ? namesMaxPages : --currentNamesPage;
            ShowNames(Settings.CreditsNames, nameTexts.Length, currentNamesPage);
        });

        nextCreditsButton.OnClick.AddListener(() => {
            currentNamesPage = currentNamesPage >= namesMaxPages ? 0 : ++currentNamesPage;
            ShowNames(Settings.CreditsNames, nameTexts.Length, currentNamesPage);
        });

        #endregion

        #region Sign In

        signInButton.OnClick.AddListener(() => {
            Backend.RegisterOrAuthenticate(emailInputField.text, passwordInputField.text, displayNameInputField.text,
            () => {
                signInUi.Hide(false);
            }, null);
        });

        signInBackButton.OnClick.AddListener(() => {
            signInUi.Hide(false);
        });

        #endregion

        #region Sign Out

        signOutButton.OnClick.AddListener(() => {
            Backend.SignOut();
            signOutUi.Hide(false);
        });

        signOutBackButton.OnClick.AddListener(() => {
            signOutUi.Hide(false);
        });

        #endregion
    }

    private void Start() {
        #region Credits

        // Init credits page
        namesMaxPages = Settings.CreditsNames.Length / nameTexts.Length;
        currentNamesPage = 0;
        ShowNames(Settings.CreditsNames, nameTexts.Length, currentNamesPage);

        #endregion

        #region Sign In 

        signInStatusText.SetText("");

        #endregion

        #region Sign Out

        signOutStatusText.SetText("");

        #endregion
    }

    private void Update() {
        #region Main Menu

        manageAccountButtonText.SetText(Backend.IsAuthenticated || Backend.UserInfo.IsSignedInLastTime ? "SIGN OUT" : "SIGN IN");

        #endregion

        #region Sign In

        signInStatusText.SetText(Backend.StatusInfo);
        // Only enable sign in function if the backend service
        // is available
        signInButton.Interactable = Backend.IsAvailable;

        #endregion

        #region Sign Out

        signOutNameText.SetText(Backend.UserInfo.DisplayName);
        // Only enable sign out function if the backend service
        // is available
        signOutButton.Interactable = Backend.IsAvailable;

        #endregion
    }
}
