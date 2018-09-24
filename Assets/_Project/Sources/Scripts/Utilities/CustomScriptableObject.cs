using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


public abstract class CustomScriptableObject : SerializedScriptableObject {
    [SerializeField, FoldoutGroup("Singleton References")] protected GameData Game;
    [SerializeField, FoldoutGroup("Singleton References")] protected SettingsData Settings;
    [SerializeField, FoldoutGroup("Singleton References")] protected ADebugScriptable Debugger;
    [SerializeField, FoldoutGroup("Singleton References")] protected bool ResetOnPlayMode = true;


    [Button("Resolve References"), FoldoutGroup("Singleton References")]
    private void ResolveReferences() {
        Game = Singleton.Get<GameData>();
        Settings = Singleton.Get<SettingsData>();
        Debugger = Singleton.Get<ADebugScriptable>();
    }

    protected virtual void OnEnable() {
        if(ResetOnPlayMode) {
            ResolveReferences();
            Reset();
        }
    }

    protected virtual void OnDisable() {

    }

    protected virtual void Reset() {
        
    }
}
