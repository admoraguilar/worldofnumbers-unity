using UnityEngine;
using Sirenix.OdinInspector;


public class CustomMonoBehaviour : MonoBehaviour {
    [SerializeField, FoldoutGroup("Singleton References")] protected GameData Game;
    [SerializeField, FoldoutGroup("Singleton References")] protected SettingsData Settings;
    [SerializeField, FoldoutGroup("Singleton References")] protected ABackendScriptable Backend;
    [SerializeField, FoldoutGroup("Singleton References")] protected AAdsScriptable Ads;
    [SerializeField, FoldoutGroup("Singleton References")] protected ANativeShareScriptable NativeShare;
    [SerializeField, FoldoutGroup("Singleton References")] protected AUiScriptable Ui;
    [SerializeField, FoldoutGroup("Singleton References")] protected ADebugScriptable Debugger;


    [Button("Resolve References"), FoldoutGroup("Singleton References")]
    private void ResolveReferences() {
        //if(Game == null) Game = Singleton.Get<GameData>();
        //if(Settings == null) Settings = Singleton.Get<SettingsData>();
        //if(Backend == null) Backend = Singleton.Get<ABackendScriptable>();
        //if(Ads == null) Ads = Singleton.Get<AAdsScriptable>();
        //if(NativeShare == null) NativeShare = Singleton.Get<ANativeShareScriptable>();
        //if(Ui == null) Ui = Singleton.Get<AUiScriptable>();
        //if(Debugger == null) Debugger = Singleton.Get<ADebugger>();

        Game = Singleton.Get<GameData>();
        Settings = Singleton.Get<SettingsData>();
        Backend = Singleton.Get<ABackendScriptable>();
        Ads = Singleton.Get<AAdsScriptable>();
        NativeShare = Singleton.Get<ANativeShareScriptable>();
        Ui = Singleton.Get<AUiScriptable>();
        Debugger = Singleton.Get<ADebugScriptable>();
    }

    protected virtual void OnEnable() {
        ResolveReferences();
    }

    protected virtual void OnDisable() {

    }

    protected virtual void Reset() {
        ResolveReferences();
    }
}
