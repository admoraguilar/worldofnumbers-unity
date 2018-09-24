using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptables/Singleton/DefaultDebugger")]
public class DefaultDebugger : ADebugScriptable {
    [SerializeField] private bool isEnabled = true;


    public override void Log(DebuggerType type, string message, params object[] args) {
        if(!isEnabled) return;

        switch(type) {
            case DebuggerType.Log: Debug.Log(FormatMessage(message, args)); break;
            case DebuggerType.Warning: Debug.LogWarning(FormatMessage(message, args)); break;
            case DebuggerType.Error: Debug.LogError(FormatMessage(message, args)); break;
        }
    }

    private string FormatMessage(string message, params object[] args) {
        return string.Format(message, args);
    }
}


public abstract class ADebugScriptable : CustomScriptableObject {
    public abstract void Log(DebuggerType type, string message, params object[] args);
}


public enum DebuggerType {
    Log,
    Warning,
    Error
}
